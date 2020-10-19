using BeinLab.Util;
using Karler.Lib.Data;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 打包管理器
/// 1，优化资源打包的交互方式，由单个按钮变为可视化窗体，容易操作同时不易出错
/// 2，优化资源打包的方式，加入合集打包，以及合并打包的方法，大大提高打包的速度，明确打包的资源，同时将减小包体资源的大小，提升加载速度
/// 3，选择文件夹时，显示打包的素材总数，同时给出列表显示
/// 4，指定式打包，合并式打包
/// 
/// 打包的要点：分为散式打包和合集打包。
/// 散式打包，将多次被引用的资源也打成包，优点是组合方便，
/// 集合打包：将引用资源类似的资源打成一个包。优点是减少打包资源的数量，提高加载数据的数量，缺点是单个
/// 
/// 一个资源被多个对象引用，则此资源要加入到打包列表中，例如CHR的车模型，同时被XRay和车辆展示引用，还有环境贴图，被多个材质球引用，环境贴图要打到资源包中
/// 资源合集何时使用：多个对象打到一个包中的条件：
/// 1，需要同时加载的，例如汽车的默认材质合集，帧序列动画，要打到一个包中去
/// 2，多个资源引用的资源类似时，合成一个包。例如CHR 的XRay和CHR本体引用的资源类似（模型），这个时候可以打成一个包。多个反光材质球引用的资源（环境贴图）
/// ，shader，资源贴图等等，非常类似。可以打成一个包。
/// 3，
/// </summary>
namespace BeinLab
{
    public class BuilderWindow : EditorWindow
    {
        /// <summary>
        /// 是否压缩  默认不压缩
        /// </summary>
        public BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
        private bool isFllowBuildTarget;

        /// <summary>
        /// 打包的平台
        /// </summary>
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        /// <summary>
        /// 根路径，相对于项目与Assets同级目录
        /// </summary>
        public string buildRootPath = "OtherPackage";
        public bool isAssetPath = true;
        public string rootBuildPath = "";
        /// <summary>
        /// 打包的素材路径
        /// </summary>
        public static string buildArt;
        /// <summary>
        /// 资源打包时忽略的路径
        /// 以|或者空格分割
        /// </summary>
        public string ignoreDirName = "LocalArt";
        /// <summary>
        /// 资源打包时忽略的文件后缀名
        /// 以|或者空格分割
        /// </summary>
        public string ignoreFileName = ".meta";

        /// <summary>
        /// 资源打包时需要合并的路径名称
        /// 以|或者空格分割
        /// </summary>
        public string mergeDir = "BaleArt";
        public string suffix = ".unity3d";
        /// <summary>
        /// 是否开启自动后缀
        /// 如果开启，将以目标文件的后缀名作为文件名
        /// </summary>
        public bool isAutoSuffix = false;
        /// <summary>
        /// 方便复制，不参与计算
        /// </summary>
        public string buildPath;
        private int labelCount = 0;
        private bool isShowRes;
        private bool isAutoPath;
        private char[] splits = new char[] { '|', ' ' };
        private string assetView;
        private Dictionary<string, List<string>> assetLabelDic;
        private Dictionary<string, List<string>> assetNameDic;
        private int selectIndex = 0;
        private Vector2 scrollPos;
        /// <summary>
        /// 已弃用
        /// </summary>
        [Tooltip("已弃用")]
        private bool isAutoDependen = false;
        public BuildConfig buildConf;
        //public AssetsVersionConf versionConf;
        /// <summary>
        /// 用来存储
        /// </summary>
        /// 
        [MenuItem("Tools/AssetBundle/BuildWindow &B")]
        static void BuildWindow()
        {
            BuilderWindow bw = EditorWindow.GetWindow(typeof(BuilderWindow), false, "打包管理器", true) as BuilderWindow;
            bw.ShowPopup();
            bw.autoRepaintOnSceneChange = true;
        }
        private void OnEnable()
        {
            OnProjectChange();
            AssetFlash.OnFlash += FlashAsset;
        }

        private void FlashAsset()
        {
            Repaint();
        }

        private void OnDisable()
        {
            AssetFlash.OnFlash -= FlashAsset;
        }

        private void Awake()
        {
            OnProjectChange();
            AutoSetPath();
        }
        public void AddAssetLabel(string path, string asset)
        {
            path = path.ToLower();
            if (assetLabelDic == null)
            {
                assetLabelDic = new Dictionary<string, List<string>>();
            }

            List<string> assetList = null;
            if (assetLabelDic.ContainsKey(path))
            {
                assetList = assetLabelDic[path];
            }
            if (assetList == null) assetList = new List<string>();

            if (!assetList.Contains(asset))
            {
                assetList.Add(asset);
            }
            if (!assetLabelDic.ContainsKey(path))
            {
                assetLabelDic.Add(path, assetList);
            }

            // assetLabelDic[path] = assetList;
        }
        public void AddAssetName(string label, string assetName)
        {
            if (assetNameDic == null)
            {
                assetNameDic = new Dictionary<string, List<string>>();
            }

            List<string> assetList = null;
            if (assetNameDic.ContainsKey(label))
            {
                assetList = assetNameDic[label];
            }
            if (assetList == null) assetList = new List<string>();

            if (!assetList.Contains(assetName))
            {
                assetList.Add(assetName);
            }
            if (!assetNameDic.ContainsKey(label))
            {
                assetNameDic.Add(label, assetList);
            }

            // assetLabelDic[path] = assetList;
        }
        /// <summary>  
        /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
        /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
        /// </summary>  
        static void ClearAssetBundlesName()
        {
            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
            length = AssetDatabase.GetAllAssetBundleNames().Length;
        }

        private void Director(string dir)
        {

            DirectoryInfo d = new DirectoryInfo(dir);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();

            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)     //判断是否为文件夹
                {
                    if (!IsIgnoreDir(fsinfo.Name))
                        Director(fsinfo.FullName);//递归调用
                }
                else
                {
                    string full = fsinfo.FullName;
                    if (!IsIgnoreFile(full))
                    {
                        AssetsVersionConf avc = new AssetsVersionConf();
                        full = full.Replace("\\", "/").Replace(buildPath + "/", "");
                        avc.PriKey = full.Replace("/", "..");
                        avc.MD5 = UnityUtil.GetMD5HashFromFile(fsinfo.FullName);
                        FileInfo file = new FileInfo(fsinfo.FullName);
                        avc.Size = file.Length;
                        MySql.Insert(avc, false, "PriKey", "Version");
                    }
                }

            }
        }

        public void PackChild(string dir)
        {
            DirectoryInfo d = new DirectoryInfo(dir);

            FileInfo[] fs = d.GetFiles();
            MySql.WorkPath = dir;
            if (MySql.Open<AssetsVersionConf>())
            {
                for (int i = 0; i < fs.Length; i++)
                {
                    string full = fs[i].FullName;
                    if (!IsIgnoreFile(full))
                    {
                        AssetsVersionConf avc = new AssetsVersionConf();
                        full = full.Replace("\\", "/").Replace(buildPath + "/", "");
                        avc.PriKey = full.Replace("/", "..");
                        avc.MD5 = UnityUtil.GetMD5HashFromFile(fs[i].FullName);
                        FileInfo file = new FileInfo(fs[i].FullName);
                        avc.Size = file.Length;
                        MySql.Insert(avc, false, "PriKey", "Version");
                    }
                }
            }
            MySql.Close();
            DirectoryInfo[] dis = d.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                if (!IsIgnoreDir(dis[i].Name))
                {
                    var childs = dis[i].GetDirectories();
                    for (int j = 0; j < childs.Length; j++)
                    {
                        if (!IsIgnoreDir(childs[j].Name))
                        {
                            string path = childs[j].FullName;
                            MySql.WorkPath = path;
                            MySql.Open<AssetsVersionConf>();
                            Director(path);
                            MySql.Close();
                        }
                    }
                }
            }
        }


        public void RePackChild(string dir)
        {
            UnityUtil.DelXMLData<AssetsVersionConf>(dir);
            DirectoryInfo d = new DirectoryInfo(dir);
            DirectoryInfo[] dis = d.GetDirectories();
            for (int i = 0; i < dis.Length; i++)
            {
                if (!IsIgnoreDir(dis[i].Name))
                {
                    var childs = dis[i].GetDirectories();
                    for (int j = 0; j < childs.Length; j++)
                    {
                        if (!IsIgnoreDir(childs[j].Name))
                        {
                            UnityUtil.DelXMLData<AssetsVersionConf>(childs[j].FullName);
                        }
                    }
                }
            }
        }

        private void OnGUI()
        {
            buildConf = EditorGUI.ObjectField(new Rect(0, 5, 600, 20), "BuildSetting", buildConf, typeof(BuildConfig), true) as BuildConfig;

            scrollPos = GUI.BeginScrollView(new Rect(0, 30, position.width, position.height),
           scrollPos, new Rect(0, 0, 1920, 2160));

            //buildConf = EditorGUILayout.ObjectField(buildConf, typeof(BuildConfig), null) as BuildConfig;
            //buildConf = EditorGUI.ObjectField(new Rect(0,0,600,30), buildConf, typeof(BuildConfig), true) as BuildConfig;
            //versionConf = EditorGUILayout.ObjectField(versionConf, typeof(AssetsVersionConf), null) as AssetsVersionConf;
            buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("压缩方式", buildAssetBundleOptions);
            isFllowBuildTarget = EditorGUILayout.BeginToggleGroup("平台一致", isFllowBuildTarget);
            if (isFllowBuildTarget)
            {
                if (buildConf)
                {
                    if (buildConf.buildTarget == BuildPlatform.Android)
                    {
                        buildTarget = BuildTarget.Android;
                    }
                    if (buildConf.buildTarget == BuildPlatform.iOS)
                    {
                        buildTarget = BuildTarget.iOS;
                    }
                    if (buildConf.buildTarget == BuildPlatform.StandaloneWindows)
                    {
                        buildTarget = BuildTarget.StandaloneWindows;
                    }
                }
                EditorGUILayout.LabelField("目标平台 :", buildTarget.ToString());
            }
            EditorGUILayout.EndToggleGroup();
            if (!isFllowBuildTarget)
            {
                buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台", buildTarget);
            }

            buildArt = EditorGUILayout.TextField("素材路径", buildArt);
            if (AssetFlash.assetPathList != null)
            {
                int select = EditorGUILayout.Popup("打包素材", selectIndex, AssetFlash.assetPathList.ToArray());
                if (select != selectIndex || string.IsNullOrEmpty(buildArt))
                {
                    selectIndex = select;
                    buildArt = AssetFlash.assetPathList[selectIndex];
                    if (buildConf)
                    {
                        buildConf.artPackagePath = buildArt + buildConf.artSuffix;
                    }
                }
            }

            isAutoDependen = EditorGUILayout.ToggleLeft("自动依赖(已弃用)", false);
            isAssetPath = EditorGUILayout.ToggleLeft("是否相对路径", isAssetPath);
            if (!isAssetPath)
            {
                rootBuildPath = EditorGUILayout.TextField("打包路径", rootBuildPath);
            }
            else
            {
                string path = Application.dataPath;
                path = path.Substring(0, path.LastIndexOf("/"));
                rootBuildPath = path;
                buildConf.editorURL = Path.Combine(rootBuildPath, buildRootPath)+"/ServerVisionConf.xml";
            }
            buildRootPath = EditorGUILayout.TextField("相对路径", buildRootPath);
            ignoreDirName = EditorGUILayout.TextField("忽略路径", ignoreDirName);
            ignoreFileName = EditorGUILayout.TextField("忽略文件", ignoreFileName);
            mergeDir = EditorGUILayout.TextField("合集路径", mergeDir);
            AutoSetPath();
            buildPath = EditorGUILayout.TextField("输出路径", buildPath);
            suffix = EditorGUILayout.TextField("后缀名", suffix);
            //artSuffix = EditorGUILayout.TextField("美术包后缀", artSuffix);
            isAutoSuffix = EditorGUILayout.ToggleLeft("自动后缀", isAutoSuffix);
            isAutoPath = EditorGUILayout.ToggleLeft("自动路径", isAutoPath);
            ///当前标签的数量
            labelCount = AssetDatabase.GetAllAssetBundleNames().Length;
            //GUILayout.Label("待打包素材:   " + buildArt);
            GUILayout.Label("总标签数量" + labelCount);

            if (assetLabelDic != null && assetLabelDic.ContainsKey(buildArt.ToLower()))
            {
                GUILayout.Label(buildArt + " 标签数量:" + assetLabelDic[buildArt.ToLower()].Count);
            }
            isShowRes = EditorGUILayout.BeginToggleGroup("资源列表预览", isShowRes);

            if (isShowRes)
            {
                EditorGUILayout.TextArea(assetView);
            }

            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("添加默认服务器", GUILayout.Width(100)))
            {
                ServerVisionConf svc = new ServerVisionConf();
                svc.PriKey = "Sixeco_BeinLab_LocalTest";
                string sPath = rootBuildPath;
                if (isAssetPath)
                {
                    sPath = Path.Combine(rootBuildPath, buildRootPath);
                }
                svc.Server = sPath;
                UnityUtil.WriteXMLData<ServerVisionConf>(sPath, svc);
            }
            if (GUILayout.Button("刷新APP版本号", GUILayout.Width(100)))
            {
                if (buildConf)
                {
                    buildConf.version = UnityUtil.GetTime();
                    string sPath = rootBuildPath;
                    if (isAssetPath)
                    {
                        sPath = Path.Combine(rootBuildPath, buildRootPath);
                    }
                    List<ServerVisionConf> serverList = UnityUtil.ReadXMLData<ServerVisionConf>(sPath);

                    if (serverList != null)
                    {
                        MySql.WorkPath = sPath;
                        bool isOpen = MySql.Open<ServerVisionConf>();
                        if (isOpen)
                        {
                            for (int i = 0; i < serverList.Count; i++)
                            {
                                if (buildConf.buildTarget == BuildPlatform.iOS)
                                {
                                    serverList[i].IOSVersion = buildConf.version;
                                }
                                else if (buildConf.buildTarget == BuildPlatform.Android)
                                {
                                    serverList[i].Androidversion = buildConf.version;
                                }
                                MySql.Update<ServerVisionConf>(serverList[i]);
                            }
                        }
                        MySql.Close();
                    }
                }
            }
            ///刷新指定路径下的所有资源的标签
            if (GUILayout.Button("刷新标签", GUILayout.Width(100)))
            {
                ReFreshArt();
            }

            ///刷新指定路径下的所有资源的标签
            if (GUILayout.Button("清空标签", GUILayout.Width(100)))
            {
                ClearAssetBundlesName();
            }

            ///刷新指定路径下的所有资源的标签
            if (GUILayout.Button("清除输出路径", GUILayout.Width(100)))
            {
                Directory.Delete(buildPath, true);
            }
            if (GUILayout.Button("刷新MD5", GUILayout.Width(80), GUILayout.Height(20)))
            {
                PackChild(buildPath);
                //MySql.WorkPath = buildPath;
                //MySql.Open<AssetsVersionConf>();
                //Director(buildPath);
                //MySql.Close();
            }
            if (GUILayout.Button("清除MD5", GUILayout.Width(80), GUILayout.Height(20)))
            {
                RePackChild(buildPath);
            }

            ///指定式打包
            ///从字典中获取对应的资源然后打包输出
            if (GUILayout.Button("打包", GUILayout.Width(80), GUILayout.Height(20)))
            {
                if (isAutoDependen)
                {
                    //if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);
                    //BuildPipeline.BuildAssetBundles(buildPath, buildAssetBundleOptions, buildTarget);
                    //AssetDatabase.Refresh();
                    //return;
                }
                if (assetLabelDic.ContainsKey(buildArt.ToLower()))
                {
                    List<string> assetLabels = assetLabelDic[buildArt.ToLower()];
                    if (assetLabels.Count > 0)
                    {
                        AssetBundleBuild[] abs = new AssetBundleBuild[assetLabels.Count];
                        for (int i = 0; i < assetLabels.Count; i++)
                        {
                            AssetBundleBuild ab = new AssetBundleBuild();
                            ab.assetBundleName = assetLabels[i];
                            //string[] names = assetNameDic[assetLabels[i]].ToArray();
                            ab.assetNames = assetNameDic[assetLabels[i]].ToArray();
                            abs[i] = ab;
                        }
                        if (abs != null)
                        {
                            if (!Directory.Exists(buildPath)) Directory.CreateDirectory(buildPath);

                            AssetDatabase.Refresh();
                            BuildPipeline.BuildAssetBundles(buildPath, abs, buildAssetBundleOptions, buildTarget);
                            AssetDatabase.Refresh();

                            ///打包之后更新MD5值
                            //UnityUtil.DelXMLData<AssetsVersionConf>(buildPath);
                            PackChild(buildPath);
                            //MySql.WorkPath = buildPath;
                            //MySql.Open<AssetsVersionConf>();
                            //Director(buildPath);
                            //MySql.Close();
                        }
                        else
                        {
                            Debug.LogError("所选资源列表为空");
                        }
                    }
                    else
                    {
                        Debug.LogWarning(buildArt + "资源不存在打包标签，请刷新一下再做尝试");
                    }
                }
                else
                {
                    Debug.LogWarning(buildArt + "资源不存在，请刷新尝试");
                }
            }
            GUI.EndScrollView();
        }

        /// <summary>
        /// 自动刷新打包资源的路径
        /// 
        /// </summary>
        private void AutoSetPath()
        {
            if (isAutoPath)
            {

                if (buildConf && !string.IsNullOrEmpty(buildConf.projectName))
                {
                    buildPath = Path.Combine(rootBuildPath, buildRootPath + "/" + buildConf.projectName + "/" + buildTarget.ToString()) + "/" + buildArt + buildConf.artSuffix;
                }
                else
                {
                    buildPath = Path.Combine(rootBuildPath, buildRootPath + "/" + buildTarget.ToString()) + "/" + buildArt;
                }
                buildPath = buildPath.Replace("\\", "/");
            }
        }



        /// <summary>
        /// 仅支持Assets路径下的直接文件夹打包
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsRootPath(string path)
        {
            if (AssetFlash.assetPathList != null)
            {
                for (int i = 0; i < AssetFlash.assetPathList.Count; i++)
                {
                    if (AssetFlash.assetPathList[i] == path)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 刷新素材的标签
        /// 获取当前激活对象的素材，同时刷新
        /// 
        /// 刷新字典
        /// </summary>
        private void ReFreshArt()
        {
            string bpath = buildArt.ToLower();
            if (assetLabelDic == null) assetLabelDic = new Dictionary<string, List<string>>();
            List<string> assetList = new List<string>();
            if (assetLabelDic.ContainsKey(bpath))
            {
                assetLabelDic[bpath].Clear();
            }
            else
            {
                assetLabelDic.Add(bpath, assetList);
            }

            ///资源的根路径
            string artRootPath = Application.dataPath + "/" + buildArt;
            SetAssetLabel(artRootPath);
            AssetDatabase.RemoveUnusedAssetBundleNames();
            ///设置完成之后要刷新
            AssetDatabase.Refresh();
            Debug.Log(buildArt + "标签：" + assetLabelDic[bpath].Count);
            assetList = assetLabelDic[bpath];
            assetView = "";
            for (int i = 0; i < assetList.Count; i++)
            {
                assetView += assetList[i] + "\n";
            }
        }

        /// <summary>
        /// 设置路径名
        /// </summary>
        /// <param name="source"></param>
        private void SetAssetLabel(string source)
        {
            DirectoryInfo folder = new DirectoryInfo(source);
            if (IsIgnoreDir(folder.Name)) return;
            FileSystemInfo[] files = folder.GetFileSystemInfos();
            int length = files.Length;
            for (int i = 0; i < length; i++)
            {
                if (files[i] is DirectoryInfo)
                {
                    SetAssetLabel(files[i].FullName);
                }
                else
                {
                    if (!IsIgnoreFile(files[i].Name))
                    {
                        string str = CountAssetLabel(files[i].FullName);
                        if (!string.IsNullOrEmpty(str))
                        {
                            AddAssetLabel(buildArt, str);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据文件名计算标签
        /// 注意，要将合集中的资源打包到一起，即共用一个标签
        /// 
        /// 从相对于Asset的路径开始标记
        /// </summary>
        /// <param name="fullName">资源的全称路径</param>
        private string CountAssetLabel(string source)
        {
            string _source = source.Replace("\\", "/");
            string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
            string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
            //Debug.Log (_assetPath);  

            //在代码中给资源设置AssetBundleName  
            AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
            if (assetImporter != null)
            {
                string assetName = GetAssetName(_assetPath2);
                if (!assetName.EndsWith(suffix))
                {
                    assetName += suffix;
                }
                //Debug.Log (assetName);  
                assetImporter.assetBundleName = assetName;
                AddAssetName(assetName, _assetPath);
                return assetName;
            }
            else
            {
                Debug.LogError(_assetPath + " Is Null!");
                return null;
            }
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private string GetAssetName(string assetPath)
        {
            if (!string.IsNullOrEmpty(mergeDir))
            {
                string[] merges = mergeDir.Split(splits);
                if (merges != null)
                {
                    for (int i = 0; i < merges.Length; i++)
                    {
                        if (assetPath.Contains(merges[i]))
                        {
                            string assName = assetPath.Substring(0, assetPath.IndexOf(merges[i]) + merges[i].Length);
                            return assName;
                        }
                    }
                }
            }
            if (isAutoSuffix)
            {
                suffix = Path.GetExtension(assetPath);
            }
            return assetPath.Replace(Path.GetExtension(assetPath), "");
        }

        /// <summary>
        /// 忽略的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool IsIgnoreFile(string fileName)
        {
            if (!string.IsNullOrEmpty(ignoreFileName))
            {
                string[] ignFiles = ignoreFileName.Split(splits);
                if (ignFiles != null)
                {
                    for (int i = 0; i < ignFiles.Length; i++)
                    {
                        if (fileName.EndsWith(ignFiles[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为忽略的路径名
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        private bool IsIgnoreDir(string dirName)
        {
            if (!string.IsNullOrEmpty(ignoreDirName))
            {
                string[] igndirs = ignoreDirName.Split(splits);
                if (igndirs != null)
                {
                    for (int i = 0; i < igndirs.Length; i++)
                    {
                        if (igndirs[i] == dirName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 当资源发生变化时
        /// 刷新根资源路径
        /// 大小写与Project一致
        /// 移除没有使用的标签
        /// 刷新项目
        /// </summary>
        private void OnProjectChange()
        {
            AssetFlash.ChangeProject();
            Repaint();
        }
    }
}