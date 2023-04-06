using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class OceanDetetingCollider : MonoBehaviour
{
    [SerializeField] private GameFlowController gameFlowController;

    public UnityEvent furnitureFallInOceanEvent;

    private GameObject triggerPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (gameFlowController.gameStarted)
        {
            if (other.CompareTag("Furniture"))
            {
                if (triggerPoint != null)
                    Destroy(triggerPoint);

                triggerPoint = new GameObject("triggerPoint");

                triggerPoint.transform.position = other.ClosestPoint(transform.position);

                CameraManager.instance.ChangeCameraTarget(triggerPoint.transform);

                // if (gameFlowController.isRevivedInThisStage)
                // furnitureFallInOceanEvent.Invoke();
                // else

                gameFlowController.TriggerInWater();
                FurnitureSpawnManager.instance.TriggerInWater();

                AudioManager.instance.PlaySFX("Water" + Random.Range(1, 3).ToString());
            }
        }
    }
}
