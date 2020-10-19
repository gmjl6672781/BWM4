using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.UI;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    public class GoalSwipeConf : GoalVRInputActionConf
    {
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (MenuDlg.Instance)
            {
                MenuDlg.Instance.OnSwipeMenu += OnSwipeMenu;
                if (!MenuDlg.Instance.Dlg.gameObject.activeSelf)
                {
                    MenuDlg.Instance.Dlg.gameObject.SetActive(true);
                }
            }
        }
        public override void OnTaskDoing(TaskConf taskConf)
        {
            //base.OnTaskDoing(taskConf);
        }
        //public override void OnTaskInit(TaskConf taskConf)
        //{
        //    base.OnTaskInit(taskConf);

        //}
        /// <summary>
        /// 滑动菜单
        /// </summary>
        private void OnSwipeMenu()
        {
            Debug.Log("OnSwipeMenu");
            if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
            {
                AchieveGoal(true);
            }
        }
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            if (MenuDlg.Instance)
            {
                MenuDlg.Instance.OnSwipeMenu -= OnSwipeMenu;
                TimerMgr.Instance.CreateTimer(MenuDlg.Instance.CloseDlg,3);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSwipeConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSwipeConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSwipeConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSwipeConf) + " is null");
                }
            }
        }
#endif
    }
}