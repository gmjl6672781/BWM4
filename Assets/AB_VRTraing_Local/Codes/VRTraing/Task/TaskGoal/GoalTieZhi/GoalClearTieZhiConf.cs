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
  
    public class GoalClearTieZhiConf : GoalUseToolConf
    {
        /// <summary>
        /// 需放置的工具
        /// </summary>
        public ToolGoalConf toolTieZhi;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolConf"> 需要放置的工具位置</param>
        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);

            if (!BeforeCheck())
                return;

            if(toolConf == toolTieZhi.toolConf)
            {
                SetToolInfo(toolTieZhi);
                //TextMesh textMesh = toolTieZhi.toolConf.toolBasic.GetComponentInChildren<TextMesh>();
                //if (textMesh)
                //    textMesh.text = "";
                AchieveGoal(true);
            }
        }
        

        private void ListenTool()
        {
            toolTieZhi.toolConf.toolBasic.OnHoverBegin += Check;
        }

        private void UnListenTool()
        {
            toolTieZhi.toolConf.toolBasic.OnHoverBegin -= Check;
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
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalClearTieZhiConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalClearTieZhiConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalClearTieZhiConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalClearTieZhiConf) + " is null");
                }
            }
        }

#endif
    }
}


