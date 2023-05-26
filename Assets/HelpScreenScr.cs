using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HelpScreenScr : MonoBehaviour
{
    public GameObject mHelpScreen;
    public GameObject[] mHelpScreens;
    int mCurrentScreenIndex;
    public Text mAllPageBtnTxt;
    public Button mNextBtn, mPrevBtn, mReturnToGameBtn, mPositiveNextBtn;
    // Start is called before the first frame update
    void Start()
    {
        mNextBtn.onClick.RemoveAllListeners();
        mPrevBtn.onClick.RemoveAllListeners();
        mReturnToGameBtn.onClick.RemoveAllListeners(); 
        mPositiveNextBtn.onClick.RemoveAllListeners();

        mNextBtn.onClick.AddListener(NextPageBtnClick);
        mPrevBtn.onClick.AddListener(PrevPageBtnClick);
        mReturnToGameBtn.onClick.AddListener(ReturnToGameBtnClick);
        mPositiveNextBtn.onClick.AddListener(NextPageBtnClick);
    }

    void NextPageBtnClick()
    {
        mCurrentScreenIndex++;
        if(mCurrentScreenIndex > mHelpScreens.Length - 1)
        {
            mCurrentScreenIndex = 0;
        }
        SetScreen();
    }

    void PrevPageBtnClick()
    {
        mCurrentScreenIndex--;
        if (mCurrentScreenIndex < 0)
        {
            mCurrentScreenIndex = mHelpScreens.Length - 1;
        }
        SetScreen();
    }

    void ReturnToGameBtnClick()
    {
        mHelpScreen.SetActive(false);
    }

    void InitializeHelpScreen()
    {
        mCurrentScreenIndex = 0;
    }

    void SetScreen()
    {
        for (int i = 0; i < mHelpScreens.Length; i++)
        {
            mHelpScreens[i].SetActive(false);
        }
        mHelpScreens[mCurrentScreenIndex].SetActive(true);
        mAllPageBtnTxt.text = (mCurrentScreenIndex + 1).ToString() + "/" + mHelpScreens.Length.ToString();
    }
}
