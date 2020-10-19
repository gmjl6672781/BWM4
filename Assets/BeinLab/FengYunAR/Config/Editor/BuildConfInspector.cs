using BeinLab.Util;
using UnityEditor;
using UnityEngine;
namespace BeinLab.CarShow.Modus
{
    [InitializeOnLoad]
    [CustomEditor(typeof(BuildConfig))]
    public class BuildConfInspector : Editor
    {
        /// 编译的平台
        public SerializedProperty buildTarget;

        // 资源访问的方式
        public SerializedProperty buildType;

        public SerializedProperty buildTargetGroup;
        public string IOSDef = "IOS_APP";
        public string AndroidDef = "Android_APP";


        private void OnEnable()
        {
            buildTarget = serializedObject.FindProperty("buildTarget");
            buildType = serializedObject.FindProperty("buildType");
            buildTargetGroup = serializedObject.FindProperty("buildTargetGroup");
        }
        /// <summary>
        /// 当程序完成编译时，读取当前的版本号，然后自动
        /// </summary>
        //[UnityEditor.Callbacks.DidReloadScripts]
        //private static void AllScriptsReloaded()
        //{
        //    BuildConfig.IsUpdate = true;
        //    BuildConfig.curTime = UnityUtil.GetTime();
        //}

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            var conf = target as BuildConfig;
            if (conf.buildTarget == BuildPlatform.Android)
            {
                conf.buildTargetGroup = BuildTargetGroup.Android;
            }
            if (conf.buildTarget == BuildPlatform.iOS)
            {
                conf.buildTargetGroup = BuildTargetGroup.iOS;
            }
            if (conf.buildTarget == BuildPlatform.StandaloneWindows)
            {
                conf.buildTargetGroup = BuildTargetGroup.Standalone;
            }

            EditorGUILayout.PropertyField(buildTarget, new GUIContent("运行平台"));
            EditorGUILayout.PropertyField(buildType, new GUIContent("资源访问方式"));
            EditorGUILayout.PropertyField(buildTargetGroup, new GUIContent("打包平台"));
            conf.version = EditorGUILayout.TextField("版本号", conf.version);
            conf.projectName = EditorGUILayout.TextField("项目名称", conf.projectName);
            conf.artPackagePath = EditorGUILayout.TextField("打包名称", conf.artPackagePath);
            conf.artSuffix = EditorGUILayout.TextField("美术包后缀", conf.artSuffix);
            //conf.WebDataPath = EditorGUILayout.TextField("服务器地址", conf.WebDataPath);
            //conf.serverURL = EditorGUILayout.TextField("资源服务器URL", conf.serverURL);
            conf.versionPath = EditorGUILayout.TextField("缓存版本信息", conf.versionPath);
            //conf.serverPath = EditorGUILayout.TextField("服务器路径", conf.serverPath);
            //conf.publicArt = EditorGUILayout.TextField("公共美术路径", conf.publicArt);
            //conf.mainPath = EditorGUILayout.TextField("车型列表", conf.mainPath);
            conf.maxDownCount = EditorGUILayout.IntField("同时下载的最大数量", conf.maxDownCount);
            conf.editorURL = EditorGUILayout.TextField("本地数据URL", conf.editorURL);
            conf.fps = EditorGUILayout.IntField("同时下载的最大数量", conf.fps);

            //if (BuildConfig.IsUpdate)
            //{
            //    BuildConfig.IsUpdate = false;
            //    if (!EditorApplication.isPlaying)
            //    {
            //        conf.version = BuildConfig.curTime;
            //    }
            //}
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                if (conf.buildType == BulidType.App)
                {
                    if (conf.buildTarget == BuildPlatform.iOS)
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, IOSDef);
                    }
                    else if (conf.buildTarget == BuildPlatform.Android)
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, AndroidDef);
                    }
                }
                else
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(conf.buildTargetGroup, "");
                }
            }
        }
    }
}