using BeinLab.UI;
using BeinLab.Util;
using BeinLab.VRTraing.Controller;
using BeinLab.VRTraing.Mgr;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeinLab.VRTraing.Conf
{
    public class GoalSendResult : TaskGoalConf
    {
        public string examSuccessAudio;
        public string examDefaultAudio;
        public string traingAudio;
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            string playAudio = traingAudio;            
            if (ResultDlg.Instance)
            {                
                bool isSuccess = ResultDlg.Instance.InitComponent();
                if (TaskManager.Instance && TaskManager.Instance.taskMode == TaskMode.Examination)
                {
                    playAudio = isSuccess ? examSuccessAudio : examDefaultAudio;
                }
                AchieveGoal(true);
            }
            if (LanguageMgr.Instance)
            {
                LanguageMgr.Instance.PlayAudioByKey(traingAudio);
            }
        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalSendResult", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalSendResult>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalSendResult>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalSendResult) + " is null");
                }
            }
        }
#endif
    }
}