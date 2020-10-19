using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.Util
{
    /// <summary>
    /// 打印日志的窗口
    /// </summary>
    public class DebugDlg : Singleton<DebugDlg>
    {
        public Text label;
        public Button button;
        private void Start()
        {
            //if (GameSceneManager.Instance.buildConf.buildType != BulidType.LocalPhone)
            //{
            //    gameObject.SetActive(false);
            //}
            //else
            //{
            //    if (button)
            //    {
            //        button.onClick.AddListener(Clear);
            //    }
            //    Clear();
            //    //Application.logMessageReceived += OnLogMessageReceived;
            //    Application.logMessageReceivedThreaded += OnLogMessageReceived;
            //}
        }

        /// <summary>
        /// 接收日志
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (label)
            {
                if (type == LogType.Error) {
                    label.color = Color.red;
                }
                else if (type == LogType.Warning)
                {
                    label.color = Color.yellow;
                }
                else if (type == LogType.Log)
                {
                    label.color = Color.white;
                }
                LogMsg(condition);
                LogMsg(stackTrace);
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                TimerMgr.Instance.ClearAll();
                print("ClearAll");
            }
        }
        /// <summary>
        /// 输出到屏幕上
        /// </summary>
        /// <param name="msg"></param>
        public void LogMsg(string msg)
        {
            if (label)
            {
                label.text += msg + "\n";
            }
            if (label.text.Length > 100) {
                label.text = "";
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            if (label)
                label.text = "";
        }
    }
}