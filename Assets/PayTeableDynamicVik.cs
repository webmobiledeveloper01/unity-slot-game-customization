using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PayTeableDynamicVik : MonoBehaviour
{
    float mInitialValue;
    // Start is called before the first frame update
    void Awake()
    {
        mInitialValue = float.Parse(transform.GetComponent<Text>().text.Replace(',', '.'));
    }

    private void OnEnable()
    {
        transform.GetComponent<Text>().text = "" + string.Format("{0:0.00}", mInitialValue * SlotManager.SlotManagerInstance.elos.slot.gameInfo.bet);
    }
}
