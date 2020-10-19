using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class MateAttributesDynamicConf : MateValueDynamicConf
    {
        public float fromValue;
        public float toValue;
        public override void InitValue()
        {
            base.InitValue();
            if (!string.IsNullOrEmpty(attributes))
            {
                if (isForeceOrg)
                {
                    resMate.SetFloat(attributes, fromValue);
                }
            }
        }
        public override Tweener DoValue()
        {
            Tweener tweener = base.DoValue();
            if (!string.IsNullOrEmpty(attributes))
            {
                tweener = resMate.DOFloat(toValue, attributes, doTime);
            }
            return tweener;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateValueDynamic/MateAttributesDynamic/MateAttributesDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateAttributesDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateAttributesDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateAttributesDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}