using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using BeinLab.VRTraing.Gamer;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Conf
{
    public class FllowHandConf : DynamicConf
    {
        private Coroutine updateCoroutine;
        public SteamVR_Input_Sources handType;
        public Vector3[] linePath;
        public override void DoDynamic(GameDynamicer gameDynamicer)
        {
            base.DoDynamic(gameDynamicer);
            if (updateCoroutine != null)
            {
                gameDynamicer.StopCoroutine(updateCoroutine);
            }
            updateCoroutine = gameDynamicer.StartCoroutine(FllowHand(gameDynamicer));
        }
        private IEnumerator FllowHand(GameDynamicer gameDynamicer)
        {
            yield return new WaitForFixedUpdate();
            VRHand target = null;
            for (int i = 0; i < Player.instance.handCount; i++)
            {
                if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                {
                    target = Player.instance.hands[i].GetComponent<VRHand>();
                    break;
                }
            }
            if (linePath != null && linePath.Length > 1)
            {
                if (target)
                {
                    target.SetLinePath(linePath);
                }
            }
            while (target)
            {
                yield return new WaitForFixedUpdate();
                gameDynamicer.transform.position = target.GetDlgLinePos();
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
        [MenuItem("Assets/Create/VRTracing/FllowHandConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<FllowHandConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<FllowHandConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(FllowHandConf) + " is null");
                }
            }
        }

#endif
    }
}