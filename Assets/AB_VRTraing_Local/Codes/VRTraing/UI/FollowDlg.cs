using BeinLab.Util;
using BeinLab.VRTraing.Gamer;
using BeinLab.VRTraing.Mgr;
using Karler.WarFire.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.UI
{
    //可以跟随头或手柄
    [RequireComponent(typeof(BaseDlg))]
    public class FollowDlg : MonoBehaviour
    {
        /// <summary>
        /// 跟随配置
        /// </summary>
        public FollowDlgConf followDlgConf;
        /// <summary>
        /// 哪个手柄
        /// </summary>
        private BaseDlg baseDlg;
        private VRHand target;

        private void Awake()
        {
            baseDlg = GetComponent<BaseDlg>();
        }

        private void Start()
        {
            target = null;
            if (VRHandHelper.Instance && followDlgConf && followDlgConf.followType == FollowType.FollowHand)
            {
                for (int i = 0; i < Player.instance.handCount; i++)
                {
                    if (followDlgConf.handType == SteamVR_Input_Sources.Any || Player.instance.hands[i].handType == followDlgConf.handType)
                    {
                        target = Player.instance.hands[i].GetComponent<VRHand>();
                        break;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (VRHandHelper.Instance && followDlgConf && followDlgConf.followType == FollowType.FollowHead)
            {
                if (Vector3.Angle(VRHandHelper.Instance.Target.forward, transform.position - VRHandHelper.Instance.Target.position) > 38
                    || Vector3.Distance(VRHandHelper.Instance.Target.position, transform.position) > followDlgConf.fllowDis * 1.2f ||
                    Vector3.Distance(VRHandHelper.Instance.Target.position, transform.position) < followDlgConf.fllowMinDis)
                {
                    transform.position = Vector3.Lerp(transform.position, UnityUtil.GetFrontPos(VRHandHelper.Instance.Target, followDlgConf.fllowDis, false), Time.deltaTime * followDlgConf.flowSpeed);
                }
                UnityUtil.LookAtV(transform, VRHandHelper.Instance.Target, -1);
            }

            if (VRHandHelper.Instance && followDlgConf && followDlgConf.followType == FollowType.FollowHand)
            {
                if (target && baseDlg.UiRoot.gameObject.activeSelf)
                {
                    transform.position = target.GetDlgLinePos();
                    UnityUtil.LookAtV(transform, Player.instance.hmdTransform, followDlgConf.forward);
                }
            }

        }
    }
}

