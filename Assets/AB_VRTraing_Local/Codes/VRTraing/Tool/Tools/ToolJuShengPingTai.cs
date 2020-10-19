using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolJuShengPingTai : ToolBasic
    {
        private bool isCanMove = true;
        private float moveSpeed = 3f;

        protected override void OnPress_(Hand hand)
        {
            base.OnPress_(hand);
            if(catchHand==null && isCanMove)
            {
                Vector3 handPos = hand.transform.position;
                Vector3 handForward = hand.transform.forward;
                Quaternion rotation = Quaternion.LookRotation(handForward);
                handPos.y = 0;
                transform.position = Vector3.Lerp(transform.position, handPos, Time.deltaTime * moveSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * moveSpeed);
                float objTrackedSpeed = hand.GetTrackedObjectVelocity().magnitude;
                //速度调整
            }

        }
    }
}


