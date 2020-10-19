using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Collections.Generic;
using System;
using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;

namespace BeinLab.VRTraing.Conf
{
    [Serializable]
    public struct ToolDynamic
    {
        public ToolConf toolConf;
        public List<DynamicConf> dynamicConfs;
    }
    public class GoalCatchToolConf : GoalUseToolConf
    {
        public SteamVR_Input_Sources handType;

        /// <summary>
        /// 要抓取的工具
        /// </summary>
        public List<ToolConf> rightTools;

        /// <summary>
        /// 干扰工具,只要抓取就错误完成任务
        /// </summary>
        public List<ToolConf> errorTools;
        public List<ToolDynamic> toolDynamics;
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

        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);

            if (!BeforeCheck())
                return;

            //目标达成
            if (handType == SteamVR_Input_Sources.Any || handType == hand.handType)
            {
                //完成任务目标失败
                errorTools.ForEach(t =>
                {
                    if (t == toolConf)
                    {
                        float showTime = 0;
                        ToolDynamic td = default(ToolDynamic);
                        for (int i = 0; i < toolDynamics.Count; i++)
                        {
                            if (toolDynamics[i].toolConf == toolConf)
                            {
                                td = toolDynamics[i];
                                break;
                            }
                        }

                        for (int i = 0; i < td.dynamicConfs.Count; i++)
                        {
                            DynamicConf dyn = td.dynamicConfs[i];
                            if (showTime < dyn.delayTime + dyn.showTime)
                            {
                                showTime = dyn.delayTime + dyn.showTime;
                            }
                            DynamicActionController.Instance.DoDynamic(dyn);
                        }
                        //AchieveGoal(false);
                        TimerMgr.Instance.CreateTimer(() =>
                        {
                            AchieveGoal(false);
                        }, showTime);
                    }
                });


                //成功完成任务目标
                rightTools.ForEach(t =>
                {
                    if (t == toolConf)
                    {
                        AchieveGoal(true);
                    }
                });

            }
        }

        protected override void OnAchieveGoal_(bool success)
        {
            base.OnAchieveGoal_(success);
            errorTools.ForEach(t => t.toolBasic.SetToolHighlight(false));
            rightTools.ForEach(t => t.toolBasic.SetToolHighlight(false));
        }


        private void ListenTool()
        {
            rightTools.ForEach(t => t.toolBasic.OnCatch += Check);
            errorTools.ForEach(t => t.toolBasic.OnCatch += Check);
        }

        private void UnListenTool()
        {
            rightTools.ForEach(t => t.toolBasic.OnCatch -= Check);
            errorTools.ForEach(t => t.toolBasic.OnCatch -= Check);
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalCatchToolConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalCatchToolConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalCatchToolConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalCatchToolConf) + " is null");
                }
            }
        }

#endif
    }

}

