using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modus
{
    public class ToggleActionConf : ScriptableObject
    {
        /// <summary>
        ///图标
        /// </summary>
        public Sprite openIcon;
        public Sprite offIcon;
        /// <summary>
        /// 开关事件
        /// </summary>
        public ActionConf openAction;
        public ActionConf offAction;
        /// <summary>
        /// 在退出时是否要执行关闭按钮
        /// </summary>
        public bool isOffOnDel = true;
        /// <summary>
        /// 在初始化时是否执行当前状态的效果
        /// </summary>
        public bool isDoOnActive = false;
        /// <summary>
        /// 默认状态
        /// </summary>
        public bool toggleIsOn = false;
        public string toggleName = "Toggle";
        public bool isCacheLastSelect = false;
        public bool cacheState = false;
        
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/ToggleActionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ToggleActionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ToggleActionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ToggleActionConf) + " is null");
                }
            }
        }
#endif
    }
}