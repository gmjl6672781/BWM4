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
    /// <summary>
    /// 书写警示牌
    /// </summary>
    public class GoalWriteConf : GoalUseToolConf
    {
        public ToolGoalConf toolBaiBanBi;
        public ToolGoalConf toolBaiBan;
        public BaiBanConf baiBanConf;


        /// <summary>
        /// 监听OnEnterMatch
        /// </summary>
        /// <param name="toolConf"> toolBaiBan</param>
        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);

            if (!BeforeCheck())
                return;

            //白板笔碰到白板
            if (toolBaiBan.toolConf == toolConf)
            {
                ToolBaiBan tool = (ToolBaiBan)toolConf.toolBasic;
                if(LanguageMgr.Instance.CurLanguage.PriKey == BaiBanConf.chinesePriKey)
                {
                    tool.SetMap(baiBanConf.chineseTextureWrite);
                }else
                    tool.SetMap(baiBanConf.chineseTextureWrite);

                SetToolInfo(toolBaiBan);
                SetToolInfo(toolBaiBanBi);
                AchieveGoal(true);
            }

        }

        private void ListenTool()
        {
            toolBaiBanBi.toolConf.toolBasic.OnEnterTool += Check;
        }

        private void UnListenTool()
        {
            toolBaiBanBi.toolConf.toolBasic.OnEnterTool -= Check;
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
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalWriteConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalWriteConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalWriteConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalWriteConf) + " is null");
                }
            }
        }

#endif
    }
}


