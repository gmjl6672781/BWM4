using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using Valve.VR;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 选择工具的任务
    /// </summary>
    public class GoalSelectTools : TaskGoalConf
    {
        public GoalShowChoiceDlgConf goalShowChoiceDlgConf;
        public List<ToolConf> toolList;
        public SteamVR_Input_Sources handType = SteamVR_Input_Sources.Any;

        /// <summary>
        /// 默认按照选择的选项来判定
        /// </summary>
        public int rightIndex = -1;
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (goalMode == GoalType.All || (goalMode == GoalType.OnlyExamination && TaskManager.Instance.taskMode == TaskMode.Examination)
                || goalMode == GoalType.OnlyTraining && TaskManager.Instance.taskMode == TaskMode.Training)
            {
                for (int i = 0; i < toolList.Count; i++)
                {
                    ToolConf tool = toolList[i];
                    tool.toolBasic.OnCatch += OnCatchTool;
                }
            }
        }
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            if (goalMode == GoalType.All || (goalMode == GoalType.OnlyExamination && TaskManager.Instance.taskMode == TaskMode.Examination)
                || goalMode == GoalType.OnlyTraining && TaskManager.Instance.taskMode == TaskMode.Training)
            {
                for (int i = 0; i < toolList.Count; i++)
                {
                    ToolConf tool = toolList[i];
                    tool.toolBasic.OnCatch -= OnCatchTool;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="toolConf"></param>
        private void OnCatchTool(Hand hand, ToolConf toolConf)
        {
            if (HasDone) return;
            if (handType == SteamVR_Input_Sources.Any || handType == hand.handType)
            {
                for (int i = 0; i < toolList.Count; i++)
                {
                    ToolConf tool = toolList[i];
                    tool.toolBasic.SetToolHighlight(false);
                    if (tool == toolConf)
                    {
                        if (goalShowChoiceDlgConf)
                        {
                            goalShowChoiceDlgConf.OnSelectTool(i, (bool isRight) =>
                            {
                                AchieveGoal(isRight);
                            });
                        }
                        else if (rightIndex != -1)
                        {
                            AchieveGoal(i == rightIndex);
                        }
                    }
                }
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSelectTools", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSelectTools>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSelectTools>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSelectTools) + " is null");
                }
            }
        }

#endif
    }
}