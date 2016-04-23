using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common {
    internal interface IGoogleMobileAdsInterstitialClient {
        // Create an interstitial view and add it into the view hierarchy.
        void CreateInterstitialAd(string adUnitId);

        // Request a new ad for the interstitial view.
        void LoadAd(AdRequest request);

        // Show the interstitial view on the screen.
        void ShowInterstitialAd();

        // Whether or not the interstitial has loaded yet.
        bool IsLoaded();
    }
}
