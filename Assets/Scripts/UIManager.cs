
#if UNITY_EDITOR
#define DEBUG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum UIType
{
   WelcomeUI,
   CheckConnectivityUI,
   LoginUI,
   LobbyUI,
   ShopUI,
   InventoryUI,
   SettingUI,
   GameUI
}

public enum ShopType
{
    ShopOption1,
    ShopOption2,
    ShopOption3
}

public enum LanguageType
{
    English,
    French,
    Arab,
}

public enum GiftItemState
{
    LOCKED,
    UNLOCKED,
    GRANTED
}
[System.Serializable]
public struct ItemGift{
    public GameObject lockedGO;
    public Text day;
    public Sprite itemSprite;
    public GiftItemState state;
};

//struct element need that yeah public
public struct UiUser
{
    public LanguageType language;
    public UIType uiState;
    public ShopType shopOption;
    public float musicVolume;
    public float soundEffectVolume;
    public float voiceVolume;
    public int currentItemIndex;
    public int currentOptionIndex;
}

public class UIManager : InternetCheck
{
    //UI Element
    
    //Needeed Data
    [SerializeField] Sprite[] uiUserProfileAvatarSprites;
    [SerializeField] Sprite[] uiUserProfileRankSprites;

    //UI Login State Elements
    [SerializeField] Toggle licenceAcceptToggle;
    [SerializeField] Toggle endUserAcceptToggle;
    [SerializeField] Image userProfileAvatarImage;
    [SerializeField] Text userProfileNameText;
    [SerializeField] Image userProfileRankImage;
    [SerializeField] Text userProfileLevelText;
    //UI Lobby State Elements
    [SerializeField] Image batteryLife;
    [SerializeField] Image internetIntensity;

    //UI Setting State Elements
    // ---- Audio Volumes
    [SerializeField] Text musicVolumeText;
    [SerializeField] Text soundEffectVolumeText;
    [SerializeField] Text voiceVolumeText;

    //UI Inventory Elements
    [SerializeField] List<GameObject> inventoryItems;
    [SerializeField] GameObject inventoryItemDefault;
    [SerializeField] GameObject inventoryItemOptionContainer;
    [SerializeField] GameObject inventoryItemOption;
    
    //List the option For each item
    List<GameObject> aircraftOptions;
    List<GameObject> canonOptions;
    List<GameObject> missileOptions;
    List<GameObject> agmOptions;
    List<GameObject> colorOptions;
    List<GameObject> flagOptions;
    List<GameObject> reactorOptions;
    List<GameObject> wingOptions;
    List<GameObject> armorOptions;


    [SerializeField] Sprite[] itemsSprites;
    [SerializeField] Sprite itemDefaultSprite;
    [SerializeField] Sprite itemSelectedSprite;
    [SerializeField] Sprite optionDefaultSprite;
    [SerializeField] Sprite optionSelectedSprite;

   

    [SerializeField] Text itemOptionTitle;
    
    [SerializeField] GameObject[] aircraftModels;
    [SerializeField] GameObject[] canonModels;
    [SerializeField] GameObject[] missileModels;
    [SerializeField] GameObject[] agmModels;
    [SerializeField] GameObject[] colorModels;
    [SerializeField] GameObject[] flagModels;
    [SerializeField] GameObject[] reactorModels;
    [SerializeField] GameObject[] wingModels;
    [SerializeField] GameObject[] armorModels;

    GameObject currentModel;
    bool mouseOverModelView = false;


    [Header("WindowPanels")]
    [Header("Welcome      WindowPanels")]
    [SerializeField] GameObject welcomePanel;
    [SerializeField] GameObject welcomeDefaultPanel;
    [Header("Internet Connection Check      WindowPanels")]
    [SerializeField] GameObject internetCheckPanel;
    [SerializeField] GameObject internetCheckDefaultPanel;
    [SerializeField] GameObject noInternetWindow;
    [Header("Login      WindowPanels")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject loginDefaultPanel;
    [SerializeField] GameObject languageWindow;
    [Header("ToolsWindowPopup")]
    [SerializeField] GameObject helpWindow;
    [SerializeField] GameObject repairWindow;
    [Header("LicenceWindowPopup")]
    [SerializeField] GameObject licenceWindow;
    [SerializeField] GameObject endUserWindow;
    [SerializeField] GameObject noticeWindow;
    [SerializeField] GameObject privacyPolicyWindow;
    [SerializeField] GameObject userAgreementWindow;
    [Header("Lobby      Window Panel")]
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject lobbyDefaultPanel;
    [Header("Inventory      Window Panel")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject inventoryDefaultPanel;
    [Header("Character Choice      WindowPopup")]
    [SerializeField] GameObject characterChoicePanel;
    [Header("Settings      Window Panel")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject settingsDefaultPanel;
    [SerializeField] GameObject controlsWindow;
    [SerializeField] GameObject videoWindow;
    [SerializeField] GameObject audioWindow;
    [SerializeField] GameObject gamePlayWindow;
    [SerializeField] GameObject othersWindow;
    [Header("Chop      Window Panel")]
    [SerializeField] GameObject chopPanel;
    [Header("Map Selection      Window Panel")]
    [SerializeField] GameObject mapSelectionPanel;
    [Header("Profile      Window Elements")]
    [SerializeField] GameObject profilePanel;
    //  [SerializeField] GameObject playerProfile;
    //  [SerializeField] GameObject playerCurrency;

    [Header("Wheel      Window Elements")]
    [SerializeField] GameObject spinWheelGO;
    int nbSpinItems;

    [Header("Gift      Window Elements")]
    [SerializeField] GameObject giftWindowPanel;
    [SerializeField] ItemGift[] giftItems;


    /*
    [SerializeField] GameObject giftWindowGO;
    [SerializeField] Image giftImage;
    [SerializeField] Text giftHeaderText;
    [SerializeField] Text giftMsgText;
    */

    //Touch Input variables
    float speed;
    Vector2 rotSpeed;
    float scaleSpeed = 0.1f;
    float scaleMinBound = 0.1f;
    float scaleMaxBound = 5.0f;
    private float lerpSpeed = .1f;

    public static new UIManager Instance { get; set; }
    UiUser uiUser;
    LanguageType selectedLanguage;
    float pingTime;

    //Coroutines
    Coroutine pingCoroutine;

    protected new void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }

        uiUser.uiState = UIType.InventoryUI;//For testing 
        selectedLanguage = LanguageType.English;
        pingTime = -1f;
 
        UpdateUI();
        InvokeRepeating("BatteryLifeUpdate", 1f, 60f);//60s
        InvokeRepeating("InternetIntensityCalc", 1f, 2f);
       // InvokeRepeating("CheckIntenetReachability", 1f, 2f);

        nbSpinItems = 12;

        PlayerPrefs.DeleteAll();
        UpdateItemState();
    }

    private void Start()
    {
        LoadPlayerItemsData();
        LoadPlayerSettings();
    }

   


    #region Check Internet Reachability and overrided methods
    public override void OnConnectionCheckComplete()
    {
        Debug.Log("What is happening");
        base.OnConnectionCheckComplete();
        if (!base.isConnected)
        {
           uiUser.uiState = UIType.CheckConnectivityUI;
           UpdateUI();
        }
    }

    public override void OnConnectionStatusChange()
    {
        base.OnConnectionStatusChange();
        Debug.Log("Connection change called in UI Manager");
    }
    #endregion

    #region User Phone Thins

    void BatteryLifeUpdate()
    {
        batteryLife.fillAmount = SystemInfo.batteryLevel;
        if (SystemInfo.batteryLevel < 0.0f){ return; }
        if (batteryLife.fillAmount < .25f)
        {
            batteryLife.color = new Color(1f, 0, 0);
        }else if (batteryLife.fillAmount < .50f)
        {batteryLife.color = new Color(1f, 0.5f, 0);}
        else
        { batteryLife.color = new Color(0, 1.0f, 0f);}
    }

    void InternetIntensityCalc()
    {
        if (pingTime > 0f)
        {
            if (pingTime < 150f)
            {
                internetIntensity.fillAmount = 1f;
                internetIntensity.color = new Color(0, 1f, 0f);
            }
            else if (pingTime < 300f)
            {
                internetIntensity.fillAmount = .75f;
                internetIntensity.color = new Color(1f, 1f, 0);
            }
            else if (pingTime < 500f)
            {
                internetIntensity.fillAmount = .5f;
                internetIntensity.color = new Color(1, .5f, 0);
            }
            else if (pingTime < 600f)
            {
                internetIntensity.fillAmount = .25f;
                internetIntensity.color = new Color(1, 0f, 0);
            }
            else
            {
                internetIntensity.fillAmount = .01f;
                internetIntensity.color = new Color(1, 0f, 0);
            }
        }
        else
        {
            internetIntensity.fillAmount = .01f;
            internetIntensity.color = new Color(1, 0f, 0);
        }
       if (pingCoroutine == null){pingCoroutine = StartCoroutine(PingCall());}
        StartCoroutine(PingCall());
    }

    IEnumerator PingCall()
    {
        var ping = new Ping("172.217.18.228"); // Google's Ip Adrress
        while (!ping.isDone) yield return null;
        pingTime = ping.time;
    }

    #endregion

    #region UserProfile In Lobby
    void SetUserProfile()
    {
        //Set the player name , level and rank and image (image from an array of sprites)
        userProfileAvatarImage.sprite = uiUserProfileAvatarSprites[0];
        userProfileNameText.text = CloudManager.CM.serverLoadedData.playerstatistics.playername;
        userProfileLevelText.text = CloudManager.CM.serverLoadedData.playerstatistics.playerlevel.ToString();
        userProfileRankImage.sprite = uiUserProfileRankSprites[0];
    }
    #endregion

    #region Load User PlayPrefs Here
    public void LoadPlayerSettings()
    {
        uiUser.musicVolume = PlayerPrefs.GetFloat("musicVolume", .50f);
        uiUser.soundEffectVolume = PlayerPrefs.GetFloat("soundEffectVolume", .50f);
        uiUser.voiceVolume = PlayerPrefs.GetFloat("voiceVolume", .50f);
        ///And Any Other Settings

        musicVolumeText.text = (uiUser.musicVolume * 100).ToString("0");
        soundEffectVolumeText.text = (uiUser.soundEffectVolume * 100).ToString("0");
        voiceVolumeText.text = (uiUser.voiceVolume * 100).ToString("0");
    }
    #endregion

    #region Load Inventroy Data

   
    public void GetInventoryItem(int index)
    {
        List<CloudManager.PlayerData> itemdata;

        if (index == 0)
            itemdata = CloudManager.CM.serverLoadedData.canondata;
        else if (index == 1)
            itemdata = CloudManager.CM.serverLoadedData.agmdata;
        else if (index == 2)
            itemdata = CloudManager.CM.serverLoadedData.armordata;
        else
            itemdata = CloudManager.CM.serverLoadedData.flagdata;
        foreach (CloudManager.PlayerData option in itemdata)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = option.ItemId;
                optionButton.transform.GetChild(0).GetComponent<Text>().text = option.name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
        }

    }

    private void LoadPlayerItemsData()
   {
        int optionsCount;



        aircraftOptions = new List<GameObject>();
        canonOptions = new List<GameObject>();
        missileOptions = new List<GameObject>();
        agmOptions = new List<GameObject>();
        colorOptions = new List<GameObject>();
        armorOptions = new List<GameObject>();
        reactorOptions = new List<GameObject>();
        flagOptions = new List<GameObject>();
        wingOptions = new List<GameObject>();
        //Load Aircraft Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.aircraftdata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.aircraftdata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            aircraftOptions.Add(optionButton);
        }
        //Load Canon Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.canondata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.canondata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            canonOptions.Add(optionButton);
        }
        //Load Missile Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.missiledata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.missiledata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            missileOptions.Add(optionButton);
        }
        //Load Agm Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.agmdata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.agmdata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            agmOptions.Add(optionButton);
        }
        //Load Color Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.colordata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.colordata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            canonOptions.Add(optionButton);
        }
        //Load Flag Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.flagdata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.flagdata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            flagOptions.Add(optionButton);
        }
        //Load Reactor Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.reactordata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.reactordata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            reactorOptions.Add(optionButton);
        }
        //Load Wing Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.wingdata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.wingdata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            wingOptions.Add(optionButton);
        }
        //Load Armor Item and it's Options
        optionsCount = CloudManager.CM.serverLoadedData.armordata.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            var optionButton = Instantiate(inventoryItemOption, inventoryItemOptionContainer.transform);
            {
                optionButton.name = i.ToString();
                optionButton.transform.GetChild(0).GetComponent<Text>().text = CloudManager.CM.serverLoadedData.armordata[i].name;
                optionButton.GetComponent<Button>().onClick.AddListener(OnInventoryItemOptionBtnClicked);
            }
            armorOptions.Add(optionButton);
        }

        inventoryItemDefault.GetComponent<Button>().image.sprite = itemSelectedSprite;
        SetInventorySelectedItem(0);
        inventoryItemOption.SetActive(false);
        //For testing we need Reset PlayerPrefs 
        PlayerPrefs.DeleteAll();
        //Remove the line above !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        uiUser.currentItemIndex = 0;
        uiUser.currentOptionIndex = 0;
        int selectedOption = PlayerPrefs.GetInt("aircraft", 0);
        aircraftOptions[selectedOption].GetComponent<Button>().image.sprite = optionSelectedSprite;
        SetInventorySelectedItemOption(0);
    }


    #endregion
    #region UpdateUI function
    public void UpdateUI()
    {
        welcomePanel.SetActive(false);
        internetCheckPanel.SetActive(false);
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        inventoryDefaultPanel.SetActive(false);
        characterChoicePanel.SetActive(false);
        chopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mapSelectionPanel.SetActive(false);
        profilePanel.SetActive(false);
        switch (uiUser.uiState)
        {
            case UIType.WelcomeUI:
                {
                    welcomePanel.SetActive(true);
                    welcomeDefaultPanel.SetActive(true);
                }
                break;
            case UIType.CheckConnectivityUI:
                {
                    internetCheckPanel.SetActive(true);
                    internetCheckDefaultPanel.SetActive(true);
                }
                break;
            case UIType.LoginUI:
                {
                    loginPanel.SetActive(true);
                    loginDefaultPanel.SetActive(true);
                    languageWindow.SetActive(false);
                    helpWindow.SetActive(false);
                    repairWindow.SetActive(false);
                    licenceWindow.SetActive(false);
                    endUserWindow.SetActive(false);
                    noticeWindow.SetActive(false);
                    privacyPolicyWindow.SetActive(false);
                    userAgreementWindow.SetActive(false);
                }
                break;
            case UIType.LobbyUI:
                {
                    lobbyPanel.SetActive(true);
                    lobbyDefaultPanel.SetActive(true);
                    profilePanel.SetActive(true);
                }
                break;
            case UIType.SettingUI:
                {
                    settingsPanel.SetActive(true);
                    settingsDefaultPanel.SetActive(true);
                    controlsWindow.SetActive(true);
                    videoWindow.SetActive(false);
                    audioWindow.SetActive(false);
                    gamePlayWindow.SetActive(false);
                    othersWindow.SetActive(false);
                }
                break;
            case UIType.InventoryUI:
                {
                    inventoryPanel.SetActive(true);
                    inventoryDefaultPanel.SetActive(true);
                    profilePanel.SetActive(true);
                }
                break;
            default:
                Debug.LogError(" Trying to Update unavailable UI");
                break;
        }
    }
    #endregion
    #region Connection Check Buttons
    public void OnRetryConnectionCheckBtnPressed()
    {
        if (base.isConnected)
        {
            uiUser.uiState = UIType.LoginUI;
            UpdateUI();
        }
       else
        {
            //What to do ????
        }
    }
    #endregion

    #region Login Buttons
    public void OnHelpBtnPressed() {
        UpdateUI();
        if (uiUser.uiState == UIType.LoginUI){} //Set specific ...
        helpWindow.SetActive(true);
    }

    public void OnLanguageBtnPressed()
    {
        UpdateUI();
        if (uiUser.uiState == UIType.LoginUI) { } //Set specific ...
        languageWindow.SetActive(true);
    }

    public void OnRepairBtnPressed()
    {
        UpdateUI();
        if (uiUser.uiState == UIType.LoginUI) { } //Set specific ...
        repairWindow.SetActive(true);
    }


    public void OnPrivacyPolicyBtnPressed()
    {
        UpdateUI();
        if (uiUser.uiState == UIType.LoginUI) { } //Set specific ...
        privacyPolicyWindow.SetActive(true);
    }

    public void OnUserAgreementBtnPressed()
    {
        UpdateUI();
        if (uiUser.uiState == UIType.LoginUI) { } //Set specific ...
        userAgreementWindow.SetActive(true);
    }

    public void OnlanguageTabSelected(int current){selectedLanguage = (LanguageType)current;}
    public void OnlanguageSelected(){
        uiUser.language = selectedLanguage;
        UpdateUI();
    }

    public void OnLicenceAgreementConfirmBtnPressed()
    {
        licenceAcceptToggle.isOn = true;
        licenceWindow.SetActive(false);
    }

    public void OnLicenceAgreementRefuseBtnPressed()
    {
        licenceAcceptToggle.isOn = false;
        licenceWindow.SetActive(false);
        noticeWindow.SetActive(true);
    }

    public void OnEndUserAgreementConfirmBtnPressed()
    {
        endUserAcceptToggle.isOn = true;
        endUserWindow.SetActive(false);
    }

    public void OnEndUserAgreementRefuseBtnPressed()
    {
        endUserAcceptToggle.isOn = false;
        endUserWindow.SetActive(false);
        noticeWindow.SetActive(true);  
    }

    public void OnNoticeLicenseReconsiderBtnPressed()
    {
        noticeWindow.SetActive(false);
        CheckLicencesAgreement();
    }
    public void OnNoticeLicenseDisagreeBtnPressed()
    {
        noticeWindow.SetActive(false);
        UpdateUI();
    }

    public void OnFacebookLoginBtnPressed()
    {
        CheckLicencesAgreement();
        if (licenceAcceptToggle.isOn && endUserAcceptToggle.isOn)
        {
            Debug.Log("Login Facebook");
        }
    }

    public void OnGuessLoginBtnPressed()
    {
        CheckLicencesAgreement();
        if (licenceAcceptToggle.isOn && endUserAcceptToggle.isOn)
        {
            Debug.Log("Login Guest");
        }
    }

    #endregion

    #region Lobby Buttons
    
    public void OnLobbyUserProfileBtnPressed()
    {
        uiUser.uiState = UIType.SettingUI;
    }

    public void OnLobbySettingsBtnPressed()
    {
        uiUser.uiState = UIType.SettingUI;
        UpdateUI();
    }

    public void OnLobbyCurrencyBtnPressed()
    {
        uiUser.uiState = UIType.SettingUI;
    }

    public void OnLobbyOffersBtnPressed(int index)
    {
        int offer = index;
        uiUser.uiState = UIType.SettingUI;
    }

    public void OnLobbyShopBtnPressed(int optionIndex)
    {
      //  int optionIndex = int.Parse( EventSystem.current.currentSelectedGameObject.name);
        uiUser.uiState = UIType.ShopUI;
        uiUser.shopOption = (ShopType)optionIndex;
    }

    public void OnLobbyInventroryBtnPressed()
    {
        uiUser.uiState = UIType.InventoryUI;
        UpdateUI();
    }

    public void OnStartBtnPressed()
    {
        uiUser.uiState = UIType.GameUI;
        UpdateUI();
    }
    #endregion

    #region Settings Button and Sliders

    public void OnSettingSelectedUpdate()
    {
        controlsWindow.SetActive(false);
        videoWindow.SetActive(false);
        audioWindow.SetActive(false);
        gamePlayWindow.SetActive(false);
        othersWindow.SetActive(false);
    }

    public void OnSettingsControlBtnPressed()
    {
        OnSettingSelectedUpdate();
        controlsWindow.SetActive(true);
    }
    public void OnSettingsVideoBtnPressed()
    {
        OnSettingSelectedUpdate();
        videoWindow.SetActive(true);
    }
    public void OnSettingsAudioBtnPressed()
    {
        OnSettingSelectedUpdate();
        audioWindow.SetActive(true);
    }
    public void OnSettingsGamePlayBtnPressed()
    {
        OnSettingSelectedUpdate();
        gamePlayWindow.SetActive(true);
    }
    public void OnSettingsOthersBtnPressed()
    {
        OnSettingSelectedUpdate();
        othersWindow.SetActive(true);
    }

    public void OnSettingsWindowClose()
    {
        uiUser.uiState = UIType.LobbyUI;
        UpdateUI();
    }

    public void OnSettingMusicVolumeChange(float value)
    {
        uiUser.musicVolume = value;
        musicVolumeText.text = (value * 100).ToString("0");
    }
    public void OnSettingSoundEffectVolumeChange(float value)
    {
        uiUser.soundEffectVolume = value;
        soundEffectVolumeText.text = (value * 100).ToString("0");
    }
    public void OnSettingVoiceVolumeChange(float value)
    {
        uiUser.voiceVolume = value;
        voiceVolumeText.text = (value * 100).ToString("0");
    }
    #endregion

    #region Inventory Buttons

    private void OnInventorySelectReset()
    {
        aircraftOptions.ForEach((go) => { go.SetActive(false); });
        canonOptions.ForEach((go) => { go.SetActive(false); });
        missileOptions.ForEach((go) => { go.SetActive(false); });
        agmOptions.ForEach((go) => { go.SetActive(false); });
        colorOptions.ForEach((go) => { go.SetActive(false); });
        flagOptions.ForEach((go) => { go.SetActive(false); });
        reactorOptions.ForEach((go) => { go.SetActive(false); });
        wingOptions.ForEach((go) => { go.SetActive(false); });
        armorOptions.ForEach((go) => { go.SetActive(false); });
    }

    private void OnInventorySelectUpdate(List<CloudManager.PlayerData> itemOptions, List<GameObject> optionsGO)
    {
        int optionsCount = 0;
        bool selectedOptionSet = false;
       
        optionsCount = itemOptions.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            if (itemOptions[i].unlocked)
            {
                optionsGO[i].SetActive(true);
                if (!selectedOptionSet)
                {
                    if (uiUser.currentOptionIndex == 0 || uiUser.currentOptionIndex == i)
                    {
                        optionsGO[i].GetComponent<Button>().image.sprite = optionSelectedSprite;
                        selectedOptionSet = true;
                    }
                }
            }
        }
    }


    private void SetInventorySelectedItem(int itemIndex)
    {
       // OnInventorySelectReset();
        switch (itemIndex)
        {
            case 0:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("aircraft", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.aircraftdata, aircraftOptions);
                }
                break;
            case 1:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("canon", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.canondata, canonOptions);
                }
                break;
            case 2:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("missile", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.missiledata, missileOptions);
                }
                break;

            case 3:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("agm", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.agmdata, agmOptions);
                }
                break;
            case 4:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("color", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.colordata, colorOptions);
                }
                break;
            case 5:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("flag", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.flagdata, flagOptions);
                }
                break;
            case 6:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("reactor", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.reactordata, reactorOptions);
                }
                break;
            case 7:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("wing", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.wingdata, wingOptions);
                }
                break;
            case 8:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("armor", 0);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.armordata, armorOptions);
                }
                break;
            default:break;
        }
        SetInventorySelectedItemOption(0);
    }

    private void SetAndSaveItemSelectedOption(List<GameObject> optionsGOS, string keyName, int optionIndex)
    {
        optionsGOS.ForEach((itemOption) => { itemOption.GetComponent<Button>().image.sprite = optionDefaultSprite; });
        optionsGOS[optionIndex].GetComponent<Button>().image.sprite = optionSelectedSprite;
        PlayerPrefs.SetInt(keyName, optionIndex);
    }

    private void SetInventorySelectedItemOption(int optionIndex)
    {
        Array.ForEach(aircraftModels, (go) => { go.SetActive(false); });
        Array.ForEach(canonModels, (go) => { go.SetActive(false); });
        Array.ForEach(missileModels, (go) => { go.SetActive(false); });
        Array.ForEach(agmModels, (go) => { go.SetActive(false); });
        Array.ForEach(colorModels, (go) => { go.SetActive(false); });
        Array.ForEach(flagModels, (go) => { go.SetActive(false); });
        Array.ForEach(reactorModels, (go) => { go.SetActive(false); });
        Array.ForEach(wingModels, (go) => { go.SetActive(false); });
        Array.ForEach(armorModels, (go) => { go.SetActive(false); });
        switch (uiUser.currentItemIndex)
        {
            case 0:
                {
                    currentModel = aircraftModels[optionIndex];
                    SetAndSaveItemSelectedOption(aircraftOptions, "aircraft", optionIndex);
                }
                break;
            case 1:
                {
                    currentModel = canonModels[optionIndex];
                    SetAndSaveItemSelectedOption(canonOptions, "canon", optionIndex);
                }
                break;
            case 2:
                {
                    currentModel = missileModels[optionIndex];
                    SetAndSaveItemSelectedOption(missileOptions, "missile", optionIndex);
                }
                break;
            case 3:
                {
                    currentModel = agmModels[optionIndex];
                    SetAndSaveItemSelectedOption(agmOptions, "agm", optionIndex);
                }
                break;
            case 4:
                {
                    currentModel = colorModels[optionIndex];
                    SetAndSaveItemSelectedOption(colorOptions, "color", optionIndex);
                }
                break;
            case 5:
                {
                    currentModel = flagModels[optionIndex];
                    SetAndSaveItemSelectedOption(flagOptions, "flag", optionIndex);
                }
                break;
            case 6:
                {
                    currentModel = reactorModels[optionIndex];
                    SetAndSaveItemSelectedOption(reactorOptions, "reactor", optionIndex);
                }
                break;
            case 7:
                {
                    currentModel = wingModels[optionIndex];
                    SetAndSaveItemSelectedOption(wingOptions, "wing", optionIndex);
                }
                break;
            case 8:
                {
                    currentModel = armorModels[optionIndex];
                    SetAndSaveItemSelectedOption(armorOptions, "armor", optionIndex);
                }
                break;
            default:break;
        }
        currentModel.SetActive(true);
    }

    public void OnInventoryItemBtnClicked(int index)
    {
        inventoryItems.ForEach((go) => { go.GetComponent<Button>().image.sprite = itemDefaultSprite; });
        inventoryItems[index].GetComponent<Button>().image.sprite = itemSelectedSprite;
        uiUser.currentItemIndex = index;
        SetInventorySelectedItem(index);
    }

    public void OnInventoryItemOptionBtnClicked()
    {

        var name = EventSystem.current.currentSelectedGameObject.name;
        // Debug.LogFormat("Button : {0}", name);
        itemOptionTitle.text = "Item " + name;
        int index = int.Parse(name);
        SetInventorySelectedItemOption(index);
    }


    public void OnMouseOverModelView(bool value)
    {
        mouseOverModelView = value;
    }
    #endregion

    void CheckLicencesAgreement()
    {
        if (licenceAcceptToggle.isOn == false) { licenceWindow.SetActive(true); }
        if (endUserAcceptToggle.isOn == false) { endUserWindow.SetActive(true); }
    }

    #region model Viewer
    private Vector2 RotAngle = Vector2.zero;
    void processDesktopInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        speed = 200f;
        lerpSpeed = 2f;
        rotSpeed = new Vector2(-mouseX, mouseY);

        if (!Input.GetMouseButton(0) && ! Input.GetMouseButton(1))
        {
            RotAngle = Vector2.Lerp(RotAngle, Vector2.zero, Time.deltaTime * lerpSpeed);
        }
        else
        {
            Vector2 toRotation;
            if (!mouseOverModelView) { return; }
            toRotation =  RotAngle + rotSpeed * speed * Time.deltaTime;
            RotAngle = Vector2.Lerp(RotAngle, toRotation, Time.deltaTime * lerpSpeed * 50);
        }
        RotAngle.x = Mathf.Clamp(RotAngle.x, -179f, 179f);
        RotAngle.y = Mathf.Clamp(RotAngle.y, -90f, 90f);
        currentModel.transform.rotation = Quaternion.Euler(RotAngle.y, RotAngle.x, 0);
    }


void Scale(float delta, float speed)
    {
        currentModel.transform.localScale -= Vector3.one * delta * speed * Time.deltaTime;
        currentModel.transform.localScale = new Vector3(Mathf.Clamp(currentModel.transform.localScale.x, scaleMinBound, scaleMaxBound),
            Mathf.Clamp(currentModel.transform.localScale.y, scaleMinBound, scaleMaxBound),
            Mathf.Clamp(currentModel.transform.localScale.z, scaleMinBound, scaleMaxBound));
    }

    void UpdateEvents()
    {   
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            processDesktopInput();
        }
        else
        {
            speed = 10f;
            lerpSpeed = 2f;
            
            if (Input.touchCount == 2)
            {
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                if (!mouseOverModelView) { return; }
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;
                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Scale(deltaDistance, scaleSpeed);
            }
            else if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                {
                    Vector2 touchPrevious = touch.position - touch.deltaPosition;
                    rotSpeed = touch.position - touchPrevious;
                    Vector2 toRotation = RotAngle + rotSpeed * speed * Time.deltaTime;
                    RotAngle = Vector2.Lerp(RotAngle, toRotation, Time.deltaTime * lerpSpeed * 5f);
                }
            }
            else
            {
                RotAngle =Vector2.Lerp(RotAngle, Vector2.zero, Time.deltaTime * lerpSpeed * 5);
            }
            RotAngle.x = Mathf.Clamp(RotAngle.x, -179f, 179f);
            RotAngle.y = Mathf.Clamp(RotAngle.y, -90f, 90f);
            currentModel.transform.rotation = Quaternion.Euler(RotAngle.y, -RotAngle.x, 0);
        }
    }
    #endregion

    #region Spining Wheel

    float spinSpeed = 2000f;
    bool spinFinished = false;
    float angleSpin = 0;
    int result = 0;

    IEnumerator SpinRotator(Action method)
    {
        float setTime = UnityEngine.Random.Range(0.5f, 0.8f); 
    //    float setTime = UnityEngine.Random.Range(0.1f, 0.2f); 
        float elapsedTime = 0;
        float currentSpinSpeed = 0f;

        spinFinished = false;
        currentSpinSpeed = spinSpeed;
        while (elapsedTime < setTime)
        {
            if (elapsedTime > .5f * setTime) 
            {currentSpinSpeed =  spinSpeed * .5f;}
            if (elapsedTime > .75f * setTime)
            {currentSpinSpeed = spinSpeed * .25f;}
            if (elapsedTime > .85f * setTime)
            { currentSpinSpeed = spinSpeed * .125f;}
            angleSpin +=  currentSpinSpeed * Time.deltaTime;
            spinWheelGO.transform.rotation = Quaternion.Euler(0, 0, angleSpin);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        spinFinished = true;
        method();
    }

    IEnumerator SpinRotatorA(Action method)
    {
        result = UnityEngine.Random.Range(1, 12);
        float rotAngle = (float)result * 30  + (UnityEngine.Random.Range(1, 4)) * 360;
        float currentSpinSpeed = 0f;
        Debug.Log("Rotation is " + rotAngle);
        angleSpin = 0;
        currentSpinSpeed = spinSpeed;
        while (angleSpin < rotAngle)
        {
            if (angleSpin > .5f * rotAngle)
            { currentSpinSpeed = spinSpeed * .5f; }
            if (angleSpin > .75f * rotAngle)
            { currentSpinSpeed = spinSpeed * .25f; }
            if (angleSpin > .85f * rotAngle)
            { currentSpinSpeed = spinSpeed * .125f; }
            angleSpin += currentSpinSpeed * Time.deltaTime;
            spinWheelGO.transform.rotation = Quaternion.Euler(0, 0, angleSpin);
            yield return new WaitForSeconds(0.1f);
        }
        spinFinished = true;
        method();
    }

    int SpinWheel()
    {
     
        Action printSomthing = () =>
        {
           // DisplayGift();
            Debug.LogFormat("Choosen index {0}", 1 + result);
        };

        StartCoroutine("SpinRotatorA", printSomthing);
        return result;
    }

    #endregion

    #region Daily Gift System
    DateTime lastDay;
    DateTime toDay = DateTime.Now;

    bool granted = false;

    int  LoadPlayerSavedItem()
    {
        var lastDateString = PlayerPrefs.GetString("lastDay");
        var lastItem = PlayerPrefs.GetInt("lastItem", 0);
        if (String.IsNullOrEmpty(lastDateString))
        { return -1;}


        lastDay = DateTime.Parse(lastDateString);
        TimeSpan elapsed = toDay - lastDay;
        if (elapsed.TotalHours >= 24)
        {
            granted = true;
        }
        return lastItem;
    }
    
    void SetPlayerGiftItem(int index)
    {
        for (int i = 0; i < index; i++)
        { giftItems[i].state = GiftItemState.GRANTED; }
        giftItems[index].state = GiftItemState.UNLOCKED;
        for (int i = giftItems.Length - 1; i > index; i--)
        { giftItems[i].state = GiftItemState.LOCKED; }
    }

    void UpdateItemState()
    {
        
        int lastIndex = LoadPlayerSavedItem();
       
        if (lastIndex == -1)
        {
            giftItems[0].state = GiftItemState.UNLOCKED;
            SetPlayerGiftItem(0);
        }
        else if (lastIndex >= 0&& lastIndex < 8 && granted)
        {
            giftItems[lastIndex + 1].state = GiftItemState.UNLOCKED;
        }
    }

    public void DisplayGiftWindow()
    {
        giftWindowPanel.SetActive(true);

        for (int i = giftItems.Length - 1; i >= 0; i--)
        {
            /*  if (giftItems[i].state == GiftItemState.LOCKED)
              {giftItems[i].lockedGO.SetActive(true);}
              else
              {giftItems[i].lockedGO.SetActive(false);}*/

            if (giftItems[i].state == GiftItemState.LOCKED)
            { giftItems[i].lockedGO.GetComponent<Text>().text = "LOCKED"; }
            else if (giftItems[i].state == GiftItemState.UNLOCKED)
            { giftItems[i].lockedGO.GetComponent<Text>().text = "UNLOCKED"; }
            else if (giftItems[i].state == GiftItemState.GRANTED)
            { giftItems[i].lockedGO.GetComponent<Text>().text = "GRANTED"; }
        }
    }


    public void OnDebugFastForwardDayBtnClicked()
    {
        toDay = toDay.AddDays(1);
        Debug.Log(toDay);
        UpdateItemState();
    }



    public void OnGiftItemBtnClicked(int index)
    {
        /*ItemGift currentGift = giftItems[index];
        if (currentGift.state == GiftItemState.UNLOCKED)
        {currentGift.state = GiftItemState.GRANTED;}*/


        ItemGift currentGift = giftItems[index];
        if (giftItems[index].state == GiftItemState.UNLOCKED)
        { 
            giftItems[index].state = GiftItemState.GRANTED;
            PlayerPrefs.SetString("lastDay", toDay.ToString());
            if (index == 7)
            {
                PlayerPrefs.DeleteKey("lastItem");
            }
            else
            {
                PlayerPrefs.SetInt("lastItem", index);
            }
        }   
    }
    /*
    void OnGiftWindowShow()
    {
        giftWindowGO.SetActive(true);
    }

    public void OnGiftWindowHide()
    {
        giftWindowGO.SetActive(false);
    }
    */
    #endregion

    private void Update()
    {
        if (uiUser.uiState == UIType.InventoryUI)
        {
            { UpdateEvents(); }         
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int index = SpinWheel();
            
        }
    }
}