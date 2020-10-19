using System;
using System.Collections;
using System.Collections.Generic;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using DG.Tweening;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 动效组
    /// </summary>
    [SerializeField]
    public class DynamicGroupConf : DynamicBase
    {
        /// <summary>
        /// 对象组操作的对象
        ///// </summary>
        //[Tooltip("目标对象，动效的接收者")]
        //public GameDynamicConf gameDynamicConf;
        //[Tooltip("是否强制创建，当动效对象不存在时是否强制创建")]
        //public bool isForeceCreate = false;
        /// <summary>
        /// 循环次数
        /// </summary>
        [Tooltip("循环次数，默认一次，0次停止，小于0代表将一直循环")]
        public int loop = 1;
        /// <summary>
        /// 动效组的展示周期
        /// </summary>
        [Tooltip("动效组的周期，展示时间")]
        public float showTime = 1;
        /// <summary>
        /// 初始化动效
        /// </summary>
        [Tooltip("动效组执行前要执行的动效")]
        public DynamicConf initDynmic;
        /// <summary>
        /// 动效集合
        /// </summary>
        [Tooltip("动效列表，执行有顺序性，当一个动效完成时才去执行下一个动效")]
        public List<DynamicConf> dynamicListConf;
        /// <summary>
        /// 完成时动效
        /// </summary>
        [Tooltip("动效组完成时必展示的动效")]
        public DynamicConf completeDynmic;
        [Tooltip("当改变动效列表时是否删除此对象,当下一个动效组不包含此对象时是否删除此对象")]
        public bool isDelOnChangeList = true;
        /// <summary>
        /// 将子动效的效果清除
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public void OnStop(GameDynamicer gameDynamicer)
        {
            if (dynamicListConf != null)
            {
                for (int i = 0; i < dynamicListConf.Count; i++)
                {
                    dynamicListConf[i].OnStop(gameDynamicer);
                }
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Action/DynamicGroupConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DynamicGroupConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DynamicGroupConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DynamicGroupConf) + " is null");
                }
            }
        }
#endif
    }
}