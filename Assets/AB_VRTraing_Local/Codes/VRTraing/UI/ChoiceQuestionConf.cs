using BeinLab.Util;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Collections.Generic;
using BeinLab.CarShow.Modus;
using BeinLab.VRTraing.Conf;

/// <summary>
/// 选择题配置文件
/// </summary>
namespace BeinLab.VRTraing.UI
{
    /// <summary>
    /// 选项数据结构
    /// </summary>
    [Serializable]
    public struct Answer
    {
        /// <summary>
        /// 选项键值
        /// </summary>
        public string keyAnswer;
        public List<string> keyAudios;
        public List<ToolConf> highlightTools;
        public List<DynamicConf> dynamicConfs;
    }

    public class ChoiceQuestionConf : ScriptableObject
    {
        /// <summary>
        /// 问题描述，包含语音和文字
        /// </summary>
        public string keyMessage;
        /// <summary>
        /// 选项A,B,C
        /// </summary>
        public List<Answer
            > answers;
        public GameObject goSlelection;


        /// <summary>
        /// 正确的选项
        /// </summary>
        public int rightAnswer;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/UIConf/ChoiceQuestionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<ChoiceQuestionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ChoiceQuestionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ChoiceQuestionConf) + " is null");
                }
            }
        }

#endif
    }
}

