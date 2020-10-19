using BeinLab.Util;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 动效事件配置
    /// 事件发生的节点
    /// 事件的对象
    /// 事件包含的动效列表
    /// 由事件控制器统一调度执行动效事件
    /// 事件只执行一次，动效可以执行多次
    /// </summary>
    [SerializeField]
    public class DynamicActionConf : DynamicBase
    {
        /// <summary>
        /// 动效展示的节点
        /// </summary>
        [Tooltip("动效展示的节点")]
        public NodeConf nodeConf;
        /// <summary>
        /// 动效对象
        /// </summary>
        //[Tooltip("动效对象配置")]
        //public GameDynamicConf gameDynamicConf;
        /// <summary>
        /// 动效组，动效集合列表
        /// 用来控制周期变化
        /// </summary>
        [Tooltip("动效组，动效集合列表")]
        public DynamicGroupConf dynamicGroup;

        //[Tooltip("是否强制创建动效对象")]
        //public bool isForeceCreate = true;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Action/DynamicActionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DynamicActionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DynamicActionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DynamicActionConf) + " is null");
                }
            }
        }
#endif
    }
}
