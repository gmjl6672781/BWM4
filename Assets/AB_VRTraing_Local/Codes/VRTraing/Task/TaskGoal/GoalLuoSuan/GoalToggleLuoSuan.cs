using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 螺栓的开关目标
    /// </summary>
    public class GoalToggleLuoSuan : TaskGoalConf
    {
        public LuoSuanState initState;
        public LuoSuanState endState;
        /// <summary>
        /// 目标的螺栓
        /// </summary>
        public ToolConf luosuanTool;
        /// <summary>
        /// 对应的扳手
        /// </summary>
        public ToolConf banshou;
        /// <summary>
        /// 目标角度，就是达到多少角度的时候，这个螺栓将改变形态
        /// 360为1圈，720为两圈
        /// </summary>
        public float targetAngle = 720;
        /// <summary>
        /// 正向拧还是反向拧
        /// 默认值1代表为卸螺丝
        /// 要拧紧螺栓要设置为-1
        /// </summary>
        public int direct = 1;

        public override void ForceTip()
        {
            base.ForceTip();
            luosuanTool.toolBasic.SetToolHighlight(true);
            luosuanTool.toolBasic.gameObject.SetActive(true);
            PlayStartAudio();
        }
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (luosuanTool.toolBasic is ToolLuoSuan)
            {
                ToolLuoSuan tl = luosuanTool.toolBasic as ToolLuoSuan;
                tl.InitLuoSuan(initState, targetAngle, direct);
                tl.OnChangeState += OnChangeState;
            }
            else
            {
                Debug.LogError("Target is not luosuan");
            }
            if (banshou.toolBasic is ToolBanShou)
            {
                (banshou.toolBasic as ToolBanShou).targetState = endState;
            }
            else
            {
                Debug.LogError("Target is not banshou");
            }
        }
        /// <summary>
        /// 当螺栓改变状态的时候
        /// 完成目标
        /// </summary>
        private void OnChangeState()
        {
            ToolLuoSuan tl = luosuanTool.toolBasic as ToolLuoSuan;
            tl.InitLuoSuan(endState, targetAngle, direct);
            tl.OnChangeState -= OnChangeState;
            luosuanTool.toolBasic.SetToolHighlight(false);
            (banshou.toolBasic as ToolBanShou).Complete();
            if (BeforeCheck())
            {
                AchieveGoal(true);
            }
        }

        /// <summary>
        /// 退出工具
        /// </summary>
        /// <param name="obj"></param>
        //private void OnExitTool(ToolConf obj)
        //{
        //    if (obj == luosuanTool)
        //    {
        //        banshou.toolBasic.SetHoverExitTool(obj);
        //    }
        //}

        /// <summary>
        /// 指定的扳手，接触到了某个螺栓
        /// 触发扳手的hoverTool方法
        /// </summary>
        /// <param name="obj"></param>
        //private void OnEnterTool(ToolConf obj)
        //{
        //    if (obj == luosuanTool)
        //    {
        //        ///如果没有完成状态
        //        if (!(obj.toolBasic as ToolLuoSuan).CheckIsComplete(endState))
        //        {
        //            banshou.toolBasic.SetHoverTool(obj);
        //        }
        //    }
        //}

        /// <summary>
        /// 任务结束
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            luosuanTool.toolBasic.SetToolHighlight(false);

            if (luosuanTool.toolBasic is ToolLuoSuan)
            {
                ToolLuoSuan tl = luosuanTool.toolBasic as ToolLuoSuan;
                tl.InitLuoSuan(endState, targetAngle, direct);
                tl.OnChangeState -= OnChangeState;
            }
            else
            {
                Debug.LogError("Target is not luosuan");
            }
            //if (banshou.toolBasic is ToolBanShou)
            //{
            //    banshou.toolBasic.OnEnterTool -= OnEnterTool;
            //    banshou.toolBasic.OnExitTool -= OnExitTool;
            //}
            //else
            //{
            //    Debug.LogError("Target is not banshou");
            //}
        }



#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalToggleLuoSuan", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalToggleLuoSuan>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalToggleLuoSuan>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalToggleLuoSuan) + " is null");
                }
            }
        }
#endif
    }
}