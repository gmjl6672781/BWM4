using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.Util
{
    /// <summary>
    /// 资源读取方式
    /// 编辑器读取，读取源文件，相对于Assets目录
    /// 本地编辑器读取,资源来自打包的平台
    /// 本地真机读取
    /// APP合并读取
    /// </summary>
    public enum BulidType
    {
        /// <summary>
        /// 编辑器模式下，读取编辑器Asset下资源
        /// </summary>
        Editor = -1,
        /// <summary>
        /// 编辑器模式下读取指定路径的打包后的资源
        /// </summary>
        LocalEditor = 0,
        /// <summary>
        /// 真机运行下读取游戏路径下的资源
        /// </summary>
        LocalPhone = 1,
        /// <summary>
        /// 真机运行下读取指定路径下的资源
        /// </summary>
        App = 2,
        /// <summary>
        /// 读取StreamingAsset路径下资源
        /// </summary>
        Stream = 3
    }

    /// <summary>
    /// 编译的平台
    /// 在打包的时候要选择对应的平台
    /// </summary>
    public enum BuildPlatform
    {
        StandaloneWindows = 0,
        iOS = 1,
        Android = 2
    }

    /// <summary>
    /// 打包的配置
    /// 配置发布的平台IOS windows android等
    /// 配置资源读取的方式，本地编辑器读取，本地真机读取，APP合并读取
    /// 
    /// </summary>
    public class BuildConfig : ScriptableObject
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName = "SIXECO_AR";
        /// 编译的平台
        /// StandaloneWindows=0,
        /// IOS=1,
        /// Android=2
        public BuildPlatform buildTarget = BuildPlatform.StandaloneWindows;
        /// <summary>
        /// 资源读取方式
        /// LocalEditor = 0,
        /// LocalPhone = 1,
        /// App = 2
        public BulidType buildType = BulidType.LocalEditor;

        public string artPackagePath;
        public string artSuffix = "_BeinLab";
#if UNITY_EDITOR
        public BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;
#endif
        public string version = "";
        /// <summary>
        /// 服务器资源根路径
        /// </summary>
        //public string WebDataPath = "http://10.2.97.85:8080/ARProject";
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        //public string serverURL = "http://10.2.97.85:8080/ARProject";
        //public string serverPath = "ServerVisionConf.xml";
        //public string mainPath = "fengyunar/c_100_carlist";
        //public string publicArt = "fengyunar/d_100_publicart";
        /// <summary>
        /// xml文件名称
        /// </summary>
        public string versionPath = "AssetsVersionConf.xml";
        public int maxDownCount = 10;
        /// <summary>
        /// 是否更新程序版本号
        /// </summary>
        //public static bool IsUpdate = false;
        /// <summary>
        /// 
        /// </summary>
        public static string curTime;
        public string editorURL;
        public int fps=60;

        /// <summary>
        /// 获取美术包相对路径
        /// 项目名称+打包平台+美术包名称，例如FengYunAR
        /// </summary>
        public string GamePath
        {
            get
            {
                string path = projectName + "/" + buildTarget.ToString() + "/" + artPackagePath;
                return path;
            }
        }
    }
}