using System.Collections;
using System.Collections.Generic;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.FengYun.Controller;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 材质编辑器
    /// </summary>
    [SerializeField]
    public class MateToMateDynamicConf : MateDynamicConf
    {
        [Tooltip("目标材质，将要改变的材质")]
        public Material targetMate;
        
        public override void DoMateDynamic()
        {
            base.DoMateDynamic();
            DynamicActionController.Instance.ChangeMaterial(resMate, targetMate);
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateToMateDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateToMateDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateToMateDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateToMateDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}