using BeinLab.Util;
using BeinLab.VRTraing.Mgr;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 插拔插头
    /// </summary>
    public class GoalToggleChaTou : TaskGoalConf
    {
        /// <summary>
        /// 初始化的位置
        /// </summary>
        public Vector3 initPos;
        /// <summary>
        /// 初始化的角度
        /// </summary>
        public Vector3 initAngle;
        /// <summary>
        /// 结束的位置
        /// </summary>
        public Vector3 endPos;
        /// <summary>
        /// 结束的角度
        /// </summary>
        public Vector3 endAngle;
        /// <summary>
        /// 目标工具配置
        /// </summary>
        public ToolConf targetTool;
        /// <summary>
        /// 触发的工具，例如塑料翘板等等
        /// </summary>
        public ToolConf triggerTool;
        public bool isExit = false;
        /// <summary>
        /// 是否使用手
        /// </summary>
        public bool isUseHand = false;
        /// <summary>
        /// 动效时间
        /// </summary>
        public float doTime = 0.2f;
        private bool isFinsh = false;
        public bool isAutoComplete = false;
        //public bool isHideController = false;
        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            OnTaskStart(taskConf);
        }

        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            SetState(initPos, initAngle, 0);
            isFinsh = false;
            //targetTool.toolBasic.SetToolHighlight(true);
            if (isAutoComplete)
            {
                ToAchieve();
            }
            else if (isUseHand)
            {
                if (targetTool && targetTool.toolBasic)
                {
                    targetTool.toolBasic.OnPressDown += OnPressDownTool;
                }
            }
            else
            {
                if (triggerTool != null && triggerTool.toolBasic!=null)
                {
                    if (isExit)
                    {
                        triggerTool.toolBasic.OnExitTool += OnExitTool;
                    }
                    else
                    {
                        triggerTool.toolBasic.OnEnterTool += OnEnterTool;
                    }
                }
            }
        }
        /// <summary>
        /// 当进入某个工具时
        /// </summary>
        /// <param name="obj"></param>
        private void OnEnterTool(ToolConf obj)
        {
            if (!isFinsh && obj == targetTool)
            {
                targetTool.toolBasic.SetToolHighlight(false);
                triggerTool.toolBasic.OnEnterTool -= OnEnterTool;
                //SetState(endPos, endAngle, doTime);
                ToAchieve();
            }
        }

        /// <summary>
        /// 当离开某个工具
        /// </summary>
        /// <param name="obj"></param>
        private void OnExitTool(ToolConf obj)
        {
            if (!isFinsh && targetTool == obj)
            {
                //targetTool.toolBasic.SetToolHighlight(false);
                triggerTool.toolBasic.OnExitTool -= OnExitTool;
                //SetState(endPos, endAngle, doTime);
                ToAchieve();
            }
        }

        /// <summary>
        /// 手柄按下工具
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnPressDownTool(Hand arg1, ToolConf arg2)
        {
            //Debug.LogError("OnPressDownTool");
            if (isUseHand && !isFinsh && targetTool && targetTool.toolBasic)
            {
                //SetState(endPos, endAngle, doTime);
                //targetTool.toolBasic.SetToolHighlight(false);
                targetTool.toolBasic.OnPressDown -= OnPressDownTool;
                //SetState(endPos, endAngle, doTime);
                ToAchieve();
            }
        }

        public void ToAchieve()
        {
            if (BeforeCheck())
            {
                isFinsh = true;
                AchieveGoal(true);
                targetTool.toolBasic.SetToolHighlight(false);
                SetState(endPos, endAngle, doTime);
                if (triggerTool && triggerTool.toolBasic.catchHand)
                {
                    //triggerTool.toolBasic.SetToolHighlight(false);
                    VRHandHelper.Instance.ShockHand(triggerTool.toolBasic.catchHand,
                        (ushort)(2500));
                }
            }
        }

        /// <summary>
        /// 任务结束时
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            SetState(endPos, endAngle, 0);
            isFinsh = true;
        }

        /// <summary>
        /// 设置初始化
        /// </summary>
        public void SetState(Vector3 pos, Vector3 angle, float doTime = 0.2f)
        {
            if (targetTool && targetTool.toolBasic)
            {
                targetTool.toolBasic.transform.DOKill();
                targetTool.toolBasic.transform.DOLocalMove(pos, doTime);
                targetTool.toolBasic.transform.DOLocalRotate(angle, doTime);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalToggleChaTou", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalToggleChaTou>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalToggleChaTou>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalToggleChaTou) + " is null");
                }
            }
        }
#endif
    }
}