using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Gamer;
using BeinLab.RS5.Mgr;
using BeinLab.Util;
using Karler.WarFire.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.FengYun.UI
{
    /// <summary>
    /// 切换视角的Dlg
    /// </summary>
    public class SwitchViewDlg : MonoBehaviour
    {
        public Vector3 inverPosition;
        public Vector3 inverAngle;
        private Vector3 lastPos;
        private Vector3 lastAngle;
        private Vector3 lastScale;
        private GameObject fllowTarget;
        private bool isChange = false;
        /// <summary>
        /// 目标名称
        /// </summary>
        public string targetName;
        void Start()
        {
            BaseDlg dlg = GetComponent<BaseDlg>();
            if (!fllowTarget)
            {
                fllowTarget = new GameObject();
                UnityUtil.SetParent(transform, fllowTarget.transform);
            }
            Toggle toggle = dlg.GetChildComponent<Toggle>("Toggle");
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        ///  
        /// </summary>
        private void OnDestroy()
        {
            OnValueChanged(false);
        }

        private void OnValueChanged(bool isOn)
        {
            if (!XRController.Instance || !GameNoder.Instance)
            {
                return;
            }
            if (isOn)
            {
                lastPos = GameNoder.Instance.Root.localPosition;
                lastAngle = GameNoder.Instance.Root.localEulerAngles;
                lastScale = GameNoder.Instance.Root.localScale;
                Transform target = XRController.Instance.ARCamera.transform;
                fllowTarget.transform.position = target.position;
                fllowTarget.transform.rotation = target.rotation;
                Vector3 angle = fllowTarget.transform.eulerAngles;
                angle.x = angle.z = 0;
                fllowTarget.transform.eulerAngles = angle;
                Vector3 pos = fllowTarget.transform.TransformPoint(inverPosition);
                Vector3 rot = angle + inverAngle;
                GameNoder.Instance.Root.localScale = Vector3.one;
                GameNoder.Instance.Root.position = pos;
                GameNoder.Instance.Root.eulerAngles = rot;
                isChange = true;
                //StartCoroutine(CameraFllowTarget());
            }
            else if (isChange)
            {
                GameNoder.Instance.Root.localPosition = lastPos;
                GameNoder.Instance.Root.localEulerAngles = lastAngle;
                GameNoder.Instance.Root.localScale = lastScale;
                Transform targetCamera = XRController.Instance.ARCamera.transform.parent;
                //targetCamera.position = Vector3.zero;
                //targetCamera.rotation = Quaternion.identity;
            }
            GameNoder.Instance.IsCanTransfer = !isOn;
        }

        //private IEnumerator CameraFllowTarget()
        //{
        //    GameDynamicer dynamicer = DynamicActionController.Instance.GetGameDynamicerByName(targetName);
        //    Transform targetCamera = XRController.Instance.ARCamera.transform.parent;
        //    Vector3 localPos = dynamicer.transform.InverseTransformPoint(targetCamera.position);
        //    while (dynamicer && targetCamera)
        //    {
        //        targetCamera.position =Vector3.Lerp(targetCamera.position, dynamicer.transform.TransformPoint(localPos),Time.deltaTime);
        //        yield return new WaitForFixedUpdate();
        //    }
        //}
    }
}
