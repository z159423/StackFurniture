using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum LanguageType
{
    ENGLISH
}

public abstract class VerificationObject : ScriptableObject
{
    public abstract void Verification();
}

#if UNITY_EDITOR
public class ResourceVerification : EditorWindow
{
    [MenuItem("Resource/Verification", priority = 1)]
    public static void ToLoading()
    {
        var res = Resources.LoadAll<VerificationObject>("");
        foreach (var item in res)
            item.Verification();
        Debug.Log("Resource Verified");
    }
}
#endif

public class ResourceManager : SingletonStatic<ResourceManager>
{
    [RuntimeInitializeOnLoadMethod]
    static void LoadInit() { var t = Instance; }

    Dictionary<string, Object> objectDic = new Dictionary<string, Object>();
    Dictionary<(string Path, System.Type Type), Object[]> dirDic = new Dictionary<(string Path, System.Type Type), Object[]>();

    public T GetResource<T>(string path) where T : Object
    {
        if (!objectDic.ContainsKey(path))
            objectDic[path] = Resources.Load<T>(path);        
        return (T)objectDic[path];
    }

    public T[] GetAllResource<T>(string folderPath) where T : Object
    {
        if (!dirDic.ContainsKey((folderPath, typeof(T))))
            dirDic[(folderPath, typeof(T))] = Resources.LoadAll<T>(folderPath);
        return (T[])dirDic[(folderPath, typeof(T))];
    }
}
