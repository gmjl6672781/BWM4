using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using BeinLab.VRTraing.UI;
using BeinLab.VRTraing.Mgr;
using BeinLab.VRTraing.Controller;

namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 任务目标：显示答案反馈
    /// </summary>
    public class GoalShowAnswerFeedDlgConf : TaskGoalConf
    {
        //public AnswerFeedDlgConf answerFeedDlgConf;
        public string passKey = "T1074";
        public string failKey = "T1075";


        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);

            //显示答案反馈窗口
            AnswerFeedDlg.Instance.ShowDlg();
            if (AnswerFeedDlg.Instance.IsExamPass())
            {
                LanguageMgr.Instance.PlayAudioByKey(passKey,playDeltTime);
            }
            else
            {
                LanguageMgr.Instance.PlayAudioByKey(failKey, playDeltTime);
            }
            AchieveGoal(true);
        }
        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowAnswerFeedDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowAnswerFeedDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowAnswerFeedDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowAnswerFeedDlgConf) + " is null");
                }
            }
        }

#endif
    }
}
