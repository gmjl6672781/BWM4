using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 将某工具放入凹槽内的目标
    /// </summary>
    public class GoalPutToAoCao : TaskGoalConf
    {
        public ToolConf targetTool;
        public PutTooConf aocao;
        public float tipWatieTime = 0;
        private Timer watieTipTimer;
        public bool isHighLightBox = false;
        public override void ForceTip()
        {
            base.ForceTip();
            targetTool.toolBasic.SetToolHighlight(true);
            if (isHighLightBox)
            {
                aocao.triggerTool.toolBasic.SetToolHighlight(true);
                aocao.triggerTool.toolBasic.gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            targetTool.toolBasic.OnPutAoCao += OnPutAoCao;
            if (watieTipTimer != null)
            {
                TimerMgr.Instance.DestroyTimer(watieTipTimer);
            }
            if (tipWatieTime > 0)
            {
                watieTipTimer = TimerMgr.Instance.CreateTimer(TipPlayer, tipWatieTime);
            }
            else
            {
                TipPlayer();
            }
        }

        private void TipPlayer()
        {
            if (watieTipTimer != null)
            {
                watieTipTimer = TimerMgr.Instance.DestroyTimer(watieTipTimer);
            }
            PlayStartAudio();
            HighlightStartTool();
        }

        /// <summary>
        /// 将物体放到凹槽中，任务结束
        /// </summary>
        /// <param name="obj"></param>
        private void OnPutAoCao(PutTooConf obj)
        {
            if (obj == aocao)
            {
                AchieveGoal(true);
                //targetTool.toolBasic.transform.SetParent(null);
                targetTool.toolBasic.SetTrigger(false);
                targetTool.toolBasic.OnPutAoCao -= OnPutAoCao;
                targetTool.toolBasic.SetToolHighlight(false);
                aocao.triggerTool.toolBasic.SetToolHighlight(false);
                if (watieTipTimer != null)
                {
                    watieTipTimer = TimerMgr.Instance.DestroyTimer(watieTipTimer);
                }
            }
        }

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            targetTool.toolBasic.OnPutAoCao -= OnPutAoCao;
            targetTool.toolBasic.SetToolHighlight(false);
            if (watieTipTimer != null)
            {
                watieTipTimer = TimerMgr.Instance.DestroyTimer(watieTipTimer);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalPutToAoCao", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalPutToAoCao>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalPutToAoCao>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalPutToAoCao) + " is null");
                }
            }
        }
#endif
    }
}