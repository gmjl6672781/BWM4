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
/// 做确认对话框
/// </summary>
namespace BeinLab.VRTraing.UI
{
    [RequireComponent(typeof(BaseDlg))]
    public class ConfirmUIDlg : Singleton<ConfirmUIDlg>
    {
        private BaseDlg baseDlg;

        public event Action<int> SendAnswer;
        private TextHelper lableMessage;
        private TextHelper lableNo;
        private TextHelper lableYes;
        private Button btnYes;


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
           

            lableYes = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Yes");
            lableNo = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "No");
            lableMessage = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Message");

        }

        public void HideDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(false);
        }


        public void ShowDlg(ConfirmUIConf confirmUIConf)
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(true);

            lableYes.SetMessageKey(confirmUIConf.keyYes);
            lableMessage.SetMessageKey(confirmUIConf.keyMessage);
        }

        private void AddListener()
        {
            btnYes.onClick.AddListener(() =>
            {
                if (SendAnswer != null)
                {
                    SendAnswer(1);
                }
                HideDlg();
            });

        }

        private void RemoveListener()
        {
            btnYes.onClick.RemoveAllListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListener();
        }
    }
}
