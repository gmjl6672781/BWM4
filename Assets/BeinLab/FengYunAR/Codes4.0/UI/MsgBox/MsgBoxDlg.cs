using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 消息弹出框
    /// </summary>
    public class MsgBoxDlg : Singleton<MsgBoxDlg>
    {
        private Timer showTimer;
        private Text msgLabel;

        protected override void Awake()
        {
            base.Awake();
            msgLabel = UnityUtil.GetTypeChildByName<Text>(gameObject, "MsgLabel");
            CloseBox();
        }

        public void CloseBox()
        {
            ClearTimer();
            msgLabel.text = "";
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 默认隐藏
        /// </summary>
        private void Start()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        ///销毁
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearTimer();
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="showTime"></param>
        public void ShowMsg(string msg, float showTime)
        {
            ClearTimer();
            msgLabel.text = msg;
            gameObject.SetActive(true);
            showTimer = TimerMgr.Instance.CreateTimer(delegate ()
            {
                gameObject.SetActive(false);
            }, showTime);
        }
        /// <summary>
        /// 设置文本显示的坐标
        /// </summary>
        /// <param name="pos"></param>
        public void SetShowPos(Vector2 pos)
        {
            msgLabel.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        /// <summary>
        /// 清除计时器
        /// </summary>
        private void ClearTimer()
        {
            if (showTimer != null)
            {
                if (TimerMgr.Instance)
                {
                    TimerMgr.Instance.DestroyTimer(showTimer);
                    showTimer = null;
                }
            }
        }
    }
}
