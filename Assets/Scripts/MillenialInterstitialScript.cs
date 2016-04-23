using System;
using UnityEngine;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class MillenialInterstitialScript : MonoBehaviour {
	public bool destroy = false;
	public bool useLocation = true;
	public bool returnToScene = true;
	
    private MMInterstitial interstitialView = null;
	
	void Awake()
	{
		// this is a hack to prevent the ad from being destroyed when
		// the scene is reloaded and ensures that only one exists
		// And makes sure that it is the original one on a reload but
		// a newly created one on loading a new scene
		MillenialInterstitialScript[] objects = FindObjectsOfType<MillenialInterstitialScript>();
		if (objects.Length > 1)
		{
			// destroy the new one
			if(destroy){
				DestroyImmediate(objects[objects.Length-1].gameObject);

			}
			else{
				DestroyImmediate(objects[0].gameObject);
			}
		}
	}

    void Start()
    {
		string apid = "161402";

        // Create the interstitial ad
		interstitialView = new MMInterstitial(apid, useLocation);

        // Register for ad events.
		interstitialView.SetOnAdLaunched(HandleAdLaunched);
		interstitialView.SetOnAdClosed(HandleAdClosed);
		interstitialView.SetOnRequestIsCaching(HandleRequestIsCaching);
		interstitialView.SetOnRequestCompleted(HandleRequestCompleted);
		interstitialView.SetOnRequestFailed(HandleRequestFailed);
		interstitialView.SetOnSingleTap(HandleSingleTap);

		CreateAd();

		if(!destroy){
			DontDestroyOnLoad(gameObject);
		}
    }
	
	public void CreateAd() {
        interstitialView.CreateAd();
		interstitialView.CacheAd();
    }

	public void CacheAd() {
		interstitialView.CacheAd();
	}

	public bool IsLoaded() {
		return interstitialView.IsLoaded();
	}

    public void ShowInterstitial() {
		interstitialView.Show();
		returnToScene = false;
    }

	void OnDestroy() {
		if (interstitialView != null) interstitialView.Destroy();
	}

    #region Banner callback handlers
	public void HandleAdLaunched()
    {
		print("HandleAdLaunched Interstitial event received");
    }

    public void HandleAdClosed()
    {
		print("HandleAdClosed Interstitial event received");
		returnToScene = true;
    }

	public void HandleRequestIsCaching()
    {
		print("HandleRequestIsCaching Interstitial event received");
    }

	public void HandleRequestCompleted()
	{
		print("HandleRequestCompleted Interstitial event received.");
	}
	
	public void HandleRequestFailed(string message)
	{
		print("HandleRequestFailed Interstitial event received with message: " + message);
	}

	public void HandleSingleTap()
	{
		print("HandleSingleTap Interstitial event received");
	}
    #endregion
}
