using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Sirenix.OdinInspector;

public interface IObjectPoolClean
{
    void PoolClean();
}

public class ObjectPoolTag : MonoBehaviour
{
    public string ResourcePath { get; set; }
}

public class ObjectPool : SingletonInstance<ObjectPool>
{
    enum PoolingType
    {
        GAMEOBJECT,
        UI,
        PARTICLE
    }

    [System.Serializable]
    class PoolData
    {
        public string Alias;
        public int Count;
        [ReadOnly] public string Path;
        [ReadOnly] public string GUID;
        [ReadOnly] public PoolingType Type;
    }

    [SerializeField] string poolingFolder = "Pooling";
    [SerializeField] List<PoolData> createPools;

    GameObject poolTempRoot;
    Dictionary<string, GameObject> poolRoot;
    Dictionary<string, Queue<GameObject>> poolQueue;
    Dictionary<string, PoolingType> types;
    Dictionary<string, string> alias;

    private void Awake()
    {
        poolRoot = new Dictionary<string, GameObject>();
        poolQueue = new Dictionary<string, Queue<GameObject>>();
        types = new Dictionary<string, PoolingType>();
        alias = new Dictionary<string, string>();
        DontDestroyOnLoad(gameObject);

        poolTempRoot = new GameObject("Temp");
        poolTempRoot.transform.SetParent(transform);

        foreach (var item in createPools)
        {
            CreatePool(item.Path, item.Count);
            alias[item.Alias] = item.Path;
            types[item.Path] = item.Type;
        }
    }
    
#if UNITY_EDITOR
    [Button]
    void AutoGeneratePools()
    {
        if (createPools == null)
            createPools = new List<PoolData>();

        var tempPools = createPools.ToList();
        createPools.Clear();
        foreach (var item in Resources.LoadAll<GameObject>(poolingFolder))
        {
            var data = new PoolData();
            data.Path = UnityEditor.AssetDatabase.GetAssetPath(item).Replace($"Assets/Resources/", "").Replace(".prefab", "");
            data.Alias = data.Path.Replace($"{poolingFolder}/", "");
            data.Count = 10;
            long id = 0;
            UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item, out data.GUID, out id);

            if (item.GetComponentInChildren<ParticleSystem>(true) != null)
                data.Type = PoolingType.PARTICLE;
            else if (item.GetComponentInChildren<RectTransform>(true) != null)
                data.Type = PoolingType.UI;
            else
                data.Type = PoolingType.GAMEOBJECT;

            if (tempPools.Count > 0)
            {
                var find = tempPools.Find(f => f.GUID.Equals(data.GUID));
                if (find != null)
                {
                    data.Count = find.Count;
                    data.Alias = find.Alias;
                }
            }

            createPools.Add(data);
        }
    }
#endif

    public Transform GetPoolTempRoot() => poolTempRoot.transform;

    public void AddPoolChild(Transform tf)
    {
        var count = tf.childCount;
        for (int i = 0; i < count; i++)
            _AddPool(tf.GetChild(0).gameObject);
    }

    public void CleanTempRoot()
    {
        if (poolTempRoot == null)
            return;

        int count = poolTempRoot.transform.childCount;
        for (int i = 0; i < count; i++)
            _AddPool(poolTempRoot.transform.GetChild(0).gameObject);
    }

    public void AddPoolParticle(GameObject obj)
    {
        AddPool(obj, obj.GetComponentInChildren<ParticleSystem>().main.duration);
    }

    public void AddPool(GameObject obj, float time = 0)
    {
        if (time > 0)        
            StartCoroutine(AddPoolDelay(obj, time));
        else
            _AddPool(obj);
    }

    IEnumerator AddPoolDelay(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj != null)
            _AddPool(obj);
    }

    void _AddPool(GameObject obj)
    {
        if (obj == null) return;
        var tag = obj.GetComponent<ObjectPoolTag>();
        if (tag == null)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        GameObject root = null;
        if (!poolRoot.ContainsKey(tag.ResourcePath))
        {
            root = new GameObject(tag.ResourcePath);
            root.transform.SetParent(transform);
            poolRoot[tag.ResourcePath] = root;
            poolQueue[tag.ResourcePath] = new Queue<GameObject>();
        }
        else
            root = poolRoot[tag.ResourcePath];

        if (poolQueue[tag.ResourcePath].Contains(obj))
            return;

        var clean = obj.GetComponentInChildren<IObjectPoolClean>();
        if (clean != null)
            clean.PoolClean();

        obj.transform.SetParent(root.transform);
        poolQueue[tag.ResourcePath].Enqueue(obj);
    }

    /// <summary>
    /// 풀링이 적으면 새로 생성
    /// </summary>
    public GameObject GetPool(string path, Transform parent = null)
    {
        if (alias.ContainsKey(path))
            path = alias[path];

        var resource = ResourceManager.Instance.GetResource<GameObject>(path);

        if (resource == null)
        {
            Debug.LogError($"{path} 는 등록되지 않은 풀링 오브젝트입니다.");
            return null;
        }

        if (parent == null)
            parent = GetPoolTempRoot();

        if (!poolQueue.ContainsKey(path) || poolQueue[path].Count == 0)
        {
            var clone = Instantiate(resource, parent);
            clone.AddComponent<ObjectPoolTag>().ResourcePath = path;
            return clone;
        }

        GameObject dequeue = poolQueue[path].Dequeue();
        dequeue.transform.SetParent(parent);
        dequeue.transform.localScale = resource.transform.localScale;
        dequeue.transform.localPosition = resource.transform.localPosition;
        dequeue.transform.localRotation = resource.transform.localRotation;

        dequeue.SetActive(true);

        if (types[path] == PoolingType.UI)
        {
            if (dequeue.TryGetComponent(out Button button))
                button.onClick.RemoveAllListeners();
        }

        if (types[path] == PoolingType.PARTICLE)
        {
            foreach (var ps in dequeue.GetComponentsInChildren<ParticleSystem>(true))
                ps.gameObject.SetActive(true);
        }

        return dequeue;
    }

    void CreatePool(string path, int count = 10)
    {
        var resource = ResourceManager.Instance.GetResource<GameObject>(path);

        this.TaskFor(0.01f, 0, count, 1, (i) =>
        {
            var clone = Instantiate(resource);
            clone.AddComponent<ObjectPoolTag>().ResourcePath = path;
            _AddPool(clone);
        });
    }
}
