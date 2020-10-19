using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.UI;

namespace BeinLab.VRTraing.Conf
{
    public class GoalSkipTaskCong : GoalVRInputActionConf
    {
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (PopDlg.Instance)
            {
                PopDlg.Instance.ClearShowTimer();
                PopDlg.Instance.OnSkipTask += OnSkipTask;
                PopDlg.Instance.showRoot.SetActive(true);
                TimerMgr.Instance.CreateTimer(PopDlg.Instance.ClearShowTimer,1);
            }
        }

        private void OnSkipTask()
        {
            if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
            {
                AchieveGoal(true);
            }
        }
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            if (PopDlg.Instance)
            {
                PopDlg.Instance.OnSkipTask -= OnSkipTask;
            }
        }
        public override void OnTaskDoing(TaskConf taskConf)
        {
            //base.OnTaskDoing(taskConf);
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSkipTaskCong", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSkipTaskCong>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSkipTaskCong>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSkipTaskCong) + " is null");
                }
            }
        }
#endif
    }
}