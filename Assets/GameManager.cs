using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class GameManager : MonoBehaviour {

    public GameObject loginPanel, registerPanel, homePanel, settingPanel, loadingBar;
	public GameObject loadingPanel;
	float loadingValue;

    int currentCoin;
	// Use this for initialization
	void Start () {
       
        if (PlayerPrefs.GetInt("FromGamePlay", 0) == 1)
        {
            GoToHomePanel();
        }
        else
        {
            InvokeRepeating("StartLoading", 0.1f, 0.1f);
            loadingValue = 0f;
        }
        
    }

    // Update is called once per frame
  /*  void Update () {
		
	}*/

    public void GoToSetting()
    {
        settingPanel.SetActive(true);
        settingPanel.transform.Find("MusicToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Music", 1) == 1 ? false : true;
        settingPanel.transform.Find("SoundToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Sound", 1) == 1 ? false : true;
        settingPanel.transform.Find("VibrateToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("Vibrate", 1) == 1 ? false : true;
    }

	void StartLoading()
	{

		loadingPanel.transform.GetChild (0).GetComponent<Slider> ().value = loadingValue;
		loadingValue += 0.5f;
		if (loadingValue > 10f) {
			CancelInvoke ("StartLoading");
            GoToHomePanel ();
            GameObject.Find("NotificationCanvas").GetComponent<GameControllerExample>().GenerateUserID();
        }
	}

    public void GoToQuit()
    {
        Application.Quit();
    }

    public void GoToLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        homePanel.SetActive(false);
        if (PlayerPrefs.HasKey("UserEmail"))
        {
            loginPanel.transform.Find("EmailInputField").GetComponent<InputField>().text = PlayerPrefs.GetString("UserEmail");
            loginPanel.transform.Find("PasswordInputField").GetComponent<InputField>().text = PlayerPrefs.GetString("UserPass");
        }
    }

    public void GoToRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        homePanel.SetActive(false);
    }

    public void GoToHomePanel()
    {
        PlayerPrefs.SetInt("FromGamePlay", 0);
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void LoginBtn()
    {
 		StartCoroutine("GoForLogin");

    }

	IEnumerator GoForLogin()
	{
		string email = loginPanel.transform.Find("EmailInputField").GetComponent<InputField>().text;
		string password = loginPanel.transform.Find("PasswordInputField").GetComponent<InputField>().text;
		string deviceToken = PlayerPrefs.GetString("UserID", "");
        //Camera.main.GetComponent<CreateSig> ().token;

        string deviceType = "android";

		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			deviceType = "ios";
		}

		if (email.Equals(""))
		{
			loginPanel.transform.Find("InfoText").GetComponent<Text>().text = "Email can not be empty";
		}
		else if (password.Equals(""))
		{
			loginPanel.transform.Find("InfoText").GetComponent<Text>().text = "Password can not be empty";
		}
		loadingBar.SetActive (true);
		WWWForm form1 = new WWWForm ();
		form1.AddField ("email", email);
		form1.AddField ("password", password);
		form1.AddField ("device_token", deviceToken);
		form1.AddField ("deviceType", deviceType);
		Debug.Log (deviceToken);
	//	http://demo.mankiinfotech.com/fruitslot/api/user_login?email=vv@gmail.com&password=vv&device_token=sdsd&deviceType=android

		WWW loginDetail = new WWW("http://demo.mankiinfotech.com/fruitslot/api/user_login",form1);
		yield return loginDetail;

		//yield return new WaitForSeconds(1f);
		if (string.IsNullOrEmpty (loginDetail.error))
        {
			Debug.Log ("Form upload complete! : " + loginDetail.text);
           
            var N = JSONNode.Parse(loginDetail.text);
            if (N["message"].Value.Equals("Login Successfully"))
            {
                Debug.Log("Login Success");
                Debug.Log("AS : " + N["result"]["coin"].Value.ToString());
                PlayerPrefs.SetInt("CurrBalance", int.Parse(N["result"]["coin"].Value));
                PlayerPrefs.SetString("userID", N["result"]["id"].Value);
                PlayerPrefs.SetInt("WinningRate", int.Parse(N["result"]["winning_rate_final"].Value));
                PlayerPrefs.SetInt("WinningAmount", int.Parse(N["result"]["general_winning_price"].Value));

                if (loginPanel.transform.Find("RememberToggle").GetComponent<Toggle>().isOn)
                {
                    PlayerPrefs.SetString("UserEmail", email);
                    PlayerPrefs.SetString("UserPass", password);
                }
                SceneManager.LoadSceneAsync("Demo");
            }
            else
            {
                loginPanel.transform.Find("InfoText").GetComponent<Text>().text = "Invalid id or Password";
                Invoke("clearInfo", 5f);
                loadingBar.SetActive(false);
            }
        } else {
			Debug.Log ("Error : " + loginDetail.error);
			loadingBar.SetActive (false);
            loginPanel.transform.Find("InfoText").GetComponent<Text>().text = "Check Interner Connection";
            Invoke("clearInfo", 5f);
        }
    }

    void clearInfo()
    {
        loginPanel.transform.Find("InfoText").GetComponent<Text>().text = "";
    }

    public void SubmitBtn()
    {
		StartCoroutine("GoForRegister");
    }

	IEnumerator GoForRegister()
	{
		string name = registerPanel.transform.Find("NameInputField").GetComponent<InputField>().text;
		string email = registerPanel.transform.Find("EmailInputField").GetComponent<InputField>().text;
		string password = registerPanel.transform.Find("PasswordInputField").GetComponent<InputField>().text;
		string confirmPasswrod = registerPanel.transform.Find("ConfirmPasswordInputField").GetComponent<InputField>().text;

		if (name.Equals(""))
		{
			registerPanel.transform.Find("InfoText").GetComponent<Text>().text = "Name can not be empty";
		}
		else if (email.Equals(""))
		{
			registerPanel.transform.Find("InfoText").GetComponent<Text>().text = "Email can not be empty";
		}
		else if (password.Equals(""))
		{
			registerPanel.transform.Find("InfoText").GetComponent<Text>().text = "Password can not be empty";
		}
		else if (confirmPasswrod.Equals(""))
		{
			registerPanel.transform.Find("InfoText").GetComponent<Text>().text = "Confirm Password can not be empty";
		}
		if (password.Equals(confirmPasswrod))
		{
			// Submit Data
			WWWForm form1 = new WWWForm ();
			form1.AddField ("name", name);
			form1.AddField ("email", email);
			form1.AddField ("password", password);
			form1.AddField ("cpassword", confirmPasswrod);
			WWW registerDetail = new WWW("http://demo.mankiinfotech.com/fruitslot/api/user_register",form1);
			yield return registerDetail;

			if (string.IsNullOrEmpty (registerDetail.error)) {
				Debug.Log ("Form upload complete! : " + registerDetail.text);
			} else {
				Debug.Log ("Error : " + registerDetail.error);
			}
		}
		else
		{
			registerPanel.transform.Find("InfoText").GetComponent<Text>().text = "Passwod Not Metch";
		}

	}

    public void HandleToggle(GameObject toggleBtn)
    {
        Debug.Log("HandleToggle was called");

        switch (toggleBtn.name)
        {
            case "MusicToggle":
                if (toggleBtn.GetComponent<Toggle>().isOn)
                {
                    PlayerPrefs.SetInt("Music", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("Music", 1);
                }
                break;
            case "SoundToggle":
                if (toggleBtn.GetComponent<Toggle>().isOn)
                {
                    PlayerPrefs.SetInt("Sound", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("Sound", 1);
                }
                break;
            case "VibrateToggle":
                if (toggleBtn.GetComponent<Toggle>().isOn)
                {
                    PlayerPrefs.SetInt("Vibrate", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("Vibrate", 1);
                }
                break;
        }
    }
}
