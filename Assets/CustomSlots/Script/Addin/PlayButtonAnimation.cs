using System;
using System.Collections;
using System.Collections.Generic;
using CSFramework;
using DG.Tweening;
using Elona.Slot;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonAnimation : MonoBehaviour
{
    public Image imagePlayRotate;

    public int idleRotateSpeed = 2;
    public int playRotateSpeed = 30;

    void Start()
    {

    }

    void Update()
    {
        switch (transform.GetChild(0).GetComponent<Text>().text.ToLower())
        {
            case "start":
                imagePlayRotate.transform.Rotate(Vector3.back * idleRotateSpeed);
                break;
            case "stop":
                imagePlayRotate.transform.Rotate(Vector3.back * playRotateSpeed);
                break;
        }
    }
}