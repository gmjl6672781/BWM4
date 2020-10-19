using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.UI;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing.Conf
{
    public class GoalShowMenuConf : GoalVRInputActionConf
    {
        private bool isMenuShow = false;
        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
            isMenuShow = false;
        }
        public override void OnTaskDoing(TaskConf taskConf)
        {
            //base.OnTaskDoing(taskConf);
            if (!isMenuShow&& MenuDlg.Instance.Dlg.UiRoot.gameObject.activeSelf)
            {
                OnShowMenuDlg();
                isMenuShow = true;
            }
        }

        private void OnShowMenuDlg()
        {
            if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
            {
                AchieveGoal(true);
            }
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowMenuConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowMenuConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowMenuConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowMenuConf) + " is null");
                }
            }
        }
#endif
    }
}