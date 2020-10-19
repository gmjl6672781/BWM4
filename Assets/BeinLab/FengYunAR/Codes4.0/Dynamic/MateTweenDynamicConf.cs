using BeinLab.FengYun.Gamer;
using DG.Tweening;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 材质动效
    /// </summary>
    [SerializeField]
    public class MateTweenDynamicConf : MateDynamicConf
    {
        public string attributes;
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
        [HideInInspector]
        public float realSpeed;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            DoTweener(CreateTweener(gameDynamicer.transform));
        }

        public Vector2 MateOffset
        {
            get
            {
                Vector2 lastV = Vector2.zero;
                if (string.IsNullOrEmpty(attributes))
                {
                    lastV = resMate.mainTextureOffset;
                }
                else
                {
                    lastV = resMate.GetTextureOffset(attributes);
                }
                return lastV;
            }
            set
            {
                if (string.IsNullOrEmpty(attributes))
                {
                    resMate.mainTextureOffset = value;
                }
                else
                {
                    resMate.SetTextureOffset(attributes, value);
                }
            }
        }

        private void DoTweener(Tweener tweener)
        {
            if (tweener != null)
            {
                tweener.SetEase(ease).SetLoops(loop, loopType);
            }
        }
        /// <summary>
        /// 创建一个
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual Tweener CreateTweener(Transform body)
        {
            if (isKillOld)
            {
                resMate.DOKill();
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
            resMate.DOKill();
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/MateDynamic/MateTweenDynamic/MateTweenDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<MateTweenDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<MateTweenDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(MateTweenDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}