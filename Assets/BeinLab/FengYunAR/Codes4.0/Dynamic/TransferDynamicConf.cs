using BeinLab.Util;
using UnityEngine;
using DG.Tweening;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class TransferDynamicConf : TweenDynamicConf
    {
        [Tooltip("变幻模式")]
        public TransferType transferType;
        [Tooltip("起始")]
        public Vector3 startTrans;
        [Tooltip("终态")]
        public Vector3 endTrans;
        public bool isUseStartPos = true;
        public override Tweener CreateTweener(Transform body)
        {
            Tweener tweener = base.CreateTweener(body);
            if (transferType == TransferType.Move)
            {
                if (isUseStartPos)
                    body.localPosition = startTrans;
                tweener = body.transform.DOLocalMove(endTrans, doTime);
            }
            else if (transferType == TransferType.Rote)
            {
                if (isUseStartPos)
                    body.localEulerAngles = startTrans;
                tweener = body.DOLocalRotate(endTrans, doTime);
            }
            else if (transferType == TransferType.Scale)
            {
                if (isUseStartPos)
                    body.localScale = startTrans;
                tweener = body.DOScale(endTrans, doTime);
            }
            return tweener;
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (transferType == TransferType.Move)
            {
                gameDynamicer.transform.localPosition = endTrans;
            }
            else if (transferType == TransferType.Rote)
            {
                gameDynamicer.transform.localEulerAngles = endTrans;
            }
            else if (transferType == TransferType.Scale)
            {
                gameDynamicer.transform.localScale = endTrans;
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/TransferDynamic/TransferDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<TransferDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<TransferDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(TransferDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}