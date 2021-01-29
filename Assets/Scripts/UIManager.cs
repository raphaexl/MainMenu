
#if UNITY_EDITOR
#define DEBUG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


#region enums
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
#endregion

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
    public int currentItemIndex;
    public int currentOptionIndex;
}

public class UIManager : InternetCheck
{
    [Header("Needed Data")]
    [SerializeField] Sprite[] uiUserProfileAvatarSprites;
    [SerializeField] Sprite[] uiUserProfileRankSprites;

    //UI Setting State Elements

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
    [SerializeField] Toggle licenceAcceptToggle;
    [SerializeField] Toggle endUserAcceptToggle;

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
    [SerializeField] Image batteryLife;
    [SerializeField] Image internetIntensity;
    [Header("Inventory      Window Panel")]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject inventoryDefaultPanel;
    //UI Inventory Elements
    [SerializeField] List<GameObject> inventoryItems;
    [SerializeField] GameObject inventoryItemDefault;

    [SerializeField] GameObject aircraftOptionsContainer;
    [SerializeField] GameObject canonOptionsContainer;
    [SerializeField] GameObject missileOptionsContainer;
    [SerializeField] GameObject agmOptionsContainer;
    [SerializeField] GameObject colorOptionsContainer;
    [SerializeField] GameObject flagOptionsContainer;
    [SerializeField] GameObject reactorOptionsContainer;
    [SerializeField] GameObject wingOptionsContainer;
    [SerializeField] GameObject armorOptionsContainer;
    //List the option For each item
    [SerializeField] GameObject[] aircraftOptions;
    [SerializeField] GameObject[] canonOptions;
    [SerializeField] GameObject[] missileOptions;
    [SerializeField] GameObject[] agmOptions;
    [SerializeField] GameObject[] colorOptions;
    [SerializeField] GameObject[] flagOptions;
    [SerializeField] GameObject[] reactorOptions;
    [SerializeField] GameObject[] wingOptions;
    [SerializeField] GameObject[] armorOptions;

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
    [Header("Character Choice      WindowPopup")]
    [SerializeField] GameObject characterChoicePanel;
    [Header("Settings      Window Panel")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject settingsDefaultPanel;
    [SerializeField] Sprite settingOptionSelectedSprite;
    [SerializeField] Sprite[] settingOptionDefaultSprite;
    [SerializeField] GameObject[] settingOptions;

    [SerializeField] GameObject mainSettingsWindow;
    [SerializeField] GameObject facebookIco;
    [SerializeField] GameObject guestIco;
    [SerializeField] Text accountText;
    [SerializeField] Toggle[] mainSettingsToggle;
    [SerializeField] Text cameraSettingFovText;
    [SerializeField] Slider cameraSettingFovSlider;
    [SerializeField] Dropdown langageDropDown;
    [SerializeField] Dropdown serverDropDown;

    [SerializeField] GameObject graphicsWindow;
    [SerializeField] Sprite graphicsDisplaySelectedSprite;
    [SerializeField] Sprite[] graphicsDisplayDefaultSprite;
    [SerializeField] GameObject[] graphicsDisplays;
    [SerializeField] Sprite graphicsFpsSelectedSprite;
    [SerializeField] Sprite[] graphicsFpsDefaultSprite;
    [SerializeField] GameObject[] graphicsFps;
    [SerializeField] Sprite graphicsStyleSelectedSprite;
    [SerializeField] Sprite[] graphicsStyleDefaultSprite;
    [SerializeField] GameObject[] graphicsStyle;
    [SerializeField] Toggle graphicShadowToggle;
    [SerializeField] Slider graphicBrightnessSlider;
    [SerializeField] Text graphicBrightnessText;
    private bool settingBtnHeld = false;
    [SerializeField] GameObject controlsWindow;
    [SerializeField] GameObject controlsDefaultWindow;
    [SerializeField] GameObject[] controlsOptions;
    [SerializeField] Sprite controlsOptionSelectedSprite;// Sprite
    [SerializeField] Sprite[] controlsOptionDefaultSprites;
    [SerializeField] GameObject controlsAccJoysWindow;
    [SerializeField] GameObject[] controlsAccJoysItems;
    GameObject controlsAccJoysSelected;
    [SerializeField] Sprite[] controlsAccJoysDefaultSprites;
    [SerializeField] Sprite controlsAccJoysSelectedSprite;
    [SerializeField] Slider contolsAccJoysScaleSlider;
    [SerializeField] Slider contolsAccJoysTransparencySlider;
    [SerializeField] Text contolsAccJoysScaleText;
    [SerializeField] Text contolsAccJoysTransparencyText;
    Vector3[] controlsAccJoysItemsPositions;
    Quaternion[] controlsAccJoysItemsRotations;
    Vector3[] controlsAccJoysItemsScales;
    float[] controlsAccJoysItemsTransparency;
    bool controlAccelerometerState;
    bool mouseOverAccJoysItems = false;

    [SerializeField] GameObject audioWindow;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Text musicVolumeText;
    [SerializeField] Slider soundEffectVolumeSlider;
    [SerializeField] Text soundEffectVolumeText;
    [SerializeField] Slider voiceVolumeSlider;
    [SerializeField] Text voiceVolumeText;
    [SerializeField] Slider uiSoundVolumeSlider;
    [SerializeField] Text uiSoundVolumeText;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Text masterVolumeText;
    [SerializeField] Slider microVolumeSlider;
    [SerializeField] Text microVolumeText;

    [SerializeField] GameObject gamePlayWindow;

    [Header("Chop      Window Panel")]
    [SerializeField] GameObject chopPanel;
    [Header("Map Selection      Window Panel")]
    [SerializeField] GameObject mapSelectionPanel;
    [Header("Profile      Window Elements")]
    [SerializeField] GameObject profilePanel;
    //  [SerializeField] GameObject playerProfile;
    //  [SerializeField] GameObject playerCurrency;
    [SerializeField] Image userProfileAvatarImage;
    [SerializeField] Text userProfileNameText;
    [SerializeField] Image userProfileRankImage;
    [SerializeField] Text userProfileLevelText;

    [Header("Wheel      Window Elements")]
    [SerializeField] GameObject spinWheelGO;
    private int NB_SPIN_ITEMS = 12;
    private int spinWheelResult = 0;

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

        uiUser.uiState = UIType.SettingUI;//For testing 
        selectedLanguage = LanguageType.English;
        pingTime = -1f;
 
        UpdateUI();
        InvokeRepeating("BatteryLifeUpdate", 1f, 60f);//60s
        InvokeRepeating("InternetIntensityCalc", 1f, 2f);
       //PlayerPrefs.DeleteAll();
        UpdateItemState();
        currentModel = aircraftModels[0];
    }

    private void Start()
    {
        base.Start();
    }

   


    #region Check Internet Reachability and overrided methods
    public override void OnConnectionCheckComplete()
    {
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
       // Debug.Log("Connection change called in UI Manager");
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
    
    void PlayerProfileSetup()
    {
        //SetUserProfile();

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
                    PlayerProfileSetup();
                    lobbyPanel.SetActive(true);
                    lobbyDefaultPanel.SetActive(true);
                    profilePanel.SetActive(true);
                }
                break;
            case UIType.SettingUI:
                {
                    settingsPanel.SetActive(true);
                    settingsDefaultPanel.SetActive(true);
                    mainSettingsWindow.SetActive(true);
                    graphicsWindow.SetActive(false);
                    controlsWindow.SetActive(false);
                    audioWindow.SetActive(false);
                    gamePlayWindow.SetActive(false);
                    DisplaySettingMainSetting();
                    SettingOptionClear();
                    settingOptions[0].GetComponent<Button>().image.sprite = settingOptionSelectedSprite;
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
        LoadPlayerItemsData();
        UpdateUI();
    }

    public void OnStartBtnPressed()
    {
        uiUser.uiState = UIType.GameUI;
        UpdateUI();
    }
    #endregion

    #region Settings Button and Sliders

    void SettingOptionClear()
    {
        for (int i = settingOptions.Length - 1; i >= 0; i--)
        {
            settingOptions[i].GetComponent<Button>().image.sprite = settingOptionDefaultSprite[i];
        }
    }

    void OnSettingSelectedUpdate()
    {
        mainSettingsWindow.SetActive(false);
        controlsWindow.SetActive(false);
        graphicsWindow.SetActive(false);
        audioWindow.SetActive(false);
        gamePlayWindow.SetActive(false);
        SettingOptionClear();
    }
    public void OnSettingsOptionsBtnPressed(int index)
    {
        OnSettingSelectedUpdate();
        switch (index)
        {
            case 0:
                {
                    DisplaySettingMainSetting();
                }
                break;
            case 1:
                {
                    DisplaySettingGraphics();
                }
                break;
            case 2:
                {
                    DisplaySettingControls();
                }
                break;
            case 3:
                {
                    DisplaySettingAudio();
                }
                break;
            case 4:
                {
                    DisplaySettingGamePlay();
                }
                break;
            default: break;
        }
        settingOptions[index].GetComponent<Button>().image.sprite = settingOptionSelectedSprite;
    }

    public void OnSettingsWindowClose()
    {
        uiUser.uiState = UIType.LobbyUI;
        UpdateUI();
    }

    public void OnSettingsBtnHelpUp()
    {
        settingBtnHeld = false;
    }

    #region Settings Main Settings
    public void OnSettingMainSettingToogle(int index)
    {
        PlayerPrefs.SetInt("mainSettingsToggle" + index.ToString(), mainSettingsToggle[index].isOn ? 1 : 0);
    }

    public void OnSettingsMainSettingCameraFovChanged(float value)
    {
        PlayerPrefs.SetFloat("CameraSettingFov", value * 180);
        cameraSettingFovText.text = (value * 180).ToString("0");
    }

    public void OnSettingsMainSettingCameraFovPlus()
    {
        float value = PlayerPrefs.GetFloat("CameraSettingFov", 170);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("CameraSettingFov",  cameraSettingFovSlider, cameraSettingFovText, value, +2));
    }

    public void OnSettingsMainSettingCameraFovMinus()
    {
        float value = PlayerPrefs.GetFloat("CameraSettingFov", 170);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("CameraSettingFov",  cameraSettingFovSlider, cameraSettingFovText, value, -2));
    }

    public void OnSettingMainSettingLanguageDropdownChange(int value)
    {
        langageDropDown.value = value;
        PlayerPrefs.SetInt("CountryLanguage", value);
    }

    public void OnSettingMainSettingServerDropdownChange(int value)
    {
        serverDropDown.value = value;
        PlayerPrefs.SetInt("Server", value);
    }

    void mainSettingBestServer()
    {
        int choosenServer = PlayerPrefs.GetInt("Server", 0);
    }

    void DisplaySettingMainSetting()
    {
        mainSettingsWindow.SetActive(true);
        String accountType = PlayerPrefs.GetString("accountType", "Guest");
        if (accountType == "Facebook")
        {
            facebookIco.SetActive(true);
            guestIco.SetActive(false);
            accountText.text = "Facebook";
        }else if (accountType == "Guest")
        {
            guestIco.SetActive(true);
            facebookIco.SetActive(false);
            accountText.text = "Guest";
        }

        for (int i = mainSettingsToggle.Length - 1; i >= 0; i--)
        {
            mainSettingsToggle[i].isOn = PlayerPrefs.GetInt("mainSettingsToggle" + i.ToString(), 0) == 1;
        }
        float cameraSettingFov = PlayerPrefs.GetFloat("CameraSettingFov", 170);
        cameraSettingFovText.text = cameraSettingFov.ToString("0");
        cameraSettingFovSlider.value = cameraSettingFov / 180;

        langageDropDown.value = PlayerPrefs.GetInt("CountryLanguage", 0);

        int choosenServer = PlayerPrefs.GetInt("Server", 0);
        serverDropDown.value = choosenServer;
        //serverDropDown.options[choosenServer].text = choosenServer.ToString();
    }
    #endregion
   
    IEnumerator SliderTextCoroutine(string str, Slider slider, Text textdisplay, float value, float step)
    {
        while (settingBtnHeld)
        {
            value += step;
            value = Mathf.Clamp(value, 0, 100);
            slider.value = value / 100;
            textdisplay.text = (value).ToString("0");
            yield return new WaitForSeconds(0.1f);
        }
        PlayerPrefs.SetFloat(str, value);
    }

    #region Settings Graphics Setting
    public void OnSettingGraphicsDisplayBtn(int index)
    {
        for (int i = graphicsDisplays.Length - 1; i >= 0; i--)
        {
            graphicsDisplays[i].GetComponent<Button>().image.sprite = graphicsDisplayDefaultSprite[i];
        }
        graphicsDisplays[index].GetComponent<Button>().image.sprite = graphicsDisplaySelectedSprite;
        PlayerPrefs.SetInt("GraphicDisplay", index);
    }

    public void OnSettingGraphicsFpsBtn(int index)
    {
        for (int i = graphicsFps.Length - 1; i >= 0; i--)
        {
            graphicsFps[i].GetComponent<Button>().image.sprite = graphicsFpsDefaultSprite[i];
        }
        graphicsFps[index].GetComponent<Button>().image.sprite = graphicsFpsSelectedSprite;
        PlayerPrefs.SetInt("GraphicFps", index);
    }

    public void OnSettingGraphicsStyleBtn(int index)
    {
        for (int i = graphicsStyle.Length - 1; i >= 0; i--)
        {
            graphicsStyle[i].GetComponent<Button>().image.sprite = graphicsStyleDefaultSprite[i];
        }
        graphicsStyle[index].GetComponent<Button>().image.sprite = graphicsStyleSelectedSprite;
        PlayerPrefs.SetInt("GraphicStyle", index);
    }

    public void OnSettingGraphicsShadow()
    {
        PlayerPrefs.SetInt("GraphicShadow", graphicShadowToggle.isOn ? 1 : 0);
    }

    public void OnSettingsGraphicBrightnessChanged(float value)
    {
        PlayerPrefs.SetFloat("GraphicBrightness", value * 100);
        settingBtnHeld = true;
        graphicBrightnessText.text = (value * 100).ToString("0");
    }

    public void OnSettingsGraphicBrightnessPlus()
    {
        float value = PlayerPrefs.GetFloat("GraphicBrightness", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("GraphicBrightness", graphicBrightnessSlider, graphicBrightnessText, value, +2));
    }

    public void OnSettingsGraphicBrightnessMinus()
    {
        float value = PlayerPrefs.GetFloat("GraphicBrightness", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("GraphicBrightness", graphicBrightnessSlider, graphicBrightnessText, value, -2));
    }


    void DisplaySettingGraphics()
    {
        graphicsWindow.SetActive(true);
        int displayIndex = PlayerPrefs.GetInt("GraphicDisplay", 1);
        graphicsDisplays[displayIndex].GetComponent<Button>().image.sprite = graphicsDisplaySelectedSprite;
        int fpsIndex = PlayerPrefs.GetInt("GraphicFps", 1);
        graphicsFps[fpsIndex].GetComponent<Button>().image.sprite = graphicsDisplaySelectedSprite;
        int styleIndex = PlayerPrefs.GetInt("GraphicStyle", 0);
        graphicsStyle[styleIndex].GetComponent<Button>().image.sprite = graphicsStyleSelectedSprite;
        graphicShadowToggle.isOn = PlayerPrefs.GetInt("GraphicShadow", 1) == 1;

        float brightness = PlayerPrefs.GetFloat("GraphicBrightness", 100);
        graphicBrightnessText.text = brightness.ToString("0");
        graphicBrightnessSlider.value = brightness / 100;
    }
    #endregion


    #region Settings Control Setting
    public void OnControlOptionSelectedBtn(int index)
    {
        for (int i = controlsOptions.Length - 1; i >= 0; i--)
        {
            controlsOptions[i].transform.GetChild(2).gameObject.SetActive(false);
            controlsOptions[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = controlsOptionDefaultSprites[i];
        }
        controlsOptions[index].transform.GetChild(2).gameObject.SetActive(true);
        controlsOptions[index].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = controlsOptionSelectedSprite;
    }

    public void OnControlAccJoysScaleChange(float value)
    {
        float scale = value;
        scale = Mathf.Clamp(scale, 0.5f, 1f);
        controlsAccJoysSelected.transform.localScale = Vector3.one * scale;
        contolsAccJoysScaleText.text = ((scale) * 100).ToString("0");
    }

    public void OnControlAccJoysTransparencyChange(float value)
    {
        Color color = controlsAccJoysSelected.GetComponent<Image>().color;
        controlsAccJoysSelected.GetComponent<Image>().color = new Color(color.r, color.g, color.b, value);
        contolsAccJoysTransparencyText.text = (value * 100).ToString("0");
    }

    public void OnControlAccJoysItemUpdate(int index)
    {
        for(int i = controlsAccJoysItems.Length - 1; i >= 0; i--)
        {
            controlsAccJoysItems[i].GetComponent<Image>().sprite = controlsAccJoysDefaultSprites[i];
        }
        controlsAccJoysSelected = controlsAccJoysItems[index];
        controlsAccJoysSelected.GetComponent<Image>().sprite = controlsAccJoysSelectedSprite;
    }

    public void OnControlAccJoysItemSelectedBtn(int index)
    {
        OnControlAccJoysItemUpdate(index);
        contolsAccJoysScaleSlider.value = controlsAccJoysSelected.transform.localScale.x;
        contolsAccJoysTransparencySlider.value = controlsAccJoysSelected.GetComponent<Image>().color.a;
        contolsAccJoysScaleText.text = (controlsAccJoysSelected.transform.localScale.x * 100).ToString("0");
        contolsAccJoysTransparencyText.text = (controlsAccJoysSelected.GetComponent<Image>().color.a * 100).ToString("0");
    }

    public void OnControlAccJoysExitBtn()
    {
        controlsAccJoysWindow.SetActive(false);
        controlsDefaultWindow.SetActive(true);
        controlsAccJoysSelected = null;
    }

    void LoadAccJoysItemsSettings()
    {
        Vector3 pos;
        float scale;
        float brightness;
        string str;

        for (int i = controlsAccJoysItems.Length - 1; i >= 0; i--)
        {
            if (PlayerPrefs.HasKey("ControlAccJoysItem" + i.ToString()))
            {
                str = PlayerPrefs.GetString("ControlAccJoysItem" + i.ToString());
                string[] itemParam = str.Split(';');
                pos.x = float.Parse(itemParam[0]);
                pos.y = float.Parse(itemParam[1]);
                pos.z = float.Parse(itemParam[2]);
                scale = float.Parse(itemParam[3]);
                brightness = float.Parse(itemParam[4]);
                controlsAccJoysItems[i].transform.position = pos;
                controlsAccJoysItems[i].transform.localScale = Vector3.one * scale;
                Color color = controlsAccJoysItems[i].GetComponent<Image>().color;
                controlsAccJoysItems[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, brightness);
            }
        }
    }

    public void OnControlAccJoysSaveBtn()
    {
        Vector3 pos;
        float scale;
        float brightness;
        string str;

        for (int i = controlsAccJoysItems.Length - 1; i >= 0; i--)
        {
            pos = controlsAccJoysItems[i].transform.position;
            scale = controlsAccJoysItems[i].transform.localScale.x;
            brightness = controlsAccJoysItems[i].GetComponent<Image>().color.a;
            str = String.Format("{0};{1};{2};{3};{4}", pos.x, pos.y, pos.z, scale, brightness);

            PlayerPrefs.SetString("ControlAccJoysItem" + i.ToString(), str);
        }
    }

    public void OnControlAccJoysResetBtn()
    {
        if (controlsAccJoysItemsPositions != null)
        {
            for (int i = controlsAccJoysItems.Length - 1; i >= 0; i--)
            {
                controlsAccJoysItems[i].transform.position = controlsAccJoysItemsPositions[i];
                controlsAccJoysItems[i].transform.localScale = controlsAccJoysItemsScales[i];
                controlsAccJoysItems[i].transform.rotation = controlsAccJoysItemsRotations[i];
                Color color = controlsAccJoysItems[i].GetComponent<Image>().color;
                float alpha = controlsAccJoysItemsTransparency[i];
                controlsAccJoysItems[i].GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
            }
            OnControlAccJoysItemSelectedBtn(0);
        }
    }
    
    public void OnMouseOverAccJoysItems(bool value)
    {
        Debug.Log("Ha" + value);
        mouseOverAccJoysItems = value;
    }

    public void OnControlDraggingAccJoysItems(BaseEventData bed)
    {
        PointerEventData ped = bed as PointerEventData;

        if (!OnScalingControlAccJoysItems() && controlsAccJoysWindow.GetComponent<RectTransform>().rect.Overlaps(controlsAccJoysSelected.GetComponent<RectTransform>().rect))
        {
            Debug.Log("Hey");
            controlsAccJoysSelected.transform.position = ped.position;
        }
    }

    public void OnControlPointerDownAccJoysItems(int index)
    {
        OnControlAccJoysItemSelectedBtn(index);
    }

    bool OnScalingControlAccJoysItems()
    {
        if (Input.touchCount < 2)
        {
            return false;
        }
        Touch tZero = Input.GetTouch(0);
        Touch tOne = Input.GetTouch(1);

        Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
        Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;
        float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
        float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);
        float deltaDistance = oldTouchDistance - currentTouchDistance;
        float speed = scaleSpeed;
        controlsAccJoysSelected.transform.localScale -= Vector3.one * deltaDistance * speed * Time.deltaTime;
        controlsAccJoysSelected.transform.localScale = new Vector3(Mathf.Clamp(controlsAccJoysSelected.transform.localScale.x, 0.1f, 1.0f),
            Mathf.Clamp(controlsAccJoysSelected.transform.localScale.y, 0.5f, 1.0f),
            controlsAccJoysSelected.transform.localScale.z);
        float newScale = controlsAccJoysSelected.transform.localScale.x;
        OnControlAccJoysScaleChange(newScale);
        return true;
    }

    public void OnControlOptionCustomizeBtn(int index)
    {
        controlsDefaultWindow.SetActive(false);
        controlsAccJoysWindow.SetActive(true);
        OnControlAccJoysItemUpdate(0);
        controlAccelerometerState = index == 0;
        
        //The first time
     
        if (controlsAccJoysItemsPositions == null)
        {
            controlsAccJoysItemsPositions = new Vector3[controlsAccJoysItems.Length];
            controlsAccJoysItemsRotations = new Quaternion[controlsAccJoysItems.Length];
            controlsAccJoysItemsScales = new Vector3[controlsAccJoysItems.Length];
            controlsAccJoysItemsTransparency = new float[controlsAccJoysItems.Length];
    
            for (int i = controlsAccJoysItems.Length - 1; i >= 0; i--)
            {
                Vector3 position = new Vector3(controlsAccJoysItems[i].transform.position.x,
                    controlsAccJoysItems[i].transform.position.y,
                    controlsAccJoysItems[i].transform.position.z);
                Quaternion rotation = new Quaternion(controlsAccJoysItems[i].transform.rotation.x,
                    controlsAccJoysItems[i].transform.rotation.y, controlsAccJoysItems[i].transform.rotation.z, controlsAccJoysItems[i].transform.rotation.w);
                Vector3 scale = new Vector3(controlsAccJoysItems[i].transform.localScale.x,
                    controlsAccJoysItems[i].transform.localScale.y,
                    controlsAccJoysItems[i].transform.localScale.z);
                controlsAccJoysItemsPositions[i] = position;
                controlsAccJoysItemsRotations[i] = rotation;
                controlsAccJoysItemsScales[i] = scale;
                controlsAccJoysItemsTransparency[i] = controlsAccJoysItems[i].GetComponent<Image>().color.a;
            }
        }

        LoadAccJoysItemsSettings();
        if (controlAccelerometerState)
        {
            controlsAccJoysItems[0].SetActive(false);
            controlsAccJoysSelected = controlsAccJoysItems[1];
        }
        else
        {
            controlsAccJoysItems[0].SetActive(true);
            controlsAccJoysSelected = controlsAccJoysItems[0];
        }
        controlsAccJoysSelected.GetComponent<Image>().sprite = controlsAccJoysSelectedSprite;
        contolsAccJoysScaleSlider.value = controlsAccJoysSelected.transform.localScale.x;
        contolsAccJoysTransparencySlider.value = controlsAccJoysSelected.GetComponent<Image>().color.a;
        contolsAccJoysScaleText.text = (controlsAccJoysSelected.transform.localScale.x * 100).ToString("0"); // >
        contolsAccJoysTransparencyText.text = (controlsAccJoysSelected.GetComponent<Image>().color.a * 100).ToString("0");
    }


    void DisplaySettingControls()
    {
        controlsWindow.SetActive(true);
        controlsDefaultWindow.SetActive(true);
        int lastCustomizeIndex = PlayerPrefs.GetInt("lastControlCustomized", 0);
        OnControlOptionSelectedBtn(lastCustomizeIndex);
    }

    #endregion

    #region Settings GamePlay Setting
    void DisplaySettingGamePlay()
    {
        gamePlayWindow.SetActive(true);
    }
    #endregion

    #region Settings : Music
    void DisplaySettingAudio()
    {
        audioWindow.SetActive(true);

        float value = PlayerPrefs.GetFloat("MusicVolume", 50);
        musicVolumeText.text = value.ToString("0");
        musicVolumeSlider.value = value / 100;
        value = PlayerPrefs.GetFloat("SoundEffectVolume", 50);
        soundEffectVolumeText.text = value.ToString("0");
        soundEffectVolumeSlider.value = value / 100;
        value = PlayerPrefs.GetFloat("VoiceVolume", 50);
        voiceVolumeText.text = value.ToString("0");
        voiceVolumeSlider.value = value / 100;
        value = PlayerPrefs.GetFloat("UiSoundVolume", 50);
        uiSoundVolumeText.text = value.ToString("0");
        uiSoundVolumeSlider.value = value / 100;
        value = PlayerPrefs.GetFloat("MasterVolume", 50);
        masterVolumeText.text = value.ToString("0");
        masterVolumeSlider.value = value / 100;
        value = PlayerPrefs.GetFloat("MicroVolume", 50);
        microVolumeText.text = value.ToString("0");
        microVolumeSlider.value = value / 100;
    }
    public void OnSettingsMusicVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("MusicVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MusicVolume", musicVolumeSlider, musicVolumeText, value, +2));
    }

    public void OnSettingsMusicVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("MusicVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MusicVolume", musicVolumeSlider, musicVolumeText, value, -2));
    }

    public void OnSettingMusicVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value * 100);
        musicVolumeText.text = (value * 100).ToString("0");
    }

    public void OnSettingsSoundEffectVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("SoundEffectVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("SoundEffectVolume", soundEffectVolumeSlider, soundEffectVolumeText, value, +2));
    }

    public void OnSettingsSoundEffectVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("SoundEffectVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("SoundEffectVolume", soundEffectVolumeSlider, soundEffectVolumeText, value, -2));
    }

    public void OnSettingSoundEffectVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("SoundEffectVolume", value * 100);
        soundEffectVolumeText.text = (value * 100).ToString("0");
    }

    public void OnSettingsVoiceVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("VoiceVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("VoiceVolume", voiceVolumeSlider, voiceVolumeText, value, +2));
    }

    public void OnSettingsVoiceVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("VoiceVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("VoiceVolume", voiceVolumeSlider, voiceVolumeText, value, -2));
    }

    public void OnSettingVoiceVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("VoiceVolume", value * 100);
        voiceVolumeText.text = (value * 100).ToString("0");
    }
    public void OnSettingsUiSoundVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("UiSoundVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("UiSoundVolume", uiSoundVolumeSlider, uiSoundVolumeText, value, +2));
    }

    public void OnSettingsUiSoundVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("UiSoundVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("UiSoundVolume", uiSoundVolumeSlider, uiSoundVolumeText, value, -2));
    }

    public void OnSettingUiSoundVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("UiSoundVolume", value * 100);
        uiSoundVolumeText.text = (value * 100).ToString("0");
    }

    public void OnSettingsMasterVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("MasterVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MasterVolume", masterVolumeSlider, masterVolumeText, value, +2));
    }

    public void OnSettingsMasterVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("MaterVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MasterVolume", masterVolumeSlider, masterVolumeText, value, -2));
    }

    public void OnSettingMasterVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value * 100);
        masterVolumeText.text = (value * 100).ToString("0");
    }

    public void OnSettingsMicroVolumePlus()
    {
        float value = PlayerPrefs.GetFloat("MicroVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MicroVolume", microVolumeSlider, microVolumeText, value, +2));
    }

    public void OnSettingsMicroVolumeMinus()
    {
        float value = PlayerPrefs.GetFloat("MicroVolume", 100);
        settingBtnHeld = true;
        StartCoroutine(SliderTextCoroutine("MicroVolume", microVolumeSlider, microVolumeText, value, -2));
    }
    
    public void OnSettingMicroVolumeChange(float value)
    {
        PlayerPrefs.SetFloat("MicroVolume", value * 100);
        microVolumeText.text = (value * 100).ToString("0");
    }
    #endregion
    #endregion

    #region Inventory Buttons
    #region Load Inventroy Data

    void InventoryItemOptionSetText(GameObject[] optionGO, List<CloudManager.PlayerData> data)
    {
        int optionsCount;

        optionsCount = data.Count;
        for (int i = 0; i < optionsCount; i++)
        {
            optionGO[i].transform.GetChild(0).GetComponent<Text>().text = data[i].name;
        }
    }

    private void LoadPlayerItemsData()
    {
        InventoryItemOptionSetText(aircraftOptions, CloudManager.CM.serverLoadedData.aircraftdata);
        InventoryItemOptionSetText(canonOptions, CloudManager.CM.serverLoadedData.canondata);
        InventoryItemOptionSetText(missileOptions, CloudManager.CM.serverLoadedData.missiledata);
        InventoryItemOptionSetText(agmOptions, CloudManager.CM.serverLoadedData.agmdata);
        InventoryItemOptionSetText(colorOptions, CloudManager.CM.serverLoadedData.colordata);
        InventoryItemOptionSetText(flagOptions, CloudManager.CM.serverLoadedData.flagdata);
        InventoryItemOptionSetText(reactorOptions, CloudManager.CM.serverLoadedData.reactordata);
        InventoryItemOptionSetText(wingOptions, CloudManager.CM.serverLoadedData.wingdata);
        InventoryItemOptionSetText(armorOptions, CloudManager.CM.serverLoadedData.armordata);

        inventoryItemDefault.GetComponent<Button>().image.sprite = itemSelectedSprite;
        SetInventorySelectedItem(0);

        //For testing we need Reset PlayerPrefs 
        //PlayerPrefs.DeleteAll();
        //Remove the line above !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        uiUser.currentItemIndex = 0;
        uiUser.currentOptionIndex = 0;
        int selectedOption = PlayerPrefs.GetInt("aircraft", 0);
        aircraftOptions[selectedOption].GetComponent<Button>().image.sprite = optionSelectedSprite;
        SetInventorySelectedItemOption(selectedOption);
    }


    #endregion
    private void OnInventorySelectReset()
    {
        //This should be sufficient
        aircraftOptionsContainer.SetActive(false);
        canonOptionsContainer.SetActive(false);
        missileOptionsContainer.SetActive(false);
        agmOptionsContainer.SetActive(false);
        colorOptionsContainer.SetActive(false);
        flagOptionsContainer.SetActive(false);
        reactorOptionsContainer.SetActive(false);
        wingOptionsContainer.SetActive(false);
        armorOptionsContainer.SetActive(false);
    }

    private void OnInventorySelectUpdate(List<CloudManager.PlayerData> itemOptions, GameObject[] optionsGO)
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
        OnInventorySelectReset();
        switch (itemIndex)
        {
            case 0:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("aircraft", 0);
                    aircraftOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.aircraftdata, aircraftOptions);
                }
                break;
            case 1:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("canon", 0);
                    canonOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.canondata, canonOptions);
                }
                break;
            case 2:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("missile", 0);
                    missileOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.missiledata, missileOptions);
                }
                break;

            case 3:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("agm", 0);
                    agmOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.agmdata, agmOptions);
                }
                break;
            case 4:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("color", 0);
                    colorOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.colordata, colorOptions);
                }
                break;
            case 5:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("flag", 0);
                    flagOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.flagdata, flagOptions);
                }
                break;
            case 6:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("reactor", 0);
                    reactorOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.reactordata, reactorOptions);
                }
                break;
            case 7:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("wing", 0);
                    wingOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.wingdata, wingOptions);
                }
                break;
            case 8:
                {
                    uiUser.currentOptionIndex = PlayerPrefs.GetInt("armor", 0);
                    armorOptionsContainer.SetActive(true);
                    OnInventorySelectUpdate(CloudManager.CM.serverLoadedData.armordata, armorOptions);
                }
                break;
            default:break;
        }
       SetInventorySelectedItemOption(uiUser.currentOptionIndex);
    }

    private void SetAndSaveItemSelectedOption(GameObject[] optionsGOS, string keyName, int optionIndex)
    {
        Array.ForEach(optionsGOS, (itemOption) => { itemOption.GetComponent<Button>().image.sprite = optionDefaultSprite; });
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

    public void OnInventoryItemAircraftOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemCanonOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemMissileOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemAgmOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemColorOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemFlagOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemReactorOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemWingOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnInventoryItemArmorOptionBtnClicked(int index)
    {
        SetInventorySelectedItemOption(index);
    }

    public void OnMouseOverModelView(bool value)
    {
        mouseOverModelView = value;
    }

    public void OnInventoryCloseBtnClicked()
    {
        inventoryDefaultPanel.SetActive(false);
        uiUser.uiState = UIType.LobbyUI;
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

    IEnumerator SpinRotator(Action method)
    {
        spinWheelResult = UnityEngine.Random.Range(1, NB_SPIN_ITEMS);
        float rotAngle = (float)spinWheelResult * 30  + (UnityEngine.Random.Range(1, 4)) * 360;
        float currentSpinSpeed = 0f;

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

    int  SpinWheel()
    {
     
        Action OnSpinWheelComplete = () =>
        {
            Debug.LogFormat("Choosen index {0}", 1 + spinWheelResult);
        };

        StartCoroutine("SpinRotator", OnSpinWheelComplete);
        return spinWheelResult;
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
                PlayerPrefs.DeleteKey("lastDay");
            }
            else
            {
                PlayerPrefs.SetInt("lastItem", index);
            }
        }
        DisplayGiftWindow();
    }

    #endregion

    private void Update()
    {
        if (uiUser.uiState == UIType.SettingUI)
        {
            if (controlsAccJoysSelected)
            {
                OnScalingControlAccJoysItems();
            }
        }else if (uiUser.uiState == UIType.InventoryUI)
        {
            UpdateEvents();
        }
    }


}