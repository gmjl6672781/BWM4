using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Gamer;
using BeinLab.RS5.Mgr;
using BeinLab.Util;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.CarShow.Modus
{
    public class FllowXRDynamicConf : DynamicConf
    {
        private Coroutine updateCoroutine;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            gameDynamicer.ShowBody.SetActive(false);
            updateCoroutine = gameDynamicer.StartCoroutine(UpdatePosition(gameDynamicer));
        }
        /// <summary>
        /// 实时跟随动效
        /// </summary>
        /// <param name="gameDynamicer"></param>
        /// <returns></returns>
        private IEnumerator UpdatePosition(GameDynamicer gameDynamicer)
        {
            if (!XRTracker.Instance)
            {
                yield return new WaitForSeconds(1);
            }
            
            while (XRTracker.Instance)
            {
                yield return new WaitForFixedUpdate();
                
                if (XRTracker.isHit)
                {
                    if (!gameDynamicer.ShowBody.activeSelf)
                    {
                        gameDynamicer.ShowBody.SetActive(true);
                    }
                    gameDynamicer.transform.position = XRTracker.arAnchorPos;
                    if (XRController.Instance && XRController.Instance.ARCamera)
                    {
                        UnityUtil.LookAtV(gameDynamicer.transform, XRController.Instance.ARCamera.transform);
                    }
                }
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
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/FllowXRDynamicConf", false, 5)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;   //选择放置位置（的文件夹）
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj); //获取文件夹的路径
                ScriptableObject bullet = CreateInstance<FllowXRDynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<FllowXRDynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);  //创建配置文件
                }
                else
                {
                    Debug.Log(typeof(FllowXRDynamicConf) + " is null");
                }
            }
        }
#endif
    }
}