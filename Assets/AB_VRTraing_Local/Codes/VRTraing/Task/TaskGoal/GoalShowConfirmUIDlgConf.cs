using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.UI;
using BeinLab.VRTraing.Mgr;
using BeinLab.FengYun.Controller;
using BeinLab.CarShow.Modus;
using BeinLab.VRTraing.Controller;
using System;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 任务目标：
    /// </summary>
    public class GoalShowConfirmUIDlgConf : TaskGoalConf
    {
        public ConfirmUIConf confirmUIConf;
        public TaskGoalConf taskGoalConf;

        public override void OnTaskStart(TaskConf targetTask)
        {
            base.OnTaskStart(targetTask);            

            ConfirmUIDlg.Instance.SendAnswer += SendAnswer;

            //显示选择题窗口
            if (confirmUIConf)
            {
                ConfirmUIDlg.Instance.ShowDlg(confirmUIConf);
            }
            else
            {
                Debug.LogError("未添加需要显示的UI");
            }
        }
        public void SendAnswer(int answer)
        {
            if (targetTask == TaskManager.Instance.CurrentTask)
            {
                AchieveGoal(true);
                if (taskGoalConf)
                {
                   // taskGoalConf.OnGoalStart();
                }
            }
        }
       

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            ConfirmUIDlg.Instance.SendAnswer -= SendAnswer;


        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowConfirmUIDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowConfirmUIDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowConfirmUIDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowConfirmUIDlgConf) + " is null");
                }
            }
        }

#endif
    }
}

