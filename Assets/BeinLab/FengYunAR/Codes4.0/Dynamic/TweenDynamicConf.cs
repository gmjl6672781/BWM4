using BeinLab.FengYun.Gamer;
using UnityEngine;
using DG.Tweening;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class TweenDynamicConf : DynamicConf
    {
        [Tooltip("时间")]
        public float doTime = 1;
        [Tooltip("曲线")]
        public Ease ease = Ease.InBack;
        [Tooltip("循环方式")]
        public LoopType loopType;
        [Tooltip("循环次数")]
        public int loop;
        [Tooltip("是否杀掉老进程")]
        public bool isKillOld = true;
        public string tweenTargetName;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            Transform target = gameDynamicer.transform;
            if (!string.IsNullOrEmpty(tweenTargetName))
            {
                var obj = UnityUtil.GetChildByName(gameDynamicer.gameObject, tweenTargetName);
                if (obj)
                {
                    target = obj.transform;
                }
            }
            DoTweener(CreateTweener(target));
        }

        public void DoTweener(Tweener tweener)
        {
            if (tweener != null)
            {
                tweener.SetEase(ease).SetLoops(loop, loopType);
            }
        }

        public virtual Tweener CreateTweener(Transform body)
        {
            if (isKillOld)
            {
                body.DOKill();
            }
            return null;
        }
        /// <summary>
        /// 当动效变化时停止
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            gameDynamicer.transform.DOKill();
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/TweenDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<TweenDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<TweenDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(TweenDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}