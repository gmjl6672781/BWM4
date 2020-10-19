using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using BeinLab.VRTraing.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;

/// <summary>
/// 判读题配置文件
/// </summary>
namespace BeinLab.VRTraing.UI
{
    public class JudgementQuestionConf : ScriptableObject
    {
        public string keyMessage;
        public string keyYes;
        public string KeyNo;
        public SteamVR_Action_Boolean vr_Action_Button;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/UIConf/JudgementQuestionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<JudgementQuestionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<JudgementQuestionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(JudgementQuestionConf) + " is null");
                }
            }
        }

#endif
    }
}

