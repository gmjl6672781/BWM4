using BeinLab.UI;
using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using Karler.WarFire.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
/// <summary>
/// 用户登陆窗口
/// </summary>
namespace BeinLab.VRTraing.UI
{
    /// <summary>
    /// 弹出窗口
    /// </summary>
    public class PopDlg : Singleton<PopDlg>
    {
        public VRInputConf inputConf;
        private BaseDlg dlg;

        /// <summary>
        /// 标题label
        /// </summary>
        private TextHelper titleLabel;
        /// <summary>
        /// 内容标签
        /// </summary>
        private TextHelper messageLabel;
        /// <summary>
        /// 跳过标签
        /// </summary>
        private TextHelper skipLabel;
        /// <summary>
        /// 跳过按钮
        /// </summary>
        private Button skipButton;
        /// <summary>
        /// 显示
        /// </summary>
        private Timer showTimer;
        private bool isInit;

        //public event Action OnClickSkipButton;

        //private PopDlgConf popDlgConf;
        //private RectTransform simpleBG;
        public GameObject showRoot;

        //public RectTransform SimpleBG { get => simpleBG; set => simpleBG = value; }
        public Button confirmButton;
        public TextHelper confirmLabel;

        public string examTitleKey;
        public string examSkipKey;
        public string examConfrigKey;

        public string traingSkipKey;

        public string teachingTitleKey;
        private VRHand target;
        private int forward = -1;
        public Vector3[] linePath;
        public SteamVR_Input_Sources handType;
        public TaskConf selectTask;
        public event Action OnSkipTask;
        private Timer autoHideTimer;
        public float autoHideTime = 3;

        private VerticalLayoutGroup layoutGroup;
        private ContentSizeFitter sizeFitter;

        //private bool isAutoShow = false;
        //public void SetData(PopDlgConf popDlgConf)
        //{
        //    this.popDlgConf = popDlgConf;
        //    InitComponent();
        //}
        public void ClearShowTimer()
        {
            if (autoHideTimer != null)
            {
                autoHideTimer = TimerMgr.Instance.DestroyTimer(autoHideTimer);
            }
        }
        private void Start()
        {
            InitComponent();
            TaskManager.Instance.OnTaskChange += OnTaskChange;
            target = null;
            for (int i = 0; i < Player.instance.handCount; i++)
            {
                if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                {
                    target = Player.instance.hands[i].GetComponent<VRHand>();
                    break;
                }
            }
            if (TaskManager.Instance.CurrentTask)
            {
                ShowPopDlg(TaskManager.Instance.CurrentTask);
                TaskManager.Instance.CurrentTask.OnTaskAchieve += OnTaskEnd;
            }
            if (showRoot != null)
            {
                layoutGroup = showRoot.gameObject.GetComponent<VerticalLayoutGroup>();
                sizeFitter = showRoot.gameObject.GetComponent<ContentSizeFitter>();
            }
        }

        private void OnTaskChange(TaskConf taskConf)
        {
            showRoot.SetActive(false);
            target.DlgLine.enabled = false;
            if (TaskManager.Instance.CurrentTask)
            {
                if (TaskManager.Instance.taskMode != TaskMode.Examination)
                {
                    //isAutoShow = true;
                    if (TaskManager.Instance.CurrentTask.isCanPopDlg)
                    {
                        ShowPopDlg(TaskManager.Instance.CurrentTask);
                    }
                }
                TaskManager.Instance.CurrentTask.OnTaskAchieve += OnTaskEnd;
            }
            else
            {
                showRoot.SetActive(false);
                target.DlgLine.enabled = false;
            }
        }

        public void OnTaskEnd(TaskConf obj)
        {
            if (obj)
            {
                obj.OnTaskAchieve -= OnTaskEnd;
            }
            showRoot.SetActive(false);
            target.DlgLine.enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitComponent()
        {
            if (!isInit)
            {
                dlg = GetComponent<BaseDlg>();
                titleLabel = dlg.GetChildComponent<TextHelper>("TitleText");
                messageLabel = dlg.GetChildComponent<TextHelper>("MessageText");
                skipButton = dlg.GetChildComponent<Button>("JumpButton");
                //SimpleBG = dlg.GetChildComponent<RectTransform>("Image");

                skipLabel = dlg.GetChildComponent<TextHelper>(skipButton.gameObject, "JumpText");
                skipButton.onClick.AddListener(SkipButtonClick);
                confirmButton.onClick.AddListener(ConfirmButtonClick);
                isInit = true;
            }
            //if (this.popDlgConf)
            //{
            //    titleLabel.SetMessageKey(popDlgConf.titleKey);
            //    messageLabel.SetMessageKey(popDlgConf.msgKey);
            //    skipLabel.SetMessageKey(popDlgConf.skipKey);
            //}
        }
        /// <summary>
        /// 确认结果
        /// </summary>
        private void ConfirmButtonClick()
        {
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                //OnClickSkipButton?.Invoke();
                TaskManager.Instance.CurrentTask.ToCompleteTask();
            }
        }

        /// <summary>
        /// 考试模式的取消，仅仅是隐藏当前的面板
        /// 教程模式的取消，跳过教程进入模式选择
        /// 培训模式的取消，跳过当前任务
        /// </summary>
        private void SkipButtonClick()
        {
            showRoot.SetActive(false);
            if (TaskManager.Instance.taskMode == TaskMode.Examination)
            {
                //OnClickSkipButton?.Invoke();
                showRoot.SetActive(false);
            }
            else if (TaskManager.Instance.taskMode == TaskMode.Teaching)
            {
                ///跳到指定的步骤
                TaskManager.Instance.SkipTask(selectTask);
                OnSkipTask?.Invoke();
                //TaskManager.Instance.StartWorkTask();
            }
            else
            {
                if (TaskManager.Instance.CurrentTask)
                {
                    TaskManager.Instance.CurrentTask.SkipTask();
                }
            }
            target.DlgLine.enabled = false;
        }

        /// <summary>
        /// 清除计时器
        /// </summary>
        public void ClearTimer()
        {
            if (TimerMgr.Instance != null)
            {
                showTimer = TimerMgr.Instance.DestroyTimer(showTimer);
            }
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public void ShowMessage(float showTime = 1)
        {
            ClearTimer();
            gameObject.SetActive(true);
            if (showTime > 0)
            {
                showTimer = TimerMgr.Instance.CreateTimer(() => { gameObject.SetActive(false); }, showTime);
            }
        }
        public void HideDlg()
        {
            showRoot.SetActive(false);
            target.DlgLine.enabled = false;
        }
        /// <summary>
        /// 按钮的显示
        /// </summary>
        /// <param name="isShow"></param>
        public void SetButtonActive(bool isShow = true)
        {
            skipButton.gameObject.SetActive(isShow);
        }
        private void Update()
        {
            ///左手侧键按下时
            if (inputConf.GetKeyDown(Valve.VR.SteamVR_Input_Sources.LeftHand))
            {
                if (TaskManager.Instance.CurrentTask.judgementDlgConf != null)
                    return;
                showRoot.SetActive(!showRoot.activeSelf);
                if (showRoot.activeSelf)
                {
                    if (linePath != null && linePath.Length > 1)
                    {
                        if (target)
                        {
                            target.SetLinePath(linePath);
                            //target.DlgLine.enabled = true;
                        }
                    }
                    //isAutoShow = false;
                    ShowPopDlg(TaskManager.Instance.CurrentTask, true);
                }
                target.DlgLine.enabled = showRoot.activeSelf;
            }
            if (showRoot.activeSelf)
            {
                if (target && target.DlgLine)
                {
                    transform.position = target.GetDlgLinePos(dlg.transPos);
                    UnityUtil.LookAtV(transform, Player.instance.hmdTransform, forward);
                    Vector2 tmp = -showRoot.GetComponent<RectTransform>().sizeDelta / 2;
                    tmp.x = 0;
                    Vector3 pos = showRoot.transform.TransformPoint(tmp);
                    target.DlgLine.SetPosition(target.DlgLine.positionCount - 1,
                        target.DlgLine.transform.InverseTransformPoint(pos));
                }
            }
        }
        /// <summary>
        /// 考试模式显示确认任务完成，做考试提交，需要显示取消和确认按钮，同时title只显示一条信息
        /// 培训和教程模式，显示当前的任务目标。任务title和任务内容都要显示
        /// 教程模式是跳过所有教程进入模式选择
        /// 培训模式的跳过是跳过当前的任务
        /// </summary>
        /// <param name="task"></param>
        private void ShowPopDlg(TaskConf task, bool isForceShow = false)
        {
            ///如果是考试模式，必须手动将提示弹出，自动不弹出
            ///手动不自动弹出
            if (TaskManager.Instance.taskMode == TaskMode.Examination && !isForceShow)
            {
                showRoot.SetActive(false);
                target.DlgLine.enabled = false;
                return;
            }
            if ((!task.isCanPopDlg || task.IsChoise()) && !isForceShow)
            {
                if (!task.isForceShow)
                {
                    showRoot.SetActive(false);
                    target.DlgLine.enabled = false;
                    return;
                }
            }
            string titleKey = null;
            string msgKey = null;
            string skipKey = null;
            string confrigKey = null;

            if (TaskManager.Instance.taskMode == TaskMode.Teaching)
            {
                titleKey = teachingTitleKey;
            }
            else
            {
                titleKey = task.keyTitle;
            }
            msgKey = "";
            if (task.isShowTaskGoalsMsg || TaskManager.Instance.taskMode != TaskMode.Examination)
            {
                for (int i = 0; i < task.taskGoals.Count; i++)
                {
                    if (!string.IsNullOrEmpty(task.taskGoals[i].goalKey))
                    {
                        LanguageConf conf = LanguageMgr.Instance.GetMessage(task.taskGoals[i].goalKey);
                        if (conf != null)
                        {
                            msgKey += conf.Message;
                            if (i != task.taskGoals.Count - 1)
                            {
                                msgKey += "\n";
                            }
                        }
                    }
                }
            }
            if (task.taskName == "Trigger")
            {
                skipKey = "T74";
            }
            else
            {
                if (TaskManager.Instance.taskMode != TaskMode.Examination)
                {
                    skipKey = traingSkipKey;
                }
            }

            //string titleKey = null;
            //string msgKey = null;
            //string skipKey = null;
            //string confrigKey = null;
            ShowPopDlg(titleKey, msgKey, skipKey, confrigKey);
        }

        private void ShowPopDlg(string titleKey, string msgKey, string skipKey, string confrigKey)
        {
            ClearShowTimer();
            titleLabel.gameObject.SetActive(false);
            messageLabel.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            bool isShow = false;
            if (!string.IsNullOrEmpty(titleKey))
            {
                titleLabel.gameObject.SetActive(true);
                titleLabel.SetMessageKey(titleKey);
                isShow = true;
            }

            if (!string.IsNullOrEmpty(skipKey))
            {
                skipButton.gameObject.SetActive(true);
                skipLabel.SetMessageKey(skipKey);
                isShow = true;
            }
            if (!string.IsNullOrEmpty(confrigKey))
            {
                confirmButton.gameObject.SetActive(true);
                confirmLabel.SetMessageKey(confrigKey);
                isShow = true;
            }
            if (!string.IsNullOrEmpty(msgKey))
            {
                messageLabel.gameObject.SetActive(true);
                messageLabel.SetMessage(msgKey);
                isShow = true;
            }
            else
            {
                if (TaskManager.Instance.taskMode != TaskMode.Examination)
                {
                    isShow = false;
                }
            }

            layoutGroup?.SetLayoutHorizontal();
            layoutGroup?.SetLayoutVertical();
            sizeFitter?.SetLayoutHorizontal();
            sizeFitter?.SetLayoutVertical();
            showRoot.SetActive(isShow);
            if (isShow && ((MenuDlg.Instance && MenuDlg.Instance.Dlg && MenuDlg.Instance.Dlg.UiRoot.gameObject.activeSelf)
                || (ChoiceQuestionDlg.Instance && ChoiceQuestionDlg.Instance.isActiveAndEnabled)))
            {
                ClearShowTimer();
                autoHideTimer = TimerMgr.Instance.CreateTimer(() =>
                {
                    showRoot.SetActive(false);
                    if (target && target.DlgLine)
                        target.DlgLine.enabled = false;
                }, autoHideTime);
            }
            if (target && target.DlgLine)
            {
                target.DlgLine.enabled = isShow;
            }
        }
    }
}