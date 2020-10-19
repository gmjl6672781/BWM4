#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.Util;
using BeinLab.VRTraing.UI;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    public class GoalSelectionModelConf : TaskGoalConf
    {
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            if (!BeforeCheck())
                return;
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

            SelectModeDlg.Instance.ShowDlg();
        }
        /// <summary>
        /// 当做出选择之后
        /// </summary>
        private void OnSelectModel()
        {
            if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
            {
                AchieveGoal(true);
            }
            SelectModeDlg.Instance.HideDlg();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            if (SelectModeDlg.Instance)
            {
                SelectModeDlg.Instance.OnSelectModel -= OnSelectModel;
            }
            base.OnTaskEnd(taskConf);
            if (SelectModeDlg.Instance)
            {
                SelectModeDlg.Instance.HideDlg();
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSelectionModelConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSelectionModelConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSelectionModelConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSelectionModelConf) + " is null");
                }
            }
        }
#endif
    }
}