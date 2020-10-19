using BeinLab.RS5.Gamer;
using BeinLab.RS5.UI;
using BeinLab.Util;
using Karler.WarFire.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 检测设备支持性
/// 不再提供离线模式，强制联网检测更新?强制联网检测更新
/// </summary>
namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 检测系统是否支持AR
    /// </summary>
    public class CheckSupportDlg : MonoBehaviour
    {
        private string unSupportMsg = "您的设备不支持AR\n程序将自动退出";
        private string onUpdateMsg = "当前APP有新版本\n请下载并安装新版";
        private BaseDlg baseDlg;
        private Text unSupportLabel;
        public float quitTime = 5;
        private string appURL;
        /// <summary>
        /// </summary>
        private void Start()
        {
            GameServerChecker.Instance.OnUpdateVersion += OnUpdateVersion;
            baseDlg = GetComponent<BaseDlg>();
            if (CheckXRGamer.Instance)
            {
                if (CheckXRGamer.IsHadCheck)
                {
                    OnCheckSupport(CheckXRGamer.supportAR);
                }
                else
                {
                    CheckXRGamer.Instance.OnCheckSupport += OnCheckSupport;
                }
            }
            unSupportLabel = baseDlg.GetChildComponent<Text>(baseDlg.UiRoot.gameObject, "UnSupportLabel");
            unSupportLabel.text = unSupportMsg;
            Button btn = baseDlg.GetChildComponent<Button>(baseDlg.UiRoot.gameObject, "QuitButton");
            btn.onClick.AddListener(QuitAR);
            baseDlg.UiRoot.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            if (GameServerChecker.Instance)
            {
                GameServerChecker.Instance.OnUpdateVersion -= OnUpdateVersion;
            }
        }


        private void OnUpdateVersion(string obj)
        {
            appURL = obj;
            baseDlg.UiRoot.gameObject.SetActive(false);
            unSupportLabel.text = onUpdateMsg;
            
            if (ServerCheckerDlg.Instance)
            {
                ServerCheckerDlg.Instance.ReConnect();
            }
            StartCoroutine(ShowUnSupportDlg(onUpdateMsg));
        }

        /// <summary>
        /// 退出AR
        /// </summary>
        private void QuitAR()
        {
            if (!string.IsNullOrEmpty(appURL))
            {
                Application.OpenURL(appURL);
            }
            StopAllCoroutines();
            GameSceneManager.Instance.QuitUnity();
        }

        /// <summary>
        /// 检测是否支持
        /// </summary>
        /// <param name="isSupport"></param>
        private void OnCheckSupport(bool isSupport)
        {
            ///不支持，直接退出
            if (!isSupport)
            {
                StartCoroutine(ShowUnSupportDlg(unSupportMsg));
            }
        }


        /// <summary>
        /// 显示不支持的信息提示
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowUnSupportDlg(string msg = "")
        {
            baseDlg.UiRoot.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            float time = Time.time;
            float watieTime = quitTime;
            while ((watieTime = (quitTime - (Time.time - time))) > 0)
            {
                unSupportLabel.text = msg + "  " + (int)watieTime + " S";
                yield return new WaitForFixedUpdate();
            }

            QuitAR();
        }
    }
}