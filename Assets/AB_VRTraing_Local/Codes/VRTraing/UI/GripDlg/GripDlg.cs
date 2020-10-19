using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.UI
{
    /// <summary>
    /// 侧键菜单，侧键在培训和考试模式下的功能不同
    /// 培训模式下展示
    /// </summary>
    public class GripDlg : Singleton<GripDlg>
    {
        private bool isInit = false;
        public TextHelper goalLabel;
        public GameObject confrigRoot;
        public Button confirmButton;
        public Button cancleButton;
        public GameObject showRoot;
        public VRInputConf inputConf;
        private void Start()
        {
            isInit = true;
            showRoot.SetActive(false);
            TaskManager.Instance.OnTaskChange += InitComponent;
            InitComponent(TaskManager.Instance.CurrentTask);
            confirmButton.onClick.AddListener(OnClickComplete);
            cancleButton.onClick.AddListener(OnClickCancle);
        }
        /// <summary>
        /// 点击了取消的按钮,关闭此面板
        /// </summary>
        private void OnClickCancle()
        {
            showRoot.SetActive(false);
        }

        /// <summary>
        /// 点击了完成此任务
        /// </summary>
        private void OnClickComplete()
        {
            TaskManager.Instance.CurrentTask.ToCompleteTask();
            showRoot.SetActive(false);
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent(TaskConf task)
        {
            if (task == null)
            {
                showRoot.SetActive(false);
                return;
            }
            confrigRoot.SetActive(true);
            if (TaskManager.Instance.taskMode != VRTraing.TaskMode.Examination)
            {
                confrigRoot.SetActive(false);
                if (goalLabel)
                {
                    string showText = "";
                    for (int i = 0; i < task.taskGoals.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(task.taskGoals[i].goalKey))
                        {
                            LanguageConf conf = LanguageMgr.Instance.GetMessage(task.taskGoals[i].goalKey);
                            if (conf != null)
                            {
                                showText += conf.Message;
                                if (i != task.taskGoals.Count - 1)
                                {
                                    showText += "\n";
                                }
                            }
                        }
                    }
                    goalLabel.SetMessage(showText);
                }
            }
            else
            {
                goalLabel.SetMessage("是否完成任务");
            }
        }
        private void Update()
        {
            ///左手侧键按下时
            if (inputConf.GetKeyDown(Valve.VR.SteamVR_Input_Sources.LeftHand))
            {
                showRoot.SetActive(!showRoot.activeSelf);
                if (showRoot.activeSelf)
                {
                    InitComponent(TaskManager.Instance.CurrentTask);
                }
            }
        }
    }
}