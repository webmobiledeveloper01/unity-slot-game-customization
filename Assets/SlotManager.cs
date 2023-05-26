using System;
using System.Collections;
using System.Collections.Generic;
using CSFramework;
using DG.Tweening;
using Elona.Slot;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour {

    private static SlotManager Instance;
    public static SlotManager SlotManagerInstance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<SlotManager>();
            }
            return Instance;
        }
    }

    [Header("Elos")] public Elos elos;

    public bool IsGambleActive;
    public Button mMoreGames, mHelpBtn, mGambleBtn, mAutoStartBtn, mLinesBtn, mBetBtn, mPlayBtn, mFreeSpinStartCongoBtn, mFreeSpinEndCongoBtn;
    public GameObject mFreeSpinGameCountLabel;
    public bool autoStartVik;
    public GameObject mWinnerPanel, mWinAmountTxt;

    public GameObject mHitInfoPanel, mLineTxt, mWinLineAmtTxt;
    public GameObject[] mSmallSym;
    public Sprite[] mSmallSymbSprite;
    public Sprite mNormalBg;
    public Sprite mFreeSpinBg;

    public AudioClip[] SymbolClip;

    public int mFreeSpinMultiplier = 1;
    public int mWildSymbolMultiplier = 1;

    public int mNoOfHitChainReqForFreeSpin = 3;
    public bool mIsActivatedFreeSpin = false;
    public bool mIsSymbolExpansionAvailable = true;

    bool IsWinAmountChanged = false;

    float mlastWinAmount;
    float mLastToLastWinAmount;
    public Text mLastWinAmountTxt;

    private Tweener _moneyTween;

    public Color[] mLineColors;

    public GameObject mLinesLbl, mBetLineLbl, mLastWinLbl, mBalanceLbl;

    // Use this for initialization
    void Start() {
        SetSelectedTerminalData();
        // GetTerminals();
        autoStartVik = false;

        mMoreGames.onClick.RemoveAllListeners();
        mGambleBtn.onClick.RemoveAllListeners();
        mFreeSpinStartCongoBtn.onClick.RemoveAllListeners();
        mFreeSpinEndCongoBtn.onClick.RemoveAllListeners();

        mMoreGames.onClick.AddListener(MoreGamesClick);
        mGambleBtn.onClick.AddListener(GambleBtnClick);
        mFreeSpinStartCongoBtn.onClick.AddListener(FreeSpingStartCongoBtnClick);
        mFreeSpinEndCongoBtn.onClick.AddListener(FreeSpinEndCongoBtnClick);

    }

    public void LoadMenu()
	{
        SceneManager.LoadScene("Menu");
	}

    public void StopDisplayingHitInfo()
    {
        mHitInfoPanel.SetActive(false);
    }


    public void DisplayHitInfo(HitInfo _AllHitInfo)
    {
        mHitInfoPanel.SetActive(true);
        if (_AllHitInfo.line)
        {
            mLineTxt.GetComponent<Text>().text = "LINE " + (_AllHitInfo.line.order + 1) + ":";
        }
        else
        {
            mLineTxt.GetComponent<Text>().text = "LINE " + "SCATTER" + ":";
        }
        elos.assets.audioSymbol.clip = SymbolClip[int.Parse(_AllHitInfo.hitSymbol.sprite.name.Split('_')[1]) - 1];
        elos.assets.audioSymbol.Play();

        for (int i = 0; i < 5; i++)
        {
            if (i < _AllHitInfo.hitHolders.Count)
            {
                int sptireIndex = int.Parse(_AllHitInfo.hitHolders[i].reel.symbols[_AllHitInfo.hitHolders[i].symbolIndex].name) - 1;
                mSmallSym[i].SetActive(true);
                mSmallSym[i].GetComponent<Image>().sprite = mSmallSymbSprite[sptireIndex];
            }
            else
            {
                mSmallSym[i].SetActive(false);
            }
        }

        if (mIsActivatedFreeSpin && mFreeSpinMultiplier > 1)
        {
            float _Amnt = (_AllHitInfo.payout * elos.slot.gameInfo.bet);
            mWinLineAmtTxt.GetComponent<Text>().text = "WIN: " + string.Format("{0:0.00}", _Amnt) + " X " + mFreeSpinMultiplier;
        }
        else if(elos.slot.gameInfo.Check2WildSymboContains(_AllHitInfo) && mWildSymbolMultiplier > 1)
        {
            float _Amnt = (_AllHitInfo.payout * elos.slot.gameInfo.bet);
            mWinLineAmtTxt.GetComponent<Text>().text = "WIN: " + string.Format("{0:0.00}", _Amnt) + " X " + mWildSymbolMultiplier;
        }
        else
        {
            float _Amnt = (_AllHitInfo.payout * elos.slot.gameInfo.bet);
            mWinLineAmtTxt.GetComponent<Text>().text = "WIN: " + string.Format("{0:0.00}", _Amnt);
        }
    }

    public void MoreGamesClick()
    {
        //Application.OpenURL("http://3.22.97.195/gamedev-admin/public/index");
    }

    void GambleBtnClick()
    {
        mGambleBtn.GetComponent<ToggleBtn>().DeActivateSpriteAnim();
        if (IsGambleActive)
        {
            IsGambleActive = !IsGambleActive;
            mGambleBtn.GetComponent<ToggleBtn>().SetButtonState(IsGambleActive, "GAMBLE OFF");
        }
        else
        {
            IsGambleActive = !IsGambleActive;
            mGambleBtn.GetComponent<ToggleBtn>().SetButtonState(IsGambleActive, "GAMBLE ON");
        }

        if (mWinnerPanel.activeSelf)
        {
            elos.slot.AddEvent(new SlotEvent(ActivateGamble));
        }
    }

    public void autoPlayVik()
    {
        if (autoStartVik)
        {
            autoStartVik = false;
            mAutoStartBtn.transform.GetChild(0).GetComponent<Text>().text = "AUTO START\nOFF";
        }
        else
        {
            autoStartVik = true;
            mAutoStartBtn.transform.GetChild(0).GetComponent<Text>().text = "AUTO START\nON";
            elos.Play();
        }
    }

    SlotEvent mSlotEvent;
    public void GotTheResult()
    {
        if (elos.slot.gameInfo.freeSpins > 0)
        {
            // Check if game is currently in free spin mode
            if (mIsActivatedFreeSpin)
            {
                // Game is Currently in free spin mode.

                // Check for symbol expansion possibility
                bool _IsPossibleSymbolExpansion = false;// CheckSymbolExpansionPosibility();

                if (elos.slot.gameInfo.roundHits > 0)
                {
                    // Game is Won
                    // If symbols expansion is true then expand the symbols
                    ActivateWinnerPanel(elos.slot.gameInfo);
                    if (elos.slot.effects.lineHitEffect.displayAsPlayback) elos.slot.StartPlaybackResult(_IsPossibleSymbolExpansion ? 1 : -1);
                }
                else
                {
                    /*    if(_IsPossibleSymbolExpansion)
                       {
                        // SymbolExpansion
                        elos.slot.AddEvent(new SlotEvent(StartSymbolExpansion));
                        }
                        */
                    if (autoStartVik)
                    {
                        elos.AutoPlay();
                    }
                }
            }
            else
            {
                // Display Free Spin Start Congo Btn
                if (elos.slot.gameInfo.roundHits > 0)
                {
                    // Game is Won
                    // Activate Free Spin Start Congo Button
                    ActivateWinnerPanel(elos.slot.gameInfo);
                    if (elos.slot.effects.lineHitEffect.displayAsPlayback) elos.slot.StartPlaybackResult(2);
                }
            }
        }
        // IT Will check if free spin is 0 and still mIsActivatedFreeSpin is true means now free spin is going to end
        else if (elos.slot.gameInfo.freeSpins <= 0 && mIsActivatedFreeSpin)
        {
            // Check for symbol expansion possibility
            bool _IsPossibleSymbolExpansion = false; // CheckSymbolExpansionPosibility();

            if (elos.slot.gameInfo.roundHits > 0)
            {
                // Game is Won
                // If symbols expansion is true then expand the symbols
                ActivateWinnerPanel(elos.slot.gameInfo);
                if (elos.slot.effects.lineHitEffect.displayAsPlayback) elos.slot.StartPlaybackResult(_IsPossibleSymbolExpansion ? 1 : -1);
            }
            else
            {
                // IF Game is Loss

                // SymbolExpansion
                if (_IsPossibleSymbolExpansion)
                {
                //    elos.slot.AddEvent(new SlotEvent(StartSymbolExpansion));
                }
                else
                {
                    elos.slot.AddEvent(new SlotEvent(DisplayFreeSpinEndCongoPopUp));
                }
            }


            // Disply Free Spin end pop up
            if (elos.slot.effects.lineHitEffect.displayAsPlayback) elos.slot.StartPlaybackResult(3);
        }
        else
        {
            // Game is Currently not in free spin mode.

            // Check if game is won or loose
            if (elos.slot.gameInfo.roundHits > 0)
            {
                Debug.Log("IT's Here");
                // Game is Won
                ActivateWinnerPanel(elos.slot.gameInfo);
                mPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "COLLECT";
                // Check If Gamble Mode is available in the game.
                if (!IsGambleActive)
                {
                    // Game Doesn't have Gamble Mode on
                    mGambleBtn.GetComponent<ToggleBtn>().ActivateSpriteAnim();
                }
                // If Gamble is active then it will pass 0 so gameble mode will be activated
                if (elos.slot.effects.lineHitEffect.displayAsPlayback) elos.slot.StartPlaybackResult(IsGambleActive ? 0 : -1);
            }
            else
            {
                // Game is Loose
                if (autoStartVik)
                {
                    elos.AutoPlay();
                }
            }
        }
    }

    float _WinAmount = 0;
    public void ActivateWinnerPanel(GameInfo _GameInfo)
    {
        mLastToLastWinAmount = _WinAmount;
        mLastWinAmountTxt.text = "" + string.Format("{0:0.00}", mLastToLastWinAmount); 
        if (mIsActivatedFreeSpin)
        {
            mWinnerPanel.GetComponent<Image>().enabled = false;
            _WinAmount = _WinAmount + _GameInfo.roundBalance;// + _GameInfo.roundCost;
        }
        else
        {
            _WinAmount = _GameInfo.roundBalance;
            mlastWinAmount = 0;
            mWinnerPanel.GetComponent<Image>().enabled = true;
        }

    /*    if (_GameInfo.roundHits > 0 && _GameInfo.freeSpins <= 0 && IsGambleActive)
        {
            Invoke("ActivateGamble", 2f);
        }
        else
        {
            if (elos.slot.currentMode == elos.slot.modes.freeSpinMode)
            {
            }
            else
            {
                mPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "COLLECT";
                mGambleBtn.GetComponent<ToggleBtn>().ActivateSpriteAnim();
            }
        }*/

     mWinnerPanel.SetActive(true);
     IsWinAmountChanged = true;
            elos.assets.audioCoinAdd.Play();
            if (_moneyTween != null && _moneyTween.IsPlaying()) _moneyTween.Complete();
            _moneyTween = DOTween.To(() => mlastWinAmount, x => mlastWinAmount = x, _WinAmount, 1f).OnComplete(() => { _moneyTween = null; elos.assets.audioCoinAdd.Stop();
                IsWinAmountChanged = false;
            });

    }

    #region Mange Gamble
    public bool mIsOnGambleScreen = false;
    public void ActivateGamble(SlotEvent slotEvent)
    {
        Debug.LogError("Activated Gamble");
        mSlotEvent = slotEvent;
        mIsOnGambleScreen = true;
        Vector3 _Temp1 = mBalanceLbl.transform.position;
        mBalanceLbl.transform.position = mLastWinLbl.transform.position;
        mLastWinLbl.transform.position = _Temp1;

        transform.GetComponent<GambleManage>().ActivateGamble(_WinAmount);
        DeActivateWinnerPanel();
        mPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "START";
    }

    public void DeActivateGamble()
    {
        mIsOnGambleScreen = false;
        Vector3 _Temp1 = mBalanceLbl.transform.position;
        mBalanceLbl.transform.position = mLastWinLbl.transform.position;
        mLastWinLbl.transform.position = _Temp1;
        mSlotEvent.Deactivate();
        if (autoStartVik)
        {
            elos.AutoPlay();
    }
    }
    #endregion

    public void DeActivateWinnerPanel()
    {
        mGambleBtn.GetComponent<ToggleBtn>().DeActivateSpriteAnim();
        if (IsGambleActive)
            mGambleBtn.GetComponent<ToggleBtn>().SetButtonState(IsGambleActive, "GAMBLE ON");
        else
            mGambleBtn.GetComponent<ToggleBtn>().SetButtonState(IsGambleActive, "GAMBLE OFF");

        elos.slot.StopPlaybackResult();
        elos.assets.audioCoinAdd.Play();
        IsWinAmountChanged = true;
        if (_moneyTween != null && _moneyTween.IsPlaying()) _moneyTween.Complete();
        _moneyTween = DOTween.To(() => mlastWinAmount, x => mlastWinAmount = x, 0, 1f).OnComplete(()
        => {_moneyTween = null;
            mWinnerPanel.SetActive(false);
            elos.assets.audioCoinAdd.Stop();
                if (autoStartVik && !mFreeSpinStartCongoBtn.gameObject.activeSelf && !mFreeSpinEndCongoBtn.gameObject.activeSelf)
                {
                    elos.Play();
                }
        });
    }

    public IEnumerator ToggleFreeSpin(bool enable)
    {
        mPlayBtn.interactable = false;
        yield return new WaitForSeconds(1f);
        if (enable)
        {
        //    ActivateFreeSpinStartCongoBtn();
        }
        else
        {
            mFreeSpinEndCongoBtn.gameObject.SetActive(true);
        }
    }

    #region Free Spin Start
    public void DisplayFreeSpinStartCongoPopUp(SlotEvent slotEvent)
    {
        mSlotEvent = slotEvent;
        mFreeSpinStartCongoBtn.gameObject.SetActive(true);
        mGambleBtn.GetComponent<ToggleBtn>().DeActivateSpriteAnim();
    }

    void FreeSpingStartCongoBtnClick()
    {
        elos.slot.StopPlaybackResult();
        mIsActivatedFreeSpin = true;
        ActivateFreeSpinUI();
    }

    void ActivateFreeSpinUI()
    {
        mMoreGames.gameObject.SetActive(false);
        mHelpBtn.gameObject.SetActive(false);
        mGambleBtn.gameObject.SetActive(false);
        mBetBtn.gameObject.SetActive(false);
        elos.ui.background.sprite = mFreeSpinBg;
        mFreeSpinGameCountLabel.gameObject.SetActive(true);
        mPlayBtn.transform.GetChild(0).GetComponent<Text>().text = "START";
        mWinnerPanel.GetComponent<Image>().enabled = false;
        mPlayBtn.interactable = false;
        elos.slot.StopPlaybackResult();
        mLinesLbl.gameObject.SetActive(false);
        mBetLineLbl.gameObject.SetActive(false);
        mLastWinLbl.gameObject.SetActive(false);

        Vector3 _Temp1 = mBalanceLbl.transform.position;
        mBalanceLbl.transform.position = mLastWinLbl.transform.position;
        mLastWinLbl.transform.position = _Temp1;

        Vector3 _Temp = mMoreGames.transform.position;
        mMoreGames.transform.position = mAutoStartBtn.transform.position;
        mAutoStartBtn.transform.position = _Temp;
        mFreeSpinStartCongoBtn.gameObject.SetActive(false);
        mPlayBtn.interactable = true;
        mSlotEvent.Deactivate();

        if (autoStartVik)
        {
            elos.AutoPlay();
        }
    }
    #endregion

    #region Free Spin End
    public void DisplayFreeSpinEndCongoPopUp(SlotEvent slotEvent)
    {
        mSlotEvent = slotEvent;
        //    if (_WinAmount > 0)
        {
            elos.slot.gameInfo.AddBalance(_WinAmount);
         //v   DeActivateWinnerPanel();

            Debug.Log("Assigning mFreeSpinEndCongoBtn");

            mFreeSpinEndCongoBtn.transform.GetChild(1).GetComponent<Text>().text = "" + string.Format("{0:0.00}", _WinAmount);
            string[] spitStr = mFreeSpinGameCountLabel.transform.GetChild(0).GetComponent<Text>().text.Split(' ');
            string GameCoun = spitStr[spitStr.Length - 1];
            mFreeSpinEndCongoBtn.transform.GetChild(2).GetComponent<Text>().text = "BONUSSPINS PLAYED: " + GameCoun;
            mFreeSpinEndCongoBtn.gameObject.SetActive(true);
        }
    }
    void FreeSpinEndCongoBtnClick()
    {
        DeActivateFreeSpinUI();
    }

    void DeActivateFreeSpinUI()
    {
        Vector3 _Temp = mMoreGames.transform.position;
        mMoreGames.transform.position = mAutoStartBtn.transform.position;
        mAutoStartBtn.transform.position = _Temp;

        Vector3 _Temp1 = mBalanceLbl.transform.position;
        mBalanceLbl.transform.position = mLastWinLbl.transform.position;
        mLastWinLbl.transform.position = _Temp1;

        mLinesLbl.gameObject.SetActive(true);
        mBetLineLbl.gameObject.SetActive(true);
        mLastWinLbl.gameObject.SetActive(true);

        mMoreGames.gameObject.SetActive(true);
        mHelpBtn.gameObject.SetActive(true);
        mGambleBtn.gameObject.SetActive(true);
        mBetBtn.gameObject.SetActive(true);
        elos.ui.background.sprite = mNormalBg;
        mFreeSpinGameCountLabel.gameObject.SetActive(false);
        mWinnerPanel.GetComponent<Image>().enabled = true;
    
    /* v Not Sure   if (_WinAmount > 0)
        {
            elos.slot.gameInfo.AddBalance(_WinAmount);
            DeActivateWinnerPanel();
            mFreeSpinEndCongoBtn.transform.GetChild(1).GetComponent<Text>().text = "" + string.Format("{0:0.00}", _WinAmount);
            string[] spitStr = mFreeSpinGameCountLabel.transform.GetChild(0).GetComponent<Text>().text.Split(' ');
            string GameCoun = spitStr[spitStr.Length - 1];
            mFreeSpinEndCongoBtn.transform.GetChild(2).GetComponent<Text>().text = "BONUSSPINS PLAYED: " + GameCoun;
        }*/

        DeActivateWinnerPanel();
        mPlayBtn.interactable = true;
        mIsActivatedFreeSpin = false;
        mFreeSpinEndCongoBtn.gameObject.SetActive(false);

        mSlotEvent.Deactivate();
    }

    #endregion

    private void Update()
    {
        if (IsWinAmountChanged)
        {
            mWinAmountTxt.GetComponent<Text>().text = string.Format("{0:0.00}", mlastWinAmount);
        }
    }

    #region WEB API MANAGE USER DATA
    void GetTerminals()
    {
        StartCoroutine(CheckTerminal());
    }

    RootObject myAllTerminalObject;
    public Terminal mSelectedTermianl;

    IEnumerator CheckTerminal()
    {
        WWWForm form = new WWWForm();

        UnityWebRequest www = UnityWebRequest.Get("http://3.22.97.195/gamedev/public/api/get_all_terminals");
        yield return www.SendWebRequest();


        string url;
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
        {
            url = "http://3.22.97.195/gamedev-admin/public/Games/BookOfAztek/Terminal1";
        }
        else
        {
            url = Application.absoluteURL;
        }
        string[] urlSplited = url.Split('/');
        string _CurrTerminal = urlSplited[urlSplited.Length - 1];
        _CurrTerminal = _CurrTerminal.Split('l')[1];
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            jsonData = "{\"mTerminals\":" + jsonData + "}";

            myAllTerminalObject = new RootObject();
            myAllTerminalObject = JsonUtility.FromJson<RootObject>(jsonData);

            for (int i = 0; i < myAllTerminalObject.mTerminals.Length; i++)
            {
                if (myAllTerminalObject.mTerminals[i].user_id.ToString().Equals(_CurrTerminal))
                {
                    mSelectedTermianl = myAllTerminalObject.mTerminals[i];
                    break;
                }
            }
        }
        SetSelectedTerminalData();
    }

    public void UpdateTerminal()
    {
        StartCoroutine(UpdateTerminalOnServer());
    }

    IEnumerator UpdateTerminalOnServer()
    {
        mSelectedTermianl.balance = string.Format("{0:0.00}", elos.slot.gameInfo.balance);
        WWWForm form = new WWWForm();
        form.AddField("id", mSelectedTermianl.id);
        form.AddField("user_id", mSelectedTermianl.user_id);
        form.AddField("is_active", mSelectedTermianl.is_active);
        form.AddField("percentage", mSelectedTermianl.percentage);
        form.AddField("balance", mSelectedTermianl.balance.ToString());

        UnityWebRequest www = UnityWebRequest.Post("http://3.22.97.195/gamedev/public/api/edit_terminal", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;

            SetSelectedTerminalData();
        }
    }

    void SetSelectedTerminalData()
    {
        mSelectedTermianl = new Terminal();
        mSelectedTermianl.balance = "10000";
       

        elos.slot.gameInfo.balance = float.Parse(string.Format("{0:0.00}", mSelectedTermianl.balance));
        elos.ui.lastBalance = elos.slot.gameInfo.balance;
        elos.ui.textMoney.text = "" + string.Format("{0:0.00}", elos.slot.gameInfo.balance);

        if (elos.slot.gameInfo.balance <= 0 || elos.slot.gameInfo.balance < elos.slot.gameInfo.bet)
        {
            mGambleBtn.interactable = false;
            mAutoStartBtn.interactable = false;
            mLinesBtn.interactable = false;
            mBetBtn.interactable = false;
            mPlayBtn.interactable = false;
        }
    }

    #endregion
}


[Serializable]
public class Terminal
{
    public int id;
    public int user_id;
    public int is_active;
    public int percentage;
    public string balance;
    public string created_at;
    public string updated_at;
}

[Serializable]
public class RootObject
{
    public Terminal[] mTerminals;
}
