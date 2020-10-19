using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolDianDongBanShou :  ToolBasic
    {
        public Rigidbody rig;

        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);

            if (rig!=null)
            {
                rig.useGravity = true;
                rig.isKinematic = false;
            }
        }
    }
}
