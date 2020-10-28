using BeinLab.FengYun.UI;
using BeinLab.RS5.Gamer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeinLab.Util
{
    /// <summary>
    /// 游戏数据管理
    /// 重构GameAssetData管理类
    /// 检测资源完整性
    /// </summary>
    public class GameDataMgr : Singleton<GameDataMgr>
    {
        /// <summary>
        /// 编辑配置，URL，主配置路径等等
        /// </summary>
        public BuildConfig buildConf;
        /// <summary>
        /// 本地的资源根路径,根据游戏模式来定义，APP，LocalPhone，Steam等
        /// </summary>
        private string localRootPath;
        /// <summary>
        /// 本地资源路径/// <summary>
        /// 游戏路径 = 根路径+  （项目名称+平台+数据包名称）
        /// </summary>
        private string assetPath;
        /// <summary>
        /// 服务器数据缓存路径
        /// </summary>
        private string serverCachePath;
        /// <summary>
        /// 服务器资源路径
        /// 服务器资源路径 = 服务器URL + （项目名称+平台+数据包名称）
        /// </summary>
        private string serverPath;
        /// <summary>
        /// 指定的资源路径，例如电脑测试时，输入的资源路径
        /// </summary>
        public string editPath;
        /// <summary>
        /// 默认看车路径
        /// </summary>
        private string showPath = "fengyunar/d_100_publicart";
        /// <summary>
        /// 游戏模式
        /// </summary>
        private GameModel model = GameModel.XRConf_EditorAR;
        public Vector2 extraUI = Vector2.zero;
        public bool isCanLoadScene = true;
        public bool isRunInBack = true;
        public Dictionary<string, Dictionary<string, string>> DataConfDic;

        //private string gameServerURL;
        /// <summary>
        /// 本地游戏根路径
        /// </summary>
        public string LocalRootPath
        {
            get
            {
                return localRootPath;
            }

            set
            {
                localRootPath = value;
            }
        }
        /// <summary>
        /// 本地游戏资源路径 绝对路径
        /// 游戏路径 = 根路径 + 项目名称+平台+美术包名称
        /// </summary>
        public string AssetPath
        {
            get
            {
                return assetPath;
            }

            set
            {
                assetPath = value;
            }
        }
        /// <summary>
        /// 服务器路径
        /// </summary>
        public string ServerPath
        {
            get
            {
                return serverPath;
            }

            set
            {
                serverPath = value;
            }
        }
        /// <summary>
        /// 选择的展示路径，选择进入哪个看车场景，加载哪个模块等等
        /// </summary>
        public string ShowPath
        {
            get
            {
                return showPath;
            }

            set
            {
                showPath = value;
            }
        }

        /// <summary>
        /// 游戏模式
        /// </summary>
        public GameModel Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        public string ServerCachePath { get => serverCachePath; set => serverCachePath = value; }

        //public string GameServerURL
        //{
        //    get
        //    {
        //        return gameServerURL;
        //    }

        //    set
        //    {
        //        gameServerURL = value;
        //    }
        //}

        /// <summary>
        /// 初始化本地资源路径
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            InitGamePath();
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Application.targetFrameRate = buildConf.fps;
#if UNITY_EDITOR
            Application.runInBackground = isRunInBack;
#endif
        }
        #region 初始化游戏路径
        /// <summary>
        /// 初始化游戏路径
        /// </summary>
        public void InitGamePath()
        {
            if (transform.parent == null)
                DontDestroyOnLoad(gameObject);
            Debug.Log(buildConf);
            if (buildConf.buildType == BulidType.LocalPhone || buildConf.buildType == BulidType.App)
            {
                LocalRootPath = Application.dataPath;
            }
            else if (buildConf.buildType == BulidType.Editor)
            {
                LocalRootPath = Application.dataPath;
            }
            else if (buildConf.buildType == BulidType.Stream)
            {
                LocalRootPath = Application.streamingAssetsPath;
            }
            else
            {
                LocalRootPath = editPath;
            }
            if (buildConf.buildType == BulidType.Editor)
            {
                //AssetPath = "Assets";
                AssetPath = Application.dataPath;
            }
            else
            {
                ///游戏路径 = 根路径 + 项目名称+平台+美术包名称
                AssetPath = Path.Combine(LocalRootPath, buildConf.GamePath);
            }
            AssetPath = AssetPath.Replace("\\", "/");
            ///确保游戏路径真实存在
            if (buildConf.buildType != BulidType.Editor)
            {
                try
                {
                    if (!Directory.Exists(AssetPath))
                    {
                        Directory.CreateDirectory(AssetPath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("读取/创建文件失败" + ex.ToString());
                }
            }
            if (buildConf.buildType == BulidType.Editor)
            {
                Model = GameModel.XRConf_EditorAR;
            }
            else
            {
#if UNITY_IPHONE
                Model = GameModel.XRConf_ARKit;
#elif UNITY_ANDROID
                Model = GameModel.XRConf_ARCore;
#endif
            }
#if UNITY_EDITOR
            print(AssetPath);
#endif
            ServerCachePath = AssetPath + "/" + "ServerCache";
        }

        private void Start()
        {
            //if (ServerChecker.Instance)
            //{
            //    ServerChecker.Instance.OnReConnectServer += OnReConnectServer;
            //    ServerChecker.Instance.OnConnectServer += OnConnectServer;
            //}
        }

        /// <summary>
        /// 服务器连接
        /// </summary>
        /// <param name="url"></param>
        private void OnConnectServer(string url)
        {
            if (string.IsNullOrEmpty(ServerPath))
            {
                ServerPath = url + "/" + buildConf.GamePath;
                print("Server Path :" + ServerPath);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnReConnectServer()
        {
            ServerPath = null;
        }
        #endregion

        /// <summary>
        /// 获取指定文件夹下的MD5资源
        /// </summary>
        /// <param name="directory">相对路径</param>
        public Dictionary<string, string> GetMD5FormDirectory(string directory, bool isDirChild = true)
        {
#if !UNITY_EDITOR
            if (buildConf.buildType == BulidType.Stream)
            {
                return new Dictionary<string, string>();
            }
#endif
            if (buildConf.buildType == BulidType.Editor)
            {
                return new Dictionary<string, string>();
            }
            string path = Path.Combine(AssetPath, directory);
            Dictionary<string, string> md5Map = new Dictionary<string, string>();
            //UnityUtil.GetFileMD5FormDirectory(path, AssetPath, ref md5Map, isDirChild);
            List<AssetsVersionConf> versionList = ReadXMLData<AssetsVersionConf>(path, true);
            for (int i = 0; i < versionList.Count; i++)
            {
                string key = versionList[i].PriKey.Replace("..", "/");
                md5Map.Add(key, versionList[i].MD5);
            }
            return md5Map;
        }
        /// <summary>
        /// 获取指定文件夹下的XML文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directory"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public List<T> ReadXMLData<T>(string directory, bool isCreate = true)
        {
            string path = Path.Combine(AssetPath, directory);
            return UnityUtil.ReadXMLData<T>(path, isCreate);
        }
        /// <summary>
        /// 申请服务器的XML数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dir"></param>
        /// <param name="onReqData"></param>
        /// <param name="process"></param>
        public void ReqWebXMLData(string server, string dir, Action<string, WWW> onReqData, Action<float> process = null)
        {
            string url = server + "/" + buildConf.GamePath + (string.IsNullOrEmpty(dir) ? "" : ("/" + dir));
            StartCoroutine(UnityUtil.ReqDataByWWW(url, onReqData, process));
        }
        /// <summary>
        /// 申请服务器的XML数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dir"></param>
        /// <param name="onReqData"></param>
        /// <param name="process"></param>
        public void ReqWebXMLData<T>(string server, string dir, Action<string, WWW> onReqData, Action<float> process = null)
        {
#if !UNITY_EDITOR
            if (buildConf.buildType == BulidType.Stream || string.IsNullOrEmpty(server))
            {
                if (onReqData != null)
                    onReqData(null, null);
                if (process != null)
                    process(1);
                return;
            }
#endif
            if (buildConf.buildType == BulidType.Editor || string.IsNullOrEmpty(server))
            {
                if (onReqData != null)
                    onReqData(null, null);
                if (process != null)
                    process(1);
                return;
            }
            string url = server + "/" + buildConf.GamePath
                + ((string.IsNullOrEmpty(dir) ? "" : ("/" + dir)) + "/" + typeof(T).Name + ".xml");
            StartCoroutine(UnityUtil.ReqDataByWWW(url, onReqData, process));
        }
        /// <summary>
        /// 申请服务器的XML数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dir"></param>
        /// <param name="onReqData"></param>
        /// <param name="process"></param>
        public void ReqWebXMLData<T>(string dir, Action<string, WWW> onReqData, Action<float> process = null)
        {
            ReqWebXMLData<T>(GameServerChecker.Instance.GameServer, dir, onReqData, process);
        }
        /// <summary>
        /// 申请服务器的XML数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="dir"></param>
        /// <param name="onReqData"></param>
        /// <param name="process"></param>
        public void ReqWebXMLData(string server, string dir, Action<WWW> onReqData, Action<float> process = null)
        {
            string url = server + "/" + buildConf.GamePath + "/" + dir;
            StartCoroutine(UnityUtil.ReqDataByWWW(url, onReqData, process));
        }

        /// <summary>
        /// 检测本地某个文件夹资源是否是完整的
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="isDirChild"></param>
        public bool CheckDataComplete(string directory, bool isDirChild = true)
        {
            Dictionary<string, string> md5Map = GetMD5FormDirectory(directory, isDirChild);
            return CheckDataComplete(directory, md5Map);
        }

        /// <summary>
        /// 检测本地某个文件夹资源是否是完整的
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="isDirChild"></param>
        public bool CheckDataComplete(string directory, Dictionary<string, string> md5Map)
        {
            ///全量包无需检测更新
#if !UNITY_EDITOR
            if (buildConf.buildType == BulidType.Stream)
            {
                return true;
            }
#endif
            if (buildConf.buildType == BulidType.Editor)
            {
                return true;
            }
            string path = Path.Combine(AssetPath, directory);
            if (!Directory.Exists(path))
            {
                return false;
            }
            List<AssetsVersionConf> versionList = ReadXMLData<AssetsVersionConf>(directory, false);

            return CheckDataComplete(versionList, md5Map);
        }

        /// <summary>
        /// 检测本地某个文件夹资源是否是完整的
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="isDirChild"></param>
        public bool CheckDataComplete(List<AssetsVersionConf> versionList, Dictionary<string, string> md5Map)
        {
            if (versionList == null || versionList.Count < 1)
            {
                return false;
            }
            if (md5Map == null || md5Map.Count < 1)
            {
                return false;
            }
            bool isComplete = true;
            for (int i = 0; i < versionList.Count; i++)
            {
                string fileName = versionList[i].PriKey.Replace("..", "/");
                if (!fileName.EndsWith(typeof(AssetsVersionConf).Name + ".xml") && (!md5Map.ContainsKey(fileName) || md5Map[fileName] != versionList[i].MD5))
                {
                    isComplete = false;
                    break;
                }
            }
            return isComplete;
        }

        public string GetRealPath(string prefabPath)
        {
            prefabPath = ShowPath + "/" + prefabPath;
            return prefabPath;
        }

        /// <summary>
        /// 比较版本缓存与真实MD5是否相同
        /// 同步 比较两个Dic是否相同，如不同则返回不同的集合名称
        /// </summary>
        /// <param name="list1">服务器版本缓存  一定不为空</param>
        /// <param name="list2">本地真实文件的MD5 可以为空</param>
        /// <param name="msg"></param>
        /// <returns>返回两个集合中不同元素的集合</returns>
        public List<AssetsVersionConf> ComparisonVersion(List<AssetsVersionConf> list1, Dictionary<string, string> list2)
        {
            List<AssetsVersionConf> updateList = new List<AssetsVersionConf>();
#if !UNITY_EDITOR
            if (buildConf.buildType == BulidType.Stream)
            {
                return updateList;
            }
#endif
            if (buildConf.buildType == BulidType.Editor)
            {
                return updateList;
            }
            List<string> delList = new List<string>();
            List<string> webList = new List<string>();
            ///获得服务器数据
            if (list1 != null)
            {

                ///本地不存在任何数据时
                if (list2 == null || list2.Count < 1)
                {
                    updateList.AddRange(list1);
                }
                else
                {
                    foreach (var item in list1)
                    {
                        string key = item.PriKey.Replace("..", "/");
                        if (!list2.ContainsKey(key))
                        {
                            updateList.Add(item);
                        }
                        else if (item.MD5 != list2[key])
                        {
                            updateList.Add(item);
                        }
                        webList.Add(key);
                    }
                    ///
                    if (list2.Count > list1.Count)
                    {
                        foreach (var item in list2)
                        {
                            if (!webList.Contains(item.Key))
                            {
                                delList.Add(item.Key);
                            }
                        }
                    }
                }

                if (updateList.Count == 1)
                {
                    bool isUpdateVersion = false;
                    ///更新的条件：本地版本缓存数据不存在，版本缓存记录不一致，内容不一致时将进行更新
                    List<AssetsVersionConf> localList = ReadXMLData<AssetsVersionConf>(updateList[0].PriKey.Replace("..", "/"), false);
                    if (localList != null && localList.Count > 0 && localList.Count == list1.Count)
                    {
                        for (int i = 0; i < localList.Count; i++)
                        {
                            if (localList[i].MD5 != list1[i].MD5)
                            {
                                isUpdateVersion = true;
                                Debug.Log("MD5不一致" + localList[i]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        isUpdateVersion = true;
                        Debug.Log("本地不存在" + updateList[0].PriKey);
                    }

                    if (!isUpdateVersion)
                    {
                        updateList = null;
                    }
                }
            }
            if (delList.Count > 0)
            {
                DelData(delList);
            }
            return updateList;
        }
        /// <summary>
        /// 删除本地多于的资源
        /// </summary>
        /// <param name="delList"></param>
        public void DelData(List<string> delList)
        {
            if (delList != null)
            {
                for (int i = 0; i < delList.Count; i++)
                {
                    UnityUtil.DelDataByPath(Path.Combine(AssetPath, delList[i]));
                }
            }
        }


        /// <summary>
        /// 下载数据
        /// 如果下载暂停或者断网如何？
        /// 如何计算当前的下载进度？
        /// </summary>
        public void DownGameDataByVersion(string serverUrl, string dataPath, List<AssetsVersionConf> listConf, Action<float> process, Action OnDownComplete)
        {
            StartCoroutine(DownGameData(serverUrl, dataPath, listConf, process, OnDownComplete));
        }
        /// <summary>
        /// 下载数据
        /// 如果下载暂停或者断网如何？
        /// 如何计算当前的下载进度？
        /// </summary>
        public void DownGameDataByVersion(List<AssetsVersionConf> listConf, Action<float> process, Action OnDownComplete)
        {
            //DownGameDataByVersion(GameServerChecker.Instance.GameServer, listConf, process, OnDownComplete);
        }
        /// <summary>
        /// 下载数据
        /// 如果下载暂停或者断网如何？
        /// 如何计算当前的下载进度？
        /// </summary>
        public void DownDataByVersion(List<AssetsVersionConf> listConf, Action<float> process, Action OnDownComplete)
        {
            //DownGameDataByVersion(GameServerChecker.Instance.GameServer, listConf, process, OnDownComplete);
        }
        /// <summary>
        /// 下载数据
        /// 如果下载暂停或者断网如何？
        /// 如何计算当前的下载进度？
        /// </summary>
        public IEnumerator DownGameData(string serverUrl, string dataPath, List<AssetsVersionConf> listConf, Action<float> process, Action OnDownComplete)
        {
            if (listConf == null)
            {
                if (OnDownComplete != null)
                {
                    OnDownComplete();
                }
                yield break;
            }
            ///逆序队列，下载时逆序下载
            listConf.Reverse();
            while (listConf.Count > 0)
            {
                yield return new WaitForFixedUpdate();
                ///当前并行下载的数量
                int downCount = 0;
                long curSize = 0;
                long downSize = 0;
                long allDownSize = UnityUtil.GetDataSize(listConf);
                for (int i = listConf.Count - 1; i >= 0; i--)
                {
                    ///没有网络的时候，暂停下载
                    while (GameNetChecker.Instance.NetState == NetworkReachability.NotReachable)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    while (downCount >= buildConf.maxDownCount)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    if (downCount < buildConf.maxDownCount)
                    {
                        int index = i;
                        AssetsVersionConf avc = listConf[index];
                        string path = avc.PriKey.Replace("..", "/");
                        string url = serverUrl + "/" + buildConf.GamePath + "/" + path;
                        downCount++;
                        StartCoroutine(UnityUtil.ReqDataByWWW(url, (WWW www) =>
                        {
                            if (www != null && string.IsNullOrEmpty(www.error))
                            {
                                string localPath = AssetPath + "/" + path;
                                UnityUtil.SaveFileToLocal(www.bytes, localPath);
                                string xmlPath = Path.Combine(AssetPath, dataPath);
                                UnityUtil.WriteXMLData(xmlPath, avc);
                                curSize += avc.Size;
                                listConf.Remove(avc);
                            }
                            downCount--;
                        }, (float curProcess) =>
                        {
                            if (curProcess < 1f)
                            {
                                long pSize = 0;
                                long.TryParse((avc.Size * curProcess).ToString(), out pSize);
                                downSize = curSize + pSize;
                                if (process != null)
                                {
                                    process(downSize * 1.0f / allDownSize);
                                }
                            }
                            else
                            {
                                downSize = curSize;
                                if (process != null)
                                {
                                    process(downSize * 1.0f / allDownSize);
                                }
                            }
                        }));
                    }
                }
                ///存在下载数据时
                while (downCount > 0)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            if (OnDownComplete != null)
            {
                OnDownComplete();
            }
        }

        /// <summary>
        /// 准备加载场景
        /// 准备加载场景之前，需要检测美术包准备情况，是否要强制联网
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadGameScene(int index)
        {
            if (index < 0)
            {
                GameSceneManager.Instance.RealQuitAR();
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            while (!isCanLoadScene)
            {
                yield return new WaitForFixedUpdate();
            }
            AsyncOperation ao = SceneManager.LoadSceneAsync(index);
            if (ProcessDlg.Instance)
            {
                ProcessDlg.Instance.gameObject.SetActive(true);
                ProcessDlg.Instance.UpdateProcess("正在加载 0%", 0);
                ao.allowSceneActivation = false;
            }
            float curProcess = 0;
            while (ao.progress < 0.9f)
            {
                curProcess = ao.progress;
                if (ProcessDlg.Instance)
                {
                    ProcessDlg.Instance.UpdateProcess("正在加载 " + ao.progress.ToString("p0"), ao.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (ProcessDlg.Instance && ProcessDlg.Instance.ProcessSlider.value < 0.98f)
            {
                ProcessDlg.Instance.ProcessSlider.value += Time.deltaTime;
                ProcessDlg.Instance.UpdateProcess("正在加载 " + ProcessDlg.Instance.ProcessSlider.value.
                    ToString("p0"), ProcessDlg.Instance.ProcessSlider.value);
                yield return new WaitForEndOfFrame();
            }
            while (!isCanLoadScene)
            {
                yield return new WaitForFixedUpdate();
            }
            ao.allowSceneActivation = true;
        }
        private Stack<string> sceneStack;
        private bool isLoading = false;
        /// <summary>
        /// 准备加载场景
        /// 准备加载场景之前，需要检测美术包准备情况，是否要强制联网
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadGameScene(string index)
        {
            if (isLoading)
            {
                yield return new WaitForEndOfFrame();
                yield break;
            }
            isLoading = true;
            SceneRecord.Instance.ChangeRecord();
            if (string.IsNullOrEmpty(index))
            {
                GameSceneManager.Instance.RealQuitAR();
                yield break;
            }
            if (sceneStack == null)
            {
                sceneStack = new Stack<string>();
            }
            if (index == "Return" && sceneStack.Count > 0)
            {
                index = sceneStack.Pop();
                if (SceneManager.GetActiveScene().name == index && sceneStack.Count > 0)
                {
                    index = sceneStack.Pop();
                }
                sceneStack.Push(index);
            }
            else
            {
                sceneStack.Push(index);
            }
            while (!isCanLoadScene)
            {
                yield return new WaitForFixedUpdate();
            }
            if (index == "Return")
            {
                yield break;
            }

            AsyncOperation ao = SceneManager.LoadSceneAsync(index);
            if (ProcessDlg.Instance)
            {
                ProcessDlg.Instance.gameObject.SetActive(true);
                ProcessDlg.Instance.UpdateProcess("正在加载 0%", 0);
                ao.allowSceneActivation = false;
            }
            while (ao.progress < 0.89f)
            {
                if (ProcessDlg.Instance)
                {
                    ProcessDlg.Instance.UpdateProcess("正在加载 " + ao.progress.ToString("p0"), ao.progress);
                }
                yield return new WaitForEndOfFrame();
            }
            while (ProcessDlg.Instance && ProcessDlg.Instance.ProcessSlider.value < 0.98f)
            {
                ProcessDlg.Instance.ProcessSlider.value += Time.deltaTime * 6.18f;
                ProcessDlg.Instance.UpdateProcess("正在加载 " + ProcessDlg.Instance.ProcessSlider.value.
                    ToString("p0"), ProcessDlg.Instance.ProcessSlider.value);
                yield return new WaitForEndOfFrame();
            }
            ///延时加载，可选择加载其他数据。
            while (!isCanLoadScene)
            {
                yield return new WaitForFixedUpdate();
            }
            ao.allowSceneActivation = true;
            isLoading = false;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearDataCache();
        }
        public void ClearDataCache()
        {
            if (DataConfDic != null)
            {
                DataConfDic.Clear();
                DataConfDic = null;
            }
        }
    }
}