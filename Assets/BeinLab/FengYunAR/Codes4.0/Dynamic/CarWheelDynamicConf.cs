using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using BeinLab.Util;
using BeinLab.FengYun.Gamer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class CarWheelDynamicConf : TransferDynamicConf
    {
        private Transform carWheelRoot;
        public string carWheelRootName = "CarWheelRoot";
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (!carWheelRoot)
            {
                var obj = UnityUtil.GetChildByName(gameDynamicer.gameObject, carWheelRootName);
                if (obj)
                {
                    carWheelRoot = obj.transform;
                }
            }
            if (carWheelRoot)
            {
                for (int i = 0; i < carWheelRoot.childCount; i++)
                {
                    if (isKillOld)
                    {
                        carWheelRoot.GetChild(i).DOKill();
                    }
                    carWheelRoot.GetChild(i).localEulerAngles = startTrans;
                    carWheelRoot.GetChild(i).DOLocalRotate(endTrans, doTime,
                        RotateMode.FastBeyond360).SetEase(ease).SetLoops(loop, loopType);
                }
            }
        }
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (carWheelRoot)
            {
                for (int i = 0; i < carWheelRoot.childCount; i++)
                {
                    carWheelRoot.GetChild(i).DOKill();
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/TweenDynamic/TransferDynamic/CarWheelDynamicConf", false, 1)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<CarWheelDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<CarWheelDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(CarWheelDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}