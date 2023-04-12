using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class BottomBanner : MonoBehaviour
{
    public static BottomBanner instance;

    private void Awake()
    {
        instance = this;
    }

    private BannerView bannerView;

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        // if (!IAPManager.instance.HadPurchased())
        //     this.RequestBanner();
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5179254807136480/4994400059";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void DestoryBanner()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }
}
