using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextAnimation : MonoBehaviour
{
    public float stopDelay = 4f;
    public float moveDelay = 10f;
    public float deltaPosition = 0.1f;

    public GameObject[] marqueeObjects; 

    private int curObjectIndex = 0;
    private float _timer = 0f;

    String[] texts = 
    {
        "WIN UP TO 32400WAYS!",
        "Multipler increases after every win!",
        "Get more Wilds with wilds-on-the-way!",
        "4 or more wilds triggers 10 or more free spins",
    };

    private void Start()
    {
        marqueeObjects[0].SetActive(true);
        for (int i = 1; i < marqueeObjects.Length; i++)
        {
            marqueeObjects[i].SetActive(false);
        }        
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (curObjectIndex > 0 && _timer >= stopDelay)
        {
            Vector3 pos = marqueeObjects[curObjectIndex].transform.position;
            pos.x -= deltaPosition;
            marqueeObjects[curObjectIndex].transform.position = pos;
        }

        //RectTransform rectCurrent = (RectTransform)marqueeObjects[curObjectIndex].transform;
        //RectTransform rect0 = (RectTransform)marqueeObjects[0].transform;

        //if (_timer >= stopDelay + (rectCurrent.rect.width - rect0.rect.width) / 10) 
        if(curObjectIndex == 0 && _timer >= stopDelay  || curObjectIndex > 0 && _timer >= stopDelay + moveDelay)
        {
            _timer = 0f;
            Vector3 pos = marqueeObjects[curObjectIndex].transform.position;
            pos.x = 0;
            marqueeObjects[curObjectIndex].transform.position = pos;

            curObjectIndex = (curObjectIndex + 1) % texts.Length;

            for (int i = 0; i < marqueeObjects.Length; i++)
            {
                if(i == curObjectIndex)
                    marqueeObjects[i].SetActive(true);
                else
                    marqueeObjects[i].SetActive(false);
            }
        }
    }
}
