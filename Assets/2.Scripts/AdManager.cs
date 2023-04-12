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

    private static string RvAdsId = "ca-app-pub-5179254807136480/4580623626";
    private static RewardedAd rewardedAd;

    public bool IrTimeTicking = false;

    public static AdManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CreateAndLoadRewardedAd();
        RequestInterstitial();
    }

    public void TimeTick()
    {
        WaitForSeconds sec = new WaitForSeconds(1);

        StartCoroutine(TimeTick());

        IEnumerator TimeTick()
        {
            while (true)
            {
                yield return sec;

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

    public static void CreateAndLoadRewardedAd()
    {

        string adUnitId;

#if UNITY_ANDROID
        adUnitId = RvAdsId;
#elif UNITY_IPHONE
             adUnitId = iosAdUnitId;
#else
             adUnitId = "unexpected_platform";
#endif

        rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        //rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        rewardedAd.LoadAd(request);

        //보상형 광고가 완료되었을때
        void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("보상형 광고를 로드함");

            FirebaseAnalytics.LogEvent("RvAdsLoadSuccess");
        }

        //보상형 광고 로드 실패함
        void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print(
                "보상형 광고 로드를 실패하였습니다: "
                                 + args.LoadAdError);

            FirebaseAnalytics.LogEvent("RvAdsLoadFailed", "errorCode", "" + args.LoadAdError);
        }

        //보상형 광고 표시중
        void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            MonoBehaviour.print("보상형 광고 표시중");
        }

        //보상형 광고 표시가 실패하였습니다.
        void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            MonoBehaviour.print(
                "광고 표시를 실패하였습니다: "
                                 + args.AdError.GetMessage());
        }

        //사용자가 보상형 광고를 취소하였을때
        void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            CreateAndLoadRewardedAd();
            MonoBehaviour.print("사용자가 보상형 광고 시청을 취소하였습니다.");
        }

        //보상형 광고를 시청하고 보상을 받아야 할때 실행
        void HandleUserEarnedReward(object sender, Reward args)
        {

        }
    }

    public void CallIrAds()
    {
        if (IAPManager.instance.HadPurchased())
            return;

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

    public static void CallRV(System.Action reward)
    {
        FirebaseAnalytics.LogEvent("RvAdsCallEvent");


        if (IAPManager.instance.HadPurchased())
            reward.Invoke();
        else if (rewardedAd.IsLoaded())
        {
            FirebaseAnalytics.LogEvent("RvAdsCallSuccess");

            //rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

            void HandleUserEarnedReward(object sender, Reward args)
            {
                reward.Invoke();

                rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
                CreateAndLoadRewardedAd();
            }

            rewardedAd.Show();
        }
        else
        {
            // if (UserDataManager.instance.currentUserData.RemoveAds)
            // {
            //     print("광고 제거를 구매해 광고 호출을 안함");
            // }
            // else 
            if (!rewardedAd.IsLoaded())
                print("광고가 없습니다");
            else
                print("알수없는 이유로 광고 호출에 실패하였습니다.");

            FirebaseAnalytics.LogEvent("RvAdsCallFailed");

            CreateAndLoadRewardedAd();
        }
    }
}
