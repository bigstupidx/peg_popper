using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdMobInterstitialScript: MonoBehaviour {
	public bool returnToScene = true;
	
    private InterstitialAd interstitialAd = null;

	// interstitial
	public bool shown = false;

	void Awake(){
		AdMobInterstitialScript[] objects = FindObjectsOfType<AdMobInterstitialScript>();
		if (objects.Length > 1)
		{
			// destroy the existing one
			DestroyImmediate(objects[objects.Length-1].gameObject);
		}
	}

    void Start()
    {
		DontDestroyOnLoad(gameObject);

        #if UNITY_EDITOR
            string adUnitId = "unused";
        #elif UNITY_ANDROID
			string adUnitId = "ca-app-pub-7789759883918325/1119867695";
		#elif UNITY_IPHONE
            string adUnitId = "INSERT_YOUR_IOS_AD_UNIT_HERE";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
		interstitialAd = new InterstitialAd(adUnitId);
        // Register for ad events.
        interstitialAd.AdLoaded += HandleAdLoaded;
		interstitialAd.AdFailedToLoad += HandleAdFailedToLoad;
		interstitialAd.AdOpened += HandleAdOpened;
		interstitialAd.AdClosing += HandleAdClosing;
		interstitialAd.AdClosed += HandleAdClosed;
		interstitialAd.AdLeftApplication += HandleAdLeftApplication;

		// Request a banner ad, with optional custom ad targeting.
		AdRequest request = new AdRequest.Builder()
			.AddTestDevice("719B63AE6F8DED7AFB9B7D38EEFAF5A8")
			.Build();
		interstitialAd.LoadAd(request);
    }

	public void LoadAd(AdRequest request) {
		interstitialAd.LoadAd(request);
	}

    public void ShowAd() {
        interstitialAd.Show();
		returnToScene = false;
    }

    public bool IsLoaded() {
		bool loaded = interstitialAd.IsLoaded();
		print("INT isLoaded = " + loaded);
		if (!loaded) returnToScene = true;
        return loaded;
    }	

    #region Banner callback handlers

    public void HandleAdLoaded()
    {
        print("HandleAdLoaded INT event received.");
    }

    public void HandleAdFailedToLoad(string message)
    {
        print("HandleFailedToReceiveAd INT event received with message: " + message);
		returnToScene = true;
    }

    public void HandleAdOpened()
    {
        print("HandleAdOpened INT event received");
		returnToScene = true;
    }

    void HandleAdClosing ()
    {
        print("HandleAdClosing INT event received");
		returnToScene = true;
    }

    public void HandleAdClosed()
    {
        print("HandleAdClosed INT event received");
		returnToScene = true;
    }

    public void HandleAdLeftApplication()
    {
        print("HandleAdLeftApplication INT event received");
		returnToScene = true;
    }

    #endregion
}
