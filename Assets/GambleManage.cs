using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GambleManage : MonoBehaviour
{
    public GameObject mGamblePanel;

    public GameObject mSmallCardParent, mAnimatingCard, mCardPrefab;

    public Button mRedBtn, mBlackBtn, mHeartBtn, mDiamondBtn, mSpadeBtn, mClubBtn;
    public Button mHalfBtn, mCollectBtn;

    public Text mGambleAmountTxt, mColorGambleAmtToWin, mSuitGambleAmtToWin, mWinAmountTxt;

    float mGambleAmount;

    public Sprite[] mSpriteSuits;
    // Start is called before the first frame update
    void Start()
    {
        mRedBtn.onClick.RemoveAllListeners();
        mBlackBtn.onClick.RemoveAllListeners();
        mHeartBtn.onClick.RemoveAllListeners();
        mDiamondBtn.onClick.RemoveAllListeners();
        mSpadeBtn.onClick.RemoveAllListeners();
        mClubBtn.onClick.RemoveAllListeners();
        mHalfBtn.onClick.RemoveAllListeners();
        mCollectBtn.onClick.RemoveAllListeners();

        mRedBtn.onClick.AddListener(RedCardClick);
        mBlackBtn.onClick.AddListener(BlackCardClick);
        mHeartBtn.onClick.AddListener(HearCardClick);
        mDiamondBtn.onClick.AddListener(DiamondCardClick);
        mSpadeBtn.onClick.AddListener(SpadeCardClick);
        mClubBtn.onClick.AddListener(ClubCardClick);
        mHalfBtn.onClick.AddListener(HalfBtnClick);
        mCollectBtn.onClick.AddListener(CollectBtnClick);
    }

    public void ActivateGamble(float _WinAmount)
    {
        mGambleAmount = _WinAmount;
        InitializeGamble();
        mGamblePanel.SetActive(true);
    }

    public void DeActivateGamble()
    {
        mGamblePanel.SetActive(false);
        SlotManager.SlotManagerInstance.DeActivateGamble();
    }

    public void BlackCardClick()
    {
        GetRandomizedCard(0, -1);
    }

    public void RedCardClick()
    {
        GetRandomizedCard(1, -1);
    }

    public void HearCardClick()
    {
        GetRandomizedCard(-1, 0);
    }

    public void DiamondCardClick()
    {
        GetRandomizedCard(-1, 1);
    }

    public void SpadeCardClick()
    {
        GetRandomizedCard(-1, 2);
    }

    public void ClubCardClick()
    {
        GetRandomizedCard(-1, 3);
    }

    /// <summary>
    /// Gets the randomized card.
    /// </summary>
    /// <param name="_CardColor">Card color. 0=Black, 1=Red</param>
    /// <param name="_CarSuit">Car suit. 0=Heart, 1=Diamond, 2=Spade, 3=Club</param>
    void GetRandomizedCard(int _CardColor, int _CarSuit = -1)
    {
        if (mGambleAmount <= 0)
            return;

        int _SelectedSuit = -1;

        //Select based on Card Suit

        int _RandomSuit = Random.Range(0, 4);      // 0=Heart, 1=Diamond, 2=Spade, 3=Club
        _SelectedSuit = _RandomSuit;


        if (mSmallCardParent.transform.childCount > 7)
        {
            Destroy(mSmallCardParent.transform.GetChild(0).gameObject);
        }

        _InstantiatedCard.GetComponent<Image>().sprite = mSpriteSuits[_SelectedSuit];

        mAnimatingCard.GetComponent<Animator>().enabled = false;
        mAnimatingCard.GetComponent<Image>().sprite = mSpriteSuits[_SelectedSuit];

        if(_CardColor == -1)
        {
            // For Suit Metch
            if(_CarSuit == _SelectedSuit)
            {
                // Correct Guess
                CorrectGuess(2);
            }
            else
            {
                // Wrong Guess
                WrongGuess();
            }
        }
        else
        { 
            //For Color Metch
            if(_CardColor == 0 && (_SelectedSuit == 2 || _SelectedSuit == 3))
            {
                // Correct Guess
                CorrectGuess(4);
            }
            else if (_CardColor == 1 && (_SelectedSuit == 0 || _SelectedSuit == 1))
            {
                // Correct Guess
                CorrectGuess(4);
            }
            else
            {
                // Wrong Guess
                WrongGuess();
            }
        }
    }

    void CorrectGuess(int _Multiply)
    {
        Debug.LogError("Correct");
        mGambleAmount = mGambleAmount * _Multiply;
        Invoke("InitializeGamble", 3f);
    }

    void WrongGuess()
    {
        mGambleAmount = 0;
        SetGamleAmount(mGambleAmount);
        Invoke("DeActivateGamble", 2f);
    }

    void HalfBtnClick()
    {
        float newGambleAmount = mGambleAmount / 2;
        float AddToBalanceAmt = mGambleAmount - newGambleAmount;
        AddToBal(AddToBalanceAmt);
        mGambleAmount = newGambleAmount;
        SetGamleAmount(mGambleAmount);
    }

    void CollectBtnClick()
    {
        if (mGambleAmount > 0)
        { 
            AddToBal(mGambleAmount);
            SetGamleAmount(0);
            mGambleAmount = 0;
            Invoke("DeActivateGamble", 2f);
        }
    }

    void AddToBal(float _amnt)
    {
        SlotManager.SlotManagerInstance.elos.slot.gameInfo.AddBalance(_amnt);
    }

    GameObject _InstantiatedCard;
    void InitializeGamble()
    {

        for (int i = 0; i < mSmallCardParent.transform.childCount; i++)
        {
            if (mSmallCardParent.transform.GetChild(i).GetComponent<Image>().sprite.name.Contains("RedCard"))
            {
                Destroy(mSmallCardParent.transform.GetChild(i).gameObject);
            }
        }

        SetGamleAmount(mGambleAmount);
        mAnimatingCard.GetComponent<Animator>().enabled = true;

        _InstantiatedCard = Instantiate(mCardPrefab) as GameObject;
        _InstantiatedCard.transform.SetParent(mSmallCardParent.transform);
        _InstantiatedCard.transform.localScale = Vector3.one;
    }

    void SetGamleAmount(float _Amnt)
    {
        mGambleAmountTxt.text = "" + string.Format("{0:0.00}", _Amnt);
        mColorGambleAmtToWin.text = "" + string.Format("{0:0.00}", _Amnt * 2);
        mSuitGambleAmtToWin.text = "" + string.Format("{0:0.00}", _Amnt * 4);

        mWinAmountTxt.text = "" + string.Format("{0:0.00}", _Amnt);
    }
}
