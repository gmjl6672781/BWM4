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
    /// 照明灯检查
    /// </summary>
    public class GoalZhaoMingCheckConf : GoalUseToolConf
    {
        public ToolGoalConf toolZhaoMingDeng;
        /// <summary>
        /// 需要检查的物体Tag
        /// </summary>
        public List<ToolGoalConf> checkTools;

        public Dictionary<ToolGoalConf, bool> dicCheckTools = new Dictionary<ToolGoalConf, bool>();

        private int checkCount = 0;

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            dicCheckTools.Clear();
            checkTools.ForEach(t => dicCheckTools.Add(t, false));
            checkCount = 0;
        }

        protected override void Check(string content)
        {
            base.Check(content);

            if (!BeforeCheck())
                return;

            checkTools.ForEach(t =>
            {
                if (t.toolConf.toolBasic.gameObject.tag == content && !dicCheckTools[t])
                {
                    checkCount++;
                    dicCheckTools[t] = true;
                    SetToolInfo(t);
                }
            });

            if (checkCount == checkTools.Count)
            {
                SetToolInfo(toolZhaoMingDeng);
                AchieveGoal(true);
            }
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
            ToolZhaoMingDeng tool = (ToolZhaoMingDeng)toolZhaoMingDeng.toolConf.toolBasic;
            tool.OnCheckSurface += Check;
        }

        private void UnListenTool()
        {
            ToolZhaoMingDeng tool = (ToolZhaoMingDeng)toolZhaoMingDeng.toolConf.toolBasic;
            tool.OnCheckSurface -= Check;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalZhaoMingCheckConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalZhaoMingCheckConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalZhaoMingCheckConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalZhaoMingCheckConf) + " is null");
                }
            }
        }

#endif
    }

}

