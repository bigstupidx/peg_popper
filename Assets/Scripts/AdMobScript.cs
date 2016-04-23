using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdMobScript : MonoBehaviour {

	public bool top = false;
	public bool destroy = false;
	
    private BannerView bannerView = null;

	// interstitial
	public bool shown = false;

	void Awake()
	{
		// this is a hack to prevent the ad from being destroyed when
		// the scene is reloaded and ensures that only one exists
		// And makes sure that it is the original one on a reload but
		// a newly created one on loading a new scene
		AdMobScript[] objects = FindObjectsOfType<AdMobScript>();
		if (objects.Length > 1)
		{
			// destroy the existing one
			if(destroy){
				DestroyImmediate(objects[0].gameObject);
			}
			else{
				DestroyImmediate(objects[objects.Length-1].gameObject);
			}
		}
	}

    void Start()
    {
        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
			string adUnitId = "ca-app-pub-7789759883918325/7598660494";
		#elif UNITY_IPHONE
            string adUnitId = "INSERT_YOUR_IOS_AD_UNIT_HERE";
        #else
            string adUnitId = "unexpected_platform";
        #endif

		AdPosition position = top?AdPosition.Top:AdPosition.Bottom;

        // Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, position);
        // Register for ad events.
        bannerView.AdLoaded += HandleAdLoaded;
        bannerView.AdFailedToLoad += HandleAdFailedToLoad;
        bannerView.AdOpened += HandleAdOpened;
        bannerView.AdClosing += HandleAdClosing;
        bannerView.AdClosed += HandleAdClosed;
        bannerView.AdLeftApplication += HandleAdLeftApplication;

		RequestBanner();
		ShowBanner();

		if(!destroy){
			DontDestroyOnLoad(gameObject);
		}
    }

    void RequestBanner() {
        // Request a banner ad, with optional custom ad targeting.
		AdRequest request = new AdRequest.Builder()
			.AddTestDevice("719B63AE6F8DED7AFB9B7D38EEFAF5A8")
			.Build();
        bannerView.LoadAd(request);
    }

    public void ShowBanner() {
        bannerView.Show();
    }

    public void HideBanner() {
        bannerView.Hide();
    }

	public void SetPosition(AdPosition position){
		bannerView.SetPosition(position);
	}

	void OnDestroy(){
		if (bannerView != null) bannerView.Destroy();
	}

    #region Banner callback handlers

    public void HandleAdLoaded()
    {
        print("HandleAdLoaded event received.");
    }

    public void HandleAdFailedToLoad(string message)
    {
        print("HandleFailedToReceiveAd event received with message: " + message);
    }

    public void HandleAdOpened()
    {
        print("HandleAdOpened event received");
    }

    void HandleAdClosing ()
    {
        print("HandleAdClosing event received");
    }

    public void HandleAdClosed()
    {
        print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication()
    {
        print("HandleAdLeftApplication event received");
    }

    #endregion
}
