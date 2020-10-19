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
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    public class GoalShowSketchMapDlgConf : TaskGoalConf
    {
        //是否在考试模式隐藏 UI
        public bool isHideOnExam;

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            SketchMapDlg.Instance.ShowDlg();
            if (isHideOnExam && TaskManager.Instance.taskMode==TaskMode.Examination)
            {
                SketchMapDlg.Instance.HideTips();
            }
            
            AchieveGoal(true);
        }


        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            SketchMapDlg.Instance.HideDlg();
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowSketchMapDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowSketchMapDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowSketchMapDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowSketchMapDlgConf) + " is null");
                }
            }
        }

#endif

    }
}


