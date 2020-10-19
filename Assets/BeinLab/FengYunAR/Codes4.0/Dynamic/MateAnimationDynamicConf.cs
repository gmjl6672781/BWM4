using DG.Tweening;
using UnityEngine;
using BeinLab.Util;
using BeinLab.FengYun.Gamer;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    public class MateAnimationDynamicConf : MateTweenDynamicConf
    {
        /// <summary>
        /// 开始位移  一般是00
        /// 也可指代速度
        /// </summary>
        public Vector2 startVector;
        /// <summary>
        /// 结束位移 计算配置
        /// 也可指代速度
        /// </summary>
        public Vector2 endVector;
        public bool isContinue;
        private Coroutine updateCoroutine;
        public bool isTween = true;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (!isTween)
            {
                if (updateCoroutine != null)
                {
                    gameDynamicer.StopCoroutine(updateCoroutine);
                }
                updateCoroutine = gameDynamicer.StartCoroutine(UpdateSpeed(gameDynamicer));
            }
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
        }

        private IEnumerator UpdateSpeed(GameDynamicer gameDynamicer)
        {
            if (!isContinue)
            {
                MateOffset = (startVector);
            }
            Vector2 curOffset = startVector;
            float sTime = Time.time; 
            while (resMate&&Time.time-sTime< doTime)
            {
                yield return new WaitForFixedUpdate();
                curOffset = Vector2.Lerp(curOffset, endVector, Time.deltaTime / doTime);
                MateOffset += curOffset * Time.deltaTime;
            }
        }

        public override Tweener CreateTweener(Transform body)
        {
            Tweener tweener = base.CreateTweener(body);
            if (isTween)
            {
                if (string.IsNullOrEmpty(attributes))
                {
                    if (isContinue)
                    {
                        tweener = resMate.DOOffset(resMate.mainTextureOffset + endVector - startVector, doTime).SetEase(ease).SetLoops(loop, loopType);
                    }
                    else
                    {
                        resMate.mainTextureOffset = startVector;
                        tweener = resMate.DOOffset(endVector, doTime).SetEase(ease).SetLoops(loop, loopType);
                    }
                }
                else
                {
                    if (isContinue)
                    {
                        tweener = resMate.DOOffset(resMate.mainTextureOffset + endVector - startVector,
                            attributes, doTime).SetEase(ease).SetLoops(loop, loopType);
                    }
                    else
                    {
                        resMate.SetTextureOffset(attributes, startVector);
                        tweener = resMate.DOOffset(endVector, attributes, doTime).SetEase(ease).SetLoops(loop, loopType);
                    }
                }
            }
            return tweener;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateAnimationDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateAnimationDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateAnimationDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateAnimationDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}