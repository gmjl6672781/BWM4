using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.UI;
using Karler.WarFire.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace BeinLab.UI
{
    /// <summary>
    /// 结果窗口
    /// </summary>
    public class ResultDlg : Singleton<ResultDlg>
    {
        private GameObject player;
        private GameObject gameController;

        public string examPassKey = "T1074";
        public string examNoPassKey = "T1075";
        public string traingKey = "T1076";

        public string examNoPassResultKey = "T99";
        public string traingNoPassResultKey = "T110";

        /// <summary>
        /// 结果标题
        /// </summary>
        public TextHelper titleLabel;
        /// <summary>
        /// 结果统计
        /// </summary>
        public TextHelper resultLabel;
        /// <summary>
        /// 按钮预制体
        /// </summary>
        public Button buttonPrefab;
        public float normalScore = 70;
        public Transform contentRoot;
        public List<Sprite> resultIcon;
        public Button returnButton;
        public Button reStartButton;
        private BaseDlg baseDlg;
        //public string url;
        private VRHand target;
        public SteamVR_Input_Sources handType;
        public int forward = -1;

        public Text totalCountText;
        public GameObject resultRoot;
        public GameObject histroyRoot;
        public Button personBtn;
        public Text examSocreText;
        private void Start()
        {
            player = GameObject.Find("Player");
            gameController = GameObject.Find("GameController");
            baseDlg = GetComponent<BaseDlg>();
            returnButton.onClick.AddListener(ClickReturnButton);
            reStartButton.onClick.AddListener(ClickReStartButton);
            personBtn.onClick.AddListener(ClickPerson);
            for (int i = 0; i < Player.instance.handCount; i++)
            {
                if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                {
                    target = Player.instance.hands[i].GetComponent<VRHand>();
                    break;
                }
            }
            resultRoot.SetActive(false);
            histroyRoot.SetActive(false);
            gameObject.SetActive(false);
            //InitComponent();
        }

        private void ClickPerson()
        {
            gameObject.SetActive(false);

            if (MenuDlg.Instance.Dlg)
            {
                MenuDlg.Instance.Dlg.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        private void ClickReStartButton()
        {
            ClearComponent();
            gameObject.SetActive(false);

            TaskManager.Instance.RestartTask();
            //Destroy(gameObject);
        }
        /// <summary>
        /// 返回首页
        /// </summary>
        private void ClickReturnButton()
        {
            ClearComponent();
            SceneManager.LoadScene(0);
            Destroy(player);
            Destroy(gameController);
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        /// <summary>
        /// 清理组件
        /// </summary>
        public void ClearComponent()
        {
            for (int i = 0; i < contentRoot.childCount; i++)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        public bool InitComponent()
        {
            //if (UserDlg.user == null) {
            //    gameObject.SetActive(false);
            //}
            resultRoot.SetActive(true);
            histroyRoot.SetActive(false);
            bool isSuccess = false;
            gameObject.SetActive(true);
            ClearComponent();
            string titleKey = traingKey;
            string resultKey = traingNoPassResultKey;
            List<TaskConf> taskList = null;
            List<ResultInfo> infoList = new List<ResultInfo>();
            if (TaskManager.Instance)
            {
                taskList = TaskManager.Instance.dicRecordTasks[TaskManager.Instance.taskMode];
            }
            //string jsonResult = "";
            if (taskList != null && taskList.Count > 0)
            {
                if (TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination)
                {
                    titleKey = examNoPassKey;
                    resultKey = examNoPassResultKey;
                }
                float score = 0;
                int notPass = 0;
                for (int i = 0; i < taskList.Count; i++)
                {
                    TaskConf conf = taskList[i];
                    Button btn = Instantiate(buttonPrefab);
                    UnityUtil.SetParent(contentRoot, btn.transform);
                    string[] keytips = LanguageMgr.Instance.GetMessage(conf.keyTip).Message.Split('、');
                    string keytip = keytips[0];
                    if (keytips.Length > 1)
                    {
                        keytip = (i + 1) + "、" + keytips[1];
                    }
                    btn.GetComponentInChildren<Text>().text =UnityUtil.SplitToLine( keytip);// LanguageMgr.Instance.GetMessage(taskList[i].keyTip).Message;

                    int sprite = 0;
                    Image img = UnityUtil.GetTypeChildByName<Image>(btn.gameObject, "Image");
                    Text scoreText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "Text (1)");
                    ResultInfo info = new ResultInfo();
                    info.key = conf.keyTip;
                    info.value = 0;
                    //print(taskList[i] + "  " + conf.IsPass);
                    float curScore = TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination ? conf.examScore : conf.traingScore;
                    scoreText.text = curScore.ToString();
                    //img.gameObject.SetActive(TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination);
                    if (conf.IsPass)
                    {
                        btn.interactable = true;
                        score += curScore;
                        info.value = 1;
                        //if (TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination)
                        //{

                        //}
                        sprite = 1;
                        notPass++;
                    }
                    else
                    {
                        btn.interactable = false;
                    }
                    if (img.gameObject.activeSelf)
                    {
                        img.sprite = resultIcon[sprite];
                    }

                    //Debug.Log("taskList.Count = "+taskList.Count + "        notPass = "+notPass);
                    totalCountText.text = taskList.Count - notPass + "";
                    examSocreText.text = score.ToString();
                    infoList.Add(info);
                }
                ///如果是考试模式
                if (TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination)
                {
                    if (score >= normalScore)
                    {
                        titleKey = examPassKey;
                        isSuccess = true;
                    }
                }
                titleLabel.SetMessageKey(titleKey);
                LanguageConf resultLC = LanguageMgr.Instance.GetMessage(resultKey);
                string reslutMsg = "";
                if (resultLC != null)
                {
                    reslutMsg = resultLC.Message;
                }
                //reslutMsg += ": " + notPass;
                resultLabel.SetMessage(reslutMsg);
                //tdwc.TransformData((int)score);
                //TestSendResult(infoList, (int)score);
            }
            return isSuccess;
        }

        public TransferDataWhenComplete tdwc;

        /// <summary>
        /// 初始化组件
        /// 点击了历史记录
        /// </summary>
        /// <param name="resultConf"></param>
        public void InitComponent(HistroyData resultConf)
        {
            //if (UserDlg.user == null) {
            //    gameObject.SetActive(false);
            //}
            resultRoot.SetActive(false);
            histroyRoot.SetActive(true);
            gameObject.SetActive(true);
            ClearComponent();
            string titleKey = traingKey;
            string resultKey = traingNoPassResultKey;
            //List<TaskConf> taskList = null;
            List<ExamResult> examResult = resultConf.examResult;
            //if (TaskManager.Instance)
            //{
            //    taskList = TaskManager.Instance.dicRecordTasks[TaskManager.Instance.taskMode];
            //}
            if (examResult != null && examResult.Count > 0)
            {
                VRTraing.TaskMode taskMode = VRTraing.TaskMode.Training;
                //if (TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination)
                if (resultConf.modelType==1)
                {
                    titleKey = examNoPassKey;
                    resultKey = examNoPassResultKey;
                    taskMode = VRTraing.TaskMode.Examination;
                }
                float score = 0;
                int notPass = 0;
                for (int i = 0; i < examResult.Count; i++)
                {
                    
                    LanguageConf lc = LanguageMgr.Instance.GetMessage(
                        examResult[i].key);
                    if (lc == null)
                    {
                        continue;
                    }
                    TaskConf task = TaskManager.Instance.GetTaskConf(examResult[i].key, (int)taskMode);
                    if (!task)
                    {
                        continue;
                    }
                    Button btn = Instantiate(buttonPrefab);
                    UnityUtil.SetParent(contentRoot, btn.transform);
                    string[] keytips = LanguageMgr.Instance.GetMessage(task.keyTip).Message.Split('、');
                    string keytip = keytips[0];
                    if (keytips.Length > 1)
                    {
                        keytip = (i + 1) + "、" + keytips[1];
                    }
                    btn.GetComponentInChildren<Text>().text = UnityUtil.SplitToLine(keytip);
                    int sprite = 0;
                    Image img = UnityUtil.GetTypeChildByName<Image>(btn.gameObject, "Image");
                    Text scoreText = UnityUtil.GetTypeChildByName<Text>(btn.gameObject, "Text (1)");
                    
                    //img.gameObject.SetActive(TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination);
                    float curScore = taskMode == VRTraing.TaskMode.Examination ? task.examScore : task.traingScore;
                    scoreText.text = curScore.ToString();
                    if (examResult[i].value == 1)
                    {
                        btn.interactable = true;
                        if (task == null)
                        {
                            continue;
                        }
                        score += curScore;
                        //if (TaskManager.Instance.taskMode == VRTraing.TaskMode.Examination)
                        //{

                        //}
                        sprite = 1;
                        notPass++;
                    }
                    else
                    {
                        btn.interactable = false;
                    }
                    if (img.gameObject.activeSelf)
                    {
                        img.sprite = resultIcon[sprite];
                    }

                    //Debug.Log("taskList.Count = "+taskList.Count + "        notPass = "+notPass);
                    totalCountText.text = examResult.Count - notPass + "";
                    examSocreText.text = score.ToString();
                }


                ///如果是考试模式
                if (taskMode == VRTraing.TaskMode.Examination)
                {
                    if (score >= normalScore)
                    {
                        titleKey = examPassKey;
                    }
                }
                titleLabel.SetMessageKey(titleKey);
                LanguageConf resultLC = LanguageMgr.Instance.GetMessage(resultKey);
                string reslutMsg = "";
                if (resultLC != null)
                {
                    reslutMsg = resultLC.Message;
                }
                //reslutMsg += ": " + notPass;
                resultLabel.SetMessage(reslutMsg);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultConf"></param>
        public void TestSendResult(List<ResultInfo> infoList, int examScores)
        {
            //if (string.IsNullOrEmpty(LoginDlg.userName) || UserDlg.user == null)
            //{
            //    Debug.LogErrorFormat("Error Send Msg {0},{1}", LoginDlg.userName, UserDlg.user);
            //    return;
            //}
            SendResult sr = new SendResult();
            sr.userCode = LoginDlg.userName;
            sr.userId = UserDlg.user.IdNum;
            sr.modelType = (int)TaskManager.Instance.taskMode;
            sr.examDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sr.examScores = examScores;
            sr.examResult = infoList;
            sr.vrName = LanguageMgr.Instance.settings.VrName;
            string msg = JsonConvert.SerializeObject(sr);// JsonUtility.ToJson(sr);// JsonUtility.ToJson(sr);
            //Debug.LogError(msg);
            StartCoroutine(UnityUtil.HttpJsonPost(LanguageMgr.Instance.settings.SendResultUrl, msg,
                OnReqSendResult));
        }

        private void OnReqSendResult(string msg)
        {
            print(msg);
        }

        private void Update()
        {
            if (baseDlg && target)
            {
                transform.position = target.GetDlgLinePos(baseDlg.transPos);
                UnityUtil.LookAtV(transform, Player.instance.hmdTransform, forward);
            }
        }
    }
}