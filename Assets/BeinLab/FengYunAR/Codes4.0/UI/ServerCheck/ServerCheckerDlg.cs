using BeinLab.Util;
using Karler.WarFire.UI;
using UnityEngine.UI;

namespace BeinLab.RS5.UI
{
    /// <summary>
    /// 强制性网络检测窗口
    /// 当需要下载数据，而无网络连接，或者服务器地址不存在的情况下，调用此窗口
    /// </summary>
    public class ServerCheckerDlg : Singleton<ServerCheckerDlg>
    {
        private BaseDlg baseDlg;
        private bool isStartOffLine;

        public bool IsStartOffLine
        {
            get { return isStartOffLine; }
            set
            {
                isStartOffLine = value;
            }
        }

        private void Start()
        {
            baseDlg = GetComponent<BaseDlg>();
            IsStartOffLine = false;
            //ReConnect();
            baseDlg.GetChildComponent<Button>(baseDlg.UiRoot.gameObject, "ReButton").onClick.AddListener(ReConnect);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 强制性调起网络连接
        /// </summary>
        public void ShowConnect()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 尝试重连
        /// </summary>
        public void ReConnect()
        {
            gameObject.SetActive(false);
            IsStartOffLine = true;
            //GameServerChecker.Instance.ReConnect();
        }
    }
}