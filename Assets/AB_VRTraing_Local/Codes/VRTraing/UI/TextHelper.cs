using System;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Controller;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.VRTraing.Gamer
{
    public class TextHelper : MonoBehaviour
    {
        [HideInInspector]
        public Text uiText;
        [HideInInspector]
        public TextMesh worldText;
        [HideInInspector]
        public string messageKey;
        public string realMessage;
        private int sort = -1;
        //private int maxLen = -1;
        public void Start()
        {
            uiText = GetComponent<Text>();
            worldText = GetComponent<TextMesh>();
            LanguageMgr.Instance.changeLanuage += OnChangeLaunge;
            OnChangeLaunge(null);
        }
        /// <summary>
        /// 当听到语言改变时事件
        /// </summary>
        /// <param name="obj"></param>
        public void OnChangeLaunge(LanguagePackConf obj)
        {
            LanguageConf conf = LanguageMgr.Instance.GetMessage(messageKey);
            if (conf != null)
            {
                //Debug.LogError(conf.Message);
                SetMessage(conf.Message);
            }
        }
        public void SetMessageKey(string key)
        {
            this.messageKey = key;
            OnChangeLaunge(null);
        }
        /// <summary>
        /// 直接的设置语言
        /// </summary>
        /// <param name="realMessage"></param>
        public void SetMessage(string realMessage)
        {
            this.realMessage = UnityUtil.SplitToLine(realMessage);
            //Debug.LogError(this.realMessage);

            if (!uiText)
            {
                uiText = GetComponent<Text>();
            }
            if (!worldText)
            {
                worldText = GetComponent<TextMesh>();
            }
            if (uiText)
            {
                uiText.text = this.realMessage;
            }
            if (worldText)
            {
                worldText.text = this.realMessage;
            }
            if (this.sort != -1)
            {
                SortNum(this.sort);
            }
            //if (maxLen > 0)
            //{
            //    ToMulLine(maxLen);
            //}
        }
        /// <summary>
        /// 一行转化为多行
        /// </summary>
        /// <param name="maxLen"></param>
        //private void ToMulLine(int maxLen)
        //{
        //    this.maxLen = maxLen;
        //    string target = null;
        //    if (uiText)
        //    {
        //        target = uiText.text;//= UnityUtil.SplitToLine(this.realMessage);
        //    }
        //    if (worldText)
        //    {
        //        target = worldText.text;//= UnityUtil.SplitToLine(this.realMessage);
        //    }
        //    if (!string.IsNullOrEmpty(target))
        //    {

        //    }
        //    if (uiText)
        //    {
        //        uiText.text = target;//= UnityUtil.SplitToLine(this.realMessage);
        //    }
        //    if (worldText)
        //    {
        //        uiText.text = target;//= UnityUtil.SplitToLine(this.realMessage);
        //    }
        //}

        //public void SimpleLine(int maxLen)
        //{
        //    this.maxLen = maxLen;
        //}

        /// <summary>
        /// 销毁时去掉监听事件
        /// </summary>
        private void OnDestroy()
        {
            if (LanguageMgr.Instance)
            {
                LanguageMgr.Instance.changeLanuage -= OnChangeLaunge;
            }
        }

        public void SortNum(int index)
        {
            this.sort = index;
            string target = null;
            if (uiText)
            {
                target = uiText.text;//= UnityUtil.SplitToLine(this.realMessage);
            }
            if (worldText)
            {
                target = worldText.text;//= UnityUtil.SplitToLine(this.realMessage);
            }
            if (!string.IsNullOrEmpty(target))
            {
                var tmp = target.Split('.');
                if (tmp.Length > 1)
                {
                    target = tmp[1];
                }
                target = index + "." + target;
            }
            if (uiText)
            {
                uiText.text = target;// UnityUtil.SplitToLine(target);
            }
            if (worldText)
            {
                uiText.text = target;// UnityUtil.SplitToLine(target);
            }
        }
    }
}