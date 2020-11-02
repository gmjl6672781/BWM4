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
    /// 放置工具到指定位置
    /// </summary>
    public class GoalPlaceToolConf : GoalUseToolConf
    {
        /// <summary>
        /// 需放置的工具
        /// </summary>
        public ToolGoalConf toolToPlace;
        /// <summary>
        /// 放置位置
        /// </summary>
        public ToolGoalConf toolToPlaceFlag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolConf"> 需要放置的工具位置</param>
        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);

            if (!BeforeCheck())
                return;

            //碰到标记物
            if (toolConf == toolToPlaceFlag.toolConf)
            {
                ToolBasic tool = toolToPlace.toolConf.toolBasic;
                if (tool.catchHand)
                {
                    Debug.Log(toolConf.name+"  放置成功");
                    tool.catchHand.DetachObject(tool.gameObject);
                    tool.SetToolCatch(false);
                    tool.transform.position = toolConf.toolBasic.transform.position;
                    tool.transform.eulerAngles = toolConf.toolBasic.transform.eulerAngles;
                    AchieveGoal(true);
                    SetToolInfo(toolToPlace);
                    SetToolInfo(toolToPlaceFlag);
                }
            }
            
        }

        private void OnCatch(Hand hand,ToolConf toolConf)
        {
            toolToPlaceFlag.toolConf.toolBasic.gameObject.SetActive(true);
        }

        private void OnDetach(Hand arg1, ToolConf arg2)
        {
            toolToPlaceFlag.toolConf.toolBasic.gameObject.SetActive(false);
        }

        private void ListenTool()
        {
            toolToPlace.toolConf.toolBasic.OnEnterTool += Check;
            toolToPlace.toolConf.toolBasic.OnCatch += OnCatch;
            toolToPlace.toolConf.toolBasic.OnDetach += OnDetach;
        }



        private void UnListenTool()
        {
            toolToPlace.toolConf.toolBasic.OnEnterTool -= Check;
            toolToPlace.toolConf.toolBasic.OnCatch -= OnCatch;
            toolToPlace.toolConf.toolBasic.OnDetach -= OnDetach;
        }
        public override void ForceTip()
        {
            base.ForceTip();
            toolToPlace.toolConf.toolBasic.SetToolHighlight(true);
            PlayStartAudio();
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
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalPlaceToolConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalPlaceToolConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalPlaceToolConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalPlaceToolConf) + " is null");
                }
            }
        }

#endif
    }

}
