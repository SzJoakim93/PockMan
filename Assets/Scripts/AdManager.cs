using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour {
    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;
    public UnityEvent OnAdLeavingApplicationEvent;
    public bool enableBanner;
    public bool enableInterstitial;
    public bool enableRewarded;
	private BannerView bannerView = null;
    private InterstitialAd interstitial = null;
    private RewardedAd rewardedAd = null;

	// Use this for initialization
	void Start () {
		// Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        if (enableBanner)
            this.RequestBanner();
        if (enableInterstitial)            
            this.RequestInterstitial();
        if (enableRewarded)
            this.CreateAndLoadRewardedAd();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool ShowInterstitialAd()
    {
        if (interstitial.IsLoaded()) {
            interstitial.Show();
            return true;
        }

        return false;
    }

    public bool UserChoseToWatchAd()
    {
        if (this.rewardedAd.IsLoaded()) {
            this.rewardedAd.Show();
            return true;
        }

        return false;
    }

    void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

	public void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
            Debug.Log("Android");
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
            Debug.Log("Iphone");
        #else
            string adUnitId = "unexpected_platform";
            Debug.Log("Unexpected");
        #endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

		 // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += (sender, args) => this.OnAdLoadedEvent.Invoke();
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += (sender, args) => this.OnAdFailedToLoadEvent.Invoke();
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += (sender, args) => this.OnAdOpeningEvent.Invoke();
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += (sender, args) => this.OnAdClosedEvent.Invoke();
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += (sender, args) => this.OnAdLeavingApplicationEvent.Invoke();

		// Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
                                .AddExtra("color_bg", "9B30FF")
                                .Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void RequestInterstitial()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += (sender, args) => this.OnAdLoadedEvent.Invoke();
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += (sender, args) => this.OnAdFailedToLoadEvent.Invoke();
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += (sender, args) => this.OnAdOpeningEvent.Invoke();
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += (sender, args) => this.OnAdClosedEvent.Invoke();
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += (sender, args) => this.OnAdLeavingApplicationEvent.Invoke();

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

	public void CreateAndLoadRewardedAd() {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
            Debug.Log("Android");
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
            Debug.Log("Iphone");
        #else
            string adUnitId = "unexpected_platform";
            Debug.Log("Unexpected");
        #endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += (sender, args) => this.OnAdLoadedEvent.Invoke();
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += (sender, args) => this.OnAdFailedToLoadEvent.Invoke();
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += (sender, args) => this.OnAdOpeningEvent.Invoke();
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += (sender, args) => this.OnAdFailedToLoadEvent.Invoke();
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += (sender, args) => this.OnUserEarnedRewardEvent.Invoke();
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += (sender, args) => this.OnAdClosedEvent.Invoke();

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }
}
