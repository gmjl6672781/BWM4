#if UNITY_EDITOR
using UnityEditor;
#endif
using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 当问答题做错的时候，播放动效
    /// </summary>
    public class GoalPlayDynamic : TaskGoalConf
    {
        /// <summary>
        /// 播放的动效
        /// </summary>
        public List<DynamicConf> playerDynamic;
        private Timer playerTimer;
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (playerTimer != null)
            {
                playerTimer = TimerMgr.Instance.DestroyTimer(playerTimer);
            }
            taskConf.OnDoErrorGoal += OnDoErrorGoal;
        }
        /// <summary>
        /// 当此任务的某个任务目标错误时，开始播放对应的动效
        /// </summary>
        /// <param name="obj"></param>
        private void OnDoErrorGoal(TaskGoalConf obj)
        {
            if (playerTimer != null)
            {
                playerTimer = TimerMgr.Instance.DestroyTimer(playerTimer);
            }
            if (obj == this)
            {
                AchieveGoal(true);
            }
            else
            {
                float doTime = 0;
                for (int i = 0; i < playerDynamic.Count; i++)
                {
                    DynamicActionController.Instance.DoDynamic(playerDynamic[i]);
                    if (doTime < playerDynamic[i].showTime)
                    {
                        doTime = playerDynamic[i].showTime;
                    }
                }
                playerTimer = TimerMgr.Instance.CreateTimer(OnPlayComplete, doTime);
            }
        }
        /// <summary>
        /// 播放完毕
        /// </summary>
        private void OnPlayComplete()
        {
            AchieveGoal(true);
        }
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            if (playerTimer != null)
            {
                playerTimer = TimerMgr.Instance.DestroyTimer(playerTimer);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalPlayDynamic", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalPlayDynamic>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalPlayDynamic>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalPlayDynamic) + " is null");
                }
            }
        }
#endif
    }
}