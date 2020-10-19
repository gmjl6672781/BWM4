#if UNITY_EDITOR
using UnityEditor;
#endif
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Mgr;
using System;
using System.Collections.Generic;
using UnityEngine;
using BeinLab.Util;

namespace BeinLab.VRTraing.Conf
{
    [Serializable]
    public struct HighlightTool
    {
        public ToolConf toolConf;
        public bool isHighlight;
    }

    //任务目标，不分先后顺序,须有判断成功完成目标或者错误完成目标的功能
    public class TaskGoalConf : ScriptableObject
    {
        /// <summary>
        /// 任务目标描述
        /// </summary>
        public string goalName;
        public GoalType goalMode;
        /// <summary>
        /// 任务目标优先级
        /// </summary>
        public int priority;
        protected TaskConf targetTask;
        protected bool isAchieveGoal = false;
        public bool isAutoPlayStartAudio = true;
        public bool isAutoHighlightStartTool = true;
        public string goalKey;
        //培训模式语音 
        /// <summary>
        /// 提示培训模式任务目标开始语音
        /// </summary>
        [Tooltip("提示培训模式任务目标开始语音")]
        public List<string> goalTrainingStartAudioKeys;
        public List<string> goalTrainingErrorAudioKeys;
        public List<HighlightTool> goalTrainingStartHighlightTools;
        /// <summary>
        /// 提示考试模式任务目标开始语音
        /// </summary>
        [Tooltip("提示考试模式任务目标开始语音")]
        public List<string> goalExaminationStartAudioKeys;
        public List<string> goalExaminationErrorAudioKeys;
        public List<HighlightTool> goalExaminationStartHighlightTools;
        /// <summary>
        /// 必要目标，如果此目标不达成，则无法通过此任务
        /// </summary>
        //public bool isNecessaryGoal = true;
        /// <summary>
        /// 教学引导任务语音
        /// </summary>
        public List<string> goalTeachingStartAudioKeys;

        public event Action<bool> OnAchieveGoal;
        public float playDeltTime;
        /// <summary>
        /// 是否最后的任务目标，如果被先执行，则此任务失败
        /// </summary>
        public bool isFinalGoal = false;
        public virtual void ForceTip()
        {

        }

        private bool hasDone = false;

        public bool HasDone { get => hasDone; set => hasDone = value; }
        /// <summary>
        /// 任务目标初始化
        /// </summary>
        /// <param name="taskConf"></param>
        public virtual void OnTaskInit(TaskConf taskConf)
        {
            HasDone = false;
            targetTask = taskConf;
            isAchieveGoal = false;
        }

        private void PlayAudios(List<string> audios)
        {
            if (LanguageMgr.Instance && audios != null)
            {
                LanguageMgr.Instance.PlayAudioByKey(audios, playDeltTime);
            }
        }

        public void PlayStartAudio()
        {
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                if (goalMode != GoalType.OnlyTraining)
                    PlayAudios(goalExaminationStartAudioKeys);
            }
            else if (TaskManager.Instance.taskMode == TaskMode.Training)
            {
                if (goalMode != GoalType.OnlyExamination)
                    PlayAudios(goalTrainingStartAudioKeys);
                //Debug.Log("CurrentTask-----" + TaskManager.Instance.CurrentTask);
                //foreach (var item in goalTrainingStartAudioKeys)
                //{
                //    Debug.Log("PlayStartAudio-----" + item);
                //}
            }
            else
                PlayAudios(goalTeachingStartAudioKeys);
        }

        public void HighlightStartTool()
        {
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                goalExaminationStartHighlightTools.ForEach(t => t.toolConf.toolBasic.SetToolHighlight(t.isHighlight));
            }
            else if (TaskManager.Instance.taskMode == TaskMode.Training)
            {
                goalTrainingStartHighlightTools.ForEach(t => t.toolConf.toolBasic.SetToolHighlight(t.isHighlight));
            }
        }

        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public virtual void OnTaskStart(TaskConf taskConf)
        {
            HasDone = false;
            isAchieveGoal = false;
            OnGoalStart();

            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                if (goalMode == GoalType.OnlyTraining)
                {
                    HasDone = true;
                    isAchieveGoal = true;
                    targetTask.AchieveGoal(true);
                }
            }
            else
            {
                if (goalMode == GoalType.OnlyExamination)
                {
                    HasDone = true;
                    isAchieveGoal = true;
                    targetTask.AchieveGoal(true);
                }
            }

        }

        /// <summary>
        /// 任务中一直需要监测的事件：比如手柄
        /// </summary>
        /// <param name="targetTask"></param>
        public virtual void OnTaskDoing(TaskConf taskConf)
        {

        }

        /// <summary>
        /// 任务结束时需要的处理
        /// </summary>
        /// <param name="taskConf"></param>
        public virtual void OnTaskEnd(TaskConf taskConf)
        {
            //targetTask = null;
            HasDone = true;
        }

        /// <summary>
        /// 完成任务目标发送
        /// </summary>
        /// <param name="success">是否成功完成任务目标</param>
        protected virtual void AchieveGoal(bool success)
        {
            if (!targetTask)
                return;
            //Debug.LogFormat("{0}完成任务目标:{1}", success ? "成功" : "失败", goalName);
            HasDone = true;
            isAchieveGoal = success;
            OnAchieveGoal_(success);
            targetTask.AchieveGoal(success);
        }

        public virtual void OnGoalStart()
        {
            if (isAutoPlayStartAudio)
                PlayStartAudio();
            if (isAutoHighlightStartTool)
                HighlightStartTool();
        }

        /// <summary>
        /// 目标成功触发事件，一般时提示信息
        /// </summary>
        protected virtual void OnAchieveGoal_(bool success)
        {
            //语音
            if (!success)
            {
                if (TaskManager.Instance.taskMode == TaskMode.Examination)
                {
                    PlayAudios(goalExaminationErrorAudioKeys);

                }
                else if (TaskManager.Instance.taskMode == TaskMode.Training)
                {
                    PlayAudios(goalTrainingErrorAudioKeys);
                }
            }


            //动画
            if (OnAchieveGoal != null)
                OnAchieveGoal(success);
        }


        public bool CheckAchieveGoal()
        {
            return isAchieveGoal;
        }

        protected bool BeforeCheck()
        {
            //已完成不再检测
            if (isAchieveGoal)
                return false;

            if (TaskManager.Instance.CurrentTask != targetTask)
                return false;

            if (TaskManager.Instance.CurrentTask == targetTask &&
                 (targetTask.taskState == TaskState.End))
                return false;

            return true;
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/TaskGoalConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<TaskGoalConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<TaskGoalConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(TaskGoalConf) + " is null");
                }
            }
        }
#endif
    }
}

