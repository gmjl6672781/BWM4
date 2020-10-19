using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class BMWNetTest : MonoBehaviour
{
    /// <summary>
    /// 登录的接口
    /// </summary>
    public string loginUrl = "https://testjoylearning.bmw.com.cn:9500/auth/login";
    /// <summary>
    /// 申请个人信息的接口
    /// </summary>
    public string reqUserUrl = "https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/personalDetails";
    /// <summary>
    /// 发送考试结果的接口
    /// </summary>
    public string sendResultUrl = "https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/examination";
    /// <summary>
    /// 查看历史记录的接口
    /// </summary>
    public string reqHistroyUrl = " https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/history";
    public string token;
    /// <summary>
    /// 登录结果
    /// </summary>
    private LoginResult loginResult;
    /// <summary>
    /// 登录后的数据信息
    /// </summary>
    private LoginData loginData;
    /// <summary>
    /// 登录后的个人信息
    /// </summary>
    private UserLoginInfo userLoginInfo;
    public string testUserName;
    public string testPassword;
    private UserInfo userInfo;
    public int size = 1;
    public string testSendMsg;
    // Start is called before the first frame update
    void Start()
    {
        //TestLogin();
        //TestGetUserInfon();
        //TestGetHistroy();
        TestSendResult();
    }
    /// <summary>
    /// 测试登录
    /// </summary>
    public void TestLogin()
    {
        LoginJson lj = new LoginJson();
        lj.username = testUserName;
        lj.password = testPassword;
        string msg = JsonUtility.ToJson(lj);
        StartCoroutine(HttpJsonPost(loginUrl, msg, OnReqLogin));
    }
    /// <summary>
    /// 接受到登录请求
    /// </summary>
    /// <param name="obj"></param>
    private void OnReqLogin(string msg)
    {
        print(msg);
        ///登录失败
        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogError("登录错误，网络相关");
            return;
        }
        Dictionary<string, System.Object> loginResult = Json.Deserialize(msg) as Dictionary<string, System.Object>;
        if (loginResult["code"].ToString() != "1")
        {
            Debug.LogError("用户名密码错误" + loginResult["code"]);
            return;
        }
        var data = loginResult["data"] as Dictionary<string, System.Object>;
        token = data["token"].ToString();
        print(token);
    }
    /// <summary>
    /// 获取用户账号密码
    /// </summary>
    public void TestGetUserInfon()
    {
        string msg = token;
        StartCoroutine(HttpJsonPost(reqUserUrl + "/" + testUserName, msg, OnReqUserInfo, false));
    }

    /// <summary>
    /// 获取用户账号密码
    /// </summary>
    public void TestGetHistroy()
    {
        string msg = token;
        StartCoroutine(HttpJsonPost(reqHistroyUrl + "/" + testUserName + "/" + size, msg, OnReqHistroy, false));
    }

    /// <summary>
    /// 提交考试结果
    /// </summary>
    public void TestSendResult()
    {
        /*
         public string userCode;
    //"userId": "1",
    public string userId;

    //"modelType": 0,
    public int modelType;

    //"examDate": "2020-01-05 16:12:52",
    public string examDate;
    //"examScores": 95,
    public int examScores;
    public List<ResultInfo> examResult;
         */
        SendResult sr = new SendResult();
        sr.userCode = "ta.ta";
        sr.userId = "1";
        sr.modelType = 0;
        sr.examDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        sr.examScores = 90;
        //Dictionary<string, int> resultDic = new Dictionary<string, int>();
        List<ResultInfo> infoList = new List<ResultInfo>();
        for (int i = 0; i < 10; i++)
        {
            ResultInfo info = new ResultInfo();
            info.key = "T" + i;
            info.value = (int)(UnityEngine.Random.value + 0.5f);
            infoList.Add(info);
            //resultDic.Add(info.key, info.value);
        }
        //print(Json.Serialize(resultDic));
        //string examResult = Json.Serialize(resultDic);
        sr.examResult = infoList;
        print(JsonUtility.ToJson(sr));
        string msg = testSendMsg;// JsonUtility.ToJson(sr);
        StartCoroutine(HttpJsonPost(sendResultUrl, msg, OnReqSendResult));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="msg"></param>
    private void OnReqSendResult(string msg)
    {
        print(msg);
    }

    /// <summary>
    /// 获取考试结果的记录
    /// </summary>
    /// <param name="obj"></param>
    private void OnReqHistroy(string msg)
    {
        print(msg);
    }


    private void OnReqUserInfo(string msg)
    {
        print(msg);
        ///登录失败
        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogError("获取失败，可能网络原因");
            return;
        }
        Dictionary<string, System.Object> loginResult = Json.Deserialize(msg) as Dictionary<string, System.Object>;
        if (loginResult["data"].ToString() == "null")
        {
            Debug.LogError("用户信息错误" + loginResult["data"]);
            return;
        }
        var data = loginResult["data"] as Dictionary<string, System.Object>;
        userInfo = new UserInfo();
        if (data["orgName"] != null)
            userInfo.orgName = data["orgName"].ToString();
        if (data["orgNameEn"] != null)
            userInfo.orgNameEn = data["orgNameEn"].ToString();
        if (data["userCode"] != null)
            userInfo.userCode = data["userCode"].ToString();
        if (data["userId"] != null)
            userInfo.userId = data["userId"].ToString();
        if (data["userNameCn"] != null)
            userInfo.userNameCn = data["userNameCn"].ToString();
        print(userInfo.userId);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public IEnumerator HttpJsonPost(string url, string jsonParam, Action<string> OnReqData, bool isPost = true)
    {
        print(url + "\n" + jsonParam);
        byte[] body = Encoding.UTF8.GetBytes(jsonParam);
        UnityWebRequest unityWeb = new UnityWebRequest(url, isPost ? "POST" : "GET");
        unityWeb.uploadHandler = new UploadHandlerRaw(body);
        //unityWeb.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        unityWeb.SetRequestHeader("Content-Type", "application/json");
        unityWeb.downloadHandler = new DownloadHandlerBuffer();
        yield return unityWeb.SendWebRequest();
        string result = null;
        if (unityWeb.isDone)
        {
            result = unityWeb.downloadHandler.text;
        }
        else
        {
            Debug.Log("Http 请求失败");
            Debug.Log(unityWeb.error);
        }
        OnReqData?.Invoke(result);
    }


    public T PackToObj<T>(Dictionary<string, System.Object> dict)
    {
        T obj = default(T);
        if (dict != null)
        {
            foreach (var item in dict)
            {
                if (dict[item.Key] is Dictionary<System.Object, System.Object>)
                {

                }
                else
                {

                }
            }
        }
        return obj;
    }
}
