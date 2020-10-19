using BeinLab.FengYun.Gamer;
using BeinLab.RS5.Mgr;
using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;
namespace BeinLab.FengYun.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class XRTracker : Singleton<XRTracker>
    {
        //private ARRaycastManager rayManager;
        private Vector2 touchPos = Vector2.zero;
        //private List<ARRaycastHit> hitList;
        public static Vector3 arAnchorPos = Vector3.zero;
        public static bool isHit = false;
        //[EnumFlags]
        //private TrackableType trakerType;
        public LayerMask layer;
        public LayerMask locklayer;
        //public static bool isLock = false;
        public static Vector3 localAngle;
        //private ARPointCloud pointCloud;
        public int minPointCount = 30;
        private EdgeCheckAnimation cameraAnimation;
        // Start is called before the first frame update
        void Start()
        {
            isHit = false;
            arAnchorPos = Vector3.zero;
            //rayManager = GameObject.Find("AR Session Origin").GetComponent<ARRaycastManager>();
            //pointCloud = rayManager.GetComponentInChildren<ARPointCloud>();
            //cameraAnimation = rayManager.GetComponentInChildren<EdgeCheckAnimation>();
            //hitList = new List<ARRaycastHit>();
            //touchPos.x = Screen.width / 2;
            //touchPos.y = Screen.height / 2;
            //trakerType = TrackableType.PlaneWithinPolygon |
            //    TrackableType.PlaneWithinBounds | TrackableType.FeaturePoint | TrackableType.PlaneEstimated
            //    | TrackableType.Planes;
            //cameraAnimation.enabled = true;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (cameraAnimation)
            {
                cameraAnimation.enabled = false;
            }
        }


        // Update is called once per frame
        void Update()
        {
//            if (rayManager)
//            {
//                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
//                {
//                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
//                    {
//                        return;
//                    }
//                    RaycastHit mhit;
//                    if (Physics.Raycast(XRController.Instance.ARCamera.ScreenPointToRay(
//                        Input.GetTouch(0).position), out mhit, 1000, locklayer))
//                    {
//                        localAngle = mhit.collider.GetComponentInParent<GameDynamicer>().transform.eulerAngles;
//                        XRController.Instance.XRShow();
//                    }
//                }
//                if (Input.GetMouseButtonDown(0))
//                {
//                    RaycastHit mhit;
//                    if (Physics.Raycast(XRController.Instance.ARCamera.ScreenPointToRay(
//                       Input.mousePosition), out mhit, 1000, locklayer))
//                    {
//                        localAngle = mhit.collider.GetComponentInParent<GameDynamicer>().transform.eulerAngles;
//                        XRController.Instance.XRShow();
//                    }
//                }
//#if UNITY_EDITOR
//                RaycastHit hit;
//                if (Physics.Raycast(rayManager.GetComponentInChildren<Camera>().ScreenPointToRay(touchPos), out hit, 1000, layer))
//                {
//                    isHit = true;
//                    arAnchorPos = hit.point;
//                }
//                else
//                {
//                    isHit = false;
//                }
//#else
//                if (rayManager.Raycast(touchPos, hitList, trakerType))
//                {
//                    isHit = true;
//                    arAnchorPos = hitList[0].pose.position;
//                }
//                else
//                {
//                    isHit = false;
//                }
//#endif
//            }
        }
    }
}