using BeinLab.FengYun.Modus;
using BeinLab.FengYun.Moudus;
using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 组件配置
    /// 列表展示的形式
    /// </summary>
    [SerializeField]
    public class CellListConf : ScriptableObject
    {
        /// <summary>
        /// Cell组件配置
        /// </summary>
        public List<CellConf> cellConfList;
        /// <summary>
        /// 预制体配置
        /// </summary>
        public GameCell cellPrefab;
        /// <summary>
        /// 滑动配置
        /// </summary>
        public ScrollConf scrollConf;
        /// <summary>
        /// 当列表被创建时调用的事件
        /// </summary>
        public ActionConf createAction;
        /// <summary>
        /// 销毁时的事件
        /// </summary>
        public ActionConf delectAction;
        /// <summary>
        /// 当选中时的添加的图标
        /// </summary>
        public Sprite selectBG;
        /// <summary>
        /// 是否用公共选中图片
        /// </summary>
        public bool isUseSelectBG = true;
        /// <summary>
        /// 缩放尺寸
        /// </summary>
        public Vector3 scale = Vector3.one;
        public int defSelect = -1;
        public Vector2 bgExtra;
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/CellListConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<CellListConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<CellListConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(CellListConf) + " is null");
                }
            }
        }
#endif
    }
}