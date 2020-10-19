using BeinLab.Util;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using BeinLab.VRTraing.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System.Collections.Generic;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{

    public class GoalBiaoJiTieZhiConf : GoalUseToolConf
    {
        public ToolGoalConf tieZhi;
        public ToolGoalConf tieZhiFlag;
        public ToolGoalConf tieZhiFlagRight;
        public List<ToolGoalConf> tieZhiFlagError;

        public override void OnTaskInit(TaskConf taskConf)
        {
            //Debug.Log("tiezhigoalInit-------------------------------tiezhigoalInit-------------------------------");
            base.OnTaskInit(taskConf);
            tieZhiFlagRight.toolConf.isLightFlash = true;
        }

        /// <summary>
        /// 监听OnEnterMatch
        /// </summary>
        /// <param name="toolConf"></param>
        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);

            if (!BeforeCheck())
                return;

            if(TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                foreach (ToolGoalConf t in tieZhiFlagError)
                {
                    if (toolConf == t)
                    {
                        AchieveGoal(false);
                        return;
                    }
                };
            }

            if (toolConf == tieZhiFlagRight.toolConf)
            {
                ToolBasic tool = tieZhi.toolConf.toolBasic;
                if (tool.catchHand)
                {
                    tool.catchHand.DetachObject(tool.gameObject);
                    tool.SetToolCatch(false);
                    SetToolInfo(tieZhi);
                    SetToolInfo(tieZhiFlag);
                    SetToolInfo(tieZhiFlagRight);
                }
                AchieveGoal(true);

                SketchMapDlg.Instance.SetHideTips(tool.gameObject.transform.GetChild(0).name);
            }        
        }


        private void ListenTool()
        {
            tieZhi.toolConf.toolBasic.OnEnterTool += Check;
        }

        private void UnListenTool()
        {
            tieZhi.toolConf.toolBasic.OnEnterTool -= Check;
        }

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            tieZhiFlagRight.toolConf.isLightFlash = false;
            ListenTool();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            tieZhiFlagRight.toolConf.isLightFlash = true;
            UnListenTool();
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalBiaoJiTieZhiConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalBiaoJiTieZhiConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalBiaoJiTieZhiConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalBiaoJiTieZhiConf) + " is null");
                }
            }
        }

#endif
    }
}

