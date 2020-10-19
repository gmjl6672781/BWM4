using BeinLab.Util;
using BeinLab.VRTraing;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BeinLab.UI
{
    public class TaskDlg : MonoBehaviour
    {
        /// <summary>
        /// 按钮的预制体
        /// </summary>
        public Button btnPrefab;
        private bool isInit = false;
        public Transform contentRoot;
        public GameObject confirmRoot;
        public Button confirmButton;
        public Button cancleButton;
        private TaskConf jumpTaskConf;
        public GameObject mask;
        public Button reStartBtn;
        public Button overBtn;
        private void Start()
        {
            if (!isInit)
            {
                TaskMode mode = TaskManager.Instance.taskMode;
                if (mode == TaskMode.Teaching)
                {
                    mode = TaskMode.Training;
                }
                InitComponent(mode);
                isInit = true;
            }
            confirmRoot.SetActive(false);
            confirmButton.onClick.AddListener(() =>
            {
                OnClickConfirm(true);
            });
            cancleButton.onClick.AddListener(() =>
            {
                OnClickConfirm(false);
            });
            reStartBtn.onClick.AddListener(ReStartButton);
            overBtn.onClick.AddListener(SummerButton);
            if (SelectModeDlg.Instance)
            {
                SelectModeDlg.Instance.OnSelectModel += OnSelectModel;
            }
            else
            {
                SelectModeDlg.InitComplte += () =>
                {
                    SelectModeDlg.Instance.OnSelectModel += OnSelectModel;
                };
            }
        }

        private void SummerButton()
        {
            jumpTaskConf = TaskManager.Instance.finalTask;
            OnClickConfirm(true);
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        private void ReStartButton()
        {
            TaskManager.Instance.RestartTask();
        }

        /// <summary>
        /// 当选择结束后，更新面板
        /// </summary>
        private void OnSelectModel()
        {
            InitComponent(TaskManager.Instance.taskMode);
        }

        private void OnClickConfirm(bool isJump)
        {
            confirmRoot.gameObject.SetActive(false);
            if (isJump && jumpTaskConf)
            {
                if (TaskManager.Instance.taskMode == TaskMode.Teaching)
                {
                    TaskManager.Instance.taskMode = TaskMode.Training;
                }
                TaskManager.Instance.SkipTask(jumpTaskConf);
            }
            jumpTaskConf = null;
            confirmRoot.SetActive(false);
            MenuDlg.Instance.CloseDlg();
        }

        private void OnEnable()
        {
            jumpTaskConf = null;
            if (isInit)
            {
                InitComponent(TaskManager.Instance.taskMode);
            }
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent(TaskMode taskMode)
        {
            if (!TaskManager.Instance || TaskManager.Instance.dicRecordTasks == null || TaskManager.Instance.dicRecordTasks.Count < 1)
            {
                return;
            }

            jumpTaskConf = null;

            if (contentRoot && contentRoot.childCount > 0)
            {
                for (int i = contentRoot.childCount - 1; i >= 0; i--)
                {
                    Destroy(contentRoot.GetChild(i).gameObject);
                }
            }
            if (contentRoot && btnPrefab)
            {
                mask.SetActive(false);
                if (taskMode == TaskMode.Teaching)
                {
                    mask.SetActive(true);
                    taskMode = TaskMode.Training;
                }
                List<TaskConf> list = TaskManager.Instance.dicRecordTasks[taskMode];
                //print(list);
                for (int i = 0; i < list.Count; i++)
                //for (int i = 0; i < TaskManager.Instance.allTasks.Count; i++)
                {
                    //print(list[i]);
                    TaskConf conf = list[i];
                    Button btn = Instantiate(btnPrefab);
                    UnityUtil.SetParent(contentRoot, btn.transform);

                    //var msg = LanguageMgr.Instance.GetMessage(conf.keyTitle);
                    //if (msg != null)
                    //{
                    //    btn.GetComponentInChildren<Text>().text = msg.Message;
                    //}
                    btn.GetComponentInChildren<TextHelper>().SetMessageKey(conf.keyTitle);
                    btn.GetComponentInChildren<TextHelper>().SortNum(i + 1);
                    btn.onClick.AddListener(() =>
                    {
                        OnClickTask(conf);
                    });
                }
            }
        }
        /// <summary>
        /// 跳跃步骤
        /// </summary>
        /// <param name="conf"></param>
        private void OnClickTask(TaskConf conf)
        {
            if (conf != TaskManager.Instance.CurrentTask)
            {
                jumpTaskConf = conf;
                confirmRoot.SetActive(true);
            }
        }
    }
}