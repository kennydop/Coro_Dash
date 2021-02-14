using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region dec
    public static GameManager Instance;
    public Store store;
    public Health PlayerHealth;
    public AdsManager adsManager;
    public AudioSource hit, bgmusic, jump, tap, bgm, PlayerActivated;

    [HideInInspector]
    public Player SelectedPlayer;
    public GameObject StartMenu, InGameUI, GameOverMenu, CountDownScreen, Store, IAPScreen, PauseScreen, InGameTutorial, ContinueBtn, Info, StartTutorials, TryHarder, RateUs, RemoveAdsBtn, OfflineInterstitialAds;
    public GameObject Player;
    enum PageState {None, Start, InGame, GameOver, Countdown, Store, IAP, Pause, StartTutorial, InTutorial}

    public bool playing = false, GameOver = false, fill = false, Muted, maskOn, isTutorial, StartObstacles, TutDeath, MaskImgAnimPlaying, rated, firstTimer, removedAds, focused;
    public int Score, inPoints, InitialTotalPoints, Points, TotalLevel = 80, CurrentLevel, quit = 0, continued = 0, numOfGameOvers, TotalPoints;
    public Image LevelImg, MaskImg;
    public TextMeshProUGUI scoreText;
    public Sprite muted, unmuted;
    public Button MuteButton, maskButton;
    public TextMeshProUGUI EndScore, EndPoints, MaskTimeText;

    int InitialLevelUpPoints = 300, HighScore, MaskTime, maxMaskTime = 20, nextRatUsNum, tryHarder;
    float fillAmount, fillLerpTime = 2f, maskFillAmount = 0.006f; 

    public Animator TutorialAnimator;
    public List<int> LevelsPoints;
    public TextMeshProUGUI HighScoreText, CurrentLevelText, LevelPointsText, TutScore;

    public GameObject[] InGameTutorialPopUps;
    #endregion
    void Awake()
    {
        Instance = this;
        firstTimer = ES3.Load<bool>("First Timer", defaultValue: true);
        removedAds = ES3.Load<bool>("Ads Removed", defaultValue: false);
        rated = ES3.Load<bool>("Rated", defaultValue: false);
        Muted = ES3.Load<bool>("Muted", defaultValue: false);

        for (int p = 1; p <= TotalLevel; p++)
        {
            LevelsPoints.Add(InitialLevelUpPoints * p);
        }

        if (GameObject.FindGameObjectWithTag("bgm") == null)
        {
            bgm = Instantiate(bgmusic);
            bgm.Play();
            DontDestroyOnLoad(bgm);
        }

    }

    void Start()
    {
        #region Checking For Preferences

        if (bgm == null)
            bgm = GameObject.FindGameObjectWithTag("bgm").GetComponent<AudioSource>();

        if (Muted)
        {
            hit.mute = true;
            bgm.mute = true;
            jump.mute = true;
            tap.mute = true;
            MuteButton.GetComponent<Image>().sprite = muted;
        }
        else
        {
            Muted = false;
            hit.mute = false;
            bgm.mute = false;
            jump.mute = false;
            tap.mute = false;
            MuteButton.GetComponent<Image>().sprite = unmuted;
            
        }
#endregion

        Player = GameObject.FindGameObjectWithTag("Player");
        SetPageState(PageState.Start);
        HighScore = ES3.Load<int>("HighScore", defaultValue: 0); 
        TotalPoints = ES3.Load<int>("TotalPoints", defaultValue: 0); 
        CurrentLevel = ES3.Load<int>("CurrentLevel", defaultValue: 0);
        numOfGameOvers = ES3.Load<int>("Number Of GameOvers", defaultValue: 0);
        nextRatUsNum = ES3.Load<int>("Next Rate Us Number", defaultValue: 2);
        Score = 0;
        scoreText.text = Score.ToString();
        HighScoreText.text = "HighScore: " + HighScore.ToString();
        CurrentLevelText.text = CurrentLevel.ToString();
        maskButton.interactable = false;
        MaskImg.fillAmount = 0;
        MaskTimeText.enabled = false; Info.SetActive(false); StartObstacles = false; TutDeath = false; MaskImgAnimPlaying = false; RateUs.SetActive(false); TryHarder.SetActive(false);

        if (firstTimer)
        {
            SetPageState(PageState.StartTutorial);
            TutorialAnimator.Play("Start(Tutorial)");
            isTutorial = true;
        }

        if (removedAds)
        {
            Destroy(RemoveAdsBtn);
            RemovingAds();
        }

        DetermineFillAmount();
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Escape) && quit < 1)
            StartCoroutine(QuitGame());

        else if (Input.GetKeyDown(KeyCode.Escape) && quit == 1)
            Application.Quit();
    }

    private void OnEnable()
    {
        DetermineSelectedPlayer();
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.Start:
                StartMenu.SetActive(true);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.InGame:
                StartMenu.SetActive(false);
                InGameUI.SetActive(true);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;


            case PageState.Countdown:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(true);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.GameOver:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(true);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.Store:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(true);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.IAP:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(true);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.Pause:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(true);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(false);
                break;

            case PageState.StartTutorial:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(true);
                InGameTutorial.SetActive(false);
                break;

            case PageState.InTutorial:
                StartMenu.SetActive(false);
                InGameUI.SetActive(false);
                GameOverMenu.SetActive(false);
                CountDownScreen.SetActive(false);
                IAPScreen.SetActive(false);
                Store.SetActive(false);
                PauseScreen.SetActive(false);
                StartTutorials.SetActive(false);
                InGameTutorial.SetActive(true);
                break;
        }
    }

    public void OnGameOver()
    {
        adsManager.ContinueOnConnected();
        if(continued > 2)
        {
            ContinueBtn.SetActive(false);
        }

        SetPageState(PageState.GameOver);
        GameOver = true;
        playing = false;
        StartObstacles = false;
        Player.GetComponent<Player>().isNotPlaying();
        CancelInvoke("AddScore");
        Points = Score * 3;

        if (continued == 0)
        {
            TotalPoints += Points;
            inPoints = Points;
        }
        else if (continued == 1)
        {
            TotalPoints += ((Score * 3) - inPoints);
            inPoints = Points;
        }
        else if (continued == 2)
        {
            TotalPoints += ((Score * 3) - inPoints);
            inPoints = Points;
        }
        else if(continued == 3)
        {
            TotalPoints += ((Score * 3) - inPoints);
        }


        EndScore.text = "Score: " + Score.ToString();
        EndPoints.text = "Points: " + Points.ToString();
        ES3.Save("TotalPoints", TotalPoints);
        
        if (Score > HighScore)
            ES3.Save("HighScore", Score);

        numOfGameOvers++;
        ES3.Save<int>("Number Of GameOvers", numOfGameOvers);

        DetermineCurrentLevel();

        if(numOfGameOvers == nextRatUsNum && !rated)
        {
            nextRatUsNum *= 2;
            ES3.Save<int>("Next Rate Us Number", nextRatUsNum);
            RateUs.SetActive(true);
        }

        if (numOfGameOvers % 3 == 0 && ! removedAds)
        {
            adsManager.ShowInterstitialAd();
        }

    }

   void Playing()
    {
        SetPageState(PageState.InGame);
        playing = true;
        StartObstacles = true;
        Player.GetComponent<Player>().isPlaying();

        if (playing)
        {
            #region scoring
            if (store.SelectedPlayer == "2")
                InvokeRepeating("AddScore", 1, 0.84f);
            else if (store.SelectedPlayer == "4")
                InvokeRepeating("AddScore", 1, 0.71f);
            else if (store.SelectedPlayer == "6")
                InvokeRepeating("AddScore", 1, 0.56f);
            else if (store.SelectedPlayer == "8")
                InvokeRepeating("AddScore", 1, 0.5f);
            else if (store.SelectedPlayer == "9")
                InvokeRepeating("AddScore", 1, 0.33f);
            else
                InvokeRepeating("AddScore", 1, 1);
            #endregion

            #region filling mask
            if (store.SelectedPlayer == "3")
                maskFillAmount = 0.0065f;
            else if (store.SelectedPlayer == "5")
                maskFillAmount = 0.0075f;
            else if (store.SelectedPlayer == "7")
                maskFillAmount = 0.009f;
            else if (store.SelectedPlayer == "9")
                maskFillAmount = 0.018f;
            else
                maskFillAmount = 0.005f;
            #endregion

            InvokeRepeating("FillMask", 0.001f, 0.01f);
        }
    }
   public void DetermineCurrentLevel()
   {
        TotalPoints = ES3.Load<int>("TotalPoints", defaultValue: 0);
        int p;
        for (int i = CurrentLevel; i < (LevelsPoints.Count + 1); i++)
        {
            if (CurrentLevel >= 80)
                return;

            p = TotalPoints - LevelsPoints[i];

            if(p == 0)
            {
                CurrentLevel = i + 1;
                if (CurrentLevel > ES3.Load<int>("CurrentLevel", defaultValue: 0))
                    ES3.Save<int>("CurrentLevel", CurrentLevel);

                TotalPoints = p;
                ES3.Save<int>("TotalPoints", TotalPoints);
                break;
            }

            else if (p > 0)
            {
                TotalPoints = p;
                CurrentLevel = i + 1;

                if (CurrentLevel > (TotalLevel - 1))
                {
                    ES3.Save<int>("CurrentLevel", CurrentLevel);
                    ES3.Save<int>("TotalPoints", TotalPoints);
                    break;
                }
                   

                if (LevelsPoints[i + 1] > TotalPoints)
                {
                    if (CurrentLevel > ES3.Load<int>("CurrentLevel", defaultValue: 0))
                        ES3.Save<int>("CurrentLevel", CurrentLevel);

                    ES3.Save<int>("TotalPoints", TotalPoints);
                    break;
                }
            }

            else if (p < 0)
            {
                break;
            }

        }

        if (TotalPoints < 0)
            TotalPoints = 0;
   }
   public void DetermineFillAmount()
   {
        if (CurrentLevel > (TotalLevel -1))
        {
            LevelImg.enabled = false;
            LevelPointsText.text = "Levels Completed";
            CurrentLevelText.text = "80";
            return;
        }
        CurrentLevelText.text = CurrentLevel.ToString();
        LevelPointsText.text = TotalPoints.ToString() + " / " + LevelsPoints[CurrentLevel];
        float tp = TotalPoints;
        float lp = LevelsPoints[CurrentLevel];
        fillAmount = (tp / lp);
        fill = true;
        LevelImg.fillAmount = 0;
        InvokeRepeating("Fill", 0.03f, 0.03f);
    }    
   void DetermineSelectedPlayer()
    {
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            for (int p = 0; p < store.players.Count; p++)
            {
                if (store.players[p].name == ES3.Load<string>("SelectedPlayer", defaultValue: "1"))
                {
                    SelectedPlayer = store.players[p];
                    Instantiate(SelectedPlayer, new Vector3(0, 7.5f), Quaternion.identity);
                    break;
                }
            }
        }
        
    }
    void TutorialPlaying()
    {
        SetPageState(PageState.InTutorial);
        playing = true;
        Player.GetComponent<Player>().isPlaying();
        InGameTutorialPopUps[0].SetActive(true);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        TutorialAnimator.Play("TapInstruct");
        Invoke("StartTutorialObs", 36);
        StartCoroutine(HowToPlay());


        if (playing)
        {
            InvokeRepeating("AddScore", 1, 1);
            InvokeRepeating("FillMask", 0.001f, 0.01f);
        }

    }

    public void TutorialContinue()
    {
        GameOver = true;
        playing = false;
        StartObstacles = false;
        Player.GetComponent<Player>().isNotPlaying();
        CancelInvoke();
        StartCoroutine(TutContinueProcesses());
        tryHarder += 1;
        if (tryHarder % 2 == 0)
        {
            adsManager.ShowInterstitialAd();
        }
    }

    void PlayMaskAnim()
    {
        if (!MaskImgAnimPlaying)
        {
            MaskImg.GetComponent<Animator>().Play("MaskUpAnim");
            MaskImgAnimPlaying = true;
        }
                       
    }
    void StartTutorialObs()
    {
        StartObstacles = true;
    }

    public void UnPause()
    {
        SetPageState(PageState.Countdown);
        Invoke("Unpausing", 4.0f);
    }
    void Unpausing()
    {
        SetPageState(PageState.InGame);
        playing = true;
        InvokeRepeating("FillMask", 0.001f, 0.01f);
    }
    void ClosingStore()
    {
        SetPageState(PageState.Start);
    }

    void ClosingIAP()
    {
        SetPageState(PageState.Start);
    }

    void AddScore()
    {
        if (playing)
        {
            Score += 1;
            scoreText.text = Score.ToString();
        }
    }
    void Fill()
    {
        if (LevelImg.fillAmount > fillAmount - 0.001f)
        {
            LevelImg.fillAmount = fillAmount;
            fill = false;
            CancelInvoke("Fill");
            return;
        }
        else if (LevelImg.fillAmount != fillAmount && fill)
        {
            LevelImg.fillAmount = Mathf.Lerp(LevelImg.fillAmount, fillAmount, Time.deltaTime * fillLerpTime);
        }
    }    
    void FillMask()
    {
        if (!playing || GameOver)
            return;
        if (MaskImg.fillAmount >= 1 && !maskOn)
        {
            maskButton.interactable = true;
            PlayMaskAnim();
            CancelInvoke("FillMask");
        }
        else
        {

            MaskImg.fillAmount += maskFillAmount * Time.deltaTime;
        }
    }

    void TurnOffMask()
    {
        if (MaskTime > 0)
            return;
        MaskImg.fillAmount = 0;
        Player.GetComponent<Player>().mask.SetActive(false);
        MaskTimeText.enabled = false;
        maskOn = false;
        MaskTimeText.text = maxMaskTime.ToString();
        CancelInvoke(methodName: "MaskCountdown");
        InvokeRepeating("FillMask", 0.001f, 0.01f);
        MaskImg.GetComponent<Animator>().Play("Ideal");
    }

    void MaskCountdown()
    {
        if (MaskTime == 0)
            return;
        if (!playing)
            return;

        MaskTime--;
        if (MaskTime == 0)
        {
            TurnOffMask();
        }
        if (MaskTime < 0)
        {
            MaskTime = 0;
        }
        MaskTimeText.text = MaskTime.ToString();
    }

    public void Continue()
    {
        GameObject[] obs = GameObject.FindGameObjectsWithTag("Obstacle");

        for (int i = 0; i < obs.Length; i++)
        {
            Destroy(obs[i]);
        }
        Player.GetComponent<Player>().PlayerReset();
        Player.transform.position = Vector3.zero;
        continued++;
        continued = Mathf.Clamp(continued, 0, 3);
        AdPlay();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            focused = true;
            adsManager.focused = true;
        }

        else
        {
            focused = false;
            adsManager.focused = true;
        }

    }

    void RemovingAds()
    {
        adsManager.GameOverAd_Holder.SetActive(false);
        adsManager.PauseAd_Holder.SetActive(false);
        adsManager.MorePointsAd_Holder.SetActive(false);
        adsManager.TryHarderAd_Holder.SetActive(false);

        foreach (var item in store.Store_Ads)
        {
            item.SetActive(false);
        }
        foreach (var item in adsManager.GameOverOfflineNativeAds)
        {
            item.SetActive(false);
        }
        foreach (var item in adsManager.MorePointsOfflineNativeAds)
        {
            item.SetActive(false);
        }
        foreach (var item in adsManager.PauseOfflineNativeAds)
        {
            item.SetActive(false);
        }

        Destroy(OfflineInterstitialAds); Destroy(RemoveAdsBtn);
    }

    #region Coroutines
    IEnumerator CloseRateUs()
    {
        RateUs.GetComponent<Animator>().Play("CloseRateUsAnim");
        yield return new WaitForSeconds(1);
        RateUs.SetActive(false);
    }
    IEnumerator QuitGame()
    {
        quit++;
        Toast.Instance.Show("Tap again to exit", 1.5f);
        yield return new WaitForSeconds(1.5f);
        quit--;
    }
    IEnumerator CloseInfo()
    {
        Info.GetComponent<Animator>().Play("CloseInfoAinm");
        yield return new WaitForSeconds(0.35f);
        Info.SetActive(false);
    }
    IEnumerator HowToPlay()
    {
        yield return new WaitForSeconds(7);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(true);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        TutorialAnimator.Play("Pit Caution");

        yield return new WaitForSeconds(6);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(true);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        TutorialAnimator.Play("Health Reveal");

        yield return new WaitForSeconds(6);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(true);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        TutScore.text = Score.ToString();
        TutorialAnimator.Play("Score Reveal");


        yield return new WaitForSeconds(6);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(true);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        TutorialAnimator.Play("MaskUp Reveal");


        yield return new WaitForSeconds(10);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(true);
        InGameTutorialPopUps[6].SetActive(false);
        TutorialAnimator.Play("Obstacle Caution");


        yield return new WaitForSeconds(5);
        if (TutDeath)
            yield break;
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(false);
        SetPageState(PageState.InGame);
        firstTimer = false;
        ES3.Save<bool>("First Timer", firstTimer);
        MaskImg.fillAmount = 0.95f;

    }
    IEnumerator TutContinueProcesses()
    {
        InGameTutorialPopUps[0].SetActive(false);
        InGameTutorialPopUps[1].SetActive(false);
        InGameTutorialPopUps[2].SetActive(false);
        InGameTutorialPopUps[3].SetActive(false);
        InGameTutorialPopUps[4].SetActive(false);
        InGameTutorialPopUps[5].SetActive(false);
        InGameTutorialPopUps[6].SetActive(true);
        TutorialAnimator.Play("Try Harder");
        yield return new WaitForSeconds(1.5f);
        TutorialAnimator.Play("Try Again");
    }
    IEnumerator RewardForFollow()
    {
        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(()=>focused == true);
        TotalPoints += 300;
        ES3.Save<int>("TotalPoints", TotalPoints);
        Toast.Instance.Show("You've recieved 300 points");
    }
    #endregion Coroutines


    #region Buttons
    public void Play()
    {
        tap.Play();
        SetPageState(PageState.Countdown);
        Invoke("Playing", 4.0f);
        GameOver = false;
        Score = 0;
        scoreText.text = Score.ToString();
        InitialTotalPoints = TotalPoints;
    }
    void AdPlay()
    {
        tap.Play();
        SetPageState(PageState.Countdown);
        Invoke("Playing", 4.0f);
        GameOver = false;

    }

    public void TutPlay()
    {
        tap.Play();
        SetPageState(PageState.Countdown);
        Invoke("TutorialPlaying", 4.0f);
        GameOver = false;
        Score = 0;
        scoreText.text = Score.ToString();
        InitialTotalPoints = TotalPoints;
    }
    public void Pause()
    {
        tap.Play();
        playing = false;
        MaskImgAnimPlaying = false; 
        SetPageState(PageState.Pause);
    }
    public void UnPauseButton()
    {
        tap.Play();
        PauseScreen.GetComponent<Animator>().Play("UnpauseAnim");
        Invoke("UnPause", 1.0f);
    }
    public void Restart()
    {
        DetermineSelectedPlayer();
        GameObject[] Viruses = GameObject.FindGameObjectsWithTag("Obstacle");

        for (int i = 0; i < Viruses.Length; i++)
        {
            Destroy(Viruses[i]);
        }

        GameObject[] Clouds = GameObject.FindGameObjectsWithTag("Cloud");
        for (int i = 0; i < Clouds.Length; i++)
        {
            Clouds[i].GetComponent<Parallaxing>().ParallaxReset();
        }

        GameObject[] Floor = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < Floor.Length; i++)
        {
            Floor[i].GetComponent<Parallaxing>().ParallaxReset();
        }

        CancelInvoke();
        Player.GetComponent<Player>().PlayerReset();
        MaskImg.fillAmount = 0;
        MaskTimeText.enabled = false;
        maskOn = false;
        MaskTimeText.text = maxMaskTime.ToString();
        CancelInvoke(methodName: "MaskCountdown");
        Play();
    }

    public void Home()
    {
        tap.Play();
        SceneManager.LoadScene("Game");
    }
    public void ContinueButton()
    {
        tap.Play();
        adsManager.LoadContinueAd();
    }

    public void OpenInfoButton()
    {
        tap.Play();
        Info.SetActive(true);
    }

    public void CloseInfoButton()
    {
        tap.Play();
        StartCoroutine(CloseInfo());
    }
    public void RemoveAdsButton()
    {
        tap.Play();
        removedAds = true;
        ES3.Save<bool>("Ads Removed", removedAds);
        RemovingAds();
    }


    public void GoToStore()
    {
        tap.Play();
        SetPageState(PageState.Store);
        store.DetermineWhichAdToShow();
    }
    public void QuitStore()
    {
        tap.Play();
        Player = GameObject.FindGameObjectWithTag("Player");
        store.GetComponent<Animator>().Play("CloseStoreAnim");
        Invoke("ClosingStore", 1.2f);
    }
    public void GoToIAPScreen()
    {
        tap.Play();
        SetPageState(PageState.IAP);
        adsManager.RewardedOnConnected();

        GameObject[] SC = GameObject.FindGameObjectsWithTag("Social");
        foreach (var go in SC)
        {
            if ((ES3.Load<bool>(go.name, defaultValue: false) == true))
            {
                go.GetComponent<Button>().interactable = false;
                go.GetComponentInChildren<TextMeshProUGUI>().text = "Recieved";
            }
        }
    }
    public void QuitIAP()
    {
        tap.Play();
        IAPScreen.GetComponent<Animator>().Play("CloseIAPAnim");
        DetermineCurrentLevel();
        DetermineFillAmount();
        Invoke("ClosingIAP", 1.4f);
    }
    public void GoToLeaderBoard()
    {
        tap.Play();
    }
    public void QuitLeaderBoard()
    {

    }
    public void Mute()
    {
        tap.Play();
        if (Muted)
        {
            UnMute();
        }
        else
        {
            hit.mute = true;
            bgm.mute = true;
            jump.mute = true;
            tap.mute = true;
            PlayerActivated.mute = true;
            MuteButton.GetComponent<Image>().sprite = muted;
            ES3.Save<bool>("Muted", true);
            Muted = true;
        }      
    }

    void UnMute()
    {
        hit.mute = false;
        bgm.mute = false;
        jump.mute = false;
        tap.mute = false;
        PlayerActivated.mute = false;
        MuteButton.GetComponent<Image>().sprite = unmuted;
        ES3.Save<bool>("Muted", false);
        Muted = false;
    }

    public void TurnMaskOn()
    {

        if (MaskImg.fillAmount >= 1)
        {
            maskButton.interactable = false;
            PlayerActivated.Play();
            Player.GetComponent<Player>().mask.SetActive(true);
            MaskTime = maxMaskTime;
            MaskTimeText.enabled = true;
            MaskTimeText.text = maxMaskTime.ToString();
            maskOn = true;
            MaskImgAnimPlaying = false;
            InvokeRepeating("MaskCountdown", 1, 1);
            MaskImg.GetComponent<Animator>().Play("Ideal");
        }
    }

    public void TutorialTryAgain()
    {
        tap.Play();
        DetermineSelectedPlayer();
        GameObject[] Viruses = GameObject.FindGameObjectsWithTag("Obstacle");

        for (int i = 0; i < Viruses.Length; i++)
        {
            Destroy(Viruses[i]);
        }

        GameObject[] Clouds = GameObject.FindGameObjectsWithTag("Cloud");
        for (int i = 0; i < Clouds.Length; i++)
        {
            Clouds[i].GetComponent<Parallaxing>().ParallaxReset();
        }

        GameObject[] Floor = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < Floor.Length; i++)
        {
            Floor[i].GetComponent<Parallaxing>().ParallaxReset();
        }

        CancelInvoke();
        Player.GetComponent<Player>().PlayerReset();
        TryHarder.SetActive(false);
        TutDeath = false;
        StopAllCoroutines();
        TutPlay();
    }

    public void IntertitialAdsClose()
    {
        foreach (GameObject g in adsManager.OfflineInterstitialAds)
        {
            if (g.activeSelf)
            {
                g.SetActive(false);
            }
        }
    }

    #region Rating
    public void RateUsButton()
    {
        tap.Play();
        rated = true;
        ES3.Save<bool>("Rated", rated);
        StartCoroutine(CloseRateUs());
        Application.OpenURL("market://details?id=" + Application.productName);
    }

    public void NoThanks()
    {
        tap.Play();
        rated = true;
        ES3.Save<bool>("Rated", rated);
        StartCoroutine(CloseRateUs());
        adsManager.ShowInterstitialAd();
    }

    public void Later()
    {
        tap.Play();
        StartCoroutine(CloseRateUs());
        adsManager.ShowInterstitialAd();
    }
    #endregion

    public void SocialButtons(string link)
    {
        tap.Play();
        Application.OpenURL(link);
        string NameOfButton = EventSystem.current.currentSelectedGameObject.name;
        EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Recieved";
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        ES3.Save<bool>(NameOfButton, true);
        StartCoroutine(RewardForFollow());
    }

    public void ReportIssue()
    {
        string email = "wearetrayl@gmail.com";
        string subject = MyEscapeURL("Found A Bug In Corona Dash");
        string body = "";

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    public void AdButton(string link)
    {
        Application.OpenURL(link);
    }
    #endregion
}
