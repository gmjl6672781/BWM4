using BeinLab.FengYun.UI;
using BeinLab.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BeinLab.FengYun.UI
{
    /// <summary>
    ///返回
    /// </summary>
    public class ReturnDlg : Singleton<ReturnDlg>
    {
        private Text msgLab;
        private Timer showTimer;
        private void ClearTimer()
        {
            if (TimerMgr.Instance)
            {
                showTimer = TimerMgr.Instance.DestroyTimer(showTimer);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearTimer();
        }
        private void Start()
        {
            msgLab = UnityUtil.GetTypeChildByName<Text>(gameObject, "Text");
            var scene = SceneManager.GetActiveScene();
            string sceneName = scene.name;
            GameCell cell = UnityUtil.GetTypeChildByName<GameCell>(gameObject, sceneName);
            cell.gameObject.SetActive(true);
        }
        public void ShowMsg(string msg, float showTime = 1, int fontSize = 30, int posX = 0, int posY = 0)
        {
            if (msgLab)
            {
                msgLab.fontSize = fontSize;
                msgLab.text = msg;
                msgLab.rectTransform.anchoredPosition3D = Vector2.right * posX + Vector2.up * posY;
                ClearTimer();
                msgLab.gameObject.SetActive(true);
                showTimer = TimerMgr.Instance.CreateTimer(delegate ()
                {
                    msgLab.gameObject.SetActive(false);
                }, showTime);
            }
        }
    }
}