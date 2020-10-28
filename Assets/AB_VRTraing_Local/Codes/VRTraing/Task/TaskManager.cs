using BeinLab.Conf;
using BeinLab.UI;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BeinLab.VRTraing.Mgr
{
    public class TaskManager : Singleton<TaskManager>
    {
        /// <summary>
        /// 测试用
        /// </summary>
        public bool isUseKeyBords = false;
        /// <summary>
        /// 第一个教学任务
        /// </summary>
        public TaskConf firstTask;
        /// <summary>
        /// 最后一个结算任务
        /// </summary>
        public TaskConf finalTask;
        /// <summary>
        /// 所有的任务
        /// </summary>
        private List<TaskConf> allTasks;

        /// <summary>
        /// 任务模式 
        /// </summary>
        [SerializeField]
        private TaskMode mTaskMode = TaskMode.Training;
        /// <summary>
        /// 任务记录
        /// </summary>
        public Dictionary<TaskMode, List<TaskConf>> dicRecordTasks;
        public Dictionary<string, TaskConf> titleTaskMap;
        public event Action<TaskConf> OnTaskChange;
        private Coroutine coroutineDoing;
        private Coroutine coroutineDelayTask;

        /// <summary>
        /// 任务模式
        /// </summary>
        public TaskMode taskMode
        {
            get
            {
                return mTaskMode;
            }
            set
            {
                if (value != mTaskMode)
                {
                    mTaskMode = value;
                }
            }
        }

        /// <summary>
        /// 当前任务
        /// </summary>
        private TaskConf mCurrentTask;
        public TaskConf CurrentTask
        {
            get
            {
                return mCurrentTask;
            }
            set
            {
                mCurrentTask = value;
            }
        }
        
        private void InitTasks()
        {
            //Debug.Log("——————————————————————————AllTaskInit--Start——————————————————————————");
            TaskConf nextTask = firstTask;
            while (nextTask != null)
            {
                nextTask.taskState = TaskState.UnInit;
                nextTask.taskState = TaskState.Init;
                nextTask = GetNextTaskLittle(nextTask);
                if (firstTask.index < 13 && nextTask != null)
                {
                    if (nextTask.index == 35|| nextTask.index == 39 || nextTask.index == 41)
                        nextTask = GetNextTaskLittle(nextTask);
                    if (nextTask.index == 42)
                        nextTask = GetNextTaskLittle(nextTask);
                }
            }
            //Debug.Log("——————————————————————————AllTaskInit--Over——————————————————————————");
        }

        protected override void Awake()
        {
            base.Awake();

            if (allTasks == null)
                allTasks = new List<TaskConf>();
            if (dicRecordTasks == null)
                dicRecordTasks = new Dictionary<TaskMode, List<TaskConf>>();

            ReadTasks(firstTask);
            GetCanSkipTasks();
        }

        private void OnVRHandActive()
        {
            if (VRHandHelper.Instance)
            {
                VRHandHelper.Instance.OnVRHandActive -= OnVRHandActive;
            }

            StartTask();
        }

        public void StartTask()
        {
            InitTasks();
            CurrentTask = firstTask;
            ExecuteTask(CurrentTask);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private void ExecuteTask(TaskConf taskConf)
        {
            if (CurrentTask == null)
                return;

            TaskConf tempTask = CurrentTask;
            CurrentTask = taskConf;
            if (CurrentTask.index == 35 || CurrentTask.index == 39 || CurrentTask.index == 41 || CurrentTask.index == 42)
            {
                CurrentTask.taskState = TaskState.Init;
            }
            //首先判断该任务是否跳步，如果跳步，做跳步处理           
            int compareResult = CompareTaskPriority(taskConf, tempTask);

            //向前执行跳步
            if (compareResult < 0)
            {
                //重新设置两个任务之间的状态      
                TaskConf last = tempTask;
                while (last && last != taskConf)
                {
                    last.taskState = TaskState.Init;
                    last = GetLastNode(last);
                    //last = GetLastTaskLittle(last);
                }
                //CurrentTask = taskConf;
                CurrentTask.taskState = TaskState.Start;
            }
            //向后执行跳步
            else if (compareResult > 0)
            {
                //跳步到这个任务之后的任务
                TaskConf next = tempTask;
                //tempTask.IsPass = false;
                while (next && next != taskConf)
                {
                    //next.IsPass = false;
                    next.taskState = TaskState.End;
                    next = GetNextNode(next);
                    if (next.index == 35 || next.index == 39 || next.index == 41 || next.index == 42)
                    {
                        next.taskState = TaskState.Init;
                    }
                    //Debug.Log(next.index);
                    //next = GetNextTaskLittle(next);
                }
                //CurrentTask = taskConf;
                CurrentTask.taskState = TaskState.Start;
            }
            else
            {
                //重新执行这个任务
                CurrentTask.taskState = TaskState.Start;
            }


            OnTaskChange?.Invoke(CurrentTask);
        }

        private IEnumerator ExecuteTaskDelay(TaskConf taskConf)
        {
            //Debug.Log("Start ExecuteTaskDelay coroutine");
            yield return new WaitForSeconds(taskConf.delayTimeToStart);
            ExecuteTask(taskConf);
        }

        private void ResetTasks()
        {
            allTasks.ForEach(t =>
            {
                t.InitTask();
                t.taskState = TaskState.UnInit;
            });
        }

        private void ReadTasks(TaskConf first)
        {
            if (!firstTask)
                return;

            allTasks.Clear();
            List<TaskScoreConf> scoreList = UnityUtil.ReadXMLData<TaskScoreConf>(Application.streamingAssetsPath);
            TaskConf nextTask = first;
            int index = 0;
            int scoreIndex = 0;
            while (nextTask)
            {
                if (nextTask && nextTask.isCanRecord)
                {
                    nextTask.traingScore = scoreList[scoreIndex].TraingScore;
                    nextTask.examScore = scoreList[scoreIndex].ExamScore;
                    scoreIndex++;
                    //TaskScoreConf taskScore = new TaskScoreConf();
                    //taskScore.PriKey = nextTask.keyTitle;
                    //UnityUtil.WriteXMLData(Application.streamingAssetsPath, taskScore, true);
                }
                allTasks.Add(nextTask);
                nextTask.index = index++;
                nextTask = (TaskConf)nextTask.NextNode;
            }

            //读完任务要重设状态
            ResetTasks();
        }



        public void StartDoingCoroutine(TaskConf taskConf)
        {
            //Debug.Log("StartDoingState");
            coroutineDoing = StartCoroutine(SetDoingState(taskConf));
        }

        public void StopDoingCoroutine()
        {
            if (coroutineDoing != null)
            {
                //Debug.Log("StopDoingState");
                StopCoroutine(coroutineDoing);
                coroutineDoing = null;
            }
        }


        private IEnumerator SetDoingState(TaskConf taskConf)
        {
            yield return new WaitForSeconds(taskConf.delayTimeStartToDoing);
            while (true)
            {
                taskConf.taskState = TaskState.Doing;
                yield return null;
            }
        }

        /// <summary>
        /// 下个任务，包括小任务
        /// </summary>
        public void ExecuteNextTaskLittle()
        {
            if (coroutineDelayTask != null)
            {
                //Debug.Log("Stop ExecuteNextTask coroutine");
                StopCoroutine(coroutineDelayTask);
                coroutineDelayTask = null;
            }

            if (CurrentTask)
            {
                TaskConf nextTaskLittle = GetNextTaskLittle(CurrentTask);

                if (nextTaskLittle != null)
                    coroutineDelayTask = StartCoroutine(ExecuteTaskDelay(nextTaskLittle));
                else
                    OnCompletAllTask();
            }
        }


        public void ExecuteNextTaskBig()
        {
            if (coroutineDelayTask != null)
            {
                Debug.Log("Stop ExecuteNextTask coroutine");
                StopCoroutine(coroutineDelayTask);
                coroutineDelayTask = null;
            }

            if (CurrentTask)
            {
                TaskConf nextTaskBig = GetNextTaskBig(CurrentTask);

                if (nextTaskBig != null)
                    coroutineDelayTask = StartCoroutine(ExecuteTaskDelay(nextTaskBig));
                else
                {
                    coroutineDelayTask = StartCoroutine(ExecuteTaskDelay(allTasks[allTasks.Count - 1]));
                    OnCompletAllTask();
                }

            }
        }

        /// <summary>
        /// 跳步到大任务
        /// </summary>
        /// <param name="taskConf"></param>
        public void SkipTask(TaskConf taskConf)
        {
            if (CurrentTask != null)
            {
                CurrentTask.IsPass = false;
                //if (CurrentTask.parent)
                //{
                //    (CurrentTask.parent as TaskConf).IsPass = false;
                //}
            }
            if (coroutineDelayTask != null)
            {
                //Debug.Log("Stop ExecuteNextTask coroutine");
                StopCoroutine(coroutineDelayTask);
                coroutineDelayTask = null;
            }

            //if (CheckCanSkip(taskConf))
            //{
            coroutineDelayTask = StartCoroutine(ExecuteTaskDelay(taskConf));
            //} 


        }

        private bool CheckCanSkip(TaskConf taskConf)
        {
            if (taskMode == TaskMode.Examination)
            {
                if (taskConf.isCanRecord && taskConf.taskType != TaskType.OnlyTraining)
                    return true;
            }
            else
            {
                if (taskConf.isCanRecord && taskConf.taskType != TaskType.OnlyExamination)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取任务的下一个节点，不考虑模式，只是下一个节点
        /// </summary>
        /// <param name="taskConf"></param>
        public TaskConf GetNextNode(TaskConf taskConf)
        {
            if (taskConf.index < allTasks.Count - 1)
            {
                //Debug.Log(allTasks[taskConf.index + 1]);
                return allTasks[taskConf.index + 1];
            }
            else
                return null;
        }

        /// <summary>
        /// 获取任务的上一个节点，不考虑模式，只是上一个节点
        /// </summary>
        /// <param name="taskConf"></param>
        public TaskConf GetLastNode(TaskConf taskConf)
        {
            if (taskConf.index > 0)
                return allTasks[taskConf.index - 1];
            else
                return null;
        }


        /// <summary>
        /// 下一个小任务:考虑任务模式
        /// </summary>
        /// <param name="taskConf"></param>
        /// <returns></returns>
        public TaskConf GetNextTaskLittle(TaskConf taskConf)
        {
            TaskConf next = GetNextNode(taskConf);
            if (taskMode == TaskMode.Examination)
            {
                while (next != null)
                {
                    if (next.taskType == TaskType.OnlyTraining)
                        next = GetNextNode(next);
                    else
                        break;
                }
                return next;
            }
            else if (taskMode == TaskMode.Training)
            {
                while (next != null)
                {
                    if (next.taskType == TaskType.OnlyExamination)
                        next = GetNextNode(next);
                    else
                        break;
                }
                return next;
            }
            else
                return next;
        }


        /// <summary>
        /// 下一个小任务:考虑任务模式
        /// </summary>
        /// <param name="taskConf"></param>
        /// <returns></returns>
        public TaskConf GetLastTaskLittle(TaskConf taskConf)
        {
            TaskConf last = GetLastNode(taskConf);
            if (taskMode == TaskMode.Examination)
            {
                while (last != null)
                {
                    if (last.taskType == TaskType.OnlyTraining)
                        last = GetLastNode(last);
                    else
                        break;
                }
                return last;
            }
            else if (taskMode == TaskMode.Training)
            {
                while (last != null)
                {
                    if (last.taskType == TaskType.OnlyExamination)
                        last = GetLastNode(last);
                    else
                        break;
                }
                return last;
            }
            else
                return last;
        }

        /// <summary>
        /// 下一个大任务:考虑任务模式
        /// </summary>
        /// <param name="taskConf"></param>
        /// <returns></returns>
        public TaskConf GetNextTaskBig(TaskConf taskConf)
        {
            TaskConf next = GetNextNode(taskConf);
            if (taskMode == TaskMode.Examination)
            {
                while (next != null)
                {
                    if (next.taskType == TaskType.OnlyTraining || !next.isCanRecord)
                        next = GetNextNode(next);
                    else
                        break;
                }
                return next;
            }
            else
            {
                while (next != null)
                {
                    if (next.taskType == TaskType.OnlyExamination || !next.isCanRecord)
                        next = GetNextNode(next);
                    else
                        break;
                }
                return next;
            }
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        public void RestartTask()
        {
            //StartTask();
            //InitTasks();
            //SkipTask(firstTask);
            if (MenuDlg.Instance)
            {
                MenuDlg.Instance.CloseDlg();
            }
            StartTask();
            InitTasks();
            FindAllTools.Instance.InitAllTools();
        }

        /// <summary>
        /// 获取工具在当前任务的状态信息
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public ToolTaskInfo GetToolInitInfo(ToolConf tool)
        {
            TaskConf last = CurrentTask;
            ToolTaskInfo toolTaskInfo = new ToolTaskInfo();
            while (last)
            {
                foreach (ToolTaskConf toolTask in last.taskTools)
                {
                    if (toolTask.toolConf == tool && toolTask != CurrentTask)
                    {
                        toolTaskInfo.isSetCanHover = toolTask.isSetEndCanHover;
                        toolTaskInfo.isCanHover = toolTask.isEndCanHover;
                        toolTaskInfo.isSetCanCatch = toolTask.isSetEndCanCatch;
                        toolTaskInfo.isCanCatch = toolTask.isEndCanCatch;
                        toolTaskInfo.isSetKinematic = toolTask.isSetEndKinematic;
                        toolTaskInfo.isKinematic = toolTask.isEndKinematic;
                        toolTaskInfo.isSetHide = toolTask.isSetEndHide;
                        toolTaskInfo.isHide = toolTask.isEndHide;
                        toolTaskInfo.isSetHighlight = toolTask.isSetEndHighlight;
                        toolTaskInfo.isHighlight = toolTask.isEndHighlight;
                        toolTaskInfo.isSetScaleSize = toolTask.isSetEndScaleSize;
                        toolTaskInfo.scaleSize = toolTask.endScaleSize;
                        toolTaskInfo.isSetPose = toolTask.isSetEndPose;
                        toolTaskInfo.position = toolTask.EndPosition;
                        toolTaskInfo.angle = toolTask.EndAngle;
                        toolTaskInfo.indexAoCao = toolTask.aocaoStartIndex;
                        //print("toolTask--" + toolTask);
                        //print("toolTaskInfo.indexAoCao--" + toolTaskInfo.indexAoCao);
                        return toolTaskInfo;
                    }
                }
                last = GetLastNode(last);
            }


            toolTaskInfo.isSetCanHover = tool.isSetInitCanHover;
            toolTaskInfo.isCanHover = tool.isInitCanHover;
            toolTaskInfo.isSetCanCatch = tool.isSetInitCanCatch;
            toolTaskInfo.isCanCatch = tool.isInitCanCatch;
            toolTaskInfo.isSetKinematic = tool.isSetInitKinematic;
            toolTaskInfo.isKinematic = tool.isInitKinematic;
            toolTaskInfo.isSetHide = tool.isSetInitHide;
            toolTaskInfo.isHide = tool.isInitHide;
            toolTaskInfo.isSetHighlight = tool.isSetInitHighlight;
            toolTaskInfo.isHighlight = tool.isInitHighlight;
            toolTaskInfo.isSetScaleSize = tool.isSetInitScaleSize;
            toolTaskInfo.scaleSize = tool.InitScaleSize;
            toolTaskInfo.isSetPose = tool.isSetInitPose;
            toolTaskInfo.position = tool.InitPosition;
            toolTaskInfo.angle = tool.InitAngle;
            return toolTaskInfo;
        }


        /// <summary>
        /// 完成所有任务
        /// </summary>
        private void OnCompletAllTask()
        {
            Debug.Log("所有任务已完成");


        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            ResetTasks();
        }

        /// <summary>
        /// 比较任务优先级，返回结果小于零 大于零 等于零
        /// </summary>
        /// <param name="taskConf0"></param>
        /// <param name="taskConf1"></param>
        /// <returns></returns>
        private int CompareTaskPriority(TaskConf taskConf0, TaskConf taskConf1)
        {
            return taskConf0.index - taskConf1.index;
        }

        private void Start()
        {
            //InitTasks();
            if (!isUseKeyBords)
            {
                if (VRHandHelper.Instance)
                {
                    VRHandHelper.Instance.OnVRHandActive += OnVRHandActive;
                }
                else
                {
                    VRHandHelper.InitComplte += () =>
                    {
                        VRHandHelper.Instance.OnVRHandActive += OnVRHandActive;
                    };
                }
            }
            if (isUseKeyBords)
            {
                StartTask();
            }
        }


        /// <summary>
        /// 获取可以跳转的任务
        /// </summary>
        /// <returns></returns>
        private void GetCanSkipTasks()
        {
            List<TaskConf> canSkipExaminationTasks = new List<TaskConf>();
            List<TaskConf> canSkipTrainingTasks = new List<TaskConf>();

            allTasks.ForEach(t =>
            {
                if (t.isCanRecord && t.taskType != TaskType.OnlyTraining)
                    canSkipExaminationTasks.Add(t);

                if (t.isCanRecord && t.taskType != TaskType.OnlyExamination)
                    canSkipTrainingTasks.Add(t);
            });

            dicRecordTasks.Add(TaskMode.Examination, canSkipExaminationTasks);
            dicRecordTasks.Add(TaskMode.Training, canSkipTrainingTasks);
        }

        /// <summary>
        /// 完成所有任务，获取任务完成情况
        /// </summary>
        /// <returns></returns>
        public List<TaskConf> GetTaskRecords()
        {
            List<TaskConf> taskRecords = new List<TaskConf>();
            if (taskMode == TaskMode.Examination)
                return dicRecordTasks[TaskMode.Examination];
            else
                return dicRecordTasks[TaskMode.Training];
        }


        public IEnumerator SendMessage(string url, string userCode, int modelType, string date, float scores, string jsonResult)
        {
            WWWForm form = new WWWForm();
            form.AddField("USERCODE", userCode);
            form.AddField("MODELTYPE", modelType);
            form.AddField("EXAM_DATE", date);
            form.AddField("EXAM_SCORES", scores.ToString());
            form.AddField("EXAM_RESULT", jsonResult);
            UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
            yield return webRequest.SendWebRequest();
            string reslut = "";
            if (!webRequest.isHttpError && !webRequest.isNetworkError)
            {
                reslut = (webRequest.downloadHandler.text);

                //if (reslut == "true")
                //{                   
                //    yield break;
                //}
            }
        }
        public ResultConf SendMessageToLocal(string url, string userCode, int modelType, string date, float scores, string jsonResult)
        {
            ResultConf rc = new ResultConf();
            rc.UserCode = userCode;
            rc.Date = date;
            rc.Score = scores.ToString();
            rc.Result = jsonResult;
            rc.ModelType = modelType;
            rc.PriKey = "Date_" + date;
            //yield return new WaitForFixedUpdate();
            if (UserDlg.Instance)
            {
                UserDlg.Instance.AddResoult(rc);
            }
            return rc;
        }
        public TaskConf GetTaskConf(string titleKey, int model)
        {
            TaskConf conf = null;
            if (titleTaskMap == null)
            {
                titleTaskMap = new Dictionary<string, TaskConf>();
            }
            if (titleTaskMap.ContainsKey(titleKey))
            {
                return titleTaskMap[titleKey];
            }
            TaskMode taskmodel = (TaskMode)model;
            List<TaskConf> confList = dicRecordTasks[taskmodel];
            if (confList != null)
            {
                ///
                for (int i = 0; i < confList.Count; i++)
                {
                    if (confList[i].keyTip == titleKey)
                    {
                        conf = confList[i];
                        if (!titleTaskMap.ContainsKey(titleKey))
                        {
                            titleTaskMap.Add(titleKey, conf);
                            break;
                        }
                    }
                }
            }
            return conf;
        }
    }
}