using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 颜色变化的动效
    /// </summary>
    [SerializeField]
    public class MateColorDynamicConf : MateValueDynamicConf
    {
        public Color formColor;
        public Color toColor;

        public override void InitValue()
        {
            base.InitValue();
            if (!string.IsNullOrEmpty(attributes))
            {
                if (isForeceOrg)
                {
                    resMate.SetColor(attributes, formColor);
                }
            }
            else
            {
                if (isForeceOrg)
                {
                    resMate.color = formColor;
                }
            }
        }
        public override Tweener DoValue()
        {
            Tweener tweener = base.DoValue();
            if (!string.IsNullOrEmpty(attributes))
            {
                tweener = resMate.DOColor(toColor, attributes, doTime);
            }
            else
            {
                tweener = resMate.DOColor(toColor, doTime);
            }
            return tweener;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateValueDynamic/MateColorDynamicConf", false, 2)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateColorDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateColorDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateColorDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}