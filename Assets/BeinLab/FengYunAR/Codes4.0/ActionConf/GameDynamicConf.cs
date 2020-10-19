using BeinLab.Util;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 游戏动效配置
    /// 创建对象的动效对象
    /// 具体执行什么动效，由动效组件控制
    /// </summary>
    [SerializeField]
    public class GameDynamicConf : ScriptableObject
    {
        public NodeConf nodeConf;
        /// <summary>
        /// 动效的名称，具有唯一性
        /// </summary>
        public string dynamicName;
        /// <summary>
        /// 预制体
        /// </summary>
        [Tooltip("预制体的路径")]
        public string prefabPath;
        /// <summary>
        /// 是否异步加载,默认直接加载
        /// </summary>
        [Tooltip("是否异步加载，默认直接加载")]
        public bool isLoadSyn = false;
        [Tooltip("当系统重置时是否要删除此对象")]
        public bool isDelOnReset = true;
        public Object localPrefab;
        public bool isRoot = false;
        public bool isLookCamera;
        public bool isUpdate;
        public int forward = 1;
        public bool isListen = true;
        /// <summary>
        /// 依附对象，父物体
        /// </summary>
        public GameDynamicConf parentDynamicer;
        public bool isAllFllow = false;
        //#if UNITY_EDITOR
        //        [HideInInspector]
        //        public Object prefab;
        //#endif

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Action/GameDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<GameDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GameDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GameDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}
