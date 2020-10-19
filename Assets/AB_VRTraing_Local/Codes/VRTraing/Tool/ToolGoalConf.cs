using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 工具达成目标配置
    /// </summary>
    public class ToolGoalConf : ScriptableObject
    {
        public ToolConf toolConf;
        //目标完成时设置  
        public bool isSetCanHover;
        public bool isCanHover;
        public bool isSetCanCatch;
        public bool isCanCatch;
        public bool isSetKinematic;
        public bool isKinematic;
        public bool isSetHighlight;
        public bool isHighlight;
        public bool isSetHide;
        public bool isHide;
        public bool isSetScaleSize;
        public Vector3 scaleSize;
        public bool isSetPose;
        public Vector3 Position;
        public Vector3 Angle;


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/ToolGoalConf", false, 0)]
        static void CreateDynamicConf()
        {
            Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<ToolGoalConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ToolGoalConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ToolGoalConf) + " is null");
                }
            }
        }

#endif
    }
}

