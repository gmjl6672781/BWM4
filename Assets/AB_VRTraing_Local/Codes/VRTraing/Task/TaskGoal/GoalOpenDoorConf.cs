using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;
using BeinLab.VRTraing.Mgr;
using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;

namespace BeinLab.VRTraing.Conf
{
    public class GoalOpenDoorConf : GoalUseToolConf
    {
        public ToolGoalConf toolDoor;
        public List<TaskGoalConf> earlyGoals;
        public List<DynamicConf> errorActionDyn;

        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);

            if (!BeforeCheck())
                return;


            if (toolConf == toolDoor.toolConf)
            {
                foreach (TaskGoalConf goal in earlyGoals)
                {
                    if (!goal.CheckAchieveGoal())
                    {
                        float dynShowTime = 0;
                        for (int i = 0; i < errorActionDyn.Count; i++)
                        {
                            if (dynShowTime < errorActionDyn[i].delayTime + errorActionDyn[i].showTime)
                            {
                                dynShowTime = errorActionDyn[i].delayTime + errorActionDyn[i].showTime;
                            }
                            DynamicActionController.Instance.DoDynamic(errorActionDyn[i]);
                        }
                        //if (TaskManager.Instance.taskMode == TaskMode.Examination)
                        //{
                        //    AchieveGoal(false);
                        //    //TimerMgr.Instance.CreateTimer(() =>
                        //    //{
                        //    //    AchieveGoal(false);
                        //    //}, dynShowTime);
                        //    return;
                        //}
                        //else
                        //{
                        //    //goal.ForceTip();
                        //    //return;
                        //}
                        AchieveGoal(false);
                        return ;
                    }
                }
            }

            AchieveGoal(true);
        }

        private void ListenTool()
        {
            toolDoor.toolConf.toolBasic.OnPressDown += Check;
        }

        private void UnListenTool()
        {
            toolDoor.toolConf.toolBasic.OnPressDown -= Check;
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
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalOpenDoorConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalOpenDoorConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalOpenDoorConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalOpenDoorConf) + " is null");
                }
            }
        }

#endif
    }
}

