using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Furniture : MonoBehaviour
{
    [SerializeField] private bool isTriggered = false;
    [SerializeField] private bool isFreeze = false;

    private Vector3 lastPos;
    private float freezeTime = 5;

    [Space]

    [SerializeField] private Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isTriggered && !isFreeze)
        {
            if (freezeTime < 0)
            {
                Freeze();
            }

            CheckChangePosition();
        }
    }

    public void Freeze()
    {
        rigid.isKinematic = true;
        isFreeze = true;

        AudioManager.instance.PlaySFX("pause", Random.Range(0.8f, 1.2f));

        var freeze = new GameObject();
        freeze.transform.position = GetComponentInParent<FurniturePivot>().transform.position;

        var child = Instantiate(GetComponentInParent<FurniturePivot>().gameObject, freeze.transform);

        child.GetComponentInChildren<MeshCollider>().enabled = false;
        child.GetComponentInChildren<Rigidbody>().isKinematic = true;
        child.GetComponentInChildren<Furniture>().enabled = false;

        foreach (var collider in child.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        child.transform.DOScale(child.transform.localScale * 1.5f, 0.7f).OnComplete(() => Destroy(freeze));

        var renderers = child.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            Material[] newMat = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                var color = renderer.materials[i].color;

                newMat[i] = Instantiate(Resources.Load<Material>("FadeMat"));

                newMat[i].color = color;

                // print(i);
                // child.GetComponentInChildren<MeshRenderer>().materials[i] = Instantiate(Resources.Load<Material>("FadeMat"));

                // child.GetComponentInChildren<MeshRenderer>().materials[i].color = color;
            }

            renderer.materials = newMat;


            foreach (var mat in renderer.materials)
            {
                // mat.SetFloat("_Mode", 2);

                // mat.renderQueue = 3000;

                // mat.color = mat.color;

                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.7f);

                mat.DOColor(new Color(mat.color.r, mat.color.g, mat.color.b, 0f), 0.7f);
                // mat.DOFade(0f, 0.7f);
            }
        }



        // ObjectPool.Instance.GetPool("FreezeParticle").GetComponent<FreezeParticle>().Freeze(GetComponentInChildren<MeshFilter>(), GetComponentInChildren<MeshCollider>(), transform.position, GetComponentInParent<FurniturePivot>().transform.rotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isTriggered && GameFlowController.instance.GetGameState() && !isFreeze)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Furniture"))
            {
                gameObject.layer = LayerMask.NameToLayer("Object");

                GetComponent<Rigidbody>().useGravity = true;
                isTriggered = true;

                GameFlowController.instance.AddScore(1);

                GameFlowController.instance.CheckHeight();
                FurnitureSpawnManager.instance.SpawnNewFurniture();

                // Vibration.Vibrate((long)50);
                MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);

                AudioManager.instance.PlaySFX("Wood" + Random.Range(1, 4).ToString(), Random.Range(0.8f, 1.2f));
                //GetComponent<MeshCollider>().enabled = false;
            }
        }
    }

    private void CheckChangePosition()
    {
        if (lastPos != transform.localPosition)
        {
            lastPos = transform.localPosition;
            freezeTime = 5;
        }
        else
        {
            freezeTime -= Time.deltaTime;
        }
    }
}
