using BeinLab.Util;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.Conf
{
    /// <summary>
    /// 动画效果
    /// </summary>
    public class TransferConf : ScriptableObject
    {
        public TransferType transferType;
        [Tooltip("起始")]
        public Vector3 startTrans;
        [Tooltip("终态")]
        public Vector3 endTrans;
        public bool isUseStartPos = true;

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
        /// <summary>
        /// 创建一个移动动画
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public void CreateTweener(Transform body)
        {
            if (isKillOld)
            {
                body.DOKill();
            }

            Tweener tweener = null;
            if (transferType == TransferType.Move)
            {
                if (isUseStartPos)
                    body.localPosition = startTrans;
                tweener = body.DOLocalMove(endTrans, doTime);
            }
            else if (transferType == TransferType.Rote)
            {
                if (isUseStartPos)
                    body.localEulerAngles = startTrans;
                tweener = body.DOLocalRotate(endTrans, doTime);
                Debug.LogError(tweener);
            }
            else if (transferType == TransferType.Scale)
            {
                if (isUseStartPos)
                    body.localScale = startTrans;
                tweener = body.DOScale(endTrans, doTime);
            }
            if (tweener != null)
            {
                tweener.SetEase(ease).SetLoops(loop, loopType);
            }
            
        }

        /// <summary>
        /// 当动效变化时停止
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public void OnStop(Transform transform)
        {
            transform.DOKill();
            if (transferType == TransferType.Move)
            {
                Vector3 pos = Vector3.zero;
                if (isUseStartPos)
                {
                    pos = startTrans;
                }
                transform.localPosition = pos;
            }
            else if (transferType == TransferType.Rote)
            {
                Vector3 pos = Vector3.zero;
                if (isUseStartPos)
                {
                    pos = startTrans;
                }
                transform.localEulerAngles = pos;
            }
            else if (transferType == TransferType.Scale)
            {
                Vector3 pos = Vector3.one;
                if (isUseStartPos)
                {
                    pos = startTrans;
                }
                transform.localScale = pos;
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/ARLauncher/Transfer", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<TransferConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<TransferConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(TransferConf) + " is null");
                }
            }
        }

#endif
    }
}