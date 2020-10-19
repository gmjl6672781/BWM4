using BeinLab.UI;
using BeinLab.Util;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using Karler.WarFire.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;

/// <summary>
/// 用户登陆窗口
/// </summary>
namespace BeinLab.VRTraing.UI
{
    public class LoginParm
    {
        public string username;
        public string password;
    }
    public class LoginResult
    {
        public string msg;
        public string code;
        public string data;
    }
    public class LoginDlg : Singleton<LoginDlg>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        private InputField userField;
        /// <summary>
        /// 密码
        /// </summary>
        private InputField passwordField;

        private Button loginBtn;
        private SimpleTextTips simpleTextTips;
        public float tipsTime = 2;
        public string nullErrorKey;
        public string userErrorKey;
        public string NetworkErrorKey;

        public static string token;
        public static string userName;
        private SingleSelectButton languageButton;
        /// <summary>
        /// {"username":"ta.ta","password":"Joylearning@2019"}
        /// </summary>
        //public string testJson;
#if UNITY_EDITOR
        public bool isRealTest = true;
#endif
        /*https://training1.bmw.com.cn/BMWTAP/emailConfirmation/checkUser?username=guotao.zhou&password=welcome1 */
        // Start is called before the first frame update
        void Start()
        {
            BaseDlg dlg = GetComponent<BaseDlg>();
            userField = dlg.GetChildComponent<InputField>("UserField");
            passwordField = dlg.GetChildComponent<InputField>("PasswordField");
            loginBtn = dlg.GetChildComponent<Button>("LoginBtn");
            simpleTextTips = dlg.GetChildComponent<SimpleTextTips>("SimpleTextTips");
            languageButton = dlg.GetChildComponent<SingleSelectButton>("LanguageButton");
            //userField.onEndEdit.AddListener(OnEnterMsg);
            //passwordField.onEndEdit.AddListener(OnEnterMsg);
            loginBtn.onClick.AddListener(OnLoginBtn);
            languageButton.OnSelect += OnClickSelect;
            StartCoroutine(InitLanguage());
            //languageButton.SetKeys();
#if !UNITY_EDITOR
            userField.text = "";
            passwordField.text = "";
#endif
        }
        /// <summary>
        /// 1，等待语言控制器初始化
        /// 2，读取语言默认设置
        /// 3，设置语言
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitLanguage()
        {
            while (LanguageMgr.Instance.LanguageList == null || LanguageMgr.Instance.LanguageList.Count < 1)
            {
                yield return new WaitForFixedUpdate();
            }
            List<string> languages = new List<string>();
            for (int i = 0; i < LanguageMgr.Instance.LanguageList.Count; i++)
            {
                languages.Add(LanguageMgr.Instance.LanguageList[i].Language);
            }
            languageButton.SetKeys(languages, LanguageMgr.Instance.settingsConf[0].LanguageIndex);
        }

        private void OnClickSelect(int index)
        {
            LanguageMgr.Instance.SelectLanguage(index);

        }

        /// <summary>
        /// 点击了登陆按钮
        /// </summary>
        private void OnLoginBtn()
        {
            if ((string.IsNullOrEmpty(userField.text) || string.IsNullOrEmpty(passwordField.text)))
            {
                simpleTextTips.ShowMessage(nullErrorKey, 2);
                return;
            }
            //StartCoroutine(HttpPost(LanguageMgr.Instance.url, usnerField.text, passwordField.text));
            //StartCoroutine(SendMessage(userField.text, passwordField.text));
            LoginJson lj = new LoginJson();
            lj.username = userField.text;
            lj.password = passwordField.text;
            string msg = JsonUtility.ToJson(lj);
            //StartCoroutine(HttpJsonPost(loginUrl, msg, OnReqLogin));
            Debug.Log(LanguageMgr.Instance.settings.LoginUrl);
            StartCoroutine(UnityUtil.HttpJsonPost(LanguageMgr.Instance.settings.LoginUrl, msg, OnReqLogin));
        }

        private void OnReqLogin(string msg)
        {
            if (LanguageMgr.Instance.settings.IsFoceLogin)
            {
                token = LanguageMgr.Instance.settings.Token;
                userName = LanguageMgr.Instance.settings.UserName;
                SceneManager.LoadScene(1);
                return;
            }
            if (string.IsNullOrEmpty(msg))
            {
                Debug.LogError("登录错误，网络相关");
                simpleTextTips.ShowMessage(NetworkErrorKey, 2);
                //OnEnterError();
                //SceneManager.LoadScene(1);
                return;
            }
            Dictionary<string, System.Object> loginResult = Json.Deserialize(msg) as Dictionary<string, System.Object>;
            if (loginResult["code"].ToString() != "1")
            {
                Debug.LogError("用户名密码错误" + loginResult["code"]);
                OnEnterError();
                return;
            }
            var data = loginResult["data"] as Dictionary<string, System.Object>;
            token = data["token"].ToString();
            userName = userField.text;
            SceneManager.LoadScene(1);
        }

        public void OnEnterError()
        {
            simpleTextTips.ShowMessage(userErrorKey, 3);
        }

        //public void OnEnterTaring()
        //{
        //    UserDlg.Login(userField.text, passwordField.text);
        //    SceneManager.LoadScene(1);
        //}
        //public void OnEnterTaring(string data)
        //{
        //    UserDlg.Login(data);
        //    SceneManager.LoadScene(1);
        //}
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        //public IEnumerator SendMessage(string user, string password)
        //{
        //    loginBtn.interactable = false;
        //    WWWForm form = new WWWForm();
        //    form.AddField("username", userField.text);
        //    form.AddField("password", passwordField.text);
        //    //form.AddField("application/json", testJson);
        //    UnityWebRequest webRequest = UnityWebRequest.Post("", form);
        //    yield return webRequest.SendWebRequest();
        //    loginBtn.interactable = true;
        //    string relut = "";
        //    if (!webRequest.isHttpError && !webRequest.isNetworkError)
        //    {
        //        relut = (webRequest.downloadHandler.text);
        //        if (relut == "true")
        //        {
        //            OnEnterTaring();
        //            yield break;
        //        }
        //        else
        //        {
        //            Debug.Log(webRequest.url);
        //        }
        //    }
        //    Debug.Log(webRequest.error + relut);
        //    OnEnterError();
        //}
        /// <summary>
        /// 使用Application/Json模式，输出
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //public IEnumerator HttpPost(string url, string user, string password)
        //{
        //    LoginParm lp = new LoginParm();
        //    lp.username = user;
        //    lp.password = password;
        //    string message = JsonConvert.SerializeObject(lp);

        //    string jsonParam = message;/// JsonConvert.SerializeObject(param);
        //    print(jsonParam);
        //    byte[] body = Encoding.UTF8.GetBytes(jsonParam);
        //    UnityWebRequest unityWeb = new UnityWebRequest(url, "POST");
        //    unityWeb.uploadHandler = new UploadHandlerRaw(body);
        //    unityWeb.SetRequestHeader("Content-Type", "application/json");
        //    unityWeb.downloadHandler = new DownloadHandlerBuffer();
        //    yield return unityWeb.SendWebRequest();
        //    string result = "";
        //    if (unityWeb.isDone)
        //    {
        //        result = unityWeb.downloadHandler.text;
        //        print(result);
        //        LoginResult lr = JsonConvert.DeserializeObject<LoginResult>(result);
        //        print(lr);
        //        if (lr.code == "1")
        //        {
        //            OnEnterTaring(lr.data);
        //            yield break;
        //        }
        //        else if (lr.code == "-417")
        //        {
        //            ///账号正确，密码错误，清除密码输入框
        //            //yield break;
        //            passwordField.text = "";
        //        }
        //    }
        //    Debug.Log(unityWeb.error + result);
        //    OnEnterError();
        //}
        /// <summary>
        /// 用户中心
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        //public IEnumerator HttpPost_UserCenter(string url, string token)
        //{
        //    string message = token;// JsonConvert.SerializeObject(lp);

        //    string jsonParam = message;/// JsonConvert.SerializeObject(param);
        //    print(jsonParam);
        //    byte[] body = Encoding.UTF8.GetBytes(jsonParam);
        //    UnityWebRequest unityWeb = new UnityWebRequest(url, "POST");
        //    unityWeb.uploadHandler = new UploadHandlerRaw(body);
        //    unityWeb.SetRequestHeader("Content-Type", "application/json");
        //    unityWeb.downloadHandler = new DownloadHandlerBuffer();
        //    yield return unityWeb.SendWebRequest();
        //    string result = "";
        //    if (unityWeb.isDone)
        //    {
        //        result = unityWeb.downloadHandler.text;
        //        LoginResult lr = JsonConvert.DeserializeObject<LoginResult>(result);
        //        if (lr.code == "1")
        //        {
        //            OnEnterTaring(lr.data);
        //            yield break;
        //        }
        //        else if (lr.code == "-417")
        //        {
        //            ///账号正确，密码错误，清除密码输入框
        //            //yield break;
        //            passwordField.text = "";
        //        }
        //    }
        //    Debug.Log(unityWeb.error + result);
        //    OnEnterError();
        //}
    }
}

