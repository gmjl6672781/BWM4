
namespace BeinLab.VRTraing
{
    /// <summary>
    /// TaskState
    /// 任务状态
    /// </summary>
    public enum TaskState
    {
        UnInit = 0,
        Init = 1,
        Start = 2,
        Doing = 3,
        End = 4
    }

    public enum TaskMode
    {
        Teaching = -1,
        /// <summary>
        /// 教学模式
        /// </summary>
        Training = 0,
        /// <summary>
        /// 考试模式
        /// </summary>
        Examination = 1
    }

    public enum GoalType
    {
        /// <summary>
        /// 所有模式任务目标都执行
        /// </summary>
        All,
        /// <summary>
        /// 只有培训模式任务目标执行
        /// </summary>
        OnlyTraining,
        /// <summary>
        /// 只有考试模式任务目标执行
        /// </summary>
        OnlyExamination
    }

    public enum TaskType
    {
        /// <summary>
        /// 所有模式任务都执行
        /// </summary>
        All,
        /// <summary>
        /// 只有培训模式任务执行
        /// </summary>
        OnlyTraining,
        /// <summary>
        /// 只有考试模式任务执行
        /// </summary>
        OnlyExamination
    }
}

