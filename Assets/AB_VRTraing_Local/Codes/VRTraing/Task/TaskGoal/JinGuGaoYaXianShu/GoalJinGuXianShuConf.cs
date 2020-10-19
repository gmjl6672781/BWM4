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
using BeinLab.VRTraing.Controller;

namespace BeinLab.VRTraing.Conf
{
    public class GoalJinGuXianShuConf : GoalUseToolConf
    {
        public ToolGoalConf toolZhaDaiLine;
        public ToolGoalConf toolZhaDaiCircle;
        public ToolGoalConf toolZhaDaiCircleFlag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolConf"></param>
        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);

            if (!BeforeCheck())
                return;

            if (toolZhaDaiCircleFlag.toolConf == toolConf)
            {          
                SetToolInfo(toolZhaDaiCircle);
                SetToolInfo(toolZhaDaiCircleFlag);
                AchieveGoal(true);
            }

          
       
        }

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
   
        }

        private void ListenTool()
        {
            toolZhaDaiLine.toolConf.toolBasic.OnEnterTool += Check;
        }

        private void UnListenTool()
        {
            toolZhaDaiLine.toolConf.toolBasic.OnEnterTool -= Check;
        }

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            ListenTool();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);           

            Player.instance.rightHand.DetachObject(Player.instance.rightHand.currentAttachedObject);
            Player.instance.leftHand.DetachObject(Player.instance.rightHand.currentAttachedObject);
            //VRHandHelper.Instance.ReleaseObject(Player.instance.leftHand);
            //VRHandHelper.Instance.ReleaseObject(Player.instance.rightHand);
            UnListenTool();
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalJinGuXianShuConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalJinGuXianShuConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalJinGuXianShuConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalJinGuXianShuConf) + " is null");
                }
            }
        }

#endif
    }
}

