using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ByteDance.Union;
using UnityEngine.UI;
public class AdManager : MonoBehaviour
{
    private List<FullScreenVideoAd> fullScreenVideoAd=new List<FullScreenVideoAd>();
    private FullScreenVideoAd fullScreenVideoAd1=null;
    [SerializeField]
    // private Text information;
    static int count = 0;
    private AdNative adNative;
    private InputField inputX;
    private InputField inputY;
    private InputField width;
    private InputField heigh;
    private InputField adsRit;
    private InputField screenOrientation;

    // Start is called before the first frame update
    void Start()
    {
        LoadFullScreenVideoAd();
    }
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    private AdNative AdNative
    {

        get
        {
            if (this.adNative == null)
            {
                this.adNative = SDK.CreateAdNative();
                Debug.Log("test");
            }
#if UNITY_ANDROID
            SDK.RequestPermissionIfNecessary();
#endif
            return this.adNative;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public int getScreenOrientationConfig()
    {
        //this.screenOrientation = GameObject.Find("Canvas/screenOrientation").GetComponent<InputField>();
        int screenOrientation;
        int.TryParse(this.screenOrientation.text, out screenOrientation);

        return screenOrientation;
    }

    public void LoadFullScreenVideoAd()
    {
        Debug.Log("LoadFullScreenVideoAd");
        if (this.fullScreenVideoAd != null)
        {
            //this.fullScreenVideoAd.Dispose();
            this.fullScreenVideoAd = null;
        }

        string iosSlotID = "900546299";
        string AndroidSlotID = "946128037";   //5166855
        // 横屏
        //if (this.getScreenOrientationConfig() == 1)
        //{
        //    iosSlotID = "900546154";
        //    AndroidSlotID = "901121184";
        //}
        // Debug.LogError("before var adSlot");
        var adSlot = new AdSlot.Builder()
#if UNITY_IOS
                             .SetCodeId(iosSlotID)
#else
                             .SetCodeId(AndroidSlotID)
#endif
                             .SetSupportDeepLink(true)
                             .SetImageAcceptedSize(1080, 1920)
                             .SetOrientation(AdOrientation.Horizontal)
                             .Build();
        this.AdNative.LoadFullScreenVideoAd(adSlot, new FullScreenVideoAdListener(this));

    }

    /// <summary>
    /// Show the reward Ad.
    /// </summary>
    public void ShowFullScreenVideoAd()
    {
        Debug.Log("ShowFullScreenVideoAd");
        if (this.fullScreenVideoAd1 == null)
        {
            Debug.LogError("请先加载广告");
         //   this.information.text = "请先加载广告";
            return;
        }
        else
        {
           /* if (count > 3)
            {
                count = 0;
            }
            else
            {
                count++;
            }*/
           this.fullScreenVideoAd1.ShowFullScreenVideoAd();
        }
    }

    private sealed class FullScreenVideoAdListener : IFullScreenVideoAdListener
    {
        private AdManager admanager;

        public FullScreenVideoAdListener(AdManager admanager)
        {
            this.admanager = admanager;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("OnFullScreenError: " + message);
        //    this.admanager.information.text = "OnFullScreenError: " + message;
        }

        public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
        {
            Debug.Log("OnFullScreenAdLoad");
         //   this.admanager.information.text = "OnFullScreenAdLoad";

            ad.SetFullScreenVideoAdInteractionListener(
                new FullScreenAdInteractionListener(this.admanager));
            ad.SetDownloadListener(
                new AppDownloadListener(this.admanager));
            //add by Jason
            // this.admanager.fullScreenVideoAd.Add(ad);
            this.admanager.fullScreenVideoAd1 = ad;
        }

        // iOS
        public void OnExpressFullScreenVideoAdLoad(ExpressFullScreenVideoAd ad)
        {
            // rewrite
        }

        public void OnFullScreenVideoCached()
        {
            Debug.Log("OnFullScreenVideoCached");
        //    this.admanager.information.text = "OnFullScreenVideoCached";
        }
    }
    private sealed class FullScreenAdInteractionListener : IFullScreenVideoAdInteractionListener
    {
        private AdManager admanager;

        public FullScreenAdInteractionListener(AdManager admanager)
        {
            this.admanager = admanager;
        }

        public void OnAdShow()
        {
            Debug.Log("fullScreenVideoAd show");
         //   this.admanager.information.text = "fullScreenVideoAd show";
        }

        public void OnAdVideoBarClick()
        {
            Debug.Log("fullScreenVideoAd bar click");
       //     this.admanager.information.text = "fullScreenVideoAd bar click";
        }

        public void OnAdClose()
        {
            Debug.Log("fullScreenVideoAd close");
         //   this.admanager.information.text = "fullScreenVideoAd close";
            this.admanager.fullScreenVideoAd = null;
#if UNITY_IOS
            this.admanager.expressFullScreenVideoAd = null;
#endif
        }

        public void OnVideoComplete()
        {
            Debug.Log("fullScreenVideoAd complete");
       //     this.admanager.information.text = "fullScreenVideoAd complete";
        }

        public void OnVideoError()
        {
            Debug.Log("fullScreenVideoAd OnVideoError");
         //   this.admanager.information.text = "fullScreenVideoAd OnVideoError";
        }

        public void OnSkippedVideo()
        {
            Debug.Log("fullScreenVideoAd OnSkippedVideo");
         //   this.admanager.information.text = "fullScreenVideoAd skipped";

        }
    }

    public void DisposeAds()
    {
        if(fullScreenVideoAd!=null)
        {
            fullScreenVideoAd = null;
        }
    }

    private sealed class AppDownloadListener : IAppDownloadListener
    {
        private AdManager admanager;

        public AppDownloadListener(AdManager admanager)
        {
            this.admanager = admanager;
        }

        public void OnIdle()
        {
        }

        public void OnDownloadActive(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载中，点击下载区域暂停");
         //   this.admanager.information.text = "下载中，点击下载区域暂停";
        }

        public void OnDownloadPaused(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.Log("下载暂停，点击下载区域继续");
          //  this.admanager.information.text = "下载暂停，点击下载区域继续";
        }

        public void OnDownloadFailed(
            long totalBytes, long currBytes, string fileName, string appName)
        {
            Debug.LogError("下载失败，点击下载区域重新下载");
           // this.admanager.information.text = "下载失败，点击下载区域重新下载";
        }

        public void OnDownloadFinished(
            long totalBytes, string fileName, string appName)
        {
            Debug.Log("下载完成，点击下载区域重新下载");
         //   this.admanager.information.text = "下载完成，点击下载区域重新下载";
        }

        public void OnInstalled(string fileName, string appName)
        {
            Debug.Log("安装完成，点击下载区域打开");
          //  this.admanager.information.text = "安装完成，点击下载区域打开";
        }
    }
}
