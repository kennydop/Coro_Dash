using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AdsManager : MonoBehaviour
{
    public Button RewardButton, ContinueButton;
    public bool focused;
    bool GameOverNativeAd = false, PauseNativeAd = false, TryHarderNativeAd = false, MorePointsNativeAd = false, nowstartingcontinue, nowstartingrewarded;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private RewardedAd continueAd;
    private UnifiedNativeAd GameOverNative;
    private UnifiedNativeAd PauseNative;
    private UnifiedNativeAd TryHarderNative;
    private UnifiedNativeAd MorePointsNative;
    public List<GameObject> GameOverOfflineNativeAds;
    public List<GameObject> PauseOfflineNativeAds;
    public List<GameObject> MorePointsOfflineNativeAds;
    public List<GameObject> OfflineInterstitialAds;

    string adUnitId = "ca-app-pub-3940256099942544/5224354917";
    string IadUnitId = "ca-app-pub-3940256099942544/1033173712";
    string idNative = "ca-app-pub-3940256099942544/2247696110";
    //string appId = "ca-app-pub-7476823858135442~5010699318";
    #region GameOver Native

    public GameObject GameOverAd_Holder;
    public RawImage GameOverAd_Icon, GameOverAd_Choices;
    public Text GameOverAd_CallToAction, GameOverAd_Advertiser, GameOverAd_Headline;

    #endregion

    #region Pause Native

    public GameObject PauseAd_Holder;
    public RawImage PauseAd_Icon, PauseAd_Choices;
    public Text PauseAd_CallToAction, PauseAd_Advertiser, PauseAd_Headline;

    #endregion

    #region TryHarder Native

    public GameObject TryHarderAd_Holder;
    public RawImage TryHarderAd_Icon, TryHarderAd_Choices;
    public Text TryHarderAd_CallToAction, TryHarderAd_Advertiser, TryHarderAd_Headline;

    #endregion

    #region MorePoints Native

    public GameObject MorePointsAd_Holder;
    public RawImage MorePointsAd_Icon, MorePointsAd_Choices;
    public Text MorePointsAd_CallToAction, MorePointsAd_Advertiser, MorePointsAd_Headline;

    #endregion
    void Awake()
    {
        GameOverAd_Holder.SetActive(false);
        PauseAd_Holder.SetActive(false);
        TryHarderAd_Holder.SetActive(false);
        MorePointsAd_Holder.SetActive(false);
    }

    public void Start()
    {
        nowstartingrewarded = true;
        nowstartingcontinue = true;
        MobileAds.Initialize(initStatus => { });
        this.rewardedAd = new RewardedAd(adUnitId);
        this.continueAd = new RewardedAd(adUnitId);
        LoadRewardedAd();
        LoadContinueAd();

        foreach (var f in OfflineInterstitialAds)
        {
            f.SetActive(false);
        }

        if (GameManager.Instance.removedAds)
            return;

        StartOfflineAds();
        LoadInterstitialAd();
        RequestPauseNativeAd();
        RequestGameOverNativeAd();
        RequestTryHarderNativeAd();
        RequestMorePointsNativeAd();

    }

    void CheckPauseNative()
    {
        if (PauseNativeAd)
        {
            PauseNativeAd = false;

            Texture2D iconTexture = this.PauseNative.GetIconTexture();
            Texture2D iconAdChoices = this.PauseNative.GetAdChoicesLogoTexture();
            string headline = this.PauseNative.GetHeadlineText();
            string cta = this.PauseNative.GetCallToActionText();
            string advertiser = this.PauseNative.GetAdvertiserText();
            PauseAd_Icon.texture = iconTexture;
            PauseAd_Choices.texture = iconAdChoices;
            PauseAd_Headline.text = headline;
            PauseAd_Advertiser.text = advertiser;
            PauseAd_CallToAction.text = cta;

            //register gameobjects
            PauseNative.RegisterIconImageGameObject(PauseAd_Icon.gameObject);
            PauseNative.RegisterAdChoicesLogoGameObject(PauseAd_Choices.gameObject);
            PauseNative.RegisterHeadlineTextGameObject(PauseAd_Headline.gameObject);
            PauseNative.RegisterCallToActionGameObject(PauseAd_CallToAction.gameObject);
            PauseNative.RegisterAdvertiserTextGameObject(PauseAd_Advertiser.gameObject);

            DisablePauseOfflineNativeAds();
            PauseAd_Holder.SetActive(true); //show ad panel
        }
    }
    void CheckGameOvernative()
    {
        if (GameOverNativeAd)
        {
            GameOverNativeAd = false;

            Texture2D iconTexture = this.GameOverNative.GetIconTexture();
            Texture2D iconAdChoices = this.GameOverNative.GetAdChoicesLogoTexture();
            string headline = this.GameOverNative.GetHeadlineText();
            string cta = this.GameOverNative.GetCallToActionText();
            string advertiser = this.GameOverNative.GetAdvertiserText();
            GameOverAd_Icon.texture = iconTexture;
            GameOverAd_Choices.texture = iconAdChoices;
            GameOverAd_Headline.text = headline;
            GameOverAd_Advertiser.text = advertiser;
            GameOverAd_CallToAction.text = cta;

            //register gameobjects
            GameOverNative.RegisterIconImageGameObject(GameOverAd_Icon.gameObject);
            GameOverNative.RegisterAdChoicesLogoGameObject(GameOverAd_Choices.gameObject);
            GameOverNative.RegisterHeadlineTextGameObject(GameOverAd_Headline.gameObject);
            GameOverNative.RegisterCallToActionGameObject(GameOverAd_CallToAction.gameObject);
            GameOverNative.RegisterAdvertiserTextGameObject(GameOverAd_Advertiser.gameObject);

            DisableGameOverOfflineNativeAds();
            GameOverAd_Holder.SetActive(true); //show ad panel
        }
    }
    void CheckTryHarderNavite()
    {
        if (TryHarderNativeAd)
        {
            TryHarderNativeAd = false;

            Texture2D iconTexture = this.TryHarderNative.GetIconTexture();
            Texture2D iconAdChoices = this.TryHarderNative.GetAdChoicesLogoTexture();
            string headline = this.TryHarderNative.GetHeadlineText();
            string cta = this.TryHarderNative.GetCallToActionText();
            string advertiser = this.TryHarderNative.GetAdvertiserText();
            TryHarderAd_Icon.texture = iconTexture;
            TryHarderAd_Choices.texture = iconAdChoices;
            TryHarderAd_Headline.text = headline;
            TryHarderAd_Advertiser.text = advertiser;
            TryHarderAd_CallToAction.text = cta;

            //register gameobjects
            TryHarderNative.RegisterIconImageGameObject(TryHarderAd_Icon.gameObject);
            TryHarderNative.RegisterAdChoicesLogoGameObject(TryHarderAd_Choices.gameObject);
            TryHarderNative.RegisterHeadlineTextGameObject(TryHarderAd_Headline.gameObject);
            TryHarderNative.RegisterCallToActionGameObject(TryHarderAd_CallToAction.gameObject);
            TryHarderNative.RegisterAdvertiserTextGameObject(TryHarderAd_Advertiser.gameObject);

            TryHarderAd_Holder.SetActive(true);
        }
    }
    void CheckMorePointsNative()
    {
        if (MorePointsNativeAd)
        {
            MorePointsNativeAd = false;

            Texture2D iconTexture = this.MorePointsNative.GetIconTexture();
            Texture2D iconAdChoices = this.MorePointsNative.GetAdChoicesLogoTexture();
            string headline = this.MorePointsNative.GetHeadlineText();
            string cta = this.MorePointsNative.GetCallToActionText();
            string advertiser = this.MorePointsNative.GetAdvertiserText();
            MorePointsAd_Icon.texture = iconTexture;
            MorePointsAd_Choices.texture = iconAdChoices;
            MorePointsAd_Headline.text = headline;
            MorePointsAd_Advertiser.text = advertiser;
            MorePointsAd_CallToAction.text = cta;

            //register gameobjects
            MorePointsNative.RegisterIconImageGameObject(TryHarderAd_Icon.gameObject);
            MorePointsNative.RegisterAdChoicesLogoGameObject(TryHarderAd_Choices.gameObject);
            MorePointsNative.RegisterHeadlineTextGameObject(TryHarderAd_Headline.gameObject);
            MorePointsNative.RegisterCallToActionGameObject(TryHarderAd_CallToAction.gameObject);
            MorePointsNative.RegisterAdvertiserTextGameObject(TryHarderAd_Advertiser.gameObject);

            DisableMorePointsOfflineNativeAds();
            MorePointsAd_Holder.SetActive(true);
        }
    } 


    #region Loading Ads
    void LoadInterstitialAd()
    {
        this.interstitial = new InterstitialAd(IadUnitId);
        // Create an empty ad request.
        AdRequest irequest = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(irequest);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }
    public void LoadRewardedAd()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        if (nowstartingrewarded)
            nowstartingrewarded = false;
    }
    public void LoadContinueAd()
    {
        // Create an empty ad request.
            AdRequest crequest = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.continueAd.LoadAd(crequest);

        // Called when an ad request has successfully loaded.
        this.continueAd.OnAdLoaded += HandleContinueAdLoaded;
        // Called when an ad request failed to load.
        this.continueAd.OnAdFailedToLoad += HandleContinueAdFailedToLoad;
        // Called when an ad is shown.
        this.continueAd.OnAdOpening += HandleContinueAdOpening;
        // Called when an ad request failed to show.
        this.continueAd.OnAdFailedToShow += HandleContinueAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.continueAd.OnUserEarnedReward += HandleUserCanContinue;
        // Called when the ad is closed.
        this.continueAd.OnAdClosed += HandleContinueAdClosed;

        if (nowstartingcontinue)
            nowstartingcontinue = false;
    }
    private void RequestGameOverNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(idNative)
            .ForUnifiedNativeAd()
            .Build();
        adLoader.OnUnifiedNativeAdLoaded += this.HandleUnifiedNativeAdLoaded;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }
    public void RequestPauseNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(idNative)
            .ForUnifiedNativeAd()
            .Build();
        adLoader.OnUnifiedNativeAdLoaded += this.pHandleUnifiedNativeAdLoaded;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }
    public void RequestTryHarderNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(idNative)
            .ForUnifiedNativeAd()
            .Build();
        adLoader.OnUnifiedNativeAdLoaded += this.thHandleUnifiedNativeAdLoaded;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }
    public void RequestMorePointsNativeAd()
    {
        AdLoader adLoader = new AdLoader.Builder(idNative)
            .ForUnifiedNativeAd()
            .Build();
        adLoader.OnUnifiedNativeAdLoaded += this.mpHandleUnifiedNativeAdLoaded;
        adLoader.LoadAd(new AdRequest.Builder().Build());
    }
    
    #endregion

    #region Show Ads
    private void ShowRewardedAd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else
        {
            ConnectionToast();
        }
    }
    private void ShowContinuedAd()
    {
        if (this.continueAd.IsLoaded())
        {
            this.continueAd.Show();
        }
        else
        {
            ConnectionToast();
        }
    }
    public void ShowInterstitialAd()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            ShowOfflineInterstitialAd();
        }
    }
    #endregion

    #region Ads Methods

    #region Interstitial Ad Methods
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {

    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {

    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        this.interstitial.OnAdLoaded -= HandleOnAdLoaded;
        this.interstitial.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        this.interstitial.OnAdOpening -= HandleOnAdOpened;
        this.interstitial.OnAdClosed -= HandleOnAdClosed;
        this.interstitial.OnAdLeavingApplication -= HandleOnAdLeavingApplication;
        LoadInterstitialAd();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        ShowInterstitialAd();
    }
    #endregion

    #region Rewarded Ad Methods
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        RewardButton.interactable = true;
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        LoadRewardedAd();
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Time.timeScale = 0;
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        RewardButton.interactable = true;
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Time.timeScale = 1;
        LoadRewardedAd();
        this.rewardedAd.OnAdLoaded -= HandleRewardedAdLoaded;
        this.rewardedAd.OnAdFailedToLoad -= HandleRewardedAdFailedToLoad;
        this.rewardedAd.OnAdOpening -= HandleRewardedAdOpening;
        this.rewardedAd.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
        this.rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed -= HandleRewardedAdClosed;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardPlayer();
    }

    IEnumerator RewardPlayer()
    {
        yield return new WaitUntil(()=> focused == true);
        GameManager.Instance.TotalPoints += 300;
        Toast.Instance.Show("You've recieved 300 points");
        ES3.Save<int>("TotalPoints", GameManager.Instance.TotalPoints);
    }
    #endregion

    #region Continue Ad Methods
    public void HandleContinueAdLoaded(object sender, EventArgs args)
    {
        ContinueButton.interactable = true;
    }

    public void HandleContinueAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        LoadContinueAd();
    }

    public void HandleContinueAdOpening(object sender, EventArgs args)
    {
        Time.timeScale = 0;
    }

    public void HandleContinueAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        ContinueButton.interactable = true;
    }

    public void HandleContinueAdClosed(object sender, EventArgs args)
    {
        Time.timeScale = 1;
        LoadContinueAd();
        ContinueButton.interactable = true;
        this.rewardedAd.OnAdLoaded -= HandleContinueAdLoaded;
        this.rewardedAd.OnAdFailedToLoad -= HandleContinueAdFailedToLoad;
        this.rewardedAd.OnAdOpening -= HandleContinueAdOpening;
        this.rewardedAd.OnAdFailedToShow -= HandleContinueAdFailedToShow;
        this.rewardedAd.OnUserEarnedReward -= HandleUserCanContinue;
        this.rewardedAd.OnAdClosed -= HandleContinueAdClosed;
    }

    public void HandleUserCanContinue(object sender, Reward args)
    {
        Invoke("Continue", 1);
    }
    #endregion

    #region Native Ad Methods

    private void HandleUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    {
        GameOverNativeAd = true;
        this.GameOverNative = args.nativeAd;
        CheckGameOvernative();
    }
    private void pHandleUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    {
        PauseNativeAd = true;
        this.PauseNative = args.nativeAd;
        CheckPauseNative();
    }
    private void thHandleUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    {
        TryHarderNativeAd = true;
        this.TryHarderNative = args.nativeAd;
        CheckTryHarderNavite();
    }
    private void mpHandleUnifiedNativeAdLoaded(object sender, UnifiedNativeAdEventArgs args)
    {
        MorePointsNativeAd = true;
        this.MorePointsNative = args.nativeAd;
        CheckMorePointsNative();
    }

    #endregion
    void Continue()
    {
        GameManager.Instance.Continue();
    }

    #endregion Ads Methods

    void OnDestroy()
    {
        this.interstitial.OnAdLoaded -= HandleOnAdLoaded;
        this.interstitial.OnAdFailedToLoad -= HandleOnAdFailedToLoad;
        this.interstitial.OnAdOpening -= HandleOnAdOpened;
        this.interstitial.OnAdClosed -= HandleOnAdClosed;
        this.interstitial.OnAdLeavingApplication -= HandleOnAdLeavingApplication;

        DestroyInterstitialAd();
        this.rewardedAd.OnAdLoaded -= HandleRewardedAdLoaded;
        this.rewardedAd.OnAdFailedToLoad -= HandleRewardedAdFailedToLoad;
        this.rewardedAd.OnAdOpening -= HandleRewardedAdOpening;
        this.rewardedAd.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
        this.rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed -= HandleRewardedAdClosed;

        this.rewardedAd.OnAdLoaded -= HandleContinueAdLoaded;
        this.rewardedAd.OnAdFailedToLoad -= HandleContinueAdFailedToLoad;
        this.rewardedAd.OnAdOpening -= HandleContinueAdOpening;
        this.rewardedAd.OnAdFailedToShow -= HandleContinueAdFailedToShow;
        this.rewardedAd.OnUserEarnedReward -= HandleUserCanContinue;
        this.rewardedAd.OnAdClosed -= HandleContinueAdClosed;
    }
    public void DestroyInterstitialAd()
    {
        interstitial.Destroy();
    }

    void ConnectionToast()
    {
        Toast.Instance.Show("Please Wait");
    }
    IEnumerator CheckInternetConnection(Action<bool> action)
    {
        UnityWebRequest www = new UnityWebRequest("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    public void RewardedOnConnected()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            if (isConnected && !this.rewardedAd.IsLoaded())
            {
                LoadRewardedAd();
            }
        }));
    }

    public void ContinueOnConnected()
    {
        StartCoroutine(CheckInternetConnection((isConnected) =>
        {
            if (isConnected && !this.continueAd.IsLoaded())
            {
                LoadContinueAd();
            }
        }));
    }

    #region disabling offline ads
    void DisableGameOverOfflineNativeAds()
    {
        foreach (GameObject go in GameOverOfflineNativeAds)
        {
            go.SetActive(false);
        }
    }
    void DisablePauseOfflineNativeAds()
    {
        foreach (GameObject go in PauseOfflineNativeAds)
        {
            go.SetActive(false);
        }
    }
    void DisableMorePointsOfflineNativeAds()
    {
        foreach (GameObject go in MorePointsOfflineNativeAds)
        {
            go.SetActive(false);
        }
    }
    #endregion


    #region Starting Offline Ads
    void StartOfflineAds()
    {
        if (GameManager.Instance.removedAds)
            return;

        StartGameoverOfflineAds();

        StartPauseOfflineAds();

        StarMorePointsOfflineAds();
    }

    void StartGameoverOfflineAds()
    {
        int gameovernum = UnityEngine.Random.Range(1, 4);

        foreach (GameObject go in GameOverOfflineNativeAds)
        {
            if (int.Parse(go.name) == gameovernum)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }

    }
    void StartPauseOfflineAds()
    {
        int pausenum = UnityEngine.Random.Range(1, 4);

        foreach (GameObject go in PauseOfflineNativeAds)
        {
            if (int.Parse(go.name) == pausenum)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }

    }
    void StarMorePointsOfflineAds()
    {
        int morepointsnum = UnityEngine.Random.Range(1, 4);

        foreach (GameObject go in MorePointsOfflineNativeAds)
        {
            if (int.Parse(go.name) == morepointsnum)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }

    }

    void ShowOfflineInterstitialAd() 
    {
        int q = UnityEngine.Random.Range(1, 3);

        foreach (GameObject g in OfflineInterstitialAds)
        {
            if (int.Parse(g.name) == q)
            {
                g.SetActive(true);
            }
        }
    }
    #endregion

    #region buttons
    public void OnRewardedButtonPressed()
    {
        RewardButton.interactable = false;
        ShowRewardedAd();
    }
    public void OnContinueButtonPressed()
    {
        ContinueButton.interactable = false;
        ShowContinuedAd();
    }
    #endregion
} 

