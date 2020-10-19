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

/// <summary>
/// 答案反馈配置文件
/// </summary>
namespace BeinLab.VRTraing.UI
{
    /// <summary>
    /// 答案反馈界面选项
    /// </summary>
    [Serializable]
    public struct AnswerFeed
    {
        public string key;//key值，用于配置文字
        public int isRight;//对应任务操作是否正确 1对 0错
        public int score;//对应任务的分数
    }

    public class AnswerFeedDlgConf : ScriptableObject
    {
        public string keyTitle = "T1074";//标题的key
        public string keyJilu = "T99";//记录的key -- 两种模式不同
        public string keyDefen = "T100";//得分的key -- 考试模式显示
        public string keyReturn = "T29";//返回首页的key
        public string keyReStart = "T28";//重新开始的key
        public List<AnswerFeed> answers;//多个任务
        public GameObject goSlelection;//每个选项对应的UI预设 Content


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/UIConf/AnswerFeedDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<AnswerFeedDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<AnswerFeedDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(AnswerFeedDlgConf) + " is null");
                }
            }
        }

#endif
    }
}
