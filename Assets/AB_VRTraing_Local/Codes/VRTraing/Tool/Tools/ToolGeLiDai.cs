using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolGeLiDai : ToolBasic
    {
        public float maxScale = 0.83f;
        private float scaleFactor = 0.396f;
        private float originScale = 0.04f;
        private Vector3 originForward;
        private Hand pullHand;
        [HideInInspector]
        public bool isStartPull;
        public event Action<ToolConf> OnPull;

        protected override void Awake()
        {
            base.Awake();
            originForward = transform.forward;
        }

        protected override void OnPressDown_(Hand hand)
        {
            base.OnPressDown_(hand);

            isStartPull = true;
            pullHand = hand;
        }

        private void Update()
        {
            if (isStartPull && !isComplete)
            {
                if (VRHandHelper.Instance.VRInput_GrabPinch.GetKeyUp(pullHand))
                {
                    ReSetPos();
                    return;
                }

                Vector3 handPos = pullHand.transform.position;
                transform.localScale = new Vector3(1, 1, scaleFactor * Vector3.Distance(handPos, transform.position));
                transform.forward = handPos - transform.position;
                VRHandHelper.Instance.ShockHand(pullHand, (ushort)(1500));
                //if(Vector3.Distance(handPos, transform.position) > 1.2f)
                //    OnPull_();
            }
        }

        public void OnPull_()
        {
            transform.forward = originForward;
            transform.localScale = new Vector3(1, 1, maxScale);
            isComplete = true;
            isStartPull = false;

            //通知任务目标
            if (OnPull != null)
                OnPull(toolConf);
        }


        protected override void OnPressUp_(Hand hand)
        {
            base.OnPressUp_(hand);

            if (!isComplete)
            {
                ReSetPos();
            }
        }

        //恢复原位
        public void ReSetPos()
        {
            transform.localScale = new Vector3(1, 1, originScale);
            transform.forward = originForward;
            isComplete = false;
            isStartPull = false;
            SetToolHighlight(false);
        }

        public override void OnTaskInit(TaskConf taskConf)
        {
            base.OnTaskInit(taskConf);

            TaskConf last = TaskManager.Instance.CurrentTask;
            while (last)
            {
                foreach (ToolTaskConf toolTask in last.taskTools)
                {
                    if (toolTask.toolConf == toolConf && toolTask != TaskManager.Instance.CurrentTask)
                    {
                        toolConf.toolBasic.OnTaskEnd(last);
                        return;
                    }
                }
                last = TaskManager.Instance.GetLastNode(last);
            }

            ReSetPos();
        }

        public override void OnTaskStart(TaskConf taskConf)
        {
            base.OnTaskStart(taskConf);
            ReSetPos();
        }

        public override void OnTaskEnd(TaskConf taskConf)
        {
            base.OnTaskEnd(taskConf);
            transform.forward = originForward;
            transform.localScale = new Vector3(1, 1, maxScale);
            isComplete = true;
            isStartPull = false;
        }

    }
}

