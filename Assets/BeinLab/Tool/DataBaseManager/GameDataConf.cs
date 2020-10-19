using BeinLab.Util;
using Karler.Lib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace BeinLab.FengYun.Modu
{
    /// <summary>
    /// 要优化资源校验的速度。
    /// 原理是，不再遍历真实资源，而是将已下载的资源的信息缓存到LocalXML。
    /// 通过对比服务器XML与LocalXML，判断资源的完整性，以及需要更新的内容，从而提升校验速度
    /// 本地校对：1，获取本地服务器缓存XML，获取LocalXML
    /// 2，比较两个XML，如果两个数据不一样（数量不同，MD5不同），存在更新内容，判断为不完整
    /// 如果完全一致，代表完整。
    /// 如果不完整，需要强行联网更新
    /// 在线校对：直接更新本地服务器缓存XML，然后执行步骤2。
    /// 
    /// 下载更新：每下载更新一个数据（下载完成），就将下载的数据信息（MD5）写入到XML中。
    /// 
    /// 如何避免已经存在的资源，不再更新？
    /// 本地缓存文件不再更新下载，而是自主创建。如果旧版本存在，则以旧版本为本地缓存
    /// 服务器缓存版本另取名称和路径进行更新，放置路径Server
    /// 首次如何更新？首次必须联网检测
    /// </summary>
    public class GameDataConf : ScriptableObject
    {
        /// <summary>
        /// 数据包的路径，相对路径
        /// </summary>
        public string dataPath;
        /// <summary>
        /// 是否包含子文件夹及文件
        /// </summary>
        public bool isCloudChild;
        /// <summary>
        /// 是否是强制更新
        /// </summary>
        public bool isForceUpdate = true;
        /// <summary>
        /// 是否强制下载，必备组件
        /// </summary>
        public bool isForceDown = false;
        /// <summary>
        /// 展示的优先级
        /// </summary>
        public int sort = 1000;
        //private Dictionary<string, string> md5Map;
        // private bool isCheckMD5 = false;
        //public bool isEditorLoad = false;
        //public Dictionary<string, string> Md5Map
        //{
        //    get
        //    {
        //        return md5Map;
        //    }

        //    set
        //    {
        //        md5Map = value;
        //    }
        //}

        /// <summary>
        /// 本地文件是否完整
        /// </summary>
        /// <returns></returns>
        //public bool isLocalComplete()
        //{
        //    //Md5Map = GetLocalMD5();
        //    //return GameDataMgr.Instance.CheckDataComplete(dataPath, Md5Map);
        //    return false;
        //}
        /// <summary>
        /// 更新服务器版本缓存信息到本地,
        /// </summary>
        /// <returns></returns>
        public void UpdateServerVersion(List<AssetsVersionConf> list)
        {
            string sCachePath = Path.Combine(GameDataMgr.Instance.ServerCachePath, dataPath);
            if (!Directory.Exists(sCachePath))
            {
                Directory.CreateDirectory(sCachePath);
            }
            MySql.WorkPath = sCachePath;
            bool isOpen = MySql.Open<AssetsVersionConf>(true);
            if (isOpen)
            {
                MySql.DeleteAll<AssetsVersionConf>();
            }
            else
            {
                ///some thing error
            }
            for (int i = 0; i < list.Count; i++)
            {
                MySql.Insert(list[i], true);
            }
            MySql.Close();
        }
        /// <summary>
        /// 读取本地服务器版本缓存信息
        /// </summary>
        /// <returns></returns>
        public List<AssetsVersionConf> ReadLocalServerVersion()
        {
            string sCachePath = Path.Combine(GameDataMgr.Instance.ServerCachePath, dataPath);
            if (!Directory.Exists(sCachePath))
            {
                Directory.CreateDirectory(sCachePath);
            }
            MySql.WorkPath = sCachePath;
            bool isOpen = MySql.Open<AssetsVersionConf>(true);
            List<AssetsVersionConf> list = null;
            if (isOpen)
            {
                list = MySql.SelectAll<AssetsVersionConf>();
            }
            MySql.Close();
            return list;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public void CheckDataIsComplete(Action<bool> isComplete)
        //{
        //    GameDataMgr.Instance.CheckDataComplete();
        //}

        /// <summary>
        /// 获取本地的MD5值
        /// </summary>
        /// <returns></returns>
        //public Dictionary<string, string> GetLocalMD5()
        //{
        //    if (isEditorLoad)
        //    {
        //        isCheckMD5 = false;
        //    }
        //    else if (GameDataMgr.Instance)
        //    {
        //        if (GameDataMgr.Instance.DataConfDic == null)
        //        {
        //            GameDataMgr.Instance.DataConfDic = new Dictionary<string, Dictionary<string, string>>();
        //        }
        //        if (GameDataMgr.Instance.DataConfDic.ContainsKey(dataPath))
        //        {
        //            Md5Map = GameDataMgr.Instance.DataConfDic[dataPath];
        //            isCheckMD5 = true;
        //        }
        //        else
        //        {
        //            isCheckMD5 = false;
        //            Md5Map = null;
        //        }
        //    }

        //    if ((Md5Map != null && Md5Map.Count > 0) || isCheckMD5)
        //    {
        //        return Md5Map;
        //    }
        //    Md5Map = GameDataMgr.Instance.GetMD5FormDirectory(dataPath, isCloudChild);
        //    if (!isEditorLoad)
        //    {
        //        if (GameDataMgr.Instance.DataConfDic == null)
        //        {
        //            GameDataMgr.Instance.DataConfDic = new Dictionary<string, Dictionary<string, string>>();
        //        }
        //        if (!GameDataMgr.Instance.DataConfDic.ContainsKey(dataPath))
        //        {
        //            GameDataMgr.Instance.DataConfDic.Add(dataPath, Md5Map);
        //        }
        //        else
        //        {
        //            GameDataMgr.Instance.DataConfDic[dataPath] = Md5Map;
        //        }
        //    }
        //    return Md5Map;
        //}

        /// <summary>
        /// 检测需要更新的部分
        /// </summary>
        /// <param name="versionList"></param>
        /// <returns></returns>
        //public List<AssetsVersionConf> ComparisonVersion(List<AssetsVersionConf> versionList)
        //{
        //    List<AssetsVersionConf> updateList = null;
        //    ///获取到服务器数据
        //    if (versionList != null)
        //    {
        //        updateList = GameDataMgr.Instance.ComparisonVersion(versionList, Md5Map);
        //    }
        //    return updateList;
        //}

        /// <summary>
        /// 申请服务器的资源
        /// </summary>
        /// <param name="server"></param>
        /// <param name="OnReadVersion"></param>
        public void ReqServerVersion(Action<List<AssetsVersionConf>> OnReadVersion)
        {
            GameDataMgr.Instance.ReqWebXMLData<AssetsVersionConf>(dataPath, (string xmlNode, WWW www) =>
            {
                List<AssetsVersionConf> versionList = null;
                string msg = "";
                if (www != null && string.IsNullOrEmpty(www.error))
                {
                    msg = www.text;
                    if (!string.IsNullOrEmpty(msg))
                    {
                        versionList = UnityUtil.ReadWebXMLData<AssetsVersionConf>(xmlNode, msg);
                    }
                }

                if (OnReadVersion != null)
                {
                    OnReadVersion(versionList);
                }
            });
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        public void DownData(string server, string dataPath, List<AssetsVersionConf> updateList, Action<float> process, Action updateComplete)
        {
            if (updateList != null && updateList.Count > 0)
            {
                List<AssetsVersionConf> newUpdate = new List<AssetsVersionConf>(updateList);
                GameDataMgr.Instance.DownGameDataByVersion(server, dataPath, updateList, process, () =>
                {
                    if (updateComplete != null)
                    {
                        updateComplete();
                    }
                });
            }
            else
            {
                if (updateComplete != null)
                {
                    updateComplete();
                }
            }
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        public void UpdateData(string server, List<AssetsVersionConf> updateList, Action<float> process, Action updateComplete)
        {
            if (updateList != null && updateList.Count > 0)
            {
                List<AssetsVersionConf> newUpdate = new List<AssetsVersionConf>(updateList);
                //GameDataMgr.Instance.DownGameDataByVersion(server, updateList, process, () =>
                //{

                //});
            }
            else
            {
                if (updateComplete != null)
                {
                    updateComplete();
                }
            }
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        public void DownData(List<AssetsVersionConf> updateList, Action<float> process, Action updateComplete)
        {
            //if (updateList != null && updateList.Count > 0)
            //{
            //    GameDataMgr.Instance.DownGameDataByVersion(updateList, process, updateComplete);
            //}
            //else
            //{
            //    if (updateComplete != null)
            //    {
            //        updateComplete();
            //    }
            //}
        }



        /// <summary>
        /// 获取更新包的大小
        /// </summary>
        /// <param name="updateList"></param>
        /// <returns></returns>
        public long GetSize(List<AssetsVersionConf> updateList)
        {
            return UnityUtil.GetDataSize(updateList);
        }
        /// <summary>
        /// 获取更新包的大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public float GetSize(long size)
        {
            return UnityUtil.GetDataSize(size);
        }
        /// <summary>
        /// 比较和检查更新
        /// 服务器缓存版本和本地版本数据进行比较
        /// 如果数据不一致，则打开
        /// </summary>
        /// <param name="serverList"></param>
        /// <returns></returns>
        public List<AssetsVersionConf> CheckUpdate(List<AssetsVersionConf> serverList)
        {
#if UNITY_EDITOR
            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
            {
                return null;
            }
#endif
            List<AssetsVersionConf> localList = GameDataMgr.Instance.ReadXMLData<AssetsVersionConf>(dataPath, true);
            Dictionary<string, AssetsVersionConf> localMap = new Dictionary<string, AssetsVersionConf>();
            List<AssetsVersionConf> updateList = new List<AssetsVersionConf>();
            if (localList == null || localList.Count < 1)
            {
                if (serverList != null && serverList.Count > 0)
                {
                    for (int i = serverList.Count - 1; i >= 0; i--)
                    {
                        AssetsVersionConf avc = serverList[i];
                        Debug.Log(avc);
                        if (avc.PriKey.EndsWith(typeof(AssetsVersionConf).Name + ".xml"))
                        {
                            serverList.Remove(avc);
                            break;
                        }
                    }
                }
                return serverList;
            }

            for (int i = 0; i < localList.Count; i++)
            {
                if (!localMap.ContainsKey(localList[i].PriKey))
                {
                    localMap.Add(localList[i].PriKey, localList[i]);
                }
            }


            for (int i = 0; i < serverList.Count; i++)
            {
                if (!serverList[i].PriKey.EndsWith(typeof(AssetsVersionConf).Name + ".xml"))
                {
                    if (!localMap.ContainsKey(serverList[i].PriKey))
                    {
                        updateList.Add(serverList[i]);
                    }
                    else if (serverList[i].MD5 != localMap[serverList[i].PriKey].MD5)
                    {
                        updateList.Add(serverList[i]);
                    }
                }
            }
            return updateList;
        }
    }
}