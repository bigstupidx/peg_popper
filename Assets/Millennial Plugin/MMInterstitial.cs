using UnityEngine;
using System;

public class MMInterstitial
{

	private const string InterstitialViewClass = "com.forwardthinking.unity.millennialads.InterstitialView";
	private const string RequestClass = "com.millennialmedia.android.MMRequest";

	private RequestListener listener = new RequestListener();
	
	private string apid;
	private bool useLocation = false;

	private AndroidJavaObject interstitialView;

	// Create a BannerView and add it into the view hierarchy.
	public MMInterstitial(string apid, bool useLocation)
	{
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity =
			playerClass.GetStatic<AndroidJavaObject>("currentActivity");

		interstitialView = new AndroidJavaObject(InterstitialViewClass, activity, listener);
	
		this.apid = apid;
		this.useLocation = useLocation;

	}

	// Create the Interstitial.
	public void CreateAd()
	{
		AndroidJavaObject request = new AndroidJavaObject(RequestClass);

		interstitialView.Call("create",
			new object[3] { apid, request, useLocation});
	}

	public void CacheAd(){
		interstitialView.Call("cache");
	}

	// Show the Interstitial on the screen.
	public void Show()
	{
		interstitialView.Call("show");
	}

	public bool IsLoaded() {
		return interstitialView.Call<bool>("isLoaded");
	}

	public void Destroy(){
		interstitialView.Call("destroy");
	}
	
	public void SetOnAdLaunched(Action func){
		listener.OnAdLaunched += func;
	}
	public void SetOnAdClosed(Action func){
		listener.OnAdClosed += func;
	}
	public void SetOnRequestIsCaching(Action func){
		listener.OnRequestIsCaching += func;
	}
	public void SetOnRequestCompleted(Action func)	{
		listener.OnRequestCompleted += func;
	}
	public void SetOnRequestFailed(Action<string> func){
		listener.OnRequestFailed += func;
	}
	public void SetOnSingleTap(Action func){
		listener.OnSingleTap += func;
	}

}
