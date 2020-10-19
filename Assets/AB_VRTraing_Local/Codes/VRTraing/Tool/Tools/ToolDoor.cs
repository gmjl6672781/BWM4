using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using DG.Tweening;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;
using BeinLab.VRTraing.Mgr;

namespace BeinLab.VRTraing
{
    public class ToolDoor : ToolBasic
    {
        private Vector3 originDir;

        protected override void Awake()
        {
            base.Awake();
            originDir = transform.up;
        }

        protected override void OnPressDown_(Hand hand)
        {
            base.OnPressDown_(hand);
            hand.HideController(true);
        }

        protected override void OnPressUp_(Hand hand)
        {
            base.OnPressUp_(hand);
            hand.ShowController(true);
        }

        protected override void OnPress_(Hand hand)
        {
            base.OnPress_(hand);

            Vector3 handPos = hand.transform.position;
            Vector3 selfPos = transform.position;

            handPos.y = selfPos.y;
            if(Mathf.Abs(Vector3.Angle(originDir, handPos - transform.position))<80)
            {
                float angle = Vector3.Angle(transform.up, handPos - transform.position);
                float dir = (Vector3.Dot(Vector3.up, Vector3.Cross(transform.up, handPos - transform.position)) < 0 ? -1 : 1);
                angle *= dir;
                transform.Rotate(Vector3.forward, angle);
            }

        }
    }
}
