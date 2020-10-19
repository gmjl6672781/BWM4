using BeinLab.Util;
using BeinLab.VRTraing.Gamer;
using UnityEngine;
namespace BeinLab.VRTraing.UI
{
    /// <summary>
    /// 简易的文本提示
    /// </summary>
    [RequireComponent(typeof(TextHelper))]
    public class SimpleTextTips : MonoBehaviour
    {
        private string messageKey;
        public bool isHideOnStart;
        private Timer showTimer;
        private TextHelper textHelper;
        private void Start()
        {
            textHelper = GetComponent<TextHelper>();
            gameObject.SetActive(!isHideOnStart);
        }

        public void ClearTimer()
        {
            if (TimerMgr.Instance != null)
            {
                showTimer = TimerMgr.Instance.DestroyTimer(showTimer);
            }
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public void ShowMessage(string messageKey, float showTime = 1)
        {
            textHelper.SetMessageKey(messageKey);
            ShowMessage(showTime);
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public void ShowMessage(float showTime = 1)
        {
            ClearTimer();
            gameObject.SetActive(true);
            if (showTime > 0)
            {
                showTimer = TimerMgr.Instance.CreateTimer(() => { gameObject.SetActive(false); }, showTime);
            }
        }

        private void OnDestroy()
        {
            ClearTimer();
        }
    }
}