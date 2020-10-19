using System;
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
    [SerializeField]
    public class MateValueDynamicConf : MateTweenDynamicConf
    {
        public bool isForeceOrg = true;
        public override Tweener CreateTweener(Transform body)
        {
            InitValue();
            return DoValue();
        }

        public virtual Tweener DoValue()
        {
            return null;
        }

        public virtual void InitValue()
        {

        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateValueDynamic/MateValueDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateValueDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateValueDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateValueDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}