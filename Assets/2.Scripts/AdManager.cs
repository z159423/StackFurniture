using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using Firebase.Analytics;

public class AdManager : MonoBehaviour
{
    public int IrAdsCallDelay = 60;

    private int currentIrAdTimeTick = 0;

    private InterstitialAd interstitial;

    public bool IrTimeTicking = false;

    public static AdManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RequestInterstitial();

        StartCoroutine(TimeTick());

        IEnumerator TimeTick()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                if (IrTimeTicking)
                {
                    currentIrAdTimeTick++;
                }
            }
        }
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5179254807136480/1055155048";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd. 
        // 전면광고 초기화
        this.interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.  
        // 전면광고 요청
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        // 전면광고 로드      
        this.interstitial.LoadAd(request);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        void HandleOnAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("전면 광고 로드됨");

            FirebaseAnalytics.LogEvent("IrAdsLoadSuccess");


        }

        void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print("전면 광고 로드 실패: "
                                + args.LoadAdError);

            FirebaseAnalytics.LogEvent("IrAdsLoadFailed", "errorCode", "" + args.LoadAdError);
        }

        void HandleOnAdOpening(object sender, EventArgs args)
        {
            MonoBehaviour.print("전면 광고 실행중");

            currentIrAdTimeTick = 0;
            FirebaseAnalytics.LogEvent("IrAdsWatchingEvent");
        }

        void HandleOnAdClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("전면광고 꺼짐");

            RequestInterstitial();
            currentIrAdTimeTick = 0;
            FirebaseAnalytics.LogEvent("IrAdsClosedEvent");
        }
    }

    public void CallIrAds()
    {
        FirebaseAnalytics.LogEvent("IrAdsCallEvent");

        if (this.interstitial.IsLoaded() && currentIrAdTimeTick >= IrAdsCallDelay)
        {
            this.interstitial.Show();
        }
        else
        {
            if (!this.interstitial.IsLoaded())
            {
                print("광고가 로드되지 않았습니다");
                FirebaseAnalytics.LogEvent("IrAdsNotReadyEvent");

                RequestInterstitial();
            }

            if (currentIrAdTimeTick < IrAdsCallDelay)
            {
                FirebaseAnalytics.LogEvent("IrAdsNotTimeEnough");
            }

        }
    }
}
