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
    /// 材质隐藏
    /// </summary>
    [SerializeField]
    public class MateFadeDynamicConf : MateAttributesDynamicConf
    {
        public override void InitValue()
        {
            if (string.IsNullOrEmpty(attributes))
            {
                Color tmp = resMate.color;
                tmp.a = (fromValue);
                resMate.color = tmp;
            }
            else
            {
                Color tmp = resMate.GetColor(attributes);
                tmp.a = (fromValue);
                resMate.SetColor(attributes, tmp);
            }
        }
        public override Tweener DoValue()
        {
            Tweener tweener = base.DoValue();

            if (string.IsNullOrEmpty(attributes))
            {
                Color tmp2 = resMate.color;
                tmp2.a = toValue;
                tweener = resMate.DOColor(tmp2, doTime);
            }
            else
            {
                Color tmp2 = resMate.color;
                tmp2.a = toValue;
                tweener = resMate.DOColor(tmp2, attributes, doTime);
            }
            return tweener;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateValueDynamic/MateAttributesDynamic/MateFadeDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateFadeDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateFadeDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateFadeDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}