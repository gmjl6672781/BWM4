using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 动效事件组
    /// 一个动效由多个动效组合而成
    /// </summary>
    [SerializeField]
    public class DynamicActionListConf : ScriptableObject
    {
        [Tooltip("动效事件组，集合")]
        public List<DynamicBase> dynamicBaseList;
        [Tooltip("动效播放次数，循环次数，默认为1")]
        public int loop = 1;
        /// <summary>
        /// 展示时间
        /// </summary>
        [Tooltip("动效展示的时间")]
        public float showTime;
        [Tooltip("是否是主事件组")]
        public bool isMainAction = true;
        public bool IsHaveGameDynamicConf(GameDynamicConf gameDynConf)
        {
            if (gameDynConf)
            {
                for (int i = 0; i < dynamicBaseList.Count; i++)
                {
                    if (dynamicBaseList[i].gameDynamicConf == gameDynConf)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Action/DynamicActionListConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DynamicActionListConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DynamicActionListConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DynamicActionListConf) + " is null");
                }
            }
        }
#endif
    }
}