using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;
using BeinLab.VRTraing.Mgr;
using Valve.VR.InteractionSystem;
using BeinLab.VRTraing.Controller;
using System.Collections.Generic;

namespace BeinLab.VRTraing.Conf
{
    public class CoroutineHelper : MonoBehaviour { }
    /// <summary>·                                                                                                                                                                                                                  
    /// 任务目标：手柄输入，供操作引导使用
    /// <summary>
    /// </summary>
    public class GoalVRInputActionConf : TaskGoalConf
    {
        /// 哪个按键
        /// </summary>
        public VRInputConf vrInputConf;
        /// <summary>
        /// 哪个手柄
        /// </summary>
        public SteamVR_Input_Sources handType;

        public KeyCode keyCode;
        public string msgKey;
        public bool isShock = true;
        private List<Coroutine> shockCoroutines;
        private CoroutineHelper coroutineHelper;
        /// <summary>
        /// 清除手柄高亮的携程
        /// </summary>
        private void ClearCoroutine()
        {
            if (coroutineHelper != null)
            {
                if (shockCoroutines != null)
                {
                    for (int i = 0; i < shockCoroutines.Count; i++)
                    {
                        Coroutine cor = shockCoroutines[i];
                        if (cor != null)
                        {
                            coroutineHelper.StopCoroutine(cor);
                        }
                    }
                    shockCoroutines.Clear();
                }
                Destroy(coroutineHelper.gameObject);
            }
            shockCoroutines = null;
            coroutineHelper = null;
        }
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            isAchieveGoal = false;
            TaskManager.Instance.OnTaskChange += OnTaskChange;
            ClearCoroutine();
            if (shockCoroutines == null)
            {
                shockCoroutines = new List<Coroutine>();
            }
            if (coroutineHelper == null)
            {
                GameObject tmp = new GameObject("CorHelper");
                coroutineHelper = tmp.AddComponent<CoroutineHelper>();
            }
            if (VRHandHelper.Instance && Player.instance)
            {
                for (int i = 0; i < Player.instance.hands.Length; i++)
                {
                    if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                    {
                        var hand = Player.instance.hands[i];
                        var langConf = LanguageMgr.Instance.GetMessage(msgKey);
                        string message = "";
                        if (langConf != null)
                        {
                            message = langConf.Message;
                        }
                        else
                        {
                            Debug.Log(message);
                        }
                        //if (shockCoroutine != null)
                        //{
                        //    VRHandHelper.Instance.StopCoroutine(shockCoroutine);
                        //}
                        if (isShock)
                        {
                            var cor = coroutineHelper.StartCoroutine(VRHandHelper.Instance.TeleportHintCoroutine(hand, vrInputConf.vr_Action_Button, message));
                            shockCoroutines.Add(cor);
                        }
                        else
                        {
                            VRHandHelper.Instance.ShowHint(hand, vrInputConf.vr_Action_Button, message);
                        }
                    }
                }
            }
        }

        private void OnTaskChange(TaskConf task)
        {
            if (task != targetTask)
            {
                OnTaskEnd(targetTask);
            }
        }

        public virtual void Check(TaskConf taskConf)
        {
            //已完成不再检测
            if (isAchieveGoal)
                return;

            //测试
            //var _result = Input.GetKeyDown(keyCode);
            //if (_result)
            //    AchieveGoal(true);
            //return;

            //监听任务
            if (targetTask && targetTask == TaskManager.Instance.CurrentTask)
            {
                if (vrInputConf)
                {
                    var result = vrInputConf.GetKeyDown(handType);
                    if (result)
                        AchieveGoal(true);
                }
            }
        }
        /// <summary>
        /// 检测对应的按钮是否被按下
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskDoing(TaskConf taskConf)
        {
            base.OnTaskDoing(taskConf);
            Check(taskConf);
        }
        /// <summary>
        /// 任务完成时，取消对应的高亮
        /// </summary>
        /// <param name="taskConf"></param>
        public override void OnTaskEnd(TaskConf taskConf)
        {
            //Debug.Log("OnTaskEnd");
            TaskManager.Instance.OnTaskChange -= OnTaskChange;
            ClearCoroutine();
            if (VRHandHelper.Instance && Player.instance)
            {
                for (int i = 0; i < Player.instance.hands.Length; i++)
                {
                    if (handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == handType)
                    {
                        var hand = Player.instance.hands[i];
                        //VRHandHelper.Instance.HideHint(hand, vrInputConf.vr_Action_Button);
                        ControllerButtonHints.HideAllTextHints(hand);
                        hand.ShowController(true);
                    }
                }
            }
            base.OnTaskEnd(taskConf);

        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/TaskGoal/GoalVRInputActionConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<GoalVRInputActionConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<GoalVRInputActionConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(GoalVRInputActionConf) + " is null");
                }
            }
        }
#endif
    }
}


