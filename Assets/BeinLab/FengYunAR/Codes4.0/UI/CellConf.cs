using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.FengYun.UI;
using UnityEngine.UI;
using BeinLab.FengYun.Controller;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modus
{
    /// <summary>
    /// Cell组件配置
    /// </summary>
    public class CellConf : ScriptableObject
    {
        /// <summary>
        /// 激活的事件配置
        /// </summary>
        public ActionConf triggerAction;
        /// <summary>
        /// 当此按钮失活时要执行的事件
        /// </summary>
        public ActionConf unActiveAction;
        public string trigger;
        public string unTrigger;

        /// <summary>
        /// 此按钮的名称
        /// </summary>
        public string cellName;
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<string> child;
        /// <summary>
        /// 默认ICON，主图标配置
        /// </summary>
        public Sprite icon;
        /// <summary>
        /// 按钮按下时图标
        /// </summary>
        public Sprite pressIcon;
        [HideInInspector]
        public Button background;
        private GameCell cell;
        private int showIndex;
        public virtual void SetData(GameCell cell, int index)
        {
            if (!background)
            {
                showIndex = index;
                background = UnityUtil.GetTypeChildByName<Button>(cell.gameObject, cell.mainBtnName);
                background.image.sprite = icon;
                this.cell = cell;
                background.onClick.AddListener(OnClickMainBtn);
            }
        }
        /// <summary>
        /// 当点击了主按钮
        /// </summary>
        public virtual void OnClickMainBtn()
        {
            if (cell)
            {
                cell.SetActiveEvent(this, showIndex);
            }
            TimerMgr.Instance.CreateTimer(delegate ()
            {
                if (DynamicActionController.Instance)
                {
                    DynamicActionController.Instance.DoAction(triggerAction);
                }
                if (!string.IsNullOrEmpty(trigger))
                {
                    DynamicActionController.Instance.DoAction(trigger);
                }
            }, 0);
        }

        public virtual void Select()
        {
            OnClickMainBtn();
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/CellConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<CellConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<CellConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(CellConf) + " is null");
                }
            }
        }
#endif
    }
}