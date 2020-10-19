//using BeinLab.Util;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//namespace BeinLab.FengYun.Gamer
//{
//    /// <summary>
//    /// 资源版本管理器
//    /// 将资源下载的信息缓存到本地
//    /// 检测当前数据包的状态，主要是从哪些方面检测？
//    /// 1，检测美术包是否存在
//    /// 2，读取美术包已完成几个了？
//    /// GameDataVersion
//    /// </summary>
//    public class GameDataVersioner : Singleton<GameDataVersioner>
//    {
//        private List<GameDataVersion> dataVersionList;
//        /// <summary>
//        /// 用来替代空串
//        /// </summary>
//        public string NULL = "NULL";
//        protected override void Awake()
//        {
//            base.Awake();
//            DontDestroyOnLoad(gameObject);
//        }

//        /// <summary>
//        /// 读取游戏资源版本
//        /// </summary>
//        public void ReadGameDataVersion()
//        {
//            dataVersionList = UnityUtil.ReadXMLData<GameDataVersion>(GameDataMgr.Instance.AssetPath);
//        }
//        /// <summary>
//        /// 检测某个美术包是否下载完毕
//        /// </summary>
//        /// <param name="artName">相对路径</param>
//        /// <returns></returns>
//        public bool CheckLocalComplte(string artName)
//        {
//            if (GameAssetData.Instance)
//            {
//                ///编辑器模式下直接返回ture
//                if (GameAssetData.Instance.buildConf.buildType == BulidType.Editor)
//                {
//                    return true;
//                }
//                ReadGameDataVersion();
//                if (dataVersionList != null)
//                {
//                    for (int i = 0; i < dataVersionList.Count; i++)
//                    {
//                        if (dataVersionList[i].ArtName == artName)
//                        {
//                            return dataVersionList[i].IsDownComplete;
//                        }
//                    }
//                }
//            }
//            return false;
//        }
//        /// <summary>
//        /// 写入版本信息
//        /// </summary>
//        /// <param name="artName"></param>
//        public void WriteDataVersion(string artName)
//        {
//            string priKey = artName;
//            if (string.IsNullOrEmpty(priKey))
//            {
//                priKey = NULL;
//            }

//            priKey = artName.Replace("\\", "/");
//            priKey = priKey.Replace("/", "..");

//            GameDataVersion gameVersion = new GameDataVersion();
//            gameVersion.PriKey = priKey;
//            gameVersion.ArtName = "";
//            gameVersion.ArtVersion = "";
//            gameVersion.UpdateTime = "";
//            gameVersion.IsDownComplete = false;
//        }

//    }
//}