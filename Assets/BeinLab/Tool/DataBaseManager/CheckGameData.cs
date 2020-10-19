using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modu;
using BeinLab.FengYun.UI;
using BeinLab.RS5.Gamer;
using BeinLab.RS5.UI;
using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.FengYun.Gamer
{
    /// <summary>
    /// 监听网络状态，并检测对应的文件资源更新状况
    /// 下载指定的文件夹资源
    /// 
    /// 正常情况的执行步骤：
    /// 检测文件夹是否完整，如果不完整，强制联网
    /// 如果完整，不强制联网，如果联网检测可以更新的内容强制启用更新。
    /// 如何判断已完成更新内容：本地完整，且无更新内容
    /// 下载文件之后需要二次校验？
    /// 建议使用do while进行校验
    /// </summary>
    public class CheckGameData : MonoBehaviour
    {
        /// <summary>
        /// 默认要下载的资源包
        /// </summary>
        public List<GameDataConf> gameDataList;
        public ActionConf completeAction;
        private void Start()
        {
            GameServerChecker.Instance.OnUpdateVersion += OnUpdateVersion;

            CheckXRGamer.Instance.OnCheckSupport += OnCheckSupport;
        }

        private void OnUpdateVersion(string obj)
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// 当检测到支持时
        /// </summary>
        /// <param name="isSupport"></param>
        private void OnCheckSupport(bool isSupport)
        {
            if (isSupport)
            {
                StartCoroutine(CheckDataAndDown(gameDataList));
            }
        }

        /// <summary>
        /// 检测基础数据
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckDataAndDown(List<GameDataConf> gameDataList)
        {
            //yield return new WaitForSeconds(1);
#if !UNITY_EDITOR
            if (GameDataMgr.Instance. buildConf.buildType == BulidType.Stream)
            {
                OnUpdateComplete();
                yield break;
            }
//#else
//            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
//            {
//                OnUpdateComplete();
//                yield break;
//            }

#endif

            if (gameDataList != null)
            {
                ProcessDlg.Instance.gameObject.SetActive(true);
                ProcessDlg.Instance.UpdateProcess("首次启动需要下载美术包", 0);
                if (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
                {
                    ServerCheckerDlg.Instance.ShowConnect();
                }
                while (ServerCheckerDlg.Instance.isActiveAndEnabled)
                {
                    if (GameNetChecker.Instance.NetState != NetworkReachability.NotReachable)
                    {
                        ServerCheckerDlg.Instance.ReConnect();
                    }
                    yield return new WaitForFixedUpdate();
                }

                float curProcess = 0;
                for (int i = 0; i < gameDataList.Count; i++)
                {
                    GameDataConf conf = gameDataList[i];
                    bool isDown = false;
                    StartCoroutine(CheckDataUpdate(conf, (float process) =>
                    {
                        float p = (i * 1.0f / gameDataList.Count + process * 1.0f / gameDataList.Count);
                        if (p > curProcess)
                        {
                            curProcess = p;
                            if ((curProcess).ToString("p0") != "100%")
                            {
                                ProcessDlg.Instance.UpdateProcess("正在下载资源" + (curProcess).ToString("p0"), curProcess);
                            }
                            else
                            {
                                ProcessDlg.Instance.UpdateProcess("解压中请稍后" + (process).ToString("p0"), process);
                            }
                        }
                    }, () => { isDown = true; }));
                    while (!isDown)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
                ProcessDlg.Instance.DoSliderEffect("解压中请稍后", 0, 1, 1);
            }
            OnUpdateComplete();
        }

        private void OnUpdateComplete()
        {
            //StartCoroutine(GameDataMgr.Instance.LoadGameScene(1));
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.DoAction(completeAction);
            }
        }

        /// <summary>
        /// 检测基础数据
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckBaseData(List<GameDataConf> gameDataList)
        {
            if (gameDataList != null)
            {
                for (int i = 0; i < gameDataList.Count; i++)
                {
                    GameDataConf conf = gameDataList[i];
                    if (conf)
                    {
                        bool isComplete = false;// conf.isLocalComplete();
                        if (!isComplete)
                        {
                            if (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
                            {
                                ServerCheckerDlg.Instance.ShowConnect();
                            }
                            while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable
                                || !GameServerChecker.Instance.IsConnectServer)
                            {
                                yield return new WaitForFixedUpdate();
                            }
                            ServerCheckerDlg.Instance.ReConnect();
                        }
                        bool isReq = false;
                        List<AssetsVersionConf> versionList = null;
                        conf.ReqServerVersion((List<AssetsVersionConf> list) =>
                        {
                            isReq = true;
                            versionList = list;
                        });
                        while (!isReq)
                        {
                            yield return new WaitForFixedUpdate();
                        }
                        List<AssetsVersionConf> updateList = null;// conf.ComparisonVersion(versionList);
                        bool isCompleteDown = false;
                        if (updateList == null || updateList.Count < 1)
                        {
                            ///无需更新数据
                            isCompleteDown = true;
                        }
                        else
                        {
                            ProcessDlg.Instance.UpdateProcess("正在准备数据,需要下载" + conf.GetSize(conf.GetSize(updateList)) + "M", 0);
                            ///存在更新数据
                            conf.DownData(GameServerChecker.Instance.GameServer, conf.dataPath, updateList, ProcessDlg.Instance.UpdateProcess, () =>
                            {
                                isCompleteDown = true;
                            });
                        }
                        while (!isCompleteDown)
                        {
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 检测某个文件夹数据是否完整，如果不完整就下载
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="updateProcess"></param>
        /// <param name="CompleteUpdate"></param>
        /// <returns></returns>
        public IEnumerator CheckDataAndDown(GameDataConf conf, Action<float> updateProcess, Action CompleteUpdate)
        {
            if (conf)
            {
                bool isComplete = false;
                bool isReq = false;
                List<AssetsVersionConf> versionList = null;
                do
                {
                    ///本地是否完整
                    isComplete = false;// conf.isLocalComplete();
                    yield return isComplete;
                    float watieTime = 0;
                    if (GameNetChecker.Instance.NetState != NetworkReachability.NotReachable)
                    {
                        ProcessDlg.Instance.gameObject.SetActive(true);
                        while (!GameServerChecker.Instance.IsConnectServer)
                        {
                            watieTime += Time.deltaTime;
                            watieTime = watieTime % 1;
                            ProcessDlg.Instance.UpdateProcess("正在连接服务器" + GameServerChecker.Instance.serverXMLURL, watieTime);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    if (!isComplete)
                    {
                        while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable
                        || !GameServerChecker.Instance.IsConnectServer)
                        {
                            ServerCheckerDlg.Instance.ShowConnect();
                            yield return new WaitForFixedUpdate();
                        }
                        ServerCheckerDlg.Instance.ReConnect();
                    }
                    else
                    {
                        ///如果没有联网，弹窗一次
                        if (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
                        {
                            if (!ServerCheckerDlg.Instance.IsStartOffLine)
                            {
                                ServerCheckerDlg.Instance.ShowConnect();
                            }
                        }
                        else
                        {
                            ///如果已经联网，则等待服务器连接
                            while (!GameServerChecker.Instance.IsConnectServer)
                            {
                                yield return new WaitForFixedUpdate();
                            }
                        }
                    }
                    while (ServerCheckerDlg.Instance.gameObject.activeSelf)
                    {
                        yield return new WaitForFixedUpdate();
                        if (GameNetChecker.Instance.NetState != NetworkReachability.NotReachable
                            && GameServerChecker.Instance.IsConnectServer)
                        {
                            ServerCheckerDlg.Instance.ReConnect();
                        }
                    }
                    if (!isReq)
                    {
                        conf.ReqServerVersion((List<AssetsVersionConf> list) =>
                        {
                            isReq = true;
                            versionList = list;
                        });
                    }
                    while (!isReq)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    List<AssetsVersionConf> updateList = null;// conf.ComparisonVersion(versionList);
                    bool isCompleteDown = false;
                    if (updateList == null || updateList.Count < 1)
                    {
                        ///无需更新数据
                        isCompleteDown = true;
                    }
                    else
                    {
                        //print("本次更新" + updateList.Count);
                        //ProcessDlg.Instance.UpdateProcess("本次更新" + updateList.Count + "个文件", 0);
                        //ProcessDlg.Instance.gameObject.SetActive(true);
                        isCompleteDown = false;
                        ///存在更新数据
                        conf.DownData(GameServerChecker.Instance.GameServer, conf.dataPath, updateList, updateProcess, () =>
                        {
                            isCompleteDown = true;
                        });
                    }
                    while (!isCompleteDown)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    yield return new WaitForFixedUpdate();
                } while (!isComplete);
            }
            updateProcess(1);
            if (CompleteUpdate != null)
            {
                CompleteUpdate();
            }
        }

        /// <summary>
        /// 检测某个文件夹下的资源是否完整，如果不完整，就需要强制联网下载资源
        /// 1，联网状态：读取服务器，如果获取到服务器信息（3秒），代表可连接，否则为断网状态
        /// 2，断网状态：只需要校验本地文件即可。
        /// 
        /// </summary>
        /// <param name="conf"></param>
        /// <param name="updateProcess"></param>
        /// <param name="CompleteUpdate"></param>
        /// <returns></returns>
        public IEnumerator CheckDataUpdate(GameDataConf conf, Action<float> updateProcess, Action CompleteUpdate)
        {
            yield return conf;
            if (conf)
            {
                float watieTime = 0;
                if (!GameServerChecker.Instance.IsConnectServer && GameNetChecker.Instance.NetState != NetworkReachability.NotReachable)
                {
                    watieTime = 0;
                    while (!GameServerChecker.Instance.IsConnectServer && watieTime < 3)
                    {
                        watieTime += Time.deltaTime;
                        float wp = watieTime % 1;
                        ProcessDlg.Instance.UpdateProcess("正在连接服务器" + GameServerChecker.Instance.serverXMLURL, wp);
                        if (!ProcessDlg.Instance.isActiveAndEnabled)
                        {
                            ProcessDlg.Instance.gameObject.SetActive(true);
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }

                ///联网状态，连接到服务器
                ///更新本地服务器版本缓存文件
                List<AssetsVersionConf> serverList = null;
                if (GameDataMgr.Instance.buildConf.buildType != BulidType.Editor)
                {
                    do
                    {
                        if (GameServerChecker.Instance.IsConnectServer)
                        {
                            bool isUpdate = false;
                            conf.ReqServerVersion((List<AssetsVersionConf> list) =>
                            {
                                conf.UpdateServerVersion(list);
                                serverList = list;
                                isUpdate = true;
                            });
                            while (!isUpdate)
                            {
                                yield return new WaitForFixedUpdate();
                            }
                        }
                        ///没有联网，读取本地的服务器缓存文件
                        if (serverList == null)
                        {
                            serverList = conf.ReadLocalServerVersion();
                        }
                        if (serverList == null || serverList.Count < 1)
                        {
                            while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable
                                || !GameServerChecker.Instance.IsConnectServer)
                            {
                                ServerCheckerDlg.Instance.ShowConnect();
                                yield return new WaitForFixedUpdate();
                            }
                            ServerCheckerDlg.Instance.ReConnect();
                        }
                        //print(serverList);
                    } while (serverList == null || serverList.Count < 1);
                }
                ///检测服务器缓存文件与本地文件进行比对，检测更新部分
                var updateList = conf.CheckUpdate(serverList);
                bool isCompleteDown = false;
                if (updateList == null || updateList.Count < 1)
                {
                    ///无需更新数据
                    isCompleteDown = true;
                }
                else
                {
                    while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable
                            || !GameServerChecker.Instance.IsConnectServer)
                    {
                        ServerCheckerDlg.Instance.ShowConnect();
                        yield return new WaitForFixedUpdate();
                    }
                    ServerCheckerDlg.Instance.ReConnect();

                    isCompleteDown = false;
                    ///存在更新数据
                    conf.DownData(GameServerChecker.Instance.GameServer, conf.dataPath, updateList, updateProcess, () =>
                    {
                        isCompleteDown = true;
                    });
                }
                while (!isCompleteDown)
                {
                    yield return new WaitForFixedUpdate();
                }

                ///
            }
            updateProcess(1);
            if (CompleteUpdate != null)
            {
                CompleteUpdate();
            }
        }
    }
}