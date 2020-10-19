using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
/// <summary>
/// APP 与 AR接口
/*
 AR与APP交互接口说明：
1，原有的分享接口不变：

share(int shareType, string content);
2，原有的AR返回至APP界面的取消：不再使用

安卓：onBackPressed();
IOS：UnityReturnIOS();

3,原有保存图片接口不变：

saveBitmap(string localImgPath);

4，原有APP进入AR接口取消：不再需要

安卓：getArFilePath();
IOS ：EnterSystem(string msg)；

5，新增获取展示车型接口：

/// AR 调用 APP的接口，获取展示的车型。
///返回值为String类型（字符串），代表具体的车型代号
///如果为"NULL"，代表是从主页AR入口进入
string ReqARCarShow();



6，新增获取车型是否支持在线预订接口：

/// AR 调用 APP的接口，获取当前车辆是否支持在线预订
///carCode为车型代号，字符串类型
///返回值为int类型，0代表不支持在线预订，1代表支持在线预订
int IsSaleOnLine(string carCode);



7，新增退出接口，退出AR并返回APP

///用户点击退出，或者点击在线预订时，将退出AR，并将退出“原因”返回给APP
///carIndex代表退出时的参数
///正常退出，carCode为"NULL"
///非正常退出（点击了在线预订等功能按钮时），carCode为展示的具体车型的代号
///switchType代表用户要进入的功能，参数为 0 - 4 int类型，关于功能代号见备注2

ARReturnAPP(string carCode, int switchType);
备注：以下为接口代号相关备注

1，车型代号备注（string carCode）：

///每辆车都有唯一的车型代号，要保持一致，同步。具体代号可以查询相关资料
///现AR中已有Camry，CHR，雷凌PHEV，全新换代雷凌

2，功能代号备注(int switchType)

/// 0代表无触发功能，正常退出
/// 1代表 在线预订
/// 2代表 车型详情入口
/// 3代表 预约看车入口
/// 4代表购车咨询入口
///后续如果有新功能接口，代号将持续累加
全新换代雷凌  —— LEVIN
雷凌PHEV    —— LEVHV（E+）
C-HR    ——— C-HR
全新第八代凯美瑞  ——— CMY-8
     */
/// </summary>
public class GameSceneManager : Singleton<GameSceneManager>
{

    public event Action<string> OnReqCarShow;
    /// <summary>
    /// 进入系统时的事件
    /// </summary>
    public event Action OnEnterSystem;
    public string defCarIndex = "NULL";
    private string suffix = ".png";
    public string carCode;
    /// <summary>
    /// 加载的状态，如果LoadState为0，代表是要加载到AR，否则是要退出AR
    /// </summary>
    private int loadState = 0;
    private int switchType = 2;

    protected override void Awake()
    {
        base.Awake();
        carCode = defCarIndex;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 当检测不支持时
    /// </summary>
    /// <param name="isSupport"></param>
    private void OnCheckSupport(bool isSupport)
    {
        //if (!isSupport)
        //{
        //    QuitUnity();
        //}
    }

    // Use this for initialization

    void Start()
    {
        //M_ReqARCarShow();
        //CheckXRGamer.Instance.OnCheckSupport += OnCheckSupport;
        TimerMgr.Instance.CreateTimer(
        EnterUnity, 0.02f, 1);
    }
    /// <summary>
    /// 程序退出
    /// </summary>
    public void QuitUnity()
    {
#if IOS_APP
        UnityReturnIOS();
#elif Android_APP
        finish();
#elif UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        //if (!string.IsNullOrEmpty(GameServerChecker.GMSURL))
        //{
        //    Application.OpenURL(GameServerChecker.GMSURL);
        //}
        Application.Quit();
#endif
    }
    /// <summary>
    /// 程序进入，开始启动
    /// </summary>
    public void EnterUnity()
    {
        if (OnEnterSystem != null)
        {
            OnEnterSystem();
        }
    }
    public IEnumerator TakePhoto(Action<string> onWriteFinsh)
    {
        yield return new WaitForEndOfFrame();
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height);
        screenShot.ReadPixels(rect, 0, 0, true);
        screenShot.Apply();
        yield return screenShot;
        string filepath = "FYAR" + UnityUtil.GetTime() + suffix;
        string realPath = Path.Combine(GameDataMgr.Instance.AssetPath, filepath);
        UnityUtil.SaveFileToLocal(screenShot.EncodeToPNG(), realPath);
        if (onWriteFinsh != null)
        {
            onWriteFinsh(realPath);
        }
    }

    #region 交互通用接口
    public string savePath;

    public int LoadState { get => loadState; set => loadState = value; }
    public int SwitchType { get => switchType; set => switchType = value; }
    /// <summary>
    /// AR埋点事件传递给APP
    /// </summary>
    /// <param name="carCode">车型信息</param>
    /// <param name="eventName">事件名称，可能是中文，例如“立即体验”</param>
    /// <param name="key">埋点key值，例如“OnClickShowBtn”</param>
    public void SendBuriedPoint(string carCode, string eventName, string key)
    {
#if !UNITY_EDITOR && (IOS_APP || Android_APP)
        ARSendBuriedPoint(carCode, eventName, key);
#endif
    }

    /// <summary>
    /// 保存图片到相册
    /// </summary>
    /// <param name="localImgPath"></param>
    public void SaveImgToPhoto(string localImgPath)
    {
        //print(localImgPath);
        savePath = localImgPath;
#if !UNITY_EDITOR && (IOS_APP || Android_APP)
        saveBitmap(localImgPath);
#endif
    }
    /// <summary>
    /// 分享到某平台
    /// </summary>
    /// <param name="target">具体的平台：微信朋友圈1，微信好友2，QQ好友3，QQ空间4</param>
    /// <param name="sharePath">分享的路径</param>
    public void ShareToTarget(int target, string sharePath)
    {
//        print(target);
//        //print("sharePath+" + sharePath);
//        ShareData sd = new ShareData();
//        sd.shareContentType = 2;
//        sd.title = "丰云行AR看车";
//        sd.img = sharePath;
//        sd.content = "丰云行AR看车";
//        sd.callBackFunctionName = "onShareCallBack";
//        string json = JsonUtility.ToJson(sd);
//        //string json = Json.Serialize(sd);
//#if !UNITY_EDITOR && (IOS_APP || Android_APP)
//        share(target, json);
//#endif
    }

    public void QuitAR(int switchType)
    {
        LoadState = 1;
        this.SwitchType = switchType;
        StartCoroutine(GameDataMgr.Instance.LoadGameScene("CarList"));
    }

    public void QuitAR(string carIndex, int switchType)
    {
        LoadState = 1;
        StartCoroutine(GameDataMgr.Instance.LoadGameScene("CarList"));
    }


    public void RealQuitAR()
    {
#if (!UNITY_EDITOR && (IOS_APP || Android_APP))
        ARReturnAPP(carCode, switchType);
#elif UNITY_EDITOR
        //print(carCode+" _"+ switchType);
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// true代表要显示车辆详情
    /// false代表不显示车辆详情
    /// </summary>
    /// <returns></returns>
    public bool IsShowXiangQing()
    {
        string tmp = carCode;
        M_ReqARCarShow();
        bool isShowDeatil = carCode == "NULL";
        carCode = tmp;
        return isShowDeatil;
    }
    /// <summary>
    /// 某辆车是否支持在线预订
    /// </summary>
    /// <param name="carIndex">0代表不显示，1代表显示</param>
    /// <returns></returns>
    public int IsCarSaleOnLine()
    {
        return IsCarSaleOnLine(carCode);
    }
    /// <summary>
    /// 某辆车是否支持在线预订
    /// </summary>
    /// <param name="carIndex">0代表不显示，1代表显示</param>
    /// <returns></returns>
    public int IsCarSaleOnLine(string carIndex)
    {
        //print(carIndex);
        int isSale = 0;
#if  !UNITY_EDITOR && (IOS_APP || Android_APP)
       isSale = IsSaleOnLine(carIndex);
#endif
        return isSale;
    }
    /// <summary>
    /// 申请AR展示
    /// </summary>
    public void M_ReqARCarShow()
    {
#if !UNITY_EDITOR && (IOS_APP || Android_APP)
#if Android_APP
        carCode=Android_ReqARCarShow();
#else
        carCode=ReqARCarShow();
#endif
#else
        carCode = defCarIndex;
#endif
    }

    #endregion
    #region  IOS AR调用的APP接口  本地/真机测试不会用到
#if IOS_APP
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern void UnityReturnIOS();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern void saveBitmap(string localImgPath);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern void share(int shareType, string content);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern string ReqARCarShow();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern int IsSaleOnLine(string carCode);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern void ARReturnAPP(string carCode, int switchType);
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern string getUserInfo();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern void ARSendBuriedPoint(string carCode, string eventName, string key);
#endif
    #endregion

    #region IOS APP调AR的接口  通知进入AR
    /// <summary>
    /// IOS 调用函数，传递参数进入到Unity场景
    /// 参数是路径
    /// </summary>
    /// <param name="msg">进入到AR的路径，可能是Camry/CHR，也可能是汉兰达</param>
    public void EnterSystem(string msg)
    {
        EnterUnity();
    }
    #endregion

    #region Android AR调用APP的接口

#if Android_APP
    ///退出AR返回至APP，退出的车型以及将要进入的页面
    public void ARReturnAPP(string carCode, int switchType)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("ARReturnAPP", carCode, switchType);
    }

    ///AR发送埋点事件
    public void ARSendBuriedPoint(string carCode, string eventName, string key)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("ARSendBuriedPoint", carCode, eventName, key);
    }
    /// <summary>
    /// 获取某辆车是否支持在线预订
    /// </summary>
    /// <param name="carIndex"></param>
    /// <returns></returns>
    public int IsSaleOnLine(string carCode)
    {
        int isSaleOnLine = 0;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //下载地址
        isSaleOnLine = jo.Call<int>("IsSaleOnLine", carCode);
        return isSaleOnLine;
    }
    ///申请展示车型的接口
    public string Android_ReqARCarShow()
    {
        string index = defCarIndex;
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        index = jo.Call<string>("ReqARCarShow");
        return index;
    }
    ///安卓的获取路径的函数
    public string saveFilePath()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //下载地址
        string filePath = jo.Call<string>("getArFilePath");
        return filePath;
    }

    ///安卓的的退出函数
    public void finish()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("onBackPressed");
    }

    /// <summary>
    /// 存储照片到相册
    /// </summary>
    /// <param name="localImgPath">程序生成的照片</param>
    public static void saveBitmap(string localImgPath)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("saveBitmap", localImgPath);
    }

    /// <summary>
    /// 分享功能
    /// </summary>
    /// <param name="shareType">分享的平台，微信，QQ，朋友圈等等</param>
    /// <param name="content">分享的内容，一个Jason文本</param>
    public static void share(int shareType, string content)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("share", shareType, content);
    }
    ///安卓的获取用户信息的接口
        /*
        1. ⽅法名：getUserInfo
        2. ⼊参数：⽆
        3. 出参数：
        userId：⽤户ID
        isLogin：是否登录
        uHeadUrl：头像图⽚地址
        uName:昵称
        示例：
        已登录：
        {"content":{"msg":"{\"isLogin\":true,\"uHeadUrl\":\"https://carapptest
        .gtmc.com.cn/fs01/20180316/2e6726702a99d7aa70324b581f98b7c8.png\",\"uName\
        ":\"咯哦\",\"userId\":\"48822\"}","resultCode":"1"},"interfaceName":"getUse
        rInfo"}
        未登录
        {"content":{"msg":"{\"isLogin\":false,\"uHeadUrl\":\"\",\"uName\":\"\"
        ,\"userId\":\"\"}","resultCode":"1"},"interfaceName":"getUserInfo"}
        */
    public static string getUserInfo()
    {
#if UNITY_EDITOR
        return null;
#else
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //下载地址
        string filePath = jo.Call<string>("getUserInfo");
        return filePath;
#endif

    }
#endif
    #endregion
    #region  APP调AR  分享回调
    /// <summary>
    /// 回调函数
    /// 返回的结果
    /// {"interfaceName":"share","result":{"resultCode":"0","errMsg":"未安装微信"}}
    /// </summary>
    public void onCallBack(string json)
    {
        //json = "{\"interfaceName\":\"saveBitmap\",\"result\":{\"resultCode\":\"1\",\"msg\":\"保存成功\"}}";

        var dict = Json.Deserialize(json) as Dictionary<string, object>;

        string interfaceName = dict["interfaceName"].ToString();
        Dictionary<string, object> result = dict["result"] as Dictionary<string, object>;
        int resultCode = int.Parse(result["resultCode"].ToString());
        string msg = result["msg"].ToString();
    }
    #endregion
}
