using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

/*public class GoogleMobileAdsDemoHandler : IDefaultInAppPurchaseProcessor
{
    private readonly string[] validSkus = { "android.test.purchased" };

    // Will only be sent on a success.
    public void ProcessCompletedInAppPurchase(IInAppPurchaseResult result)
    {
        result.FinishPurchase();
        AdManager.OutputMessage = "Purchase Succeeded! Credit user here.";
    }

    // Check SKU against valid SKUs.
    public bool IsValidPurchase(string sku)
    {
        foreach (string validSku in this.validSkus)
        {
            if (sku == validSku)
            {
                return true;
            }
        }

        return false;
    }

    // Return the app's public key.
    public string AndroidPublicKey
    {
        // In a real app, return public key instead of null.
        get { return null; }
    }
}*/

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdManager : MonoBehaviour
{
    public string bannerID = "ca-app-pub-5152976146729157/7439689029";
    private BannerView bannerView;

    //private RewardBasedVideoAd rewardBasedVideo;
    private float deltaTime = 0.0f;
    //private static string outputMessage = string.Empty;
    saveLoad SaveLoad;
    public static string OutputMessage
    {
        set
        { //outputMessage = value; }
        }
    }

    public void Awake()
    {
        DontDestroyOnLoad(this);
        SaveLoad = GameObject.Find("SaveLoad").GetComponent<saveLoad>();
    }
    public void Start()
    {
            this.RequestBanner();
    }
    public void Update()
    {
        // Calculate simple moving average for time to render screen. 0.1 factor used as smoothing
        // value.
        this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
    }

    public AdRequest CreateAdRequest()
    {
        Debug.Log("requested");
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }
    
    public void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
        Debug.Log("BannerCalled");
#elif UNITY_ANDROID
        string adUnitId = bannerID;
#elif UNITY_IPHONE
        string adUnitId = bannerID;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner,AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleAdOpened;
        this.bannerView.OnAdClosed += this.HandleAdClosed;
        this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        // Load a banner ad.
        this.bannerView.LoadAd(CreateAdRequest());
        
    }

   
    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

}
