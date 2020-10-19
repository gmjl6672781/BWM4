using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing;
using BeinLab.VRTraing.Mgr;
using UnityEngine.UI;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Conf;

/// <summary>
/// 维修完成显示答案窗口，教学模式，考试模式
/// </summary>
namespace BeinLab.VRTraing.UI
{

    public class AnswerFeedDlg : Singleton<AnswerFeedDlg>
    {
        private TextHelper lableTitle;
        private TextHelper lableJilu;
        private TextHelper lableDefen;
        private TextHelper lableBtnReturn;
        private TextHelper lableBtnReStart;
        private TextHelper lableContent;//存放多个选项的ScrollRect容器
        private Button btnReturn;
        private Button btnReStart;
        public Sprite rightSpt;//正确图片
        public Sprite errorSpt;//错误图片
        List<TaskConf> tasks = new List<TaskConf>();
        public GameObject goSlelectionPre;//每个选项对应的UI预设 Content
        public string keyTitle = "T1074";//标题的key 
        public string keyJilu = "T99";//记录文字的key -- 两种模式不同
        public string keyDefen = "T100";//得分文字的key -- 考试模式显示
        public string keyReturn = "T29";//返回首页的key
        public string keyReStart = "T28";//重新开始的key

        int errorNum = 0;
        float totalScore = 0;



        protected override void Awake()
        {
            base.Awake();
            InitComponent();
            AddListener();
            HideDlg();
        }
        private void Start()
        {
        }

        /// <summary>
        /// 配置文件中任务开始时进行调用显示窗口
        /// </summary>
        public void ShowDlg()
        {
            this.gameObject.SetActive(true);
            tasks = TaskManager.Instance.GetTaskRecords();
            if (tasks.Count == 0)
                return;

            errorNum = 0;
            totalScore = 0;
            foreach (TaskConf task in tasks)
            {
                //考试模式有2种title，培训模式只有1种title

                //设置每一项任务的文字
                GameObject goSelection = Instantiate<GameObject>(goSlelectionPre, lableContent.transform);
                TextHelper tmp = UnityUtil.GetTypeChildByName<TextHelper>(goSelection, "SelText");
                tmp.SetMessageKey(task.keyTitle);

                //设置对错图片
                bool opeBool = task.IsPass;
                Image img = UnityUtil.GetTypeChildByName<Image>(goSelection, "Answer");
                if (opeBool)
                {
                    img.sprite = rightSpt;
                    totalScore += task.score;
                }
                else
                {
                    img.sprite = errorSpt;
                    errorNum++;
                }

                if (TaskManager.Instance.taskMode == TaskMode.Examination)//考试模式
                {
                    //设置每一项对应的分数
                    Text score = UnityUtil.GetTypeChildByName<Text>(goSelection, "Score");
                    score.text = task.score + "分";
                }
            }

            if (TaskManager.Instance.taskMode == TaskMode.Examination)//考试模式
            {
                if (totalScore >= 70)
                {
                    keyTitle = "T1074";
                }
                else
                {
                    keyTitle = "T1075";
                }
                keyJilu = "T99";
                lableDefen.SetMessageKey(keyDefen);
                Text error = UnityUtil.GetTypeChildByName<Text>(gameObject, "ErrorNum");
                error.text = errorNum + "个";
                Text total = UnityUtil.GetTypeChildByName<Text>(gameObject, "TotalScore");
                total.text = totalScore + "分";
            }
            else//培训模式
            {
                keyTitle = "T1076";
                keyJilu = "T110";
                Text error = UnityUtil.GetTypeChildByName<Text>(gameObject, "ErrorNum");
                error.text = errorNum + "个";
            }
            lableTitle.SetMessageKey(keyTitle);
            lableJilu.SetMessageKey(keyJilu);
            lableBtnReturn.SetMessageKey(keyReturn);
            lableBtnReStart.SetMessageKey(keyReStart);
        }

        public void HideDlg()
        {
            this.gameObject.SetActive(false);
        }

        public void ShowResult(List<TaskInfo> infoList,int mode)
        {
            if (infoList.Count == 0)
                return;

            errorNum = 0;
            totalScore = 0;
            for (int i = 0; i < infoList.Count; i++)
            {
                TaskInfo result = infoList[i];
                TaskConf task = TaskManager.Instance.GetTaskConf(result.Key, mode);
                //考试模式有2种title，培训模式只有1种title

                //设置每一项任务的文字
                GameObject goSelection = Instantiate<GameObject>(goSlelectionPre, lableContent.transform);
                TextHelper tmp = UnityUtil.GetTypeChildByName<TextHelper>(goSelection, "SelText");
                tmp.SetMessageKey(task.keyTitle);

                //设置对错图片
                bool opeBool = task.IsPass;
                Image img = UnityUtil.GetTypeChildByName<Image>(goSelection, "Answer");
                if (opeBool)
                {
                    img.sprite = rightSpt;
                    totalScore += task.score;
                }
                else
                {
                    img.sprite = errorSpt;
                    errorNum++;
                }
                
                if (TaskManager.Instance.taskMode == TaskMode.Examination)//考试模式
                {
                    //设置每一项对应的分数
                    Text score = UnityUtil.GetTypeChildByName<Text>(goSelection, "Score");
                    score.text = task.score + "分";
                }
            }
            
            if (TaskManager.Instance.taskMode == TaskMode.Examination)//考试模式
            {
                if (totalScore >= 70)
                {
                    keyTitle = "T1074";
                }
                else
                {
                    keyTitle = "T1075";
                }
                keyJilu = "T99";
                lableDefen.SetMessageKey(keyDefen);
                Text error = UnityUtil.GetTypeChildByName<Text>(gameObject, "ErrorNum");
                error.text = errorNum + "个";
                Text total = UnityUtil.GetTypeChildByName<Text>(gameObject, "TotalScore");
                total.text = totalScore + "分";
            }
            else//培训模式
            {
                keyTitle = "T1076";
                keyJilu = "T110";
                Text error = UnityUtil.GetTypeChildByName<Text>(gameObject, "ErrorNum");
                error.text = errorNum + "个";
            }
            lableTitle.SetMessageKey(keyTitle);
            lableJilu.SetMessageKey(keyJilu);
            lableBtnReturn.SetMessageKey(keyReturn);
            lableBtnReStart.SetMessageKey(keyReStart);
        }
        
        public bool IsExamPass()
        {
            bool pass = false;
            if (totalScore >= 70)
            {
                return true;
            }
            return pass;
        }

        private void InitComponent()
        {
            //获取物体的脚本
            btnReturn = UnityUtil.GetTypeChildByName<Button>(gameObject, "ReturnBtn");
            btnReStart = UnityUtil.GetTypeChildByName<Button>(gameObject, "RestartBtn");
            lableBtnReturn = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "returnText");
            lableBtnReStart = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "restartText");
            lableTitle = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "MsgTitle");
            lableJilu = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "MsgJilu");
            lableDefen = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "MsgDefen");
            lableContent = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Content");
        }

        private void AddListener()
        {
            btnReturn.onClick.AddListener(() =>
            {
                ReturnBegin();//返回首页
                HideDlg();
            });

            btnReStart.onClick.AddListener(() =>
            {
                ReturnStart();//重新开始
                HideDlg();
            });
        }

        private void RemoveListener()
        {
            btnReturn.onClick.RemoveAllListeners();
            btnReStart.onClick.RemoveAllListeners();
        }

        //返回首页，回到设置引导步骤
        private void ReturnBegin()
        {
            //调用TaskManager.Instance的函数
        }

        //重新开始，回到任务开始阶段
        private void ReturnStart()
        {
            //调用TaskManager.Instance的函数
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListener();
        }
    }
}


