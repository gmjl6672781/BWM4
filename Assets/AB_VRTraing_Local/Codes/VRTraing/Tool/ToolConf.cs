using BeinLab.Util;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing.Conf
{
    public struct ToolTaskInfo
    {
        public bool isSetCanHover;
        public bool isCanHover;
        public bool isSetCanCatch;
        public bool isCanCatch;
        public bool isSetKinematic;
        public bool isKinematic;
        public bool isSetHide;
        public bool isHide;
        public bool isSetHighlight;
        public bool isHighlight;
        public bool isSetScaleSize;
        public Vector3 scaleSize;
        public bool isSetPose;
        public Vector3 position;
        public Vector3 angle;
        public int indexAoCao;
    }

    public class ToolConf : ScriptableObject
    {
        public string toolName;
        public float mass = 0.5f;
        [Range(0, 1)]
        [Tooltip("阻力")]
        public float drag = 0f;
        [Range(0, 1)]
        [Tooltip("角阻力")]
        public float angularDrag = 0.05f;
        public GameObject toolModel;

        //高亮信息设置        
        public bool isLightFlash;
        public bool isFlashStart;//是否在开始闪烁，闪烁3次后就恢复默认状态
        public bool isShowBody;
        public bool isLightAll;
        public float flashFrequency;
        public List<string> lightObjNames;
        public Material lightMat;
        [HideInInspector]
        public ToolBasic toolBasic;

        //拿起时的物理信息设置
        [EnumFlags]
        public Hand.AttachmentFlags catchFlags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.VelocityMovement | Hand.AttachmentFlags.TurnOffGravity;
        public Vector3 handPosition;
        public Vector3 handAngle;

        /// <summary>
        /// 抓取高亮设置
        /// </summary>
        public bool isSetCatchHighlight;
        public bool isCatchHighlight;
        //碰到其他工具时的姿势设置(OnTrigger)
        public bool isSetTriggerPose;
        public Vector3 triggerPositon;
        public Vector3 triggerAngle;

        public bool isSetInitCanHover;
        public bool isInitCanHover;
        public bool isSetInitCanCatch;
        public bool isInitCanCatch;
        public bool isSetInitKinematic;
        public bool isInitKinematic;
        public bool isSetInitHide;
        public bool isInitHide;
        public bool isSetInitHighlight;
        public bool isInitHighlight;
        public bool isSetInitScaleSize;
        public Vector3 InitScaleSize;
        public bool isSetInitPose;
        public Vector3 InitPosition;
        public Vector3 InitAngle;

        /// <summary>
        /// 可以放到的某些位置的配置
        /// </summary>
        public List<PutTooConf> putList;
        public int indexAoCao = 0;
        public bool canPutAoCao =true;


#if UNITY_EDITOR
        [MenuItem("Assets/Create/VRTracing/ToolConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = CreateInstance<ToolConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<ToolConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(ToolConf) + " is null");
                }
            }
        }
#endif
    }
}

