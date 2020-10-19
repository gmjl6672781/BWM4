using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    public class ToolMaBu : ToolBasic
    {
        public GameObject defState;
        public GameObject catchState;
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            defState.SetActive(false);
            catchState.SetActive(true);
        }
        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            defState.SetActive(true);
            catchState.SetActive(false);
        }
    }
}
