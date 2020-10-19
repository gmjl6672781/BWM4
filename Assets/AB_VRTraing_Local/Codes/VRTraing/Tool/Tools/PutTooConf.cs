using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 放置工具的配置
    /// </summary>
    public class PutTooConf : ScriptableObject
    {
        /// <summary>
        /// 放置的位置
        /// </summary>
        public Vector3 putPos;
        /// <summary>
        /// 放置的角度
        /// </summary>
        public Vector3 putAngle;
        /// <summary>
        /// 放下后是否还可以再拿起来
        /// </summary>
        public bool isCanCatchOnPut = true;
        public bool isOpenPhyOnCatch = true;
        /// <summary>
        /// 和具体的工具进行交互的内容
        /// </summary>
        public ToolConf triggerTool;
        public float doTime = 0.3f;
        public bool isReSetParent = true;
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/PutTooConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<PutTooConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<PutTooConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(PutTooConf) + " is null");
                }
            }
        }
#endif
    }
}