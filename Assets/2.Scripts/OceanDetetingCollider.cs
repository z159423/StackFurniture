using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OceanDetetingCollider : MonoBehaviour
{
    [SerializeField] private GameFlowController gameFlowController;

    public UnityEvent furnitureFallInOceanEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(gameFlowController.gameStarted)
        {
            if (other.CompareTag("Furniture"))
            {
                furnitureFallInOceanEvent.Invoke();

                AudioManager.instance.PlaySFX("Water" + Random.Range(1, 3).ToString());
            }
        }
    }
}
