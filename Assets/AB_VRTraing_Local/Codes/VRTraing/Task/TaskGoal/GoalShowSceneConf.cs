using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;
using BeinLab.VRTraing.Mgr;
using System.Collections.Generic;


namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 显示大任务通用场景
    /// 一般是语音提示 + 对话框 + 动画 + 高亮
    /// </summary>
    public class GoalShowSceneConf : TaskGoalConf
    {
        /// <summary>
        /// 高亮组件
        /// </summary>
        public List<ToolConf> highlightTools;
        //public float autoCompleteTime = 3f;

        public override void OnTaskStart(TaskConf targetTask)
        {
            base.OnTaskStart(targetTask);

            //高亮工具
            highlightTools.ForEach(t => t.toolBasic.SetToolHighlight(true));

            AchieveGoal(true);
            //语音和动画都播放完成，完成任务目标
            //if (TimerMgr.Instance)
            //{
            //    Timer timer = TimerMgr.Instance.CreateTimer(() => AchieveGoal(true), autoCompleteTime);
            //}
               
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowSceneConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowSceneConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowSceneConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowSceneConf) + " is null");
                }
            }
        }
#endif
    }
}


