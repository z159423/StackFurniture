using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FreezeParticle : MonoBehaviour
{
    [SerializeField] private MeshFilter filter;

    [SerializeField] private MeshRenderer renderer;

    public void Freeze(MeshFilter filter, MeshCollider collider, Vector3 pos, Quaternion rotation)
    {
        this.filter.mesh = filter.sharedMesh;

        transform.position = collider.bounds.center;

        this.filter.transform.localScale = Vector3.one;
        this.filter.transform.localPosition = Vector3.zero;
        this.filter.transform.rotation = rotation;

        this.filter.transform.Translate((this.filter.transform.position - this.filter.mesh.bounds.center));

        renderer.material.DOFade(0, 0.7f).OnStart(() => renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1f));
        transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.7f).SetEase(Ease.Linear).OnComplete(() => ObjectPool.Instance.AddPool(gameObject, 0));
    }
}
