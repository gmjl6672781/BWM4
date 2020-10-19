using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 路径动效
    /// </summary>
    [SerializeField]
    public class PathDynamicConf : TweenDynamicConf
    {
        [Tooltip("路径")]
        public Vector3[] path;

        public override Tweener CreateTweener(Transform body)
        {
            Tweener tweener = base.CreateTweener(body);
            tweener = body.DOPath(path, doTime);
            return tweener;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/PathDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<PathDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<PathDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(PathDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}