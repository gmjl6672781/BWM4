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
    /// 任务目标：做选择题
    /// </summary>
    public class GoalShowChoiceDlgConf : TaskGoalConf
    {
        public ChoiceQuestionConf choiceQuestionConf;

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);
        }

        public override void OnTaskStart(TaskConf targetTask)
        {
            base.OnTaskStart(targetTask);

            ChoiceQuestionDlg.Instance.SendAnswer += SendAnswer;

            //显示选择题窗口
            if (choiceQuestionConf)
            {
                ChoiceQuestionDlg.Instance.ShowDlg(choiceQuestionConf);
            }
        }
        public void OnSelectTool(int answer, Action<bool> action)
        {
            if (choiceQuestionConf.rightAnswer == answer)
            {
                LanguageMgr.Instance.PlayAudioByKey(choiceQuestionConf.answers[answer].keyAudios
                    , 0, (float audioTime) =>
                    {
                        TimerMgr.Instance.CreateTimer(() =>
                        {
                            //AchieveGoal(true);
                            action?.Invoke(true);
                        }, audioTime);
                    });
            }
            else
            {
                float showTime = 0;
                float audioTime = 100;
                ///为了保证动画演示完毕，自动计算语音以及动效的播放时间
                if (choiceQuestionConf.answers[answer].dynamicConfs.Count > 0)
                {
                    for (int i = 0; i < choiceQuestionConf.answers[answer].dynamicConfs.Count; i++)
                    {
                        DynamicConf dyn = choiceQuestionConf.answers[answer].dynamicConfs[i];
                        if (audioTime > dyn.delayTime)
                        {
                            audioTime = dyn.delayTime;
                        }
                        if (showTime < dyn.delayTime + dyn.showTime)
                        {
                            showTime = dyn.delayTime + dyn.showTime;
                        }
                        DynamicActionController.Instance.DoDynamic(dyn);
                    }
                }
                //确保等待总时间
                if (showTime <= audioTime)
                {
                    showTime = audioTime;
                }
                TimerMgr.Instance.CreateTimer(() =>
                {
                    LanguageMgr.Instance.PlayAudioByKey(choiceQuestionConf.answers[answer].keyAudios,
                        0, (float audio) =>
                        {
                            if (showTime < audio)
                            {
                                showTime = audio;
                            }
                            TimerMgr.Instance.CreateTimer(() =>
                            {
                                //AchieveGoal(false);
                                action?.Invoke(false);
                            }, showTime);
                        });
                }, audioTime);
            }
        }
        public void SendAnswer(int answer)
        {
            if (targetTask == TaskManager.Instance.CurrentTask)
            {
                OnSelectTool(answer, (bool isRight) =>
                {
                    AchieveGoal(isRight);
                });
            }
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            ChoiceQuestionDlg.Instance.SendAnswer -= SendAnswer;
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalShowChoiceDlgConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalShowChoiceDlgConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalShowChoiceDlgConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalShowChoiceDlgConf) + " is null");
                }
            }
        }

#endif
    }
}
