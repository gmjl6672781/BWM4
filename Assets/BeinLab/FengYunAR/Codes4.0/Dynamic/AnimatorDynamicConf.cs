using System.Collections;
using System.Collections.Generic;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 动画动效
    /// </summary>
    [SerializeField]
    public class AnimatorDynamicConf : DynamicConf
    {
        /// <summary>
        /// 要播放的动画片段名称
        /// </summary>
        public string clipName;
        /// <summary>
        /// 播放速度
        /// </summary>
        public float playSpeed;
        /// <summary>
        /// 目标对象的名称
        /// </summary>
        public string targetAnimator;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            GameObject aniObj = UnityUtil.GetChildByName(gameDynamicer.gameObject, targetAnimator);
            if (!aniObj)
            {
                aniObj = gameDynamicer.GetComponentInChildren<Animator>().gameObject;
            }
            Animator animator = aniObj.GetComponent<Animator>();
            if (animator)
            {
                if (animator.isInitialized)
                {
                    animator.enabled = true;
                    animator.Play(clipName);
                    animator.speed = playSpeed;
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/AnimatorDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<AnimatorDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<AnimatorDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(AnimatorDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}