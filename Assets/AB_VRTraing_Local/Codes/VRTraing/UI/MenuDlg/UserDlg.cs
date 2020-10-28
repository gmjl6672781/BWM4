using BeinLab.Conf;
using BeinLab.Util;
using BeinLab.VRTraing;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;

namespace BeinLab.UI
{
    //public class UserInfo {
    //    public string orgId;
    //    public string userCode;
    //    public string userName;
    //    public string partTimeJobcodeTwo;
    //    public string userId;
    //}
    public class UserData
    {
        public string jobStatus;
        public string nameCn;
        public string userAgent;
        public string accessToken;
        public string userId;
        public string token;
        public string roleTypeVal;
        public string cardId;
        public string expire;
        public string name;
        public string userIp;
        public string curriculumVitaeCode;
        public string position;
        public UserInfo userLoginInfo;
    }
    public class ResultData
    {
        public string vrHistoryId;
        public string id;
        public int value;
        public string key;
    }
    public class ExamResult
    {
        [SerializeField]
        public string vrHistoryId;
        [SerializeField]
        public string id;
        [SerializeField]
        public int value;
        [SerializeField]
        public string key;
    }
    public class HistroyData
    {
        [SerializeField]
        public int examScores;
        [SerializeField]
        public string examDate;
        public List<ExamResult> examResult = new List<ExamResult>();
        [SerializeField]
        public int modelType;
        [SerializeField]
        public int userId;
        [SerializeField]
        public string userCode;
    }
    /// <summary>
    ///  /// <summary>
    /// 获取考试结果的记录
    /// {"msg":"获取信息成功","code":1,
    /// "data":[{"examScores":95,"examDate":"2020-01-05 16:12:52",
    /// "examResult":[{"vrHistoryId":null,"id":null,"value":0,"key":"T0"},
    /// {"vrHistoryId":null,"id":null,"value":1,"key":"T1"},
    /// {"vrHistoryId":null,"id":null,"value":2,"key":"T2"},
    /// {"vrHistoryId":null,"id":null,"value":3,"key":"T3"},
    /// {"vrHistoryId":null,"id":null,"value":4,"key":"T4"}],
    /// "modelType":0,"userId":1,"userCode":"ta.ta"}]}
    /// </summary>
    /// </summary>
    public class Histroy
    {
        [SerializeField]
        public string msg;
        [SerializeField]
        public int code;
        public List<HistroyData> data = new List<HistroyData>();
    }
    /// <summary>
    /// 用户面板
    /// </summary>
    public class UserDlg : Singleton<UserDlg>
    {
        public static UserConf user;
        public Text userText;
        public Text nameText;
        public Text addText;
        private List<ResultConf> resultList;
        public Button btnPrefab;
        public Transform contentRoot;
        public Sprite[] sprites;
        private Histroy histroyOfResultWeb;
        // Start is called before the first frame update
        void Start()
        {
            ///获取用户信息
            //TestGetUserInfon();
            //ShowUser(user);
            TestGetHistroy();
            //resultList = UnityUtil.ReadXMLData<ResultConf>(GameDataMgr.Instance.AssetPath);
            //if (resultList != null)
            //{
            //    ShowResults(resultList);
            //}
        }
        private void OnEnable()
        {
            ///获取用户信息
            //TestGetUserInfon();
            //ShowUser(user);
            TestGetHistroy();
        }

        public void TestGetHistroy()
        {
            if (!string.IsNullOrEmpty(LoginDlg.token) && LanguageMgr.Instance.settings != null && !string.IsNullOrEmpty(LoginDlg.userName))
            {
                string msg = LoginDlg.token;
                StartCoroutine(UnityUtil.HttpJsonPost(LanguageMgr.Instance.settings.ReqHistroyUrl + "/"
                    + LoginDlg.userName + "/" + LanguageMgr.Instance.settings.Size, msg, OnReqHistroy, false));
            }
        }
        /// <summary>
        /// {"msg":"获取信息成功","code":1,
        /// "data":[{"examScores":95,"examDate":"2020-01-05 16:12:52",
        /// "examResult":[{"vrHistoryId":null,"id":null,"value":0,"key":"T0"},
        /// {"vrHistoryId":null,"id":null,"value":1,"key":"T1"},
        /// {"vrHistoryId":null,"id":null,"value":2,"key":"T2"},
        /// {"vrHistoryId":null,"id":null,"value":3,"key":"T3"},
        /// {"vrHistoryId":null,"id":null,"value":4,"key":"T4"}],
        /// "modelType":0,"userId":1,"userCode":"ta.ta"}]}
        /// </summary>
        /// <param name="msg"></param>
        private void OnReqHistroy(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Debug.LogError("获取失败，可能网络原因");
                return;
            }
            //Dictionary<string, System.Object> loginResult = Json.Deserialize(msg) as Dictionary<string, System.Object>;
            //if (loginResult["data"].ToString() == "null")
            //{
            //    Debug.LogError("用户信息错误" + loginResult["data"]);
            //    return;
            //}
            //var data = loginResult["data"] as Dictionary<string, System.Object>;
            histroyOfResultWeb = JsonConvert.DeserializeObject<Histroy>(msg);
            if (histroyOfResultWeb.data == null)
            {
                Debug.LogError("用户信息错误" + histroyOfResultWeb.data);
                return;
            }
            else
            {
                ShowResults(histroyOfResultWeb);
            }
        }

        public void TestGetUserInfon()
        {
            if (!string.IsNullOrEmpty(LoginDlg.token) && LanguageMgr.Instance.settings != null
                && !string.IsNullOrEmpty(LoginDlg.userName))
            {
                string msg = LoginDlg.token;
                StartCoroutine(UnityUtil.HttpJsonPost(LanguageMgr.Instance.settings.ReqUserUrl
                    + "/" + LoginDlg.userName, msg, OnReqUserInfo, false));
            }
            else
            {
                Debug.LogError("TestGetUserInfon");
            }
        }

        private void OnReqUserInfo(string msg)
        {
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
            UserInfo userInfo = new UserInfo();
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
            user = new UserConf();
            user.UserName = userInfo.userCode;
            user.ShowName = userInfo.userNameCn;
            user.Address = userInfo.orgName;
            user.IdNum = userInfo.userId;
            Debug.LogError(JsonConvert.SerializeObject(user));
            ShowUser(user);
        }

        /// <summary>
        /// 显示历史记录
        /// </summary>
        /// <param name="resultList"></param>
        private void ShowResults(List<ResultConf> resultList)
        {
            if (contentRoot && contentRoot.childCount > 0)
            {
                for (int i = contentRoot.childCount - 1; i >= 0; i--)
                {
                    Destroy(contentRoot.GetChild(i).gameObject);
                }
            }

            if (contentRoot && btnPrefab)
            {
                for (int i = 0; i < resultList.Count; i++)
                {
                    ResultConf resultConf = resultList[i];
                    //List<TaskInfo> taskInfoList = (List<TaskInfo>)JsonUtility.FromJson(resultConf.Result, typeof(List<TaskInfo>));
                    Button btn = Instantiate(btnPrefab);
                    UnityUtil.SetParent(contentRoot, btn.transform);
                    Text indexText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "IndexText");
                    Text taskText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "TaskText");
                    Image resultImg = UnityUtil.GetTypeChildByName<Image>(btn.gameObject, "ResultImg");
                    indexText.text = (i + 1) + ".";
                    taskText.text = resultConf.Date;
                    resultImg.sprite = sprites[float.Parse(resultConf.Score) >= LanguageMgr.Instance.settingsConf[0].NormalScore ? 0 : 1];
                    btn.onClick.AddListener(() =>
                    {
                        OnClickResult(resultConf);
                    });
                }
            }
        }
        /// <summary>
        /// 显示历史记录
        /// </summary>
        /// <param name="resultList"></param>
        private void ShowResults(Histroy histroy)
        {
            if (contentRoot && contentRoot.childCount > 0)
            {
                for (int i = contentRoot.childCount - 1; i >= 0; i--)
                {
                    Destroy(contentRoot.GetChild(i).gameObject);
                }
            }

            if (contentRoot && btnPrefab && histroy != null && histroy.data != null)
            {
                for (int i = 0; i < histroy.data.Count; i++)
                {
                    int index = i;
                    HistroyData resultConf = histroy.data[index];
                    //List<TaskInfo> taskInfoList = (List<TaskInfo>)JsonUtility.FromJson(resultConf.Result, typeof(List<TaskInfo>));
                    Button btn = Instantiate(btnPrefab);
                    UnityUtil.SetParent(contentRoot, btn.transform);
                    Text indexText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "IndexText");
                    Text taskText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "TaskText");
                    Image resultImg = UnityUtil.GetTypeChildByName<Image>(btn.gameObject, "ResultImg");
                    indexText.text = (index + 1) + ".";
                    taskText.text = resultConf.examDate;
                    resultImg.sprite = sprites[resultConf.examScores >= LanguageMgr.Instance.settingsConf[0].NormalScore ? 0 : 1];
                    btn.onClick.AddListener(() =>
                    {
                        OnClickResult(resultConf);
                    });
                }
            }
        }
        /// <summary>
        /// 点击了某条结果
        /// </summary>
        /// <param name="resultConf"></param>
        private void OnClickResult(ResultConf resultConf)
        {
            List<TaskInfo> taskInfos = (List<TaskInfo>)JsonUtility.FromJson(resultConf.Result, typeof(List<TaskInfo>));
            if (taskInfos != null)
            {
                AnswerFeedDlg.Instance.ShowResult(taskInfos, resultConf.ModelType);
            }
            else
            {
                Debug.LogError("resultConf" + resultConf.Result);
            }
        }
        /// <summary>
        /// 点击了某条结果
        /// </summary>
        /// <param name="resultConf"></param>
        private void OnClickResult(HistroyData resultConf)
        {
            //List<TaskInfo> taskInfos = (List<TaskInfo>)JsonUtility.FromJson(resultConf.Result, typeof(List<TaskInfo>));
            //if (taskInfos != null)
            //{
            //    AnswerFeedDlg.Instance.ShowResult(taskInfos, resultConf.ModelType);
            //}
            //else
            //{
            //    Debug.LogError("resultConf" + resultConf.Result);
            //}
            if (resultConf != null && ResultDlg.Instance)
            {
                ResultDlg.Instance.InitComponent(resultConf);
            }
            MenuDlg.Instance.CloseDlg();
        }
        /// <summary>
        /// 添加一条考试或者培训记录
        /// </summary>
        public void AddResoult(ResultConf conf)
        {
            if (resultList == null)
            {
                resultList = new List<ResultConf>();
            }
            resultList.Add(conf);
            UnityUtil.WriteXMLData(GameDataMgr.Instance.AssetPath, conf);
            ShowResults(resultList);
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        //public static void Login(string userName, string password)
        //{
        //    user = null;
        //    user = new UserConf();
        //    user.UserName = userName;
        //    user.PassWord = password;
        //    user.ShowName = userName;
        //}
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void Login(string jsonMessage)
        {
            UserData userData = JsonConvert.DeserializeObject<UserData>(jsonMessage);
            if (userData != null)
            {
                //user = new UserConf();
                //user.UserName = userData.userLoginInfo.userCode;
                //user.Token = userData.token;
                //user.IdNum = userData.cardId;
                //user.ShowName = userData.userLoginInfo.userName;
                //user.Address = userData.userLoginInfo.orgId;
            }
        }
        /// <summary>
        /// 登出
        /// </summary>
        public void LogOut()
        {
            user = null;
            ShowUser(new UserConf());
        }

        /// <summary>
        /// 显示用户节点
        /// </summary>
        public void ShowUser(UserConf user)
        {
            if (user != null)
            {
                userText.text = user.UserName;
                nameText.text = user.ShowName;
                addText.text = user.Address;
            }
        }
    }
}