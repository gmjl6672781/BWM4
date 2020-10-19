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
    /// 调节轧带枪档位
    /// </summary>
    public class GoalUseZhaDaiQiangConf : GoalUseToolConf
    {
        public ToolGoalConf toolZhaDaiQiang;
        public List<ToolGoalConf> toolZhaDais;
        public Dictionary<ToolGoalConf, bool> dicCheckTools = new Dictionary<ToolGoalConf, bool>();
        private int checkCount = 0;


        float currentx;
        float lastx;
        ToolZhaDaiQiang tool;
        protected override void Check(Hand hand, ToolConf toolConf)
        {
            base.Check(hand, toolConf);
            if (!BeforeCheck())
                return;
            //调整扎带枪档位
            ////Debug.Log("//调整扎带枪档位");
            GearDlg.Instance.ShowDlg();
            
        }
        protected override void Check(ToolConf toolConf)
        {

            base.Check(toolConf);
            if (!BeforeCheck())
                return;
            //扎带枪当前选中档位判断
            if (GearDlg.Instance.arcSroll.curChosenID != 3)
            {
                LanguageMgr.Instance.PlayAudioByKey("T1062", playDeltTime);
                return;
            }
            //if (GearDlg.Instance.fs.NowItemID != 3)
            //{
            //    LanguageMgr.Instance.PlayAudioByKey("T1062", playDeltTime);
            //    return;
            //}
                

            toolZhaDais.ForEach(t =>
            {
                if (t.toolConf == toolConf && !dicCheckTools[t])
                {
                    checkCount++;
                    dicCheckTools[t] = true;
                    SetToolInfo(t);
                }
            });

            if (checkCount == toolZhaDais.Count)
            {
                SetToolInfo(toolZhaDaiQiang);
                AchieveGoal(true);
            }
        }

        protected void OnDetach(Hand hand, ToolConf toolConf)
        {
            GearDlg.Instance.HideDlg();
        }
        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            base.OnTaskInit(taskConf);
            dicCheckTools.Clear();
            toolZhaDais.ForEach(t => dicCheckTools.Add(t, false));
            checkCount = 0;
        }

        private void ListenTool()
        {
            toolZhaDaiQiang.toolConf.toolBasic.OnEnterTool += Check;
            toolZhaDaiQiang.toolConf.toolBasic.OnDetach += OnDetach;
            toolZhaDaiQiang.toolConf.toolBasic.OnCatch += Check;
        }

        private void UnListenTool()
        {
            toolZhaDaiQiang.toolConf.toolBasic.OnEnterTool -= Check;
            toolZhaDaiQiang.toolConf.toolBasic.OnDetach -= OnDetach;
            toolZhaDaiQiang.toolConf.toolBasic.OnCatch -= Check;

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
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalUseZhaDaiQiangConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalUseZhaDaiQiangConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalUseZhaDaiQiangConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalUseZhaDaiQiangConf) + " is null");
                }
            }
        }

#endif
    }
}


