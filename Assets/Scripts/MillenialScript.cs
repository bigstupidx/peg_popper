using System;
using UnityEngine;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class MillenialScript : MonoBehaviour {
	public BannerPosition position = BannerPosition.Bottom;
	public bool destroy = false;
	public bool useLocation = true;
	
	public float bannerDuration = 60.0f;
	private float currentBannerTime = 0.0f;
	
    private MMBannerView bannerView = null;
	
	void Awake()
	{
		// this is a hack to prevent the ad from being destroyed when
		// the scene is reloaded and ensures that only one exists
		// And makes sure that it is the original one on a reload but
		// a newly created one on loading a new scene
		MillenialScript[] objects = FindObjectsOfType<MillenialScript>();
		if (objects.Length > 1)
		{
			// destroy the new one
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
		string apid = "161400";

        // Create a 320x50 banner at the top of the screen.
		bannerView = new MMBannerView(apid, position, useLocation);

        // Register for ad events.
		bannerView.SetOnAdLaunched(HandleAdLaunched);
		bannerView.SetOnAdClosed(HandleAdClosed);
		bannerView.SetOnRequestIsCaching(HandleRequestIsCaching);
        bannerView.SetOnRequestCompleted(HandleRequestCompleted);
        bannerView.SetOnRequestFailed(HandleRequestFailed);
		bannerView.SetOnSingleTap(HandleSingleTap);

		CreateBanner();
		ShowBanner();

		if(!destroy){
			DontDestroyOnLoad(gameObject);
		}
    }

	public void Update() {
		currentBannerTime += Time.deltaTime;
		if (currentBannerTime >= bannerDuration) {
			currentBannerTime = 0.0f;
			bannerView.GetNewAd();
		}
	}
	void CreateBanner() {
        bannerView.CreateAd();
    }

    public void ShowBanner() {
        bannerView.Show();
    }

    public void HideBanner() {
        bannerView.Hide();
    }

	public void SetPosition(BannerPosition position) {
		bannerView.SetPosition(position);
	}

	void OnDestroy() {
		if (bannerView != null) {
			bannerView.Destroy();
		}
	}

    #region Banner callback handlers
	public void HandleAdLaunched()
    {
		print("HandleAdLaunched event received");
    }

    public void HandleAdClosed()
    {
        print("HandleAdClosed event received");
    }

	public void HandleRequestIsCaching()
    {
		print("HandleRequestIsCaching event received");
    }

	public void HandleRequestCompleted()
	{
		print("HandleRequestCompleted event received.");
	}
	
	public void HandleRequestFailed(string message)
	{
		print("HandleRequestFailed event received with message: " + message);
	}

	public void HandleSingleTap()
	{
		print("HandleSingleTap event received");
	}
    #endregion
}
