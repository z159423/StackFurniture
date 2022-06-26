using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                rigid.isKinematic = true;
                isFreeze = true;
            }

            CheckChangePosition();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isTriggered && GameFlowController.instance.GetGameState())
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Furniture"))
            {
                gameObject.layer = LayerMask.NameToLayer("Object");

                GetComponent<Rigidbody>().useGravity = true;
                isTriggered = true;

                GameFlowController.instance.AddScore(1);

                GameFlowController.instance.CheckHeight();
                FurnitureSpawnManager.instance.SpawnNewFurniture();

                Vibration.Vibrate((long)50);

                AudioManager.instance.PlaySFX("Wood" + Random.Range(1, 4).ToString());
                //GetComponent<MeshCollider>().enabled = false;
            }
        }
    }

    private void CheckChangePosition()
    {
        if(lastPos != transform.localPosition)
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
