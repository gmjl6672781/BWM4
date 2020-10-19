//using BeinLab.RS5.Mgr;
//using BeinLab.RS5.UI;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//namespace BeinLab.Util
//{
//    /// <summary>
//    /// 游戏资源路径管理  已弃用，请使用GameAssetLoader、GameDataMgr
//    /// </summary>
//    public class GameAssetData : Singleton<GameAssetData>
//    {
//        /// <summary>
//        /// 编辑配置，URL，主配置路径等等
//        /// </summary>
//        public BuildConfig buildConf;
//        /// <summary>
//        /// 本地的资源根路径,根据游戏模式来定义，APP，LocalPhone，Steam等
//        /// </summary>
//        private string localRootPath;
//        /// <summary>
//        /// 本地资源路径/// <summary>
//        /// 游戏路径 = 根路径+  （项目名称+平台+数据包名称）
//        /// </summary>
//        /// </summary>
//        private string assetPath;
//        /// <summary>
//        /// 服务器资源路径
//        /// 服务器资源路径 = 服务器URL + （项目名称+平台+数据包名称）
//        /// </summary>
//        private string serverPath;
//        /// <summary>
//        /// 指定的资源路径，例如电脑测试时，输入的资源路径
//        /// </summary>
//        public string editPath;
//        private string showPath = "fengyunar/d_100_publicart";
//        /// <summary>
//        /// 游戏模式
//        /// </summary>
//        private GameModel model = GameModel.XRConf_EditorAR;
//        /// <summary>
//        /// 本地游戏根路径
//        /// </summary>
//        public string LocalRootPath
//        {
//            get
//            {
//                return localRootPath;
//            }

//            set
//            {
//                localRootPath = value;
//            }
//        }
//        /// <summary>
//        /// 本地游戏资源路径
//        /// </summary>
//        public string AssetPath
//        {
//            get
//            {
//                return assetPath;
//            }

//            set
//            {
//                assetPath = value;
//            }
//        }
//        /// <summary>
//        /// 服务器路径
//        /// </summary>
//        public string ServerPath
//        {
//            get
//            {
//                return serverPath;
//            }

//            set
//            {
//                serverPath = value;
//            }
//        }

//        public string ShowPath
//        {
//            get
//            {
//                return showPath;
//            }

//            set
//            {
//                showPath = value;
//            }
//        }

//        public GameModel Model
//        {
//            get
//            {
//                return model;
//            }

//            set
//            {
//                model = value;
//            }
//        }

//        /// <summary>
//        /// 初始化本地资源路径
//        /// </summary>
//        protected override void Awake()
//        {
//            base.Awake();
//            if (transform.parent == null)
//                DontDestroyOnLoad(gameObject);
//            InitGamePath();
//        }

//        /// <summary>
//        /// 初始化游戏路径
//        /// </summary>
//        public void InitGamePath()
//        {
//            if (transform.parent == null)
//                DontDestroyOnLoad(gameObject);
//            if (buildConf.buildType == BulidType.LocalPhone || buildConf.buildType == BulidType.App)
//            {
//                LocalRootPath = Application.persistentDataPath;
//            }
//            else if (buildConf.buildType == BulidType.Editor)
//            {
//                LocalRootPath = Application.dataPath;
//            }
//            else if (buildConf.buildType == BulidType.Stream)
//            {
//                LocalRootPath = Application.streamingAssetsPath;
//            }
//            else
//            {
//                LocalRootPath = editPath;
//            }
//            if (buildConf.buildType == BulidType.Editor)
//            {
//                AssetPath = "Assets";
//            }
//            else
//            {
//                ///游戏路径 = 根路径 + 项目名称+平台+美术包名称
//                AssetPath = Path.Combine(LocalRootPath, buildConf.GamePath);
//            }
//            ///确保游戏路径真实存在
//            if (buildConf.buildType != BulidType.Editor)
//            {
//                try
//                {
//                    if (!Directory.Exists(AssetPath))
//                    {
//                        Directory.CreateDirectory(AssetPath);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Debug.LogError("读取/创建文件失败" + ex.ToString());
//                }
//            }
//            AssetPath = AssetPath.Replace("\\", "/");

//            if (buildConf.buildType == BulidType.Editor)
//            {
//                Model = GameModel.XRConf_EditorAR;
//            }
//            else
//            {
//#if UNITY_IPHONE
//                Model = GameModel.XRConf_ARKit;
//#elif UNITY_ANDROID
//                Model = GameModel.XRConf_ARCore;
//#endif
//            }
//        }

//        /// <summary>
//        /// 检查基础资源版本情况
//        /// 主资源索引
//        /// </summary>
//        /// <returns></returns>
//        public bool CheckBaseAsset(out Dictionary<string, string> map, out List<AssetsVersionConf> list)
//        {
//            return CheckLocalAsset(AssetPath, out map, out list, false);
//        }
//        /// <summary>
//        /// 检查车型列表资源版本
//        /// 出现版本不一致的情况，强制联网检测，否则退出
//        /// </summary>
//        /// <returns></returns>
//        public bool CheckMainAsset(out Dictionary<string, string> map, out List<AssetsVersionConf> list)
//        {
//            return CheckLocalAsset(AssetPath + "/" + buildConf.mainPath, out map, out list);
//        }



//        /// <summary>
//        /// 同步 校对本地指定路径资源
//        /// 先获取本地路径下的资源文件，进行校对,检测资源是否完整
//        /// 当本地资源完备时，可以进入，非强制联网
//        /// 否则，将强制性联网
//        /// 问题：如何联网检测对应的文件？
//        /// </summary>
//        public bool CheckLocalAsset(string path, out Dictionary<string, string> realData, out List<AssetsVersionConf> lcoalXML, bool isDirChild = true)
//        {
//            lcoalXML = null;
//#if !UNITY_EDITOR
//            if (buildConf.buildType == BulidType.Stream)
//            {
//                realData = null;
//                return true;
//            }
//#endif
//            bool isComplete = true;
//            if (!Directory.Exists(path))
//            {
//                realData = null;
//                print("文件夹不存在" + path);
//                return false;
//            }
//            ///读取真实的本地文件以及文件的MD5
//            realData = GetLocalFileMD5(path, null, isDirChild);
//            if (realData != null && realData.Count > 1)
//            {
//                int errCount = 0;
//                ///读取配置缓存文件信息
//                List<AssetsVersionConf> list = UnityUtil.ReadXMLData<AssetsVersionConf>(path);
//                lcoalXML = list;
//                if (list != null && list.Count > 1)
//                {
//                    for (int i = 0; i < list.Count; i++)
//                    {
//                        string fileName = list[i].PriKey.Replace("..", "/");
//                        if (!realData.ContainsKey(fileName) || realData[fileName] != list[i].MD5)
//                        {
//                            errCount++;
//                            if (errCount > 1)
//                            {
//                                ///可能存在资源不完整，资源未完整更新等原因
//                                Debug.Log("缓存版本与真实资源不一致，需要联网检测");
//                                isComplete = false;
//                                break;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    Debug.Log("本地版本缓存信息为空");
//                    isComplete = false;
//                }
//            }
//            else
//            {
//                Debug.Log("本地数据为空" + path);
//                isComplete = false;
//            }
//            return isComplete;
//        }

//        /// <summary>
//        /// 异步 校对本地指定路径资源
//        /// 先获取本地路径下的资源文件，进行校对,检测资源是否完整
//        /// 当本地资源完备时，可以进入，非强制联网
//        /// 否则，将强制性联网
//        /// </summary>
//        public IEnumerator CheckLocalAssetSyn(string path, Action<bool> OnCheckLocalData, Action<float> process = null, bool isDirChild = true)
//        {
//            bool isComplete = true;
//            ///读取真实的本地文件以及文件的MD5
//            Dictionary<string, string> realData = GetLocalFileMD5(path, null, isDirChild);
//            if (realData != null && realData.Count > 0)
//            {
//                ///读取配置缓存文件信息
//                List<AssetsVersionConf> list = UnityUtil.ReadXMLData<AssetsVersionConf>(path);
//                if (list != null && list.Count > 0)
//                {
//                    for (int i = 0; i < list.Count; i++)
//                    {
//                        string fileName = list[i].PriKey.Replace("..", "/");
//                        if (!realData.ContainsKey(fileName) || realData[fileName] != list[i].MD5)
//                        {
//                            ///可能存在资源不完整，资源未完整更新等原因
//                            Debug.Log("缓存版本与真实资源不一致，需要联网检测");
//                            isComplete = false;
//                            break;
//                        }
//                        if (process != null)
//                        {
//                            process(1f * i / list.Count);
//                        }
//                        yield return new WaitForFixedUpdate();
//                    }
//                }
//                else
//                {
//                    Debug.Log("本地版本缓存信息为空");
//                    isComplete = false;
//                }
//            }
//            else
//            {
//                Debug.Log("本地数据为空");
//                isComplete = false;
//            }
//            if (process != null)
//            {
//                process(1f);
//            }
//            if (OnCheckLocalData != null)
//            {
//                OnCheckLocalData(isComplete);
//            }
//        }

//        /// <summary>
//        /// 检查服务器与本地数据的集合是否不同
//        /// 同步 比较两个Dic是否相同，如不同则返回不同的集合名称
//        /// </summary>
//        /// <param name="list1">一定不为空</param>
//        /// <param name="list2">可以为空</param>
//        /// <param name="msg"></param>
//        /// <returns>返回两个集合中不同元素的集合</returns>
//        public List<AssetsVersionConf> ComparisonVersion(List<AssetsVersionConf> list1, Dictionary<string, string> list2)
//        {
//            List<AssetsVersionConf> updateList = new List<AssetsVersionConf>();
//            ///获得服务器数据
//            if (list1 != null)
//            {
//                ///本地不存在任何数据时
//                if (list2 == null || list2.Count < 1)
//                {
//                    updateList.AddRange(list1);
//                    print("列表不存在");
//                }
//                else
//                {
//                    foreach (var item in list1)
//                    {
//                        string key = item.PriKey.Replace("..", "/");
//                        if (!list2.ContainsKey(key))
//                        {
//                            updateList.Add(item);
//                        }
//                        else if (item.MD5 != list2[key])
//                        {
//                            updateList.Add(item);
//                        }
//                    }
//                }
//            }
//            return updateList;
//        }


//        /// <summary>
//        /// 检查服务器与本地数据的集合是否不同
//        /// 同步 比较两个Dic是否相同，如不同则返回不同的集合名称
//        /// </summary>
//        /// <param name="list1">一定不为空</param>
//        /// <param name="list2">可以为空</param>
//        /// <param name="msg"></param>
//        /// <returns>返回两个集合中不同元素的集合</returns>
//        public List<string> ComparisonList(Dictionary<string, string> list1, Dictionary<string, string> list2)
//        {
//            List<string> updateList = new List<string>();
//            ///获得服务器数据
//            if (list1 != null)
//            {
//                ///本地不存在任何数据时
//                if (list2 == null || list2.Count < 1)
//                {
//                    updateList.AddRange(list1.Keys);
//                }
//                else
//                {
//                    foreach (var item in list1)
//                    {

//                        if (!list2.ContainsKey(item.Key))
//                        {
//                            updateList.Add(item.Key);
//                        }
//                        else if (item.Value != list2[item.Key])
//                        {
//                            updateList.Add(item.Key);
//                        }
//                    }
//                }
//            }
//            return updateList;
//        }
//        /// <summary>
//        ///  异步 比较两个Dic是否相同，如不同则返回不同的集合名称
//        /// </summary>
//        /// <param name="list1">一定不为空</param>
//        /// <param name="list2">可以为空</param>
//        /// <param name="msg"></param>
//        /// <returns>返回两个集合中不同元素的集合</returns>
//        public IEnumerator ComparisonVersionSyn(List<AssetsVersionConf> list1,
//            Dictionary<string, string> list2, Action<List<AssetsVersionConf>> OnCheckCompar, Action<float> comparProcess)
//        {
//            List<AssetsVersionConf> updateList = new List<AssetsVersionConf>();
//            ///获得服务器数据
//            if (list1 != null)
//            {
//                int process = 0;
//                ///本地不存在任何数据时
//                if (list2 == null || list2.Count < 1)
//                {
//                    updateList.AddRange(list1);
//                }
//                else
//                {
//                    foreach (var item in list1)
//                    {
//                        process++;
//                        string key = item.PriKey.Replace("..", "/");
//                        if (!list2.ContainsKey(key))
//                        {
//                            updateList.Add(item);
//                        }
//                        else if (item.MD5 != list2[key])
//                        {
//                            updateList.Add(item);
//                        }
//                        if (comparProcess != null)
//                        {
//                            comparProcess(1.0f * process / list1.Count);
//                        }
//                        yield return new WaitForFixedUpdate();
//                    }
//                }
//            }
//            if (comparProcess != null)
//            {
//                comparProcess(1);
//            }
//            if (OnCheckCompar != null)
//            {
//                OnCheckCompar(updateList);
//            }
//        }

//        /// <summary>
//        ///  异步 比较两个Dic是否相同，如不同则返回不同的集合名称
//        /// </summary>
//        /// <param name="list1">一定不为空</param>
//        /// <param name="list2">可以为空</param>
//        /// <param name="msg"></param>
//        /// <returns>返回两个集合中不同元素的集合</returns>
//        public IEnumerator ComparisonListSyn(Dictionary<string, string> list1,
//            Dictionary<string, string> list2, Action<List<string>> OnCheckCompar, Action<float> comparProcess)
//        {
//            List<string> updateList = new List<string>();
//            ///获得服务器数据
//            if (list1 != null)
//            {
//                int process = 0;
//                ///本地不存在任何数据时
//                if (list2 == null || list2.Count < 1)
//                {
//                    updateList.AddRange(list1.Keys);
//                }
//                else
//                {
//                    foreach (var item in list1)
//                    {
//                        process++;
//                        if (!list2.ContainsKey(item.Key))
//                        {
//                            updateList.Add(item.Key);
//                        }
//                        else if (item.Value != list2[item.Key])
//                        {
//                            updateList.Add(item.Key);
//                        }
//                        if (comparProcess != null)
//                        {
//                            comparProcess(1.0f * process / list1.Count);
//                        }
//                        yield return new WaitForFixedUpdate();
//                    }
//                }
//            }
//            if (comparProcess != null)
//            {
//                comparProcess(1);
//            }
//            if (OnCheckCompar != null)
//            {
//                OnCheckCompar(updateList);
//            }
//        }








//        /// <summary>
//        /// 下载资源
//        /// </summary>
//        /// <param name="downList"></param>
//        /// <param name="isCount">是否按照下载数量更新进度</param>
//        /// <param name="downProcess">更新的总进度</param>
//        /// <returns></returns>
//        public IEnumerator DownLoadData(string serverURL, List<AssetsVersionConf> downList, Action<bool> onDownComplete,
//            bool isCount = true, Action<float, float, float> downProcess = null)
//        {
//            int downCount = 0;
//            bool isComPlete = true;
//            if (downList != null && downList.Count > 0)
//            {
//                float allDataCount = 0;
//                float curDataCount = 0;
//                for (int i = 0; i < downList.Count; i++)
//                {
//                    allDataCount += downList[i].Size / (1024f * 1024f);
//                }

//                for (int i = 0; i < downList.Count; i++)
//                {
//                    ///当下载堵塞或者遇到网络连接问题时，协程暂停
//                    while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable || i - downCount > buildConf.maxDownCount)
//                    {
//                        yield return new WaitForFixedUpdate();
//                    }
//                    string path = downList[i].PriKey.Replace("..", "/");
//                    int index = i;
//                    float curProcess = 0;
//                    float tmpProcess = (downCount + curProcess) / downList.Count;
//                    ReqDownLoadData(serverURL, downList[index].PriKey.Replace("..", "/"), path, (bool isDown) =>
//                 {
//                     ///只有当下载失败时进行标识，同时只标识一次！
//                     ///因为下载完成之后还需要二次校验，在这里不再标记下载失败的标签
//                     if (isComPlete && !isDown)
//                     {
//                         isComPlete = isDown;
//                     }
//                     tmpProcess = (downCount + curProcess) / downList.Count;
//                     if (!isCount)
//                     {
//                         curDataCount += downList[index].Size / (1024f * 1024f);
//                     }
//                     downCount++;
//                 }, (float process) =>
//                 {
//                     curProcess = process;
//                     tmpProcess = (downCount + curProcess) / downList.Count;
//                     if (!isCount)
//                     {
//                         tmpProcess = (curDataCount + curProcess * downList[index].Size) / allDataCount;
//                     }
//                     if (downProcess != null)
//                     {
//                         if (isCount)
//                         {
//                             downProcess(downCount + curProcess, downList.Count, tmpProcess);
//                         }
//                         else
//                         {
//                             downProcess(curDataCount + curProcess * downList[index].Size, allDataCount, tmpProcess);
//                         }
//                     }
//                 });
//                }
//                ///等待下载完成
//                while (downCount < downList.Count)
//                {
//                    yield return new WaitForFixedUpdate();
//                }
//            }
//            if (onDownComplete != null)
//            {
//                onDownComplete(isComPlete);
//            }
//        }

//        /// <summary>
//        /// 申请服务器配置列表
//        /// </summary>
//        /// <param name="loadServer"></param>
//        public void ReqServerConf(Action<List<ServerVisionConf>> loadServer)
//        {
//            ReqAssetsVersion(buildConf.serverURL, loadServer, buildConf.projectName, buildConf.serverPath);
//        }
//        /// <summary>
//        /// 申请资源索引文件
//        /// </summary>
//        /// <param name="loadServer"></param>
//        public void ReqBaseAssetsVersion(string serverURL, Action<List<AssetsVersionConf>> loadServer)
//        {
//            ReqAssetsVersion(serverURL, loadServer);
//        }

//        /// <summary>
//        /// 申请资源索引文件
//        /// </summary>
//        /// <param name="loadServer"></param>
//        public void ReqCarAssetsVersion(string serverURL, Action<List<AssetsVersionConf>> loadServer)
//        {
//            ReqAssetsVersion(serverURL, loadServer, buildConf.mainPath);
//        }

//        /// <summary>
//        /// 申请XML配置文件
//        /// </summary>
//        /// <param name="loadServer"></param>
//        public void ReqAssetsVersion(string serverURL, Action<List<AssetsVersionConf>> loadServer, string path = "")
//        {
//            string gamePath = buildConf.GamePath;
//            string localPath = buildConf.versionPath;
//            if (!string.IsNullOrEmpty(path))
//            {
//                localPath = path + "/" + localPath;
//            }
//            ReqAssetsVersion(serverURL, loadServer, gamePath, localPath);
//        }

//        /// <summary>
//        /// 申请下载XML配置文件
//        /// </summary>
//        /// <typeparam name="T">XML文件，对象集合</typeparam>
//        /// <param name="loadServer">回调</param>
//        /// <param name="path">相对于项目工程的相对路径</param>
//        /// <param name="file">文件名称</param>
//        public void ReqAssetsVersion<T>(string serverURL, Action<List<T>> loadServer, string path, string file)
//        {
//            if (buildConf.buildType != BulidType.Editor)
//            {
//                ReqWebXmlToLocal(serverURL, file, path, file, loadServer);
//            }
//            else
//            {
//                if (loadServer != null)
//                {
//                    loadServer(null);
//                }
//            }
//        }

//        /// <summary>
//        /// 读取服务器的xml文件
//        /// 下载指定路径的文件
//        /// 保存文件，然后读取XML表格，获取到数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="loadServer"></param>
//        /// <param name="webPath">服务器路径，相对路径</param>
//        /// <param name="localPath">本地相对路径</param>
//        /// <param name="process">读取的进度</param>
//        public void ReqWebXmlToLocal<T>(string serverURL, string webPath, string localPath,
//            Action<List<T>> loadServer, Action<float> process = null)
//        {
//            ReqDownLoadData(serverURL, webPath, localPath, (bool isDown) =>
//              {
//                  if (isDown)
//                  {
//                      string loadPath = AssetPath;
//                      if (localPath.EndsWith("/AssetsVersionConf.xml"))
//                      {
//                          loadPath += "/" + localPath.Replace("/AssetsVersionConf.xml", "");
//                      }
//                      List<T> list = UnityUtil.ReadXMLData<T>(loadPath);
//                      if (loadServer != null)
//                      {
//                          loadServer(list);
//                      }
//                  }
//              }, process);
//        }

//        /// <summary>
//        /// 读取服务器的xml文件
//        /// 下载指定路径的文件
//        /// 保存文件，然后读取XML表格，获取到数据
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="loadServer"></param>
//        /// <param name="webPath">服务器路径，相对路径</param>
//        /// <param name="localPath">本地相对路径</param>
//        /// <param name="process">读取的进度</param>
//        public void ReqWebXmlToLocal<T>(string serverURL, string webPath, string gamePath, string localPath,
//            Action<List<T>> loadServer, Action<float> process = null)
//        {
//            ReqDownLoadData(serverURL, webPath, gamePath, localPath, (bool isDown) =>
//             {
//                 if (isDown)
//                 {
//                     string loadPath = AssetPath;
//                     if (localPath.EndsWith("/AssetsVersionConf.xml"))
//                     {
//                         loadPath += "/" + localPath.Replace("/AssetsVersionConf.xml", "");
//                     }
//                     List<T> list = UnityUtil.ReadXMLData<T>(loadPath);
//                     if (loadServer != null)
//                     {
//                         loadServer(list);
//                     }
//                 }
//             }, process);
//        }

//        /// <summary>
//        /// 下载服务器资源到本地
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="loadServer"></param>
//        /// <param name="webPath">服务器路径，相对路径</param>
//        /// <param name="localPath">本地相对路径</param>
//        /// <param name="process">下载的进度</param>
//        public void ReqDownLoadData(string serverURL, string webPath, string localPath,
//            Action<bool> onDownLoadComplete, Action<float> process = null)
//        {
//            ReqDownLoadData(serverURL, webPath, buildConf.GamePath, localPath, onDownLoadComplete, process);
//        }

//        /// <summary>
//        /// 下载服务器资源到本地
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="loadServer"></param>
//        /// <param name="webPath">服务器路径，相对路径</param>
//        /// <param name="localPath">本地相对路径</param>
//        /// <param name="process">下载的进度</param>
//        public void ReqDownLoadData(string serverURL, string webPath, string gamePath, string localPath,
//            Action<bool> onDownLoadComplete, Action<float> process = null)
//        {
//            //string netPath = buildConf.serverURL + "/" + gamePath + "/" + webPath;
//            string netPath = serverURL + "/" + gamePath + "/" + webPath;
//            StartCoroutine(ReqDataByWWW(netPath, (WWW www) =>
//            {
//                if (www != null && string.IsNullOrEmpty(www.error))
//                {
//                    SaveFileToLocal(www.bytes, localPath);
//                    if (onDownLoadComplete != null)
//                    {
//                        onDownLoadComplete(true);
//                    }
//                }
//                else
//                {
//                    if (onDownLoadComplete != null)
//                    {
//                        onDownLoadComplete(false);
//                    }
//                }
//            }, process));
//        }


//        /// <summary>
//        /// 申请WWW资源，
//        /// </summary>
//        /// <param name="serverURL">资源地址</param>
//        /// <param name="OnReadData">申请回调</param>
//        /// <param name="process">申请进度</param>
//        /// <returns></returns>
//        public IEnumerator ReqDataByWWW(string serverURL, Action<WWW> OnReadData, Action<float> process = null)
//        {
//            serverURL = serverURL.Replace("\\", "/");
//            print(serverURL);
//            WWW www = new WWW(serverURL);
//            while (!www.isDone)
//            {
//                if (process != null)
//                {
//                    process(www.progress);
//                }
//                yield return new WaitForFixedUpdate();
//            }
//            yield return www;
//            if (string.IsNullOrEmpty(www.error))
//            {
//                if (OnReadData != null)
//                {
//                    OnReadData(www);
//                }
//            }
//            else
//            {
//                print(serverURL);
//                Debug.LogError(www.error);
//                if (process != null)
//                {
//                    process(1);
//                }
//                if (OnReadData != null)
//                {
//                    OnReadData(null);
//                }
//            }
//            www.Dispose();
//            if (process != null)
//            {
//                process(1);
//            }
//        }


//        /// <summary>
//        /// 申请WWW资源，
//        /// </summary>
//        /// <param name="serverURL">资源地址</param>
//        /// <param name="OnReadData">申请回调</param>
//        /// <param name="process">申请进度</param>
//        /// <returns></returns>
//        public IEnumerator ReqDataByWWW(string serverURL, Action<string, string> OnReadData, Action<float> process = null)
//        {
//            serverURL = serverURL.Replace("\\", "/");
//            string fileName = serverURL.Substring(serverURL.LastIndexOf("/") + 1);
//            fileName = fileName.Split('.')[0];
//            WWW www = new WWW(serverURL);
//            while (!www.isDone)
//            {
//                if (process != null)
//                {
//                    process(www.progress);
//                }
//                yield return new WaitForFixedUpdate();
//            }
//            yield return www;
//            if (string.IsNullOrEmpty(www.error))
//            {
//                if (OnReadData != null)
//                {
//                    OnReadData(fileName, www.text);
//                }
//            }
//            else
//            {
//                Debug.LogError(www.error);
//                if (process != null)
//                {
//                    process(1);
//                }
//                if (OnReadData != null)
//                {
//                    OnReadData(fileName, null);
//                }
//            }
//            www.Dispose();
//            if (process != null)
//            {
//                process(1);
//            }
//        }
//        /// <summary>
//        /// 保存数据到本地
//        /// </summary>
//        /// <param name="bytes">www加载的数据</param>
//        /// <param name="path">相对路径</param>
//        private void SaveFileToLocal(byte[] bytes, string path)
//        {
//            try
//            {
//                string filename = Path.Combine(AssetPath, path);
//                string dir = filename.Substring(0, filename.LastIndexOf("/"));
//                ///如果保存的路径不存在，则创路径
//                if (!Directory.Exists(dir))
//                {
//                    Directory.CreateDirectory(dir);
//                }
//                ///如果文件存在，则直接覆盖
//                FileStream fs = new FileStream(filename, FileMode.Create);
//                // Create the writer for data.
//                BinaryWriter w = new BinaryWriter(fs);
//                // Write data to Test.data.
//                w.Write(bytes);
//                fs.Flush();
//                w.Flush();
//                w.Close();
//                fs.Close();
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError(ex.ToString());
//            }
//        }

//        /// <summary>
//        /// 遍历指定文件夹，并获取文件名以及对应的MD5值
//        /// 将忽略.meta格式的文件
//        /// </summary>
//        /// <param name="dir">绝对路径</param>
//        public Dictionary<string, string> GetLocalFileMD5(string dir, Dictionary<string, string> map, bool isDirChild = true)
//        {
//            if (!Directory.Exists(dir))
//            {
//                return map;
//            }

//            DirectoryInfo d = new DirectoryInfo(dir);
//            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
//            foreach (FileSystemInfo fsinfo in fsinfos)
//            {
//                if (fsinfo is DirectoryInfo)     //判断是否为文件夹
//                {
//                    if (isDirChild)
//                        GetLocalFileMD5(fsinfo.FullName, map, isDirChild);//递归调用
//                }
//                else
//                {
//                    string full = fsinfo.FullName;
//                    if (!full.EndsWith(".meta") && fsinfo.Name != buildConf.versionPath)
//                    {
//                        full = full.Replace("\\", "/");
//                        full = full.Replace(AssetPath + "/", "");
//                        string MD5 = UnityUtil.GetMD5HashFromFile(fsinfo.FullName);
//                        if (map == null)
//                        {
//                            map = new Dictionary<string, string>();
//                        }
//                        map.Add(full, MD5);
//                    }
//                }
//            }
//            return map;
//        }

//    }
//}