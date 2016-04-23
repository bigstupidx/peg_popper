#if UNITY_ANDROID

using UnityEngine;
using System;

public class RequestListener : AndroidJavaProxy
{
	private const string UnityRequestListenerClass = "com.forwardthinking.unity.millennialads.UnityRequestListener";

	// These are the ad callback events that can be hooked into.
	public event Action OnAdLaunched = delegate {};
	public event Action OnAdClosed = delegate {};
	public event Action OnRequestIsCaching = delegate {};
	public event Action OnRequestCompleted = delegate {};
	public event Action<string> OnRequestFailed = delegate {};
	public event Action OnSingleTap = delegate {};

	internal RequestListener() :
		base(UnityRequestListenerClass)
	{
	}
	
	void adLaunched(){
		OnAdLaunched();
	}
	void adClosed(){
		OnAdClosed();
	}
	void requestIsCaching(){
		OnRequestIsCaching();
	}
	void requestCompleted()	{
		OnRequestCompleted();
	}
	void requestFailed(string exception){
		OnRequestFailed(exception);
	}
	void onSingleTap(){
		OnSingleTap();
	}
}


#endif
