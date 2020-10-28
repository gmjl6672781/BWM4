using BeinLab.Util;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.Controller;
using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.VRTraing.UI;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Conf
{
    public enum GoalReslut
    {
        Init = -1,
        Error = 0,
        Sucess = 1
    }
    /// <summary>
    /// 流程没有问题了，如何根据任务类型来设定关系
    /// </summary>
    public class TaskConf : LinkConf
    {
        /// <summary>
        /// 任务的名称
        /// </summary>
        public string taskName;
        /// <summary>
        /// 此节点的下标
        /// </summary>

        public int index;
        public string keyTitle;
        public string keyTip;
        /// <summary>
        /// 任务是否可以跳步或者记录
        /// </summary>
        public bool isCanRecord;
        /// <summary>
        /// 任务是否完成
        /// </summary>
        private bool isPass = false;
        /// <summary>
        /// 分数
        /// </summary>
        public float score;
        public float traingScore = 5;
        public float examScore = 10;
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskType taskType;

        /// <summary>
        /// 任务目标，拆分任务，所有任务目标完成即认为完成任务
        /// </summary>
        public List<TaskGoalConf> taskGoals;
        public List<ToolTaskConf> taskTools;
        /// <summary>
        /// 任务未开始状态
        /// </summary>
        public event Action<TaskConf> OnTaskInit;
        /// <summary>
        /// 任务开始时事件
        /// </summary>
        public event Action<TaskConf> OnTaskStart;
        /// <summary>
        /// 任务执行中事件
        /// </summary>
        public event Action<TaskConf> OnTaskDoing;
        /// <summary>
        /// 任务结束时事件
        /// </summary>
        public event Action<TaskConf> OnTaskEnd;
        /// <summary>
        /// 动画播放
        /// </summary>
        public List<DynamicConf> dynamicConfs;
        public float delayTimeStartToDoing = 3f;
        public float delayTimeToStart = 3f;
        public bool isPlayAudioSuccessTask;
        public bool isPlayAudioOnError = true;
        public bool isPlayAudioOnErrorInExaming = true;

        public List<string> audioOnErrorKeys;
        private List<string> successTaskAudioKeys = new List<string>() { "T1000" };
        public List<string> sceneAudioKeys;

        public event Action<TaskConf> OnTaskAchieve;
        /// <summary>
        /// 将结果异常的任务目标通知出去，异常的任务目标是指：做错的任务，没做但是必要要做的任务
        /// </summary>
        public event Action<TaskGoalConf> OnDoErrorGoal;
        public event Action OnAchiveUnDone;
        public bool isJumpBigOnGoalError = true;
        public bool isRePlayStartAudio;
        public bool isPlayDynOnTaskError = false;
        public List<DynamicConf> errorTaskDynamics;

        public bool isAutoPopQuest = false;
        public float watieTime = 10;
        public bool isJustPlayAudio = false;
        public float reDoTime = 10;
        private bool isFirstReDo = true;
        public float errorSound = -1;
        /// <summary>
        /// 当一个任务目标完成时，通知任务系统
        /// 如果此目标的结果是对的，则遍历任务是否存在尚未执行的任务目标
        /// 如果全部目标都已完成，则此任务结束。如果存在未操作的任务目标，则返回，继续完成目标
        /// 如果此目标错误，则记录目标
        /// </summary>
        /// <param name="success"></param>
        private bool isHasPlayEffect = false;
        public void AchieveGoal(bool success, bool isForceJump = false)
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "AchieveGoal");
            dynTimer = TimerMgr.Instance.DestroyTimer(dynTimer);
            //bool isAllComplete = true;


            ///如果先做了最后做的任务
            bool isDoLast = false;
            bool isAllHasDone = true;
            foreach (var result in taskGoals)
            {
                if (result.isFinalGoal)
                {
                    if (result.HasDone)
                    {
                        isDoLast = true;
                    }
                }
                else
                {
                    if (!result.HasDone)
                    {
                        isAllHasDone = false;
                        break;
                    }
                }
            }
            //List<DynamicConf> effects = errorTaskDynamics;
            bool isReDoAll = false;
            ///如果做了最后一步，但是其他的没做完
            if (isDoLast && !isAllHasDone)
            {
                success = false;
                isForceJump = true;
                ///如果是考试模式，播放对应的错误动效，然后结束任务
                if (TaskManager.Instance.taskMode == TaskMode.Examination)
                {
                }
                ///如果是培训模式，播放对应的错误动效，然后引导错误流程
                else
                {
                    isReDoAll = true;
                }
            }


            foreach (var result in taskGoals)
            {
                ///存在没有完成的任务
                if (!result.HasDone)
                {
                    if (isRePlayStartAudio)
                    {
                        result.OnGoalStart();
                        //PlayStartAudio(result);                     
                    }
                    ///如果是非强制跳跃，代表继续执行此任务
                    if (!isForceJump)
                    {
                        return;
                    }
                    success = false;
                }
                else if (!result.CheckAchieveGoal())
                {
                    success = false;
                }
            }
            float dynShowTime = 0;
            if (!success && isPlayDynOnTaskError && !isHasPlayEffect)
            {
                for (int i = 0; i < errorTaskDynamics.Count; i++)
                {
                    if (dynShowTime < errorTaskDynamics[i].delayTime + errorTaskDynamics[i].showTime)
                    {
                        dynShowTime = errorTaskDynamics[i].delayTime + errorTaskDynamics[i].showTime;
                    }
                    DynamicActionController.Instance.DoDynamic(errorTaskDynamics[i]);
                }
                isHasPlayEffect = true;
                if (isPlayAudioOnError)
                {
                    if (!success && isPlayAudioOnError && audioOnErrorKeys != null && audioOnErrorKeys.Count > 0)
                    {
                        if (isPlayAudioOnErrorInExaming|| TaskManager.Instance.taskMode != TaskMode.Examination)
                        {
                            PlayAudios(audioOnErrorKeys);
                            if (dynShowTime < errorSound)
                            {
                                dynShowTime = errorSound;
                            }
                        }
                        //if (TaskManager.Instance.taskMode != TaskMode.Examination)
                        //{
                        //    return;
                        //}
                    }
                }
            }

            if (success && isPlayAudioSuccessTask)
            {
                PlayAudios(successTaskAudioKeys);
                if (TaskOverDynamicConf.Count != 0)
                {
                    TaskOverAction();
                }
            }
            if (isReDoAll)
            {
                float wtime = isFirstReDo ? reDoTime : 0;
                TimerMgr.Instance.CreateTimer(() =>
                {
                    isFirstReDo = false;
                    OnAchiveUnDone?.Invoke();
                    foreach (var result in taskGoals)
                    {
                        if (!result.HasDone)
                        {
                            result.ForceTip();
                            return;
                        }
                    }
                }, wtime);
                return;
            }
            //Debug.Log("CurrentTask-------------" + TaskManager.Instance.CurrentTask);
            //Debug.Log("dynShowTime-------------"+dynShowTime);
            //dynTimer = TimerMgr.Instance.DestroyTimer(dynTimer);
            dynTimer = TimerMgr.Instance.CreateTimer(() =>
            {
                DoNextTask(success);
            }, dynShowTime);
        }

        /// <summary>
        /// 培训模式下，将会依次执行每一个步骤
        /// 考试模式下，如果做错，跳过此任务，执行下一个任务
        /// </summary>
        /// <param name="isAllComplete"></param>
        public void DoNextTask(bool isAllComplete)
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "DoNextTask");
            dynTimer = TimerMgr.Instance.DestroyTimer(dynTimer);
            this.IsPass = isAllComplete;
            //if (parent && !isAllComplete)
            //{
            //    (parent as TaskConf).IsPass = false;
            //    Debug.LogError(parent);
            //}
            TaskManager.Instance.CurrentTask.taskState = TaskState.End;
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                if (!isAllComplete && isJumpBigOnGoalError)
                {
                    TaskManager.Instance.ExecuteNextTaskBig();
                }
                else
                {
                    TaskManager.Instance.ExecuteNextTaskLittle();
                }
            }
            else
            {
                TaskManager.Instance.ExecuteNextTaskLittle();
            }
        }

        //public void PlayStartAudio(TaskGoalConf taskGoal)
        //{
        //    if (TaskManager.Instance.taskMode == TaskMode.Examination)
        //        PlayAudios(taskGoal.goalExaminationStartAudioKeys);
        //    else if (TaskManager.Instance.taskMode == TaskMode.Training)
        //        PlayAudios(taskGoal.goalTrainingStartAudioKeys);
        //    else
        //        PlayAudios(taskGoal.goalTeachingStartAudioKeys);
        //}


        /// <summary>
        /// 跳过任务
        /// </summary>
        public void SkipTask()
        {
            Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "SkipTask");
            //所有任务目标已达成,正确执行任务
            //Debug.LogFormat("跳过任务:{0} Index:{1}", this.taskName, this.index);
            //TaskManager.Instance.CurrentTask.taskState = TaskState.End;
            //TaskManager.Instance.ExecuteNextTask();
            //AchieveGoal(false);
            ToCompleteTask();
        }
        /// <summary>
        /// 考试模式下提交结果
        /// </summary>
        public void ToCompleteTask()
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "ToCompleteTask");
            AchieveGoal(false, true);
        }

        public bool IsChoise()
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "IsChoise");
            bool isSelection = false;
            for (int i = 0; i < taskGoals.Count; i++)
            {
                if (taskGoals[i] is GoalShowChoiceDlgConf)
                {
                    isSelection = true;
                    break;
                }
            }
            return isSelection;
        }
        private void PlayAudios(List<string> audios)
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "PlayAudios");
            //foreach (var item in audios)
            //{
            //    Debug.Log("-----" + "PlayAudios---" + item);
            //}
            if (LanguageMgr.Instance && audios != null)
            {
                LanguageMgr.Instance.PlayAudioByKey(audios);
            }
        }

        /// <summary>
        /// 判断是否完成任务所有目标
        /// </summary>
        /// <param name="answer"></param>
        private void SendAnswer(int answer)
        {
            Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "SendAnswer");
            if (answer == 1)
            {
                if (TaskManager.Instance.taskMode == TaskMode.Examination)
                {
                    ToCompleteTask();
                    return;
                }
                foreach (var result in taskGoals)
                {
                    if (!result.HasDone)
                    {
                        result.PlayStartAudio();
                        result.HighlightStartTool();
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 步骤异常
        /// </summary>
        /// <param name="isComplete"></param>
        //public void SetTaskComplete(bool isComplete)
        //{
        //    bool isAllComplete = true;
        //    foreach (TaskGoalConf taskGoal in taskGoals)
        //    {
        //        //播放语音
        //        if (!taskGoal.CheckAchieveGoal())
        //        {
        //            isAllComplete = false;
        //            break;
        //        }
        //    }
        //    if (!isAllComplete)
        //    {
        //        float doDynTime = 0;
        //        for (int i = 0; i < dynamicConfs.Count; i++)
        //        {
        //            float dynTime = dynamicConfs[i].delayTime + dynamicConfs[i].showTime;
        //            if (doDynTime < dynTime)
        //            {
        //                doDynTime = dynTime;
        //            }
        //            DynamicActionController.Instance.DoDynamic(dynamicConfs[i]);
        //        }
        //        TimerMgr.Instance.CreateTimer(DoTaskError, doDynTime);
        //    }
        //    else
        //    {
        //        DoTaskRight();
        //    }

        //}
        private void SortGoals()
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "SortGoals");
            taskGoals.Sort((x, y) =>
            {
                if (x.priority > y.priority)
                    return 1;
                else if (x.priority < y.priority)
                    return -1;
                else
                    return 0;
            });
        }
        
        private void DoTaskRight()
        {
            //所有任务目标已达成,正确执行任务
            Debug.LogFormat("成功完成任务:{0} Index:{1}", this.taskName, this.index);
            this.IsPass = true;
            TaskManager.Instance.CurrentTask.taskState = TaskState.End;
            TaskManager.Instance.ExecuteNextTaskLittle();
        }

        private void DoTaskError()
        {
            //考试模式直接执行下一个任务，培训模式则不处理
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                Debug.LogFormat("失败完成任务:{0} Index:{1}", this.taskName, this.index);
                this.IsPass = false;
                TaskManager.Instance.CurrentTask.taskState = TaskState.End;
                TaskManager.Instance.ExecuteNextTaskBig();
            }
        }

        //事件源，供内部调用
        private void _OnTaskInit(TaskConf taskConf)
        {
            //Debug.LogFormat("任务:{0} TaskInit Index:{1} 时间：{2}", this.taskName, this.index, Time.time);

            //初始化
            IsPass = false;
            isFirstReDo = true;
            SortGoals();

            taskTools.ForEach(t => t.OnTaskInit(this));
            taskGoals.ForEach(t => t.OnTaskInit(this));

            if (OnTaskInit != null)
            {
                OnTaskInit(taskConf);
            }
        }

        private void _OnTaskStart(TaskConf taskConf)
        {
            isHasPlayEffect = false;
            isFirstReDo = true;

            //Debug.LogFormat("任务:{0} OnTaskStart Index:{1} 时间：{2}", this.taskName, this.index, Time.time);
            LanguageMgr.Instance.OnPlayComplete -= OnPlayComplete;

            JudgementQuestionDlg.Instance.SendAnswer += SendAnswer;

            if (sceneAudioKeys.Count != 0)
            {
                PlayAudios(sceneAudioKeys);
            }
            foreach (var result in taskGoals)
            {
                if (!result.HasDone)
                {
                    result.OnGoalStart();
                    break;
                }
            }

            TaskManager.Instance.StartDoingCoroutine(this);
            ///初始化目标的状态，将目标的信息清空
            //taskGoals.ForEach(goal => goalReslutList[goal] = GoalReslut.Init);
            taskGoals.ForEach(t => t.OnTaskStart(this));
            taskTools.ForEach(t => t.OnTaskStart(this));
            //通知工具系统
            if (OnTaskStart != null)
            {
                OnTaskStart(taskConf);
            }

            if (taskGoals == null || taskGoals.Count == 0)
            {
                if (isJustPlayAudio)
                {
                    LanguageMgr.Instance.OnPlayComplete += OnPlayComplete;
                }
                else
                {
                    AchieveGoal(true);
                }
            }

            float doDynTime = 0;
            for (int i = 0; i < dynamicConfs.Count; i++)
            {
                float dynTime = dynamicConfs[i].delayTime + dynamicConfs[i].showTime;
                if (doDynTime < dynTime)
                {
                    doDynTime = dynTime;
                }
                DynamicActionController.Instance.DoDynamic(dynamicConfs[i]);
            }
            if (isAutoPopQuest)
            {
                if (judgementDlgConf)
                {
                    if (watieTimer != null)
                    {
                        TimerMgr.Instance.DestroyTimer(watieTimer);
                    }
                    watieTimer = TimerMgr.Instance.CreateTimer(() =>
                    {
                        if (judgementDlgConf && TaskManager.Instance.CurrentTask == this)
                        {
                            //JudgementQuestionDlg.Instance.ShowDlg(judgementDlgConf);
                            VRHandHelper.Instance.AddHint(name, Player.instance.leftHand,
                                judgementDlgConf.vr_Action_Button, "Tips");
                        }
                    }, watieTime);
                }
            }
        }

        private void OnPlayComplete()
        {
            //Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "OnPlayComplete");
            LanguageMgr.Instance.OnPlayComplete -= OnPlayComplete;
            if (isJustPlayAudio&&TaskManager.Instance.CurrentTask==this)
            {
                AchieveGoal(true);
            }

        }

        private Timer watieTimer;
        private void _OnTaskDoing(TaskConf taskConf)
        {
            //Debug.LogFormat("任务:{0} OnTaskDoing Index:{1} 时间：{2}", this.taskName, this.index, Time.time);
            taskGoals.ForEach(t => t.OnTaskDoing(this));
            taskTools.ForEach(t => t.OnTaskDoing(this));

            if (VRHandHelper.Instance.VRInput_GrabGrip.GetKeyDown(Valve.VR.SteamVR_Input_Sources.LeftHand))
            {
                if (judgementDlgConf)
                {
                    VRHandHelper.Instance.RemoveHint(name);
                    JudgementQuestionDlg.Instance.ShowDlg(judgementDlgConf);
                    if (watieTimer != null)
                    {
                        TimerMgr.Instance.DestroyTimer(watieTimer);
                    }
                }
            }

            if (OnTaskDoing != null)
            {
                OnTaskDoing(taskConf);
            }
        }

        public List<DynamicConf> TaskOverDynamicConf = new List<DynamicConf>();
        private void TaskOverAction()
        {
            float time = 0;
            TimerMgr.Instance.CreateTimer(() =>
            {
                foreach (var item in TaskOverDynamicConf)
                {
                    float curItemTime = item.delayTime + item.showTime;
                    if (time < curItemTime)
                        time = curItemTime;
                    DynamicActionController.Instance.DoDynamic(item);
                }
            }, time);
        }

        private void _OnTaskEnd(TaskConf taskConf)
        {
            //Debug.LogFormat("任务:{0} OnTaskEnd Index:{1} 时间：{2}", this.taskName, this.index, Time.time);
            JudgementQuestionDlg.Instance.SendAnswer -= SendAnswer;
            if (judgementDlgConf)
                JudgementQuestionDlg.Instance.HideDlg();

            taskGoals.ForEach(t => t.OnTaskEnd(this));
            taskTools.ForEach(t => t.OnTaskEnd(this));
            if (OnTaskEnd != null)
            {
                OnTaskEnd(taskConf);
            }
            if (watieTimer != null)
            {
                TimerMgr.Instance.DestroyTimer(watieTimer);
            }
            VRHandHelper.Instance.RemoveHint(name);
        }


        //任务状态
        private TaskState mTaskState = TaskState.UnInit;
        public bool isCanPopDlg = true;
        public JudgementQuestionConf judgementDlgConf;
        private Timer dynTimer;
        public bool isForceShow = false;
        public bool isShowTaskGoalsMsg = true;
        public TaskState taskState
        {
            get
            {
                return mTaskState;
            }
            set
            {
                TaskState oldState = mTaskState;
                mTaskState = value;
                if (mTaskState != TaskState.Doing)
                    if (TaskManager.Instance)
                        TaskManager.Instance.StopDoingCoroutine();
                switch (mTaskState)
                {
                    case TaskState.Init:
                        if (oldState != TaskState.Init)
                        {
                            //Debug.Log("this------------" + this);
                            _OnTaskInit(this);
                            //Debug.Log("_OnTaskInit------------------------------"+this);
                        }
                        break;
                    case TaskState.Start:
                        if (oldState != TaskState.Start)
                        {
                            _OnTaskStart(this);
                            //Debug.Log("_OnTaskStart------------------------------" + this);
                        }
                        break;
                    case TaskState.Doing:
                        _OnTaskDoing(this);
                        break;
                    case TaskState.End:
                        if (oldState != TaskState.End)
                        {
                            _OnTaskEnd(this);
                            //Debug.Log("_OnTaskEnd------------------------------" + this);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public bool IsPass
        {
            get
            {
                if (child)
                {
                    if (!(child as TaskConf).isPass)
                    {
                        return false;
                    }
                    if (child.littleBrother)
                    {
                        if (!(child.littleBrother as TaskConf).isPass)
                        {
                            return false;
                        }
                        if (child.littleBrother.littleBrother)
                        {
                            if (!(child.littleBrother.littleBrother as TaskConf).isPass)
                            {
                                return false;
                            }
                        }
                    }
                }
                return isPass;
            }
            set
            {
                isPass = value;
                //parent
                //if (!isPass)
                //{
                //    if (parent)
                //    {
                //        (parent as TaskConf).isPass = false;
                //    }
                //}
            }
        }

        //获取该任务的某个工具配置
        public ToolTaskConf GetToolTaskConf(ToolConf toolConf)
        {
            Debug.Log(TaskManager.Instance.CurrentTask + "-----" + "GetToolTaskConf");
            foreach (ToolTaskConf taskTool in taskTools)
            {
                if (taskTool.toolConf == toolConf)
                    return taskTool;
            }
            return null;
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<TaskConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<TaskConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(TaskConf) + " is null");
                }
            }
        }

#endif
    }
}