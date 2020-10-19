using UnityEngine;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing
{
    public class ToolNumPaper : ToolBasic
    {
        protected override void Awake()
        {
            base.Awake();
            GetComponentInChildren<TextMesh>().text = "";
        }

        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            PutToAoCao(toolConf.putList[0]);
        }
    }
}