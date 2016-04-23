using UnityEngine;
using System;

public enum BannerPosition { Top, Bottom };

public class MMBannerView
{

	private const string BannerViewClass = "com.forwardthinking.unity.millennialads.BannerView";
	private const string BannerSizeClass = "com.forwardthinking.unity.millennialads.BannerSize";
	private const string RequestClass = "com.millennialmedia.android.MMRequest";

	private RequestListener listener = new RequestListener();
	
	private string apid;
	private BannerPosition position = BannerPosition.Top;
	private bool useLocation = false;

	private AndroidJavaObject bannerView;

	// Create a BannerView and add it into the view hierarchy.
	public MMBannerView(string apid, BannerPosition position, bool useLocation)
	{
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity =
			playerClass.GetStatic<AndroidJavaObject>("currentActivity");

		bannerView = new AndroidJavaObject(BannerViewClass, activity, listener);
	
		this.apid = apid;
		this.position = position;
		this.useLocation = useLocation;

	}

	// Create the bannerView.
	public void CreateAd()
	{
		AndroidJavaObject request = new AndroidJavaObject(RequestClass);

		AndroidJavaClass bannerSize = new AndroidJavaClass(BannerSizeClass);

		int width = bannerSize.GetStatic<int>("bannerWidth");
		int height = bannerSize.GetStatic<int>("bannerHeight");

	    bannerView.Call("create",
			new object[6] { apid, request, width, height, (int)position, useLocation});
	}

	// Hide the BannerView from the screen.
	public void Hide()
	{
	    bannerView.Call("hide");
	}

	// Show the BannerView on the screen.
	public void Show()
	{
		bannerView.Call("show");
	}

	// request a new ad to display
	public void GetNewAd() {
		bannerView.Call("newAd");
	}

	// Change the BannerView position.
	public void SetPosition(BannerPosition position)
	{
		bannerView.Call("setPosition", (int)position );
	}

	public void Destroy(){
		bannerView.Call("destroy");
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
