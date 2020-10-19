using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using UnityEngine.UI;
using System;
using Karler.WarFire.UI;

/// <summary>
/// 做任务选择题对话框
/// </summary>
namespace BeinLab.VRTraing.UI
{
    [RequireComponent(typeof(BaseDlg))]
    public class JudgementQuestionDlg : Singleton<JudgementQuestionDlg>
    {
        private BaseDlg baseDlg;

        public event Action<int> SendAnswer;
        private TextHelper lableMessage;
        private TextHelper lableNo;
        private TextHelper lableYes;
        private Button btnYes;
        private Button btnNo;
        

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

        private void InitComponent()
        {
            baseDlg = GetComponent<BaseDlg>();

            btnYes = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnYes");
            btnNo = UnityUtil.GetTypeChildByName<Button>(gameObject, "BtnNo");

            lableYes = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Yes");
            lableNo = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "No");
            lableMessage = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Message");

        }

        public void HideDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(false);
        }


        public void ShowDlg(JudgementQuestionConf judgementQuestionConf)
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(true);

            lableYes.SetMessageKey(judgementQuestionConf.keyYes);
            lableNo.SetMessageKey(judgementQuestionConf.KeyNo);
            lableMessage.SetMessageKey(judgementQuestionConf.keyMessage);
        }

        private void AddListener()
        {
            btnYes.onClick.AddListener(() => {
                if (SendAnswer != null)
                {
                    SendAnswer(1);
                }
                HideDlg();
            });

            btnNo.onClick.AddListener(() => {
                if (SendAnswer != null)
                {
                    SendAnswer(0);
                }
                HideDlg();
            });
        }

        private void RemoveListener()
        {
            btnYes.onClick.RemoveAllListeners();
            btnNo.onClick.RemoveAllListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListener();
        }
    }
}


