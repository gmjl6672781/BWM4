using BeinLab.FengYun.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 网络状态
    /// 检测网络连接状况
    /// </summary>
    public class GameNetChecker : Singleton<GameNetChecker>
    {
        private NetworkReachability netState = NetworkReachability.NotReachable;
        public NetworkReachability defNetState = NetworkReachability.NotReachable;
        public NetworkReachability NetState
        {
            get
            {
                return netState;
            }
            set
            {
                if (netState != value)
                {
                    netState = value;
                    if (OnChangeNetWork != null)
                    {
                        OnChangeNetWork(netState);
                    }
                }
            }
        }

        /// <summary>
        /// 实时检测网络状态
        /// </summary>
        public event Action<NetworkReachability> OnChangeNetWork;
        private bool isInit = false;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            netState = defNetState;
#else
            netState = Application.internetReachability ;
#endif
            if (CheckXRGamer.Instance)
            {
                CheckXRGamer.Instance.OnCheckSupport += OnCheckSupport;
            }
            OnChangeNetWork += OnChangeNetWorkEvent;
        }
        /// <summary>
        /// 当检测支持时，再通知事件
        /// </summary>
        /// <param name="isSupport"></param>
        private void OnCheckSupport(bool isSupport)
        {
            if (isSupport)
            {
                if (OnChangeNetWork != null)
                {
                    OnChangeNetWork(netState);
                };
            }
            isInit = isSupport;
        }

        public float showTime = 2f;
        /// <summary>
        /// 设置网络状态的弹窗间隔
        /// </summary>
        private float lastShowTime;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="netState"></param>
        private void OnChangeNetWorkEvent(NetworkReachability netState)
        {
            if (Time.time - lastShowTime < showTime)
            {
                return;
            }
            lastShowTime = Time.time;
            if (MsgBoxDlg.Instance)
            {
                MsgBoxDlg.Instance.SetShowPos(Vector2.up * Screen.height / 4);
                if (netState == NetworkReachability.NotReachable)
                {
                    MsgBoxDlg.Instance.ShowMsg("网络无连接", 1);
                }
                else if (netState == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    MsgBoxDlg.Instance.ShowMsg("已连接移动流量", 1);
                }
                else if (netState == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    MsgBoxDlg.Instance.ShowMsg("网络已连接", 1);
                }
            }
        }

        /// <summary>
        /// 实时检测网络状态
        /// </summary>
        private void Update()
        {
            //if(Input.GetMouseButtonDown(0))
            //print(Input.mousePosition);

            if (!isInit) return;
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                NetState = NetworkReachability.NotReachable;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                NetState = NetworkReachability.ReachableViaLocalAreaNetwork;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                NetState = NetworkReachability.ReachableViaCarrierDataNetwork;
            }
#else
            if (NetState != Application.internetReachability)
            {
                NetState = Application.internetReachability;
            }
#endif
        }
    }
}