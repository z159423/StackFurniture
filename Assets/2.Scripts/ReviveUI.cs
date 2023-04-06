using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReviveUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image slider;

    private void Start()
    {
        canvasGroup.DOFade(2, 2f).SetEase(Ease.Linear);
        slider.DOFillAmount(1f, 4.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameFlowController.instance.GameEnd();
                FurnitureSpawnManager.instance.GameEnd();
                Destroy(gameObject);
            });
    }

    public void OnClickReviveBtn()
    {
        AdManager.CallRV(() =>
        {
            GameFlowController.instance.Revive();
            FurnitureSpawnManager.instance.Revive();
            Destroy(gameObject);
        });
    }
}
