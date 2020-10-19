using System;
using System.Collections;
using System.Collections.Generic;
using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.FengYun.Controller;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class DistenceDynamicConf : DynamicConf
    {

        [Tooltip("触发的距离")]
        public float actionDistance;
        [Tooltip("相机，或者其他目标靠近")]
        public GameDynamicConf targetGameDynConf;
        /// <summary>
        /// 远离事件
        /// 仅触发动效，非动效组
        /// </summary>
        [Tooltip("靠近的时的动效列表")]
        public string nearTargetAction;
        /// <summary>
        /// 靠近事件
        ///仅触发动效，非动效组
        /// </summary>
        [Tooltip("远离时的动效列表")]
        public string farTargetAction;
        [HideInInspector]
        public UnityEngine.Object nearActionConf;
        [HideInInspector]
        public UnityEngine.Object farActionConf;
        /// <summary>
        /// 
        /// </summary>
        private Coroutine updateCoroutine;
        /// <summary>
        /// 默认是远距离事件
        /// </summary>
        public bool isFar = true;
        private bool isCheck = false;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            isCheck = true;
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            updateCoroutine = gameDynamicer.StartCoroutine(CheckDistence(gameDynamicer));
        }
        /// <summary>
        /// 检测距离触发事件
        /// </summary>
        /// <param name="gameDynamicer"></param>
        /// <returns></returns>
        private IEnumerator CheckDistence(GameDynamicer gameDynamicer)
        {
            bool isCreate = false;
            GameDynamicer targetCam = DynamicActionController.Instance.GetOrCreateGameDynamicer(targetGameDynConf,out isCreate);
            while (targetCam != null && isCheck)
            {
                ///每隔半秒检测一次，减少次数
                yield return new WaitForSeconds(0.5f);
                Vector3 targetPos = targetCam.transform.position;
                targetPos.y = gameDynamicer.transform.position.y;
                float curDis = Vector3.Distance(targetPos, targetCam.transform.position);
                ///执行远距离动效
                if (curDis > actionDistance * gameDynamicer.Scale && !isFar)
                {
                    isFar = true;
                    DynamicActionController.Instance.DoAction(farTargetAction);
                }
                ///要执行近距离动效
                else if (curDis < actionDistance * gameDynamicer.Scale && isFar)
                {
                    isFar = false;
                    DynamicActionController.Instance.DoAction(nearTargetAction);
                }
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            isCheck = false;
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/DistenceDynamicConf", false, 3)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DistenceDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DistenceDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DistenceDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}