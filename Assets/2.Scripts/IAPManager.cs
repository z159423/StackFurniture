using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{

    IStoreController m_StoreController; // The Unity Purchasing system.
    IExtensionProvider m_Extension;

    //Your products IDs. They should match the ids of your products in your store.
    //com.TeroGames.spacesurvivor.
    public string removeAdsId = "noads";

    [Space]

    public GameObject[] removeAdsButtons;

    [field: SerializeField] public bool initialized { get; private set; } = false;

    public static IAPManager instance;

    private void Awake()
    {
        instance = this;

        if (!ES3.KeyExists("noads"))
            ES3.Save("noads", false);
    }

    private void Start()
    {
        //UpdateUI();

        InitializePurchasing();

        StartCoroutine(InitIAP());

        IEnumerator InitIAP()
        {
            while (true)
            {
                yield return null;

                print("iap 동기화 시도중");
                if (m_StoreController != null)
                {
                    initialized = true;
                    
                    break;
                }
                else
                {

                }
            }
        }

    }

    public void PurchaseRemoveAds()
    {
        m_StoreController.InitiatePurchase(removeAdsId);
    }

    public void PurchaseRemoveAds_Success()
    {
        ES3.Save("noads", true);

        FirebaseAnalytics.LogEvent("IAP_RemoveAdsPurchaseSuccess");

        CheckRemoveAdsHasPurchase();

        UpdateUI2();

        print("구매");
    }

    private void CheckRemoveAdsHasPurchase()
    {
        var product = m_StoreController.products.WithID(removeAdsId);

        if (!product.hasReceipt)
        {
            //PurchaseRemoveAds();
            print("스타터팩 이상을 구매하였고 광고제거를 구매한적이 없기 때문에 광고제거 상품도 함께 구매");
        }

        UpdateUI2();
    }



    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add products that will be purchasable and indicate its type.
        // builder.AddProduct(removeAdsId, ProductType.NonConsumable);
        // builder.AddProduct(starterPackId, ProductType.Consumable);
        // builder.AddProduct(megaPackId, ProductType.Consumable);
        // builder.AddProduct(ultraPackId, ProductType.Consumable);

        builder.AddProduct(removeAdsId, ProductType.NonConsumable,
        new IDs()
        {
            {"noads", GooglePlay.Name}
        });


        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");

        this.m_StoreController = controller;
        this.m_Extension = extensions;

        HadPurchased();
        UpdateUI2();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");

        print("IAP 연동 실패 + " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string s)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");

        print("IAP 연동 실패 + " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;

        //Add the purchased product to the players inventory

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    bool IsSubscribedTo(Product subscription)
    {
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (subscription.receipt == null)
        {
            return false;
        }

        //The intro_json parameter is optional and is only used for the App Store to get introductory information.
        var subscriptionManager = new SubscriptionManager(subscription, null);

        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }

    private void UpdateUI2()
    {
        var product = m_StoreController.products.WithID(removeAdsId);

        print(product.hasReceipt);

        if (ES3.Load<bool>("noads"))
        {
            foreach (GameObject btn in removeAdsButtons)
            {
                btn.SetActive(false);
            }

            print("광고 제거를 구매하였기 때문에 버튼 비활성화");
            BottomBanner.instance.DestoryBanner();
        }


    }

    /// <summary>
    /// 광고 제거 기능이 있는 iap를 구매한적 있는지 반환
    /// </summary>
    public bool HadPurchased()
    {
        var product = m_StoreController.products.WithID(removeAdsId);
        bool purchased = false;

        print(product.receipt);

        if (product.hasReceipt || ES3.Load<bool>("noads"))
        {
            print("광고제거를 구매한 적이 있는 유저");
            purchased = true;

            BottomBanner.instance.DestoryBanner();
        }

        return purchased;
    }
}
