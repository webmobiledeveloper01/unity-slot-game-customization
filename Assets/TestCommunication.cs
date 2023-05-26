using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCommunication : MonoBehaviour
{
    public UnityEngine.UI.Text mText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            mText.text = Application.absoluteURL;
        }
    }
}
