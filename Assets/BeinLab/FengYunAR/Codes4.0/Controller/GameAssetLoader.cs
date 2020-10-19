using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 游戏资源加载管理
    /// </summary>
    public class GameAssetLoader : Singleton<GameAssetLoader>
    {
        /// <summary>
        /// 对象缓存列表 ，从AssetBundle中加载而来
        /// </summary>
        private Dictionary<string, UnityEngine.Object> objCacheMap;
        /// <summary>
        /// AssetBundle 缓存列表
        /// </summary>
        private Dictionary<string, AssetBundle> bundleCacheMap;
        /// <summary>
        /// 主资源索引对象  用来依赖加载资源
        /// </summary>
        private AssetBundleManifest assetManifest;
        /// <summary>
        /// 同时最大的加载数量
        /// </summary>
        public int maxLoadBundle = 5;
        protected override void Awake()
        {
            base.Awake();
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadObject(string path)
        {
            if (string.IsNullOrEmpty(path = CheckPath(path)))
            {
                return null;
            }
            print(path);
            if (objCacheMap == null) objCacheMap = new Dictionary<string, UnityEngine.Object>();
            if (objCacheMap.ContainsKey(path) && objCacheMap[path] != null)
            {
                return objCacheMap[path];
            }
            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
            {
#if UNITY_EDITOR
                return LoadObjectByPathOnEditor(path);
#endif
            }
            return LoadAssetBundle(path);
        }
#if UNITY_EDITOR
        /// <summary>
        /// 相对于Assets目录
        /// 需要文件的后缀名。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadObjectByPathOnEditor(string path)
        {
            ///打包名称
            ///移除打包后缀
            //path = path.Replace( Path.GetExtension(path),"");

            string assetPath = GameDataMgr.Instance.AssetPath + "/" + path;
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            objCacheMap.Add(path, obj);
            return obj;
        }
        public IEnumerator LoadObjectByPathOnEditor(string path, Action<UnityEngine.Object> OnLoad, Action<float> process = null)
        {
            ///打包名称
            ///移除打包后缀
            //path = path.Replace( Path.GetExtension(path),"");
            string assetPath = GameDataMgr.Instance.AssetPath + "/" + path;
            //return AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            //print(Time.realtimeSinceStartup);
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            //print(Time.realtimeSinceStartup);

            yield return obj;
            objCacheMap.Add(path,obj);
            if (OnLoad != null)
            {
                OnLoad(obj);
            }
            if (process != null)
            {
                process(1);
            }
        }
#endif
        /// <summary>
        /// 检查资源路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("路径为空");
                return null;
            }
            ///所有的相对路径都是小写
            path = path.ToLower();
            ///转换路径，保证路径格式一致
            path = path.Replace("\\", "/");
            if (!string.IsNullOrEmpty(GameDataMgr.Instance.ShowPath))
            {
                string[] split = GameDataMgr.Instance.ShowPath.Split('/');
                if (!path.StartsWith(split[0]))
                {
                    //print(path);
                    //path = GameDataMgr.Instance.ShowPath + "/" + path;
                }
            }
            return path;
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="OnLoad"></param>
        /// <param name="process"></param>
        public void LoadObjectSyn(string path, Action<UnityEngine.Object> OnLoad, Action<float> process = null)
        {
            if (string.IsNullOrEmpty(path = CheckPath(path)))
            {
                if (process != null)
                {
                    process(1);
                }
                if (OnLoad != null)
                {
                    OnLoad(null);
                }
                return;
            }
            bool isEdit = false;
            if (objCacheMap == null) objCacheMap = new Dictionary<string, UnityEngine.Object>();
            if (objCacheMap.ContainsKey(path) && objCacheMap[path] != null)
            {
                if (process != null)
                {
                    process(1);
                }
                if (OnLoad != null)
                {
                    OnLoad(objCacheMap[path]);
                }
                return;
            }
            if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
            {
#if UNITY_EDITOR
                //var tmpObj = LoadObjectByPathOnEditor(path);
                //if (process != null)
                //{
                //    process(1);
                //}
                //if (OnLoad != null)
                //{
                //    OnLoad(tmpObj);
                //}
                StartCoroutine(LoadObjectByPathOnEditor(path, OnLoad, process));
                isEdit = true;
#endif
            }
            if (!isEdit)
            {

                StartCoroutine(LoadAssetBundleSyn(path, OnLoad, process));
            }
        }

        /// <summary>
        /// 同步加载AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLadComplete"></param>
        /// <returns></returns>
        private UnityEngine.Object LoadAssetBundle(string path)
        {
            if (assetManifest == null)
            {
                LoadAssetManifest();
            }
            ///先加载依赖资源
            LoadRelyAssetBundle(path);

            ///最后加载主资源
            return LoadFromFile(path);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLadComplete"></param>
        /// <returns></returns>
        private IEnumerator LoadAssetBundleSyn(string path, Action<UnityEngine.Object> onLadComplete, Action<float> process = null)
        {
            if (objCacheMap == null) objCacheMap = new Dictionary<string, UnityEngine.Object>();
            if (objCacheMap.ContainsKey(path))
            {
                while (!objCacheMap[path])
                {
                    yield return new WaitForFixedUpdate();
                }
                if (process != null)
                {
                    process(1);
                }
                yield return objCacheMap[path];
                if (onLadComplete != null)
                {
                    onLadComplete(objCacheMap[path]);
                }
                yield break;
            }

            if (assetManifest == null)
            {
                LoadAssetManifest();
            }
            ///依赖资源是否加载完毕
            bool isLoadMain = false;
            StartCoroutine(LoadRelyAssetBundleSyn(path, () =>
             {
                 isLoadMain = true;
             }, process));
            ///当依赖资源没有加载完成时
            while (!isLoadMain)
            {
                yield return new WaitForFixedUpdate();
            }
            yield return isLoadMain;
            if (isLoadMain)
            {
                StartCoroutine(LoadFromFileAsync(path, onLadComplete));
                if (process != null)
                {
                    process(1f);
                }
            }
        }


        /// <summary>
        /// 加载AssetBundleManifest  资源索引对象，唯一
        /// </summary>
        public void LoadAssetManifest()
        {
            var mainAB = AssetBundle.LoadFromFile(GameDataMgr.Instance.AssetPath + "/" + GameDataMgr.Instance.buildConf.artPackagePath);
            assetManifest = mainAB.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            mainAB.Unload(false);
        }
        /// <summary>
        /// 同步加载依赖资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void LoadRelyAssetBundle(string path)
        {
            ///先获取依赖资源
            string[] depspath = assetManifest.GetAllDependencies(path);
            for (int i = 0; i < depspath.Length; i++)
            {
                string relyPath = depspath[i];
                LoadRelyAssetBundle(relyPath);
                LoadFromFile(relyPath);
            }
        }

        /// <summary>
        /// 加载依赖资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerator LoadRelyAssetBundleSyn(string path, Action OnLoadComplete, Action<float> process = null)
        {
            ///先获取依赖资源
            string[] depspath = assetManifest.GetAllDependencies(path);
            List<string> loadList = new List<string>();
            int count = 0;
            for (int i = 0; i < depspath.Length; i++)
            {
                int index = i;
                //string relyPath = depspath[index];
                ///先等待加载资源的依赖资源
                //yield return StartCoroutine( LoadRelyAssetBundleSyn(depspath[index],null));
                if (depspath.Length > maxLoadBundle)
                {
                    if (loadList.Count < maxLoadBundle)
                    {
                        if (!loadList.Contains(depspath[index]))
                        {
                            loadList.Add(depspath[index]);
                        }
                        StartCoroutine(LoadFromFileAsync(depspath[index], delegate (UnityEngine.Object obj)
                        {
                            if (loadList.Remove(depspath[index]))
                            {
                                if (process != null)
                                {
                                    process(count * 1.0f / depspath.Length);
                                }
                                count++;
                            }
                            else
                            {
                                Debug.LogError(depspath[index]);
                            }
                        }));
                        while (loadList.Count >= maxLoadBundle)
                        {
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
                else
                {
                    if (!loadList.Contains(depspath[index]))
                    {
                        loadList.Add(depspath[index]);
                    }
                    StartCoroutine(LoadFromFileAsync(depspath[index], delegate (UnityEngine.Object obj)
                    {
                        if (loadList.Remove(depspath[index]))
                        {
                            if (process != null)
                            {
                                process(count * 1.0f / depspath.Length);
                            }
                            count++;
                        }
                        else
                        {
                            Debug.LogError(depspath[index]);
                        }
                    }));
                }
            }
            while (loadList.Count > 0)
            {
                yield return new WaitForFixedUpdate();
            }
            if (OnLoadComplete != null)
            {
                OnLoadComplete();
            }
            if (process != null)
            {
                process(depspath.Length * 1f / (depspath.Length + 1));
            }
        }

        /// <summary>
        /// 加载依赖资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void UnLoadRelyAssetBundleSyn(string path, bool isForceDel = false)
        {
            ///先获取依赖资源
            if (assetManifest)
            {
                string[] depspath = assetManifest.GetAllDependencies(path);
                for (int i = 0; i < depspath.Length; i++)
                {
                    int index = i;
                    UnLoadFromFile(depspath[index], isForceDel);
                }
            }
        }
        public void DestroyObject(string path)
        {
            if (objCacheMap.ContainsKey(path))
            {
                var obj = objCacheMap[path];
                Destroy(obj);
            }
        }

        /// <summary>
        /// 同步加载obj  AssetBundle.LoadFromFile
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (objCacheMap.ContainsKey(path))
            {
                return objCacheMap[path];
            }
            if (bundleCacheMap == null) bundleCacheMap = new Dictionary<string, AssetBundle>();
            AssetBundle ab = null;// req.assetBundle;
            if (bundleCacheMap.ContainsKey(path))
            {
                ab = bundleCacheMap[path];
            }
            else
            {
                ab = AssetBundle.LoadFromFile(GameDataMgr.Instance.AssetPath + "/" + path);
                bundleCacheMap.Add(path, ab);
            }
            string[] depPaths = path.Split('/');
            string depPath = depPaths[depPaths.Length - 1];
            string extens = "";
            if (!string.IsNullOrEmpty(Path.GetExtension(depPath)))
            {
                extens = Path.GetExtension(depPath);
            }
            string tmpPath = depPath.Replace(extens, "");
            var tmp = ab.LoadAsset(tmpPath);
            ///将缓存列表的对象替换
            objCacheMap.Add(path, tmp);
            return tmp;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public IEnumerator LoadFromFileAsync(string path, Action<UnityEngine.Object> onLoad)
        {
            if (string.IsNullOrEmpty(path))
            {
                onLoad(null);
                yield break;
            }
            if (objCacheMap.ContainsKey(path))
            {
                while (!objCacheMap[path])
                {
                    yield return new WaitForFixedUpdate();
                }
                yield return objCacheMap[path];
                if (onLoad != null)
                {
                    onLoad(objCacheMap[path]);
                }
                yield break;
            }
            else
            {
                objCacheMap.Add(path, null);
            }

            if (bundleCacheMap == null) bundleCacheMap = new Dictionary<string, AssetBundle>();
            AssetBundle ab = null;// req.assetBundle;

            if (bundleCacheMap.ContainsKey(path))
            {
                while (!bundleCacheMap[path])
                {
                    yield return new WaitForFixedUpdate();
                }
                ab = bundleCacheMap[path];
            }
            else
            {
                bundleCacheMap.Add(path, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(GameDataMgr.Instance.AssetPath + "/" + path);
                yield return req;

                ab = req.assetBundle;
                bundleCacheMap[path] = ab;
            }
            yield return ab;
            string[] depPaths = path.Split('/');
            string depPath = depPaths[depPaths.Length - 1];
            string extens = "";
            if (!string.IsNullOrEmpty(Path.GetExtension(depPath)))
            {
                extens = Path.GetExtension(depPath);
            }
            string tmpPath = depPath.Replace(extens, "");
            var tmp = ab.LoadAsset(tmpPath);
            ///将缓存列表的对象替换
            objCacheMap[path] = tmp;
            if (onLoad != null)
            {
                onLoad(tmp);
            }
        }
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public void UnLoadFromFile(string path, bool isForce = true)
        {
            //if (objCacheMap != null)
            //{
            //    if (objCacheMap.ContainsKey(path))
            //    {
            //        objCacheMap.Remove(path);
            //    }
            //}
            if (bundleCacheMap != null)
            {
                if (bundleCacheMap.ContainsKey(path))
                {
                    bundleCacheMap[path].Unload(isForce);
                    bundleCacheMap.Remove(path);
                }
            }
        }


        /// <summary>
        /// 卸载所有未使用的资源
        /// 不常用,因无法判断具体资源的引用情况，不建议使用，仅仅封装AssetBundle
        /// 在不明确场景中已加载的对象前，不建议使用
        /// </summary>
        public void ClearUnUseBundles()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }
        /// <summary>
        /// 清除所有资源，场景退出时使用
        /// </summary>
        public void ClearBundles()
        {
            StopAllCoroutines();
            if (objCacheMap != null)
            {
                objCacheMap.Clear();
            }
            objCacheMap = null;
            if (bundleCacheMap != null)
            {
                bundleCacheMap.Clear();
            }
            bundleCacheMap = null;
            AssetBundle.UnloadAllAssetBundles(true);
#if UNITY_EDITOR
            //AssetDatabase.ReleaseCachedFileHandles();
            //AssetDatabase.Refresh();
#endif
        }
        /// <summary>
        /// 异步卸载指定资源
        /// 内存异常，已禁用
        /// </summary>
        /// <param name="path"></param>
        public void UnLoadObjectSyn(string path, bool isForceDel = true, Action onUnLoad = null)
        {
            StartCoroutine(UnLoadBundle(path, isForceDel, onUnLoad));
        }
        /// <summary>
        /// 同步卸载资源
        /// 内存释放异常，已禁用
        /// </summary>
        /// <param name="path"></param>
        public void UnLoadObject(string path, bool isForeceDel = true)
        {
            //if (objCacheMap != null)
            //{
            //    if (objCacheMap.ContainsKey(path))
            //    {
            //        objCacheMap.Remove(path);
            //    }
            //}
            ///清除AssetBundle缓存并将其卸载
            if (bundleCacheMap != null)
            {
                if (bundleCacheMap.ContainsKey(path))
                {
                    bundleCacheMap[path].Unload(isForeceDel);
                    bundleCacheMap.Remove(path);
                }
            }
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerator UnLoadBundle(string path, bool isForceDel = true, Action onUnLoad = null)
        {
            yield return new WaitForFixedUpdate();
            ///先将对象缓存清除
            UnLoadFromFile(path, isForceDel);
            UnLoadRelyAssetBundleSyn(path, isForceDel);
            yield return new WaitForFixedUpdate();
            if (onUnLoad != null)
            {
                onUnLoad();
            }
        }

    }
}