using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.Mgr;
using Valve.VR.InteractionSystem;
using BeinLab.VRTraing.Controller;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 按下PAD键完成事件
    /// </summary>
    public class GoalTeleportConf : GoalVRInputActionConf
    {
        ///// <summary>
        ///// 初始化
        ///// </summary>
        ///// <param name="taskConf"></param>
        //public override void OnTaskInit(TaskConf taskConf)
        //{
        //    base.OnTaskInit(taskConf);

        //}
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            TaskManager.Instance.taskMode = TaskMode.Teaching;
            if (Teleport.instance)
            {
                Teleport.instance.OnTeleport += OnTeleport;
            }
        }
        /// <summary>
        /// 按下位移按键
        /// </summary>
        /// <param name="isJumpSuccess"></param>
        private void OnTeleport(bool isJumpSuccess)
        {
            //if (PopDlg.Instance)
            //{
            //    PopDlg.Instance.OnTaskEnd(targetTask);
            //}
            if (isAchieveGoal)
                return;
            ///跳跃成功
            if (isJumpSuccess)
            {
                if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
                {
                    AchieveGoal(true);
                }
            }
            ///跳跃失败
            else
            {
                if (LanguageMgr.Instance)
                {
                    LanguageMgr.Instance.PlayAudioByKey(goalTrainingStartAudioKeys,playDeltTime);
                }
            }
        }
        public override void OnTaskDoing(TaskConf taskConf)
        {
            //base.OnTaskDoing(taskConf);
        }
        //public override void Check(TaskConf taskConf)
        //{
        //    //base.Check(taskConf);

        //}
        public override void OnTaskEnd(TaskConf taskConf)
        {

            if (Teleport.instance)
            {
                Teleport.instance.OnTeleport -= OnTeleport;
            }
            base.OnTaskEnd(taskConf);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalVRInputTeleportConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalTeleportConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalTeleportConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalTeleportConf) + " is null");
                }
            }
        }
#endif
    }
}