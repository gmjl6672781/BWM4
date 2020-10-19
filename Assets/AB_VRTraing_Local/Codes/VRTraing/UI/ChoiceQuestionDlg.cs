using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using DG.Tweening;
using Karler.WarFire.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 做任务选择题对话框
/// </summary>
namespace BeinLab.VRTraing.UI
{
    [RequireComponent(typeof(BaseDlg))]
    public class ChoiceQuestionDlg : Singleton<ChoiceQuestionDlg>
    {
        private BaseDlg baseDlg;

        public event Action<int> SendAnswer;
        private TextHelper lableMessage;
        private List<Button> selections;

        bool isHighlight = false;
        public int maxLen = 25;
        protected override void Awake()
        {
            base.Awake();
            InitComponent();
        }

        private void Start()
        {
            TaskManager.Instance.OnTaskChange += OnTaskChange;
            HideDlg();
        }

        private void InitComponent()
        {
            baseDlg = GetComponent<BaseDlg>();

            lableMessage = UnityUtil.GetTypeChildByName<TextHelper>(gameObject, "Message");
            if (selections == null)
                selections = new List<Button>();
        }

        public void HideDlg()
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(false);

            if (selections != null)
            {
                selections.ForEach(t =>
                {
                    Destroy(t.gameObject);
                });
                selections.Clear();
            }
            //Debug.Log("HideDlg");
        }


        public void ShowDlg(ChoiceQuestionConf choiceQuestionConf)
        {
            if (baseDlg)
                baseDlg.UiRoot.gameObject.SetActive(true);

            selections.ForEach(t => Destroy(t.gameObject));
            selections.Clear();
            lableMessage.SetMessageKey(choiceQuestionConf.keyMessage);
            //lableMessage.SimpleLine(maxLen);
            TimerMgr.Instance.CreateTimer(() =>
            {
                BoxCollider bc=lableMessage.GetComponentInParent<BoxCollider>();
                Vector3 size = lableMessage.transform.parent.GetComponent<RectTransform>().sizeDelta;
                size.z = 1;
                bc.size = size;
                size.x /= 2;
                size.y /= -2;
                size.z = 0;
                bc.center = size;
            }, 0.02f);
            for (int i = 0; i < choiceQuestionConf.answers.Count; i++)
            {
                GameObject goSelection = Instantiate<GameObject>(choiceQuestionConf.goSlelection, lableMessage.transform.parent);
                Button btnSelection = goSelection.GetComponent<Button>();
                TextHelper lableSelection = UnityUtil.GetTypeChildByName<TextHelper>(btnSelection.gameObject, "Selection");
                selections.Add(btnSelection);
                lableSelection.SetMessageKey(choiceQuestionConf.answers[i].keyAnswer);
            }

            selections.ForEach(t =>
            {
                t.onClick.AddListener(() =>
                {
                    if (SendAnswer != null)
                    {
                        SendAnswer(selections.IndexOf(t));
                    }
                    //禁止所有按钮交互
                    for (int i = 0; i < selections.Count; i++)
                    {
                        selections[i].interactable = false;
                    }
                    //题目答对

                    if (selections.IndexOf(t) == choiceQuestionConf.rightAnswer)

                    {
                        Debug.Log("RightChoice");

                        t.GetComponent<Image>().color = Color.green;
                        //LanguageMgr.Instance.PlayEffectAudioByKey(2);
                        //if (selections[choiceQuestionConf.rightAnswer].gameObject.transform.Find("UIEffect"))
                        //{
                        //    //查找UIEffect
                        //    Transform UIEffect = selections[choiceQuestionConf.rightAnswer].gameObject.transform.Find("UIEffect");
                        //    UIEffect.GetComponent<UnityEngine.UI.Image>().color = Color.green;
                        //    UIEffect.gameObject.SetActive(true);
                        //}

                    }
                    //题目答错
                    else
                    {
                        Debug.Log("ErrorChoice");

                        t.GetComponent<Image>().color = Color.red;

                        TimerMgr.Instance.CreateTimer(
                    () =>
                    {
                        isHighlight = !isHighlight;
                        if (isHighlight)
                        {
                            if (t)
                            {
                                t.GetComponent<Image>().DOColor(Color.red, .2f);
                            }
                        }

                        else
                        {
                            if (t)
                            {
                                t.GetComponent<Image>().DOColor(Color.white, .2f);
                            }
                        }

                    }, .2f, 3);
                        //if (selections[choiceQuestionConf.rightAnswer].gameObject.transform.Find("UIEffect"))
                        //{
                        //    //查找UIEffect
                        //    Transform UIEffect = selections[choiceQuestionConf.rightAnswer].gameObject.transform.Find("UIEffect");
                        //    UIEffect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                        //    UIEffect.gameObject.SetActive(true);
                        //}

                    }
                    TimerMgr.Instance.CreateTimer(() =>
                    {
                        HideDlg();
                    }, 0.86f);

                });
            });
            if (PopDlg.Instance)
                PopDlg.Instance.HideDlg();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (TaskManager.Instance != null)
                TaskManager.Instance.OnTaskChange -= OnTaskChange;
        }

        public void OnTaskChange(TaskConf t)
        {
            foreach (TaskGoalConf goal in t.taskGoals)
            {
                if (goal.GetType() == typeof(GoalShowChoiceDlgConf))
                    return;
            }
            HideDlg();
        }

    }
}

