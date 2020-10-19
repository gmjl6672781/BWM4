using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeinLab.RS5.Gamer
{
    /// <summary>
    /// 游戏服务器检测
    /// 全局监听网络状态，登入服务器
    /// 当
    /// </summary>
    public class GameServerChecker : Singleton<GameServerChecker>
    {
        public float watieTime = 1f;
        /// <summary>
        /// 服务器地址，请确保可用，只有当重新发版时才修改此链接
        /// </summary>
        public string serverXMLURL = "http://fengyun-ar-4.0.obs.cn-south-1.myhuaweicloud.com/FengYunAR4.0/BeinLab_FengYunAR/ServerVisionConf.xml";
        /// <summary>
        /// 服务器列表
        /// </summary>
        private List<ServerVisionConf> serverList;
        private string gameServer;
        /// <summary>
        /// 连接到服务器URL
        /// </summary>
        public event Action<string> OnConnectServer;
        public event Action<string> OnUpdateVersion;
        public event Action OnNotConnectServer;
        /// <summary>
        /// 重置服务器URL
        /// </summary>
        public event Action OnReConnectServer;
        /// <summary>
        /// 服务器下标
        /// </summary>
        public int ServerIndex { get; private set; }
        private bool isConnectServer;
        public static string GMSURL = "GMS://";
        public bool isUseEditTest = false;
        private string editURL;
        /// <summary>
        /// 服务器列表
        /// </summary>
        public List<ServerVisionConf> ServerList
        {
            get
            {
                return serverList;
            }

            set
            {
                serverList = value;
            }
        }

        public string GameServer
        {
            get
            {
                return gameServer;
            }

            set
            {
                gameServer = value;
            }
        }

        public bool IsConnectServer
        {
            get
            {
                return isConnectServer;
            }

            set
            {
                isConnectServer = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            ///当网络初始化完成时，开始读取服务器列表
            if (GameNetChecker.Instance)
            {
                GameNetChecker.Instance.OnChangeNetWork += ReConnectServer;
            }
            else
            {
                GameNetChecker.InitComplte += delegate ()
                {
                    GameNetChecker.Instance.OnChangeNetWork += ReConnectServer;
                };
            }
        }

        private void Start()
        {
            OnReConnectServer += OnReConnectServerEvent;
            OnUpdateVersion += UpdateVersion;
        }

        private void UpdateVersion(string appURL)
        {
            //Application.OpenURL(appURL);
        }

        private void OnReConnectServerEvent()
        {
            GameServer = null;
        }

        private void ReConnectServer(NetworkReachability obj)
        {
            ReConnect();
        }
        /// <summary>
        /// 重新连接
        /// </summary>
        public void ReConnect()
        {
            StartCoroutine(ReadServerList());
        }

        /// <summary>
        /// 读取服务器列表
        /// 如果有本地数据，代表可以进入离线模式
        /// 如果没有本地数据，等待联网
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReadServerList()
        {
            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
            {
                IsConnectServer = true;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            IsConnectServer = false;
            float lastTime = Time.time;
            while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable && Time.time - lastTime < watieTime)
            {
                yield return new WaitForFixedUpdate();
            }
            if (GameNetChecker.Instance.NetState != NetworkReachability.NotReachable)
            {
                string url = serverXMLURL;
#if UNITY_EDITOR
                if (isUseEditTest)
                {
                    editURL = GameDataMgr.Instance.buildConf.editorURL;
                    url = editURL;
                }
#endif
                ///连接服务器列表xml
                StartCoroutine(UnityUtil.ReqDataByWWW(url, OnReadData));
            }
            else
            {
                if (OnNotConnectServer != null)
                {
                    OnNotConnectServer();
                }
            }
        }

        /// <summary>
        /// 读取到服务器数据xml
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="msg"></param>
        private void OnReadData(string xmlNode, WWW www)
        {
            bool isConnect = false;
            if (www != null && string.IsNullOrEmpty(www.error))
            {
                string msg = www.text;
                if (!string.IsNullOrEmpty(msg))
                {
                    ServerList = UnityUtil.ReadWebXMLData<ServerVisionConf>(xmlNode, msg);
                    if (ServerList != null)
                    {
                        if (OnReConnectServer != null)
                        {
                            OnReConnectServer();
                        }
                        for (int i = ServerList.Count - 1; i >= 0; i--)
                        {
                            ServerVisionConf server = ServerList[i];
                            GameDataMgr.Instance.ReqWebXMLData<AssetsVersionConf>(server.Server, "", (string xmlNode1, WWW www1) => { OnReadXMLData(xmlNode1, www1, server); });
                        }
                        isConnect = true;
                    }
                }
            }
            if (!isConnect)
            {
                if (OnNotConnectServer != null)
                {
                    OnNotConnectServer();
                }
            }
        }


        /// <summary>
        /// 测试读取服务器的信息，判断服务器是否可用
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="www"></param>
        /// <param name="server"></param>
        private void OnReadXMLData(string xmlNode, WWW www, ServerVisionConf server)
        {
            if (www != null && string.IsNullOrEmpty(www.error))
            {
                if (!string.IsNullOrEmpty(www.text))
                {
                    ///测试读取服务器数据
                    var xmlData = UnityUtil.ReadWebXMLData<AssetsVersionConf>(xmlNode, www.text);
                    ///能读到数据代表服务器可用
                    if (xmlData != null && xmlData.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(server.Server))
                        {
                            if (string.IsNullOrEmpty(GameServer))
                            {
                                string appVersion = server.IOSVersion;
                                if (GameDataMgr.Instance.buildConf.buildTarget == BuildPlatform.Android)
                                {
                                    appVersion = server.Androidversion;
                                }
                                ///基础包版本不一致
                                if (appVersion != GameDataMgr.Instance.buildConf.version)
                                {
                                    if (OnUpdateVersion != null)
                                    {
                                        if (GameDataMgr.Instance.buildConf.buildTarget == BuildPlatform.iOS)
                                        {
                                            OnUpdateVersion(server.IOSAppURL);
                                        }
                                        else if (GameDataMgr.Instance.buildConf.buildTarget == BuildPlatform.Android)
                                        {
                                            OnUpdateVersion(server.AndroidAppURL);
                                        }
                                    }
                                }
                                else
                                {
                                    IsConnectServer = true;
                                    GameServer = server.Server;
                                    GMSURL = server.GmsURL;
                                    print("Server:" + GameServer);
                                    if (OnConnectServer != null)
                                    {
                                        OnConnectServer(GameServer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ServerList.Remove(server);
                        }
                    }
                    else
                    {
                        ServerList.Remove(server);
                    }
                }
                else
                {
                    ServerList.Remove(server);
                }
            }
            else
            {
                ServerList.Remove(server);
            }

            if (ServerList.Count <= 0)
            {
                if (OnNotConnectServer != null)
                {
                    OnNotConnectServer();
                }
            }
        }
    }
}