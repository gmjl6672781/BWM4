using BeinLab.Util;
using BeinLab.VRTraing.Mgr;
using Karler.WarFire.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 模式选择窗口，教学模式，考试模式
/// </summary>
namespace BeinLab.VRTraing.UI
{
    [RequireComponent(typeof(BaseDlg))]
    public class SelectModeDlg : Singleton<SelectModeDlg>
    {
        private BaseDlg baseDlg;

        //private TextHelper lableTitle;
        //private TextHelper lableMsgTraining;
        //private TextHelper lableMsgExamination;
        //private TextHelper lableBtnTraining;
        //private TextHelper lableBtnExamination;
        private Button btnTraining;
        private Button btnExamination;

        //public string keyTitle = "T9";
        //public string keyTraining = "T10";
        //public string keyExamination = "T10";
        //public string keyMsgTraining = "T10";
        //public string keyMsgExamination = "T11";
        public event Action OnSelectModel;
        protected override void Awake()
        {
            base.Awake();
            InitComponent();
            AddListener();
        }

        private void Start()
        {
            HideDlg();
        }

        public void ShowDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(true);

            //lableTitle.SetMessageKey(keyTitle);
            //lableMsgTraining.SetMessageKey(keyMsgTraining);
            //lableMsgExamination.SetMessageKey(keyMsgExamination);
            //lableBtnTraining.SetMessageKey(keyTraining);
            //lableBtnExamination.SetMessageKey(keyExamination);

        }

        public void HideDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(false);
        }

        private void InitComponent()
        {
            baseDlg = GetComponent<BaseDlg>();

            btnTraining = UnityUtil.GetTypeChildByName<Button>(gameObject, "btnTraining");
            btnExamination = UnityUtil.GetTypeChildByName<Button>(gameObject, "btnExamination");

            //lableBtnTraining = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Training");
            //lableBtnExamination = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Examination");
            //lableMsgTraining = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "MsgTraining");
            //lableMsgExamination = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "MsgExamination");
            //lableTitle = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Title");
        }

        private void AddListener()
        {
            btnTraining.onClick.AddListener(() =>
            {
                SetTaskMode(TaskMode.Training);
                HideDlg();
                OnSelectModel?.Invoke();
                //与Hub交换数据
                //LoadingHubMgr.mode = "training";
                //LoadingHubMgr.inst.SelectMode();
            });

            btnExamination.onClick.AddListener(() =>
            {
                SetTaskMode(TaskMode.Examination);
                HideDlg();
                OnSelectModel?.Invoke();
                //与Hub交换数据
                //LoadingHubMgr.mode = "examing";
                //LoadingHubMgr.inst.SelectMode();
            });
        }

        private void RemoveListener()
        {
            btnTraining.onClick.RemoveAllListeners();
            btnExamination.onClick.RemoveAllListeners();
        }

        private void SetTaskMode(TaskMode taskMode)
        {
            TaskManager.Instance.taskMode = taskMode;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListener();
        }
    }
}


