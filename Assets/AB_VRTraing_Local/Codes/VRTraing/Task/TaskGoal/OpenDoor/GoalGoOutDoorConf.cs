using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Conf
{
    public class GoalGoOutDoorConf : TaskGoalConf
    {
        public ToolConf target;
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            taskConf.OnAchiveUnDone += OnAchiveUnDone;
            if (target)
            {
                target.toolBasic.OnHoverBegin += OnHoverBegin;
            }
        }

        private void OnAchiveUnDone()
        {
            isAchieveGoal = false;
        }

        /// <summary>
        /// 接触到对应的Hover
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="toolConf"></param>
        private void OnHoverBegin(Hand hand, ToolConf toolConf)
        {
            //Debug.Log("do:OnHoverBegin");
            if (toolConf == target)
            {
                AchieveGoal(true);
                target.toolBasic.OnHoverBegin -= OnHoverBegin;
                //Debug.Log("1do____________________________________");
            }
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            taskConf.OnAchiveUnDone -= OnAchiveUnDone;
            base.OnTaskEnd(taskConf);
            if (target)
            {
                //Debug.Log("2do____________________________________");
                target.toolBasic.OnHoverBegin -= OnHoverBegin;
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalGoOutDoorConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalGoOutDoorConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalGoOutDoorConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalGoOutDoorConf) + " is null");
                }
            }
        }

#endif
    }
}