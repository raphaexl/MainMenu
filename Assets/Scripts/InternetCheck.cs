using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public enum InternetReachability
{
    UNKNOWN, TIMEOUT, OFFLINE, DISCONNECTED, ONLINE
};

public enum ConnectionCheckState
{
    CHECKING, COMPLETED
}

public class InternetCheck : MonoBehaviour
{
    public static InternetCheck Instance;
    int TIME_OUT = 1;
    InternetReachability curentInternetState;
    InternetReachability lastInternetState;
    ConnectionCheckState connectionCheckState;

    string uri;
    int androidVersion;


    private event Action state_update_event;
    public event Action ccc_event;
    public event Action cch_event;

    public bool Continous { get; set; }
    [HideInInspector]
    public bool isConnected;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        uri = PlatformUri();
        state_update_event += UpdateStates;
        Initialize();
    }

    private void ToInvoke()
    {
        Debug.Log("I'm doing my job");
        if (Continous)
        {
            {
                OnlineChecker();
            }
        }
    }

    private void Start()
    {
        Debug.Log("Internet reachability ??");
        androidVersion = (Application.platform == RuntimePlatform.Android) ? androidSDK() : 27;
        InvokeRepeating("ToInvoke", 1f, 2f);
    }

    private string PlatformUri()
    {
        string uri = "https://clients1.google.com/generate_204";
        {
            if (Application.isMobilePlatform)
            {

                if (Application.platform == RuntimePlatform.Android)
                {
                    uri = "https://clients2.google.com/generate_204";
                    //28 for android Pie (android 9)
                    if (androidVersion == 28)
                    { uri = "https://clients1.google.com/generate_204"; }
                }         //ANDROID
                else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.OSXEditor
                    )     //IPHONE
                {
                    uri = "https://captive.apple.com";
                    uri = "http://www.apple.com";
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer
                    )     //WINDOWS
                { uri = "http://www.msftncsi.com/ncsi.txt"; }
                else if (Application.platform == RuntimePlatform.LinuxEditor ||
                   Application.platform == RuntimePlatform.LinuxPlayer
                   )      //LINUX
                { uri = "http://connectivity-check.ubuntu.com"; }
            }
        }
        return uri;
    }

    private void CheckInternetReachability()
    {
        connectionCheckState = ConnectionCheckState.CHECKING;
        curentInternetState = InternetReachability.UNKNOWN;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            curentInternetState = InternetReachability.OFFLINE;
            connectionCheckState = ConnectionCheckState.COMPLETED;
            if (ccc_event != null)
            { ccc_event(); }
            if (state_update_event != null)
                state_update_event();
            return;
        }
        StartCoroutine(GetRequest(uri));
    }


    static int androidSDK()
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.timeout = TIME_OUT;
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            connectionCheckState = ConnectionCheckState.COMPLETED;
            if ((webRequest.isNetworkError || webRequest.isHttpError))
            {
                if (webRequest.error.Equals("Request timeout"))
                { curentInternetState = InternetReachability.TIMEOUT; }
                else
                { curentInternetState = InternetReachability.DISCONNECTED; }
                if (ccc_event != null)
                { ccc_event(); }

            }
            else //if (webRequest.isDone)
            {
                curentInternetState = InternetReachability.ONLINE;
                if (ccc_event != null)
                { ccc_event(); }
            }
            if (state_update_event != null)
                state_update_event();
        }
    }

    

    private void UpdateStates()
    {
        if (connectionCheckState == ConnectionCheckState.COMPLETED)
        {
            if (lastInternetState == InternetReachability.UNKNOWN)
            {
                lastInternetState = curentInternetState;
            }
            else if (lastInternetState != curentInternetState)
            {
                if (cch_event != null)
                {
                    cch_event();
                }
                lastInternetState = curentInternetState;
            }
        }
    }

    void OnlineChecker()
    {
        CheckInternetReachability();

    }

    private void Initialize()
    {
        Continous = true;
        ccc_event += OnConnectionCheckComplete;
        cch_event += OnConnectionStatusChange;
    }

    public void Restart()
    {
        Initialize();
    }

    public void Stop()
    {
        Continous = false;
        ccc_event -= OnConnectionCheckComplete;
        cch_event -= OnConnectionStatusChange;
    }

    public virtual void OnConnectionCheckComplete() { isConnected = curentInternetState == InternetReachability.ONLINE;  }
    public virtual void OnConnectionStatusChange() { }

    private void OnDestroy()
    {
        ccc_event -= OnConnectionCheckComplete;
        cch_event -= OnConnectionCheckComplete;
    }
}
