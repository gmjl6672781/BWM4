using BeinLab.FengYun.Gamer;
using System.Collections;
using UnityEngine;
using BeinLab.FengYun.Controller;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    [SerializeField]
    public class FllowDynamicConf : DynamicConf
    {
        [Tooltip("跟随的对象")]
        public GameDynamicConf targetGameDynConf;
        public bool isFllowRotation = false;
        private Coroutine updateCoroutine;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            updateCoroutine = gameDynamicer.StartCoroutine(FllowTarget(gameDynamicer));
        }

        private IEnumerator FllowTarget(GameDynamicer gameDynamicer)
        {
            bool isCreate = false;
            GameDynamicer targetCam = DynamicActionController.Instance.GetOrCreateGameDynamicer(targetGameDynConf,out isCreate);
            float stime = Time.time;
            while (targetCam &&Time.time-stime<showTime)
            {
                ///每隔半秒检测一次，减少次数
                gameDynamicer.transform.position = targetCam.transform.TransformPoint(position);
                if (isFllowRotation)
                {
                    gameDynamicer.transform.rotation = targetCam.transform.rotation;
                }
                yield return new WaitForFixedUpdate();
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public override void OnStop(GameDynamicer gameDynamicer)
        {
            base.OnStop(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/FllowDynamicConf", false, 5)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;   //选择放置位置（的文件夹）
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj); //获取文件夹的路径
                ScriptableObject bullet = CreateInstance<FllowDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<FllowDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);  //创建配置文件
                }
                else
                {
                    Debug.Log(typeof(FllowDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}