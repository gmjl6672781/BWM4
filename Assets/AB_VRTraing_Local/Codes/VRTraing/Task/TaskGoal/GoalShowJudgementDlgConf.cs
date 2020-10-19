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

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 任务目标：
    /// </summary>
    public class GoalShowJudgementDlgConf : TaskGoalConf
    {

        public override void OnTaskStart(TaskConf targetTask)
        {
            base.OnTaskStart(targetTask);
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowJudgementDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowJudgementDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowJudgementDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowJudgementDlgConf) + " is null");
                }
            }
        }

#endif
    }
}

