using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 拉隔离带
    /// </summary>
    public class GoalPullGeLeDaiConf : GoalUseToolConf
    {
        /// <summary>
        /// 四条隔离带
        /// </summary>
        public List<ToolGoalConf> toolGoalConfs;
        public Dictionary<ToolGoalConf, bool> dicToolGoalConfs = new Dictionary<ToolGoalConf, bool>();
        private int pullCount = 0;

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            dicToolGoalConfs.Clear();
            toolGoalConfs.ForEach(t => dicToolGoalConfs.Add(t, false));
            pullCount = 0;
        }
        public override void ForceTip()
        {
            base.ForceTip();
            PlayStartAudio();
        }

        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);
            if (!BeforeCheck())
                return;

            toolGoalConfs.ForEach(t =>
            {
                if (t.toolConf == toolConf && !dicToolGoalConfs[t])
                {
                    pullCount++;
                    dicToolGoalConfs[t] = true;
                    SetToolInfo(t);
                }
            });

            if (pullCount == toolGoalConfs.Count)
                AchieveGoal(true);

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

        private void ListenTool()
        {
            toolGoalConfs.ForEach(t =>
            {
                ToolGeLiDai ToolGeLiDai = (ToolGeLiDai)t.toolConf.toolBasic;
                ToolGeLiDai.OnPull += Check;
            });
        }

        private void UnListenTool()
        {
            toolGoalConfs.ForEach(t =>
            {
                ToolGeLiDai ToolGeLiDai = (ToolGeLiDai)t.toolConf.toolBasic;
                ToolGeLiDai.OnPull -= Check;
            });
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalPullGeLeDaiConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalPullGeLeDaiConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalPullGeLeDaiConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalPullGeLeDaiConf) + " is null");
                }
            }
        }

#endif
    }
}


