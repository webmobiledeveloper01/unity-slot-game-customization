using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ToggleBtn : MonoBehaviour
{
    public Sprite mNormalSprite, mActiveSprite;
    Image mBtnImage;
    Text mBtnTxt;
    // Start is called before the first frame update
    void Start()
    {
        mBtnImage = GetComponent<Image>();
        mBtnTxt = GetComponentInChildren<Text>();
    }

    public void ActivateSpriteAnim()
    {
        InvokeRepeating("ChangeSprite", 0f, 0.5f);
    }
    public void DeActivateSpriteAnim()
    {
        CancelInvoke("ChangeSprite");
    }

    void ChangeSprite()
    {
        if (mBtnImage.sprite.name.Contains("0"))
        {
            mBtnImage.sprite = mActiveSprite;
        }
        else
        { 
            mBtnImage.sprite = mNormalSprite;
        }
    }

    public void SetButtonState(bool _IsActive, string txt)
    {
        mBtnTxt.text = txt;
        if (_IsActive)
        {
            mBtnImage.sprite = mActiveSprite;
        }
        else
        { 
            mBtnImage.sprite = mNormalSprite;

        }
    }
}
