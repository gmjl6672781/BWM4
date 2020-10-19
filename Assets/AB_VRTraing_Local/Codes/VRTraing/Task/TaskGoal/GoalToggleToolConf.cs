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
    public class GoalToggleToolConf : GoalUseToolConf
    {
        public ToolGoalConf toolToggle;

        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);

            if (!BeforeCheck())
                return;

            if (toolConf == toolToggle.toolConf)
            {
                SetToolInfo(toolToggle);
                AchieveGoal(true);
            }
        }


        private void ListenTool()
        {
            toolToggle.toolConf.toolBasic.OnHoverBegin += Check;
        }

        private void UnListenTool()
        {
            toolToggle.toolConf.toolBasic.OnHoverBegin -= Check;
        }

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            ListenTool();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            UnListenTool();
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalToggleToolConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalToggleToolConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalToggleToolConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalToggleToolConf) + " is null");
                }
            }
        }

#endif
    }
}

