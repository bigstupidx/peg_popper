using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
    internal class DummyClient : IGoogleMobileAdsBannerClient
    {
        public DummyClient(IAdListener listener)
        {
            Debug.Log("Created DummyClient");
        }

        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
        {
            Debug.Log("Dummy CreateBannerView");
        }

        public void LoadAd(AdRequest request)
        {
            Debug.Log("Dummy LoadAd");
        }

        public void ShowBannerView()
        {
            Debug.Log("Dummy ShowBannerView");
        }

        public void HideBannerView()
        {
            Debug.Log("Dummy HideBannerView");
        }

		public void SetPosition(AdPosition position)
		{
			Debug.Log("Dummy SetPosition: " + (int)position );
		}

        public void DestroyBannerView()
        {
            Debug.Log("Dummy DestroyBannerView");
        }
    }

	internal class DummyInterstitialClient : IGoogleMobileAdsInterstitialClient 
	{
		public DummyInterstitialClient(IAdListener listener)
		{
			Debug.Log("Created DummyInterstitialClient");
		}
		
		public void CreateInterstitialAd(string adUnitId)
		{
			Debug.Log("Dummy CreateInterstitialAd");
		}
		
		public void LoadAd(AdRequest request)
		{
			Debug.Log("Dummy LoadAd");
		}
		
		public void ShowInterstitialAd()
		{
			Debug.Log("Dummy ShowInterstitial");
		}

		
		public bool IsLoaded()
		{
			Debug.Log("Dummy IsLoaded");
			return false;
		}

	}
}
