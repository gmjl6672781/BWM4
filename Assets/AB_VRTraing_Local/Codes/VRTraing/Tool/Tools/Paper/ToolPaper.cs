using BeinLab.VRTraing.Conf;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing
{
    public class ToolPaper : ToolBasic
    {
        public List<GameObject> listPaper;
        protected override void Awake()
        {
            base.Awake();
            foreach (var item in listPaper)
            {
                item.SetActive(true);
            }
        }
        private Hand currentHand;
        public Collider toolCollider;
        protected override void Start()
        {
            base.Start();
            ToggleCollider(false);
            toolCollider.enabled = true;
        }
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            ToggleCollider(true);
            toolCollider.enabled = false;
        }
        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            ToggleCollider(false);
            toolCollider.enabled = true;
        }
    }
}