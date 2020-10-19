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
    /// <summary>
    /// 调节轧带枪档位
    /// </summary>
    public class GoalSetZhaDaiGearConf : GoalUseToolConf
    {
        public ToolGoalConf toolZhaDaiQiang;

        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);
            if (!BeforeCheck())
                return;

            ////扣动
            //if (VRHandHelper.Instance.VRInput_GrabGrip.GetKeyDown(hand))
            //{
            //    GearDlg.Instance.ShowDlg();
            //}

            GearDlg.Instance.ShowDlg();
            if (VRHandHelper.Instance.VRInput_InteractUI.GetKeyDown(hand))
            {
                SendAnswer(4);
            }
        }

        protected void OnDetach(Hand hand, ToolConf toolConf)
        {
            GearDlg.Instance.HideDlg();
        }

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            OnTaskStart(targetTask);
        }

        public override void OnTaskStart(TaskConf targetTask)
        {
            base.OnTaskStart(targetTask);

            GearDlg.Instance.SendAnswer += SendAnswer;
            ListenTool();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            UnListenTool();
        }

        private void SendAnswer(int answer)
        {
            if (targetTask == TaskManager.Instance.CurrentTask)
            {
                ToolZhaDaiQiang tool = (ToolZhaDaiQiang)toolZhaDaiQiang.toolConf.toolBasic;
                tool.Gear = answer;
                if (answer == 4)
                {
                    AchieveGoal(true);
                    GearDlg.Instance.HideDlg();
                }
                else
                {
                    AchieveGoal(false);
                }
            }
        }


        private void ListenTool()
        {
            toolZhaDaiQiang.toolConf.toolBasic.OnCatch += Check;
            toolZhaDaiQiang.toolConf.toolBasic.OnDetach += OnDetach;
        }

        private void UnListenTool()
        {
            toolZhaDaiQiang.toolConf.toolBasic.OnCatch -= Check;
            toolZhaDaiQiang.toolConf.toolBasic.OnDetach -= OnDetach;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSetZhaDaiGearConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSetZhaDaiGearConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSetZhaDaiGearConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSetZhaDaiGearConf) + " is null");
                }
            }
        }

#endif
    }
}


