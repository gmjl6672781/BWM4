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
    [Serializable]
    public struct TieZhiInfo
    {
        public ToolGoalConf toolTieZhi;
        public string writeMessae;
    }

    public class GoalWriteTieZhiConf : GoalUseToolConf
    {

        public ToolGoalConf toolPen;
        public List<TieZhiInfo> tieZhiTools;
        public Dictionary<ToolGoalConf, bool> dicTieZhiTools = new Dictionary<ToolGoalConf, bool>();

        private int writeCount = 0;

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            dicTieZhiTools.Clear();
            tieZhiTools.ForEach(t => dicTieZhiTools.Add(t.toolTieZhi, false));
            writeCount = 0;
        }


        /// <summary>
        /// 监听OnEnterMatch
        /// </summary>
        /// <param name="toolConf"> toolTieZhi</param>
        protected override void Check(ToolConf toolConf)
        {
            base.Check(toolConf);

            if (!BeforeCheck())
                return;

            tieZhiTools.ForEach(t =>
            {
                if (t.toolTieZhi.toolConf == toolConf && !dicTieZhiTools[t.toolTieZhi])
                {
                    WriteTieZhi(t);
                    writeCount++;
                    dicTieZhiTools[t.toolTieZhi] = true;
                    SetToolInfo(t.toolTieZhi);
                }
            });

            if (writeCount == tieZhiTools.Count)
            {
                AchieveGoal(true);
            }

        }

        private void WriteTieZhi(TieZhiInfo toolInfo)
        {
            ToolBasic tool = toolInfo.toolTieZhi.toolConf.toolBasic;
            TextMesh textMesh = tool.GetComponentInChildren<TextMesh>();
            textMesh.text = toolInfo.writeMessae;
        }

        private void ListenTool()
        {
            toolPen.toolConf.toolBasic.OnEnterTool += Check;
        }

        private void UnListenTool()
        {
            toolPen.toolConf.toolBasic.OnEnterTool -= Check;
        }

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            ListenTool();
            tieZhiTools.ForEach(t => t.toolTieZhi.toolConf.toolBasic.GetComponentInChildren<TextMesh>().text = "");
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            UnListenTool();
            tieZhiTools.ForEach(t => t.toolTieZhi.toolConf.toolBasic.GetComponentInChildren<TextMesh>().text = t.writeMessae);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalWriteTieZhiConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalWriteTieZhiConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalWriteTieZhiConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalWriteTieZhiConf) + " is null");
                }
            }
        }

#endif
    }
}

