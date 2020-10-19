using BeinLab.FengYun.UI;
using BeinLab.Util;
using Karler.WarFire.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.RS5.Gamer
{
    /// <summary>
    /// 服务器检测,服务器选取
    /// 请保证服务器中配置的链接是可用的
    /// 如果没有本地数据，必须要联网才能进入
    /// </summary>
    public class ServerChecker : Singleton<ServerChecker>
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
        private BaseDlg baseDlg;
        public string basePath = "";
        public string carListPath;
        private bool isBaseComplete;
        private bool isCarListComplete;
        private Dictionary<string, string> baseMD5Map;
        private Dictionary<string, string> carListMD5Map;

        /// <summary>
        /// 连接到服务器URL
        /// </summary>
        public event Action<string> OnConnectServer;
        /// <summary>
        /// 重置服务器URL
        /// </summary>
        public event Action OnReConnectServer;
        /// <summary>
        /// 服务器下标
        /// </summary>
        public int ServerIndex { get; private set; }
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

        /// <summary>
        /// 启动时
        /// </summary>
        private void Start()
        {
            baseDlg = GetComponent<BaseDlg>();
            OnConnectServer += OnConnectServerEvent;
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

            baseDlg.UiRoot.gameObject.SetActive(false);
            baseDlg.GetChildComponent<Button>(baseDlg.UiRoot.gameObject, "ReButton").onClick.AddListener(ReConnect);
            //StartCoroutine(ReadServerList());
        }
        /// <summary>
        /// 连接到服务器时
        /// 检测更新
        /// </summary>
        /// <param name="server"></param>
        private void OnConnectServerEvent(string server)
        {
            if (string.IsNullOrEmpty(server) && isBaseComplete && isCarListComplete)
            {
                OnUpdateComplete();
                return;
            }
            ///先下载必备组件/资源，然后下载车型列表
            StartCoroutine(CheckDataUpdate(server, basePath, baseMD5Map, isBaseComplete, () =>
            {
                StartCoroutine(CheckDataUpdate(server, carListPath, carListMD5Map, isCarListComplete, OnUpdateComplete));
            }));
        }
        /// <summary>
        /// 当完成组件的下载安装时
        /// </summary>
        private void OnUpdateComplete()
        {
            StartCoroutine(GameDataMgr.Instance.LoadGameScene(1));
        }

        private IEnumerator CheckDataUpdate(string server, string path, Dictionary<string, string> md5Map, bool isComplete, Action UpdateComplete)
        {
            ///服务器版本信息
            List<AssetsVersionConf> versionList = null;
            do
            {
                ///本地数据不完整，且网络无连接
                /// 等待网络连接
                while (!isComplete && GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
                {
                    yield return new WaitForFixedUpdate();
                }
                bool isRead = false;
                GameDataMgr.Instance.ReqWebXMLData<AssetsVersionConf>(server, path, (string xmlNode, WWW www) =>
                {
                    string msg = "";
                    if (www != null && string.IsNullOrEmpty(www.error))
                    {
                        msg = www.text;
                        if (!string.IsNullOrEmpty(msg))
                        {
                            versionList = UnityUtil.ReadWebXMLData<AssetsVersionConf>(xmlNode, msg);
                        }
                    }
                    else
                    {
                        print("服务器错误，网络或者服务器文件问题");
                    }
                    isRead = true;
                });
                while (!isRead)
                {
                    yield return new WaitForFixedUpdate();
                }
            } while (!isComplete && versionList == null);  ///条件为true时将继续执行
            List<AssetsVersionConf> updateList = null;
            ///获取到服务器数据
            if (versionList != null)
            {
                updateList = GameDataMgr.Instance.ComparisonVersion(versionList, md5Map);
            }
            ///本地数据完整，同时服务器无数据更新时，完成检测
            if (isComplete && (updateList == null || updateList.Count < 1))
            {
                if (UpdateComplete != null)
                {
                    UpdateComplete();
                }
            }
            ///本地数据不完整
            else
            {
                if (updateList != null && updateList.Count > 0)
                {
                    ProcessDlg.Instance.gameObject.SetActive(true);
                    ProcessDlg.Instance.UpdateProcess("正在加载，请稍后", 0);
                    StartCoroutine(GameDataMgr.Instance.DownGameData(server, path, updateList, (float process) =>
                    {
                        ProcessDlg.Instance.UpdateProcess(process);
                    }, () =>
                    {
                        if (UpdateComplete != null)
                        {
                            UpdateComplete();
                        }
                    }));
                }
            }
        }



        /// <summary>
        /// 重连接
        /// </summary>
        /// <param name="netState"></param>
        private void ReConnectServer(NetworkReachability netState)
        {
            ReConnect();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public void ReConnect()
        {
            baseDlg.UiRoot.gameObject.SetActive(false);
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
            yield return new WaitForEndOfFrame();
            float lastTime = Time.time;
            while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable && Time.time - lastTime < watieTime)
            {
                yield return new WaitForFixedUpdate();
                if (!ProcessDlg.Instance.gameObject.activeSelf)
                {
                    ProcessDlg.Instance.gameObject.SetActive(true);
                }
                ProcessDlg.Instance.UpdateProcess("网络连接中", Time.time - lastTime);
            }
            ProcessDlg.Instance.gameObject.SetActive(false);

            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
            {
                isBaseComplete = true;
                isCarListComplete = true;
                OnReadData(null, null);
                yield break;
            }
            else
            {
                ///检测基础必备组件是否完备
                baseMD5Map = GameDataMgr.Instance.GetMD5FormDirectory(basePath, false);
                ///检测本地文件是否完整
                isBaseComplete = GameDataMgr.Instance.CheckDataComplete(basePath, baseMD5Map);

                ///检测车型列表文件是否完备
                carListMD5Map = GameDataMgr.Instance.GetMD5FormDirectory(carListPath, true);
                ///检测本地文件是否完整
                isCarListComplete = GameDataMgr.Instance.CheckDataComplete(carListPath, carListMD5Map);
            }
            ///网络无连接
            if ((!isBaseComplete || !isCarListComplete) && GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
            {
                baseDlg.UiRoot.gameObject.SetActive(true);
                yield break;
            }
            ///已连接网络，开始连接服务器
            else
            {
                StartCoroutine(UnityUtil.ReqDataByWWW(serverXMLURL, OnReadData, (float process) =>
                {
                    ProcessDlg.Instance.UpdateProcess("连接服务器", process);
                }));
            }
        }

        /// <summary>
        /// 读取到服务器数据
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="msg"></param>
        private void OnReadData(string xmlNode, WWW www)
        {
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
                    }
                }
            }
            else
            {
                ///无法连接服务器
                OnConnectServerEvent(null);
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
                            if (OnConnectServer != null)
                            {
                                OnConnectServer(server.Server);
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
                OnConnectServerEvent(null);
            }
        }


    }
}