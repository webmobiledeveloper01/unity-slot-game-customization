using UnityEngine;
using UnityEngine.UI;

public class PanelManagement : MonoBehaviour
{
    public Transform panelMenu, panelSettings, panelPaytable, panelRules, panelHistory;
    
    public Button btnMenu;
    public Button btnTurbo;
    public Button btnAutoSpin;

    public Button btnQuit;
    public Button btnSoundOpen;
    public Button btnSoundClose;
    public Button btnPaytable;
    public Button btnRules;
    public Button btnHistory;
    public Button btnCloseSettings;
    public Button btnClosePaytable;
    public Button btnCloseRules;

    // Start is called before the first frame update
    void Start()
    {
        //DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        //panelMenu.DOMove(new Vector3(0, 2, 0), 1).SetRelative().SetLoops(-1, LoopType.Yoyo);


        // Add Listeners for buttons on MenuPanel
        Button btnMenu1 = btnMenu.GetComponent<Button>();
        btnMenu1.onClick.AddListener(ShowSettingsPanel);


        // Add Listeners for buttons for SettingsPanel

        Button btnQuit1 = btnQuit.GetComponent<Button>();
        btnQuit1.onClick.AddListener(QuitGame);


        Button btnSoundOpen1 = btnSoundOpen.GetComponent<Button>();
        btnSoundOpen1.onClick.AddListener(MuteSound);

        Button btnSoundClose1 = btnSoundClose.GetComponent<Button>();
        btnSoundClose1.onClick.AddListener(PlaySound);

        Button btnPaytable1 = btnPaytable.GetComponent<Button>();
        btnPaytable1.onClick.AddListener(ShowPaytable);

        Button btnClosePaytable1 = btnClosePaytable.GetComponent<Button>();
        btnClosePaytable1.onClick.AddListener(HidePaytable);


        //  Show / Hide Rules Panel
        Button btnRules1 = btnRules.GetComponent<Button>();
        btnRules1.onClick.AddListener(ShowRulesPanel);

        Button btnCloseRules1 = btnCloseRules.GetComponent<Button>();
        btnCloseRules1.onClick.AddListener(HideRulesPanel);



        Button btnHistory1 = btnHistory.GetComponent<Button>();
        btnHistory1.onClick.AddListener(ShowHistory);


        Button btnCloseSettings1 = btnCloseSettings.GetComponent<Button>();
        btnCloseSettings1.onClick.AddListener(ShowMenuPanel);

    }

    void ShowSettingsPanel() 
    {
        panelMenu.gameObject.SetActive(false);
        panelSettings.gameObject.SetActive(true);
    }



    //===========================================//
    void QuitGame()
    {
        Application.Quit();
    }

    void MuteSound() 
    {
        btnSoundClose.gameObject.SetActive(true);
        btnSoundOpen.gameObject.SetActive(false);
    }

    void PlaySound()
    {
        btnSoundClose.gameObject.SetActive(false);
        btnSoundOpen.gameObject.SetActive(true);
    }

    void ShowPaytable()
    {
        panelPaytable.gameObject.SetActive(true);
    }

    void HidePaytable() 
    {
        panelPaytable.gameObject.SetActive(false);
    }

    void ShowRulesPanel() 
    {
        panelRules.gameObject.SetActive(true);
    }

    void HideRulesPanel() 
    {
        panelRules.gameObject.SetActive(false);
    }
    
    void ShowHistory() 
    {
    
    }

    void ShowMenuPanel() 
    {
        panelMenu.gameObject.SetActive(true);
        panelSettings.gameObject.SetActive(false);
    }
}
