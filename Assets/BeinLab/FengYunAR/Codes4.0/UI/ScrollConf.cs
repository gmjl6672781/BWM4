using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Moudus
{
    /// <summary>
    /// 配置出Scroll
    /// </summary>
    [SerializeField]
    public class ScrollConf : ScriptableObject
    {
        public string scrollName;
        public Vector2 size = Vector2.one * 100;
        public Vector2 space = Vector2.one * 100;
        public Vector3 scale = Vector3.one;
        /// <summary>
        /// 滑动效果
        /// </summary>
        public ScrollRect.MovementType swipe = ScrollRect.MovementType.Clamped;
        /// <summary>
        /// 是否自由滑动
        /// </summary>
        public bool isFreeMove = false;
        public int showCount;
        /// <summary>
        /// 是否显示下一个
        /// </summary>
        public bool isShowNext;
        /// <summary>
        /// 等待复原时间
        /// </summary>
        public float watieTime = 0;
        /// <summary>
        /// 滑动比例
        /// </summary>
        public float swipeDet = 0.5f;
        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed = 10;
        /// <summary>
        /// 垂直
        /// </summary>
        public bool vertical = true;
        /// <summary>
        /// 水平
        /// </summary>
        public bool horizontal = true;
        public int allCount;
        public Vector2 extra;
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/ScrollConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<ScrollConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ScrollConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ScrollConf) + " is null");
                }
            }
        }
#endif
    }
}