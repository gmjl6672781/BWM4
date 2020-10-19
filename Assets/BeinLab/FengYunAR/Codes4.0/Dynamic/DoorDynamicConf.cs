using UnityEngine;
using BeinLab.Util;
using DG.Tweening;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    public class DoorDynamicConf : TransferDynamicConf
    {
        public string doorName;

        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            GameObject body = UnityUtil.GetChildByName(gameDynamicer.gameObject, doorName);
            if (body)
            {
                DoTweener(CreateTweener(body.transform));
            }
        }

        public override void OnStop(GameDynamicer gameDynamicer)
        {
            GameObject body = UnityUtil.GetChildByName(gameDynamicer.gameObject, doorName);
            if (body)
            {
                body.transform.DOKill();
                if (transferType == TransferType.Move)
                {
                    body.transform.localPosition = endTrans;
                }
                else if (transferType == TransferType.Rote)
                {
                    body.transform.localEulerAngles = endTrans;
                }
                else if (transferType == TransferType.Scale)
                {
                    body.transform.localScale = endTrans;
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/TransferDynamic/DoorDynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DoorDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DoorDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DoorDynamicConf) + " is null");
                }
            }
        }

#endif
    }
}