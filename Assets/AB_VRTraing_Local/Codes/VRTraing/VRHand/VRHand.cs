using BeinLab.Util;
using BeinLab.VRTraing.Mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing.Gamer
{
    /// <summary>
    /// VR手柄的接口，通过此类将VR手柄的交互事件进行封装
    /// 例如抓取物体时，触碰到可交互的物体.
    /// 射线辅助UI触发
    /// </summary>
    public class VRHand : MonoBehaviour
    {
        private Hand hand;
        public Transform rayPoint;
        private Ray uiRay;
        private LineRenderer uiLine;
        /// <summary>
        /// 窗口的节点跟随目标
        /// </summary>
        private LineRenderer dlgLine;
        public delegate void FocusedChangedDelegate(GameObject previousObject, GameObject newObject);
        /// <summary>
        /// Indicates whether the user is currently gazing at an object.
        /// </summary>
        public bool IsGazingAtObject { get; private set; }

        /// <summary>
        /// HitInfo property gives access to information at the object being gazed at, if any.
        /// </summary>
        public RaycastHit HitInfo { get { return hitInfo; } }
        private RaycastHit hitInfo;

        /// <summary>
        /// The game object that is currently being gazed at, if any.
        /// </summary>
        public GameObject HitObject { get; private set; }

        /// <summary>
        /// Position at which the gaze manager hit an object.
        /// If no object is currently being hit, this will use the last hit distance.
        /// </summary>
        public Vector3 HitPosition { get; private set; }


        /// <summary>
        /// Origin of the gaze.
        /// </summary>
        public Vector3 GazeOrigin { get; private set; }

        /// <summary>
        /// Normal of the gaze.
        /// </summary>
        public Vector3 GazeNormal { get; private set; }

        /// <summary>
        /// Maximum distance at which the gaze can collide with an object.
        /// </summary>
        public float MaxGazeCollisionDistance = 10.0f;

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.
        ///
        /// Example Usage:
        ///
        /// // Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)
        ///
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers & ~sr;
        /// GazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// </summary>
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the HitObject when raycasting.\n\nExample Usage:\n\n// Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)\n\nint sr = LayerMask.GetMask(\"SR\");\nint nonSR = Physics.DefaultRaycastLayers & ~sr;\nGazeManager.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };")]
        public LayerMask[] RaycastLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        //[Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        //public BaseRayStabilizer Stabilizer = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and orientation.
        /// Defaults to the main camera.
        /// </summary>
        [Tooltip("Transform that should be used to represent the gaze position and orientation. Defaults to Camera.Main")]
        public Transform GazeTransform;
        public event FocusedChangedDelegate FocusedObjectChanged;
        /// <summary>
        /// Dispatched when focus shifts to a new object, or focus on current object
        /// is lost.
        /// </summary>

        private float lastHitDistance = 2.0f;

        /// <summary>
        /// Unity UI pointer event.  This will be null if the EventSystem is not defined in the scene.
        /// </summary>
        public PointerEventData UnityUIPointerEvent { get; private set; }

        /// <summary>
        /// Cached results of racast results.
        /// </summary>
        private List<RaycastResult> raycastResultList = new List<RaycastResult>();
        /// <summary>
        /// 手柄初始化标记
        /// </summary>
        private bool isActiveHand = false;
        public bool ShouldSendUnityUiEvents { get { return UnityUIPointerEvent != null && EventSystem.current != null; } }

        public LineRenderer DlgLine { get => dlgLine; set => dlgLine = value; }

        public SteamVR_Action_Boolean uiInteractAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
        public Camera mainCam;
        //private bool isPressDown = false;
        private Ray helpRay = new Ray();
        /// <summary>
        /// 等待手柄的初始化，经过测试，手柄初始化时间短于0.2S，为了安全延时1S进行手势匹配
        /// </summary>
        private float watieTime = 0.5f;
        private Camera headCam;
        public float dragDelt = 10f;
        private bool isDrag = false;
        private void Start()
        {
            DlgLine = UnityUtil.GetTypeChildByName<LineRenderer>(gameObject, "DlgLine");
            DlgLine.enabled = false;
            if (GazeTransform == null)
            {
                GazeTransform = rayPoint;
            }
            FocusedObjectChanged += FocusedChanged;
        }

        public void SetLinePath(Vector3[] path)
        {
            DlgLine.positionCount = path.Length;
            DlgLine.SetPositions(path);
        }
        public Vector3 GetDlgLinePos()
        {
            Vector3 pos = transform.position;
            if (DlgLine && DlgLine.positionCount > 1)
            {
                pos = DlgLine.transform.TransformPoint(DlgLine.GetPosition(DlgLine.positionCount - 1));
            }
            return pos;
        }
        public Vector3 GetDlgLinePos(Vector3 transPos)
        {
            Vector3 pos = transform.position;
            if (DlgLine && DlgLine.positionCount > 1)
            {
                pos = DlgLine.transform.TransformPoint(transPos);
            }
            return pos;
        }

        private void FocusedChanged(GameObject previousObject, GameObject newObject)
        {
            if (previousObject != null)
            {
                //ExecuteEvents.ExecuteHierarchy(previousObject, null, OnFocusExitEventHandler);
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(previousObject, UnityUIPointerEvent, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.ExecuteHierarchy(previousObject, UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                    ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.endDragHandler);
                }

            }

            if (newObject != null)
            {
                //ExecuteEvents.ExecuteHierarchy(newObject, null, OnFocusEnterEventHandler);
                if (ShouldSendUnityUiEvents)
                {
                    ExecuteEvents.ExecuteHierarchy(newObject, UnityUIPointerEvent, ExecuteEvents.pointerEnterHandler);
                }
            }
        }
        private Coroutine handInitCoroutine;
        /// <summary>
        /// 激活手柄的手势跟随系统
        /// </summary>
        public void OnVRHandActive()
        {
            if (hand)
            {
                hand.ShowController(true);
                if (handInitCoroutine == null)
                {
                    handInitCoroutine = StartCoroutine(StopDefaultHint());
                }
            }
        }
        private void OnDisable()
        {
            if (VRHandHelper.Instance && !VRHandHelper.Instance.isActiveHandController)
            {
                isActiveHand = false;
            }
            handInitCoroutine = null;
        }
        private IEnumerator StopDefaultHint()
        {
            hand.ShowController(true);
            yield return new WaitForSeconds(watieTime);
            for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
            {
                ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
                if (action.GetActive(hand.handType))
                {
                    ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName());
                }
            }
            ControllerButtonHints.HideAllTextHints(hand);
            hand.ShowController(true);
            ///手柄完成初始化，通知VR手柄控制器帮助类
            if (VRHandHelper.Instance)
            {
                VRHandHelper.Instance.SetHandActive(hand);
            }
            isActiveHand = true;
            handInitCoroutine = null;
            yield break;
        }

        /// <summary>
        /// 更新手柄逻辑
        /// 1，初始化手柄手势跟随效果
        /// 2，检测是否开启射线检测，如果手拿物体或者其他不可触发条件时，关闭射线
        /// 3,开启射线检测，判断规定的距离内是否存在触碰点，如果存在，则开启射线辅助，进行事件触发机制
        /// </summary>
        private void FixedUpdate()
        {
            ///初始化手柄手势，需要在Update中完成延时操作，不能在Start或者Awake中执行
            ///因为手柄初始化需要延时至少0.2S（在完成定位连接之后）
            if (!isActiveHand && hand&&hand.isActiveAndEnabled && hand.isPoseValid && hand.mainRenderModel)
            {
                OnVRHandActive();
                return;
            }

            ///当瞄准点为空，或者当前拿取物体时，屏蔽掉信息
            if (GazeTransform == null || hand.currentAttachedObject)
            {
                uiLine.enabled = false;
                return;
            }
            ///获取瞄准点信息
            UpdateGazeInfo();

            // Perform raycast to determine gazed object

            RaycastPhysics();
            // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
            if (EventSystem.current != null && IsGazingAtObject)
            {
                // NOTE: We need to do this AFTER we set the HitPosition and HitObject since we need to use HitPosition to perform the correct 2D UI Raycast.
                IsGazingAtObject = IsHitUI();
            }
            UpdateRayPoint();

            if (HitObject)
            {
                ///按下手柄
                if (uiInteractAction.GetStateDown(hand.handType))
                {
                    isDrag = false;
                    //UnityUIPointerEvent.delta = Vector2.zero;
                    //isPressDown = true;

                    //ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.beginDragHandler);
                    pressTime = Time.time;
                }
                if (uiInteractAction.GetState(hand.handType))
                {
                    ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.pointerDownHandler);
                    if (!isDrag && Time.time - pressTime > clickTime / 2)
                    {
                        //if (!isDrag && Vector2.SqrMagnitude(UnityUIPointerEvent.delta) > dragDelt)
                        //{
                        isDrag = true;

                        ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.beginDragHandler);
                        //}
                    }
                    if (isDrag)
                    {
                        ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.dragHandler);
                    }
                }
                if (uiInteractAction.GetStateUp(hand.handType))
                {
                    if (Time.time - pressTime < clickTime)
                    {
                        ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.pointerClickHandler);
                        //LanguageMgr.Instance.PlayEffectAudioByKey(1);
                    }
                    isDrag = false;
                    UnityUIPointerEvent.delta = Vector2.zero;

                    ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.endDragHandler);
                }
            }
        }
        private float pressTime = 0;
        public float clickTime = 0.2f;
        /// <summary>
        /// Updates the current gaze information, so that the gaze origin and normal are accurate.
        /// </summary>
        private void UpdateGazeInfo()
        {
            Vector3 newGazeOrigin = GazeTransform.position;
            Vector3 newGazeNormal = GazeTransform.forward;

            // Update gaze info from stabilizer
            //if (Stabilizer != null)
            //{
            //    Stabilizer.UpdateStability(newGazeOrigin, GazeTransform.rotation);
            //    newGazeOrigin = Stabilizer.StablePosition;
            //    newGazeNormal = Stabilizer.StableRay.direction;
            //}

            GazeOrigin = newGazeOrigin;
            GazeNormal = newGazeNormal;
        }

        /// <summary>
        /// Perform a Unity physics Raycast to determine which scene objects with a collider is currently being gazed at, if any.
        /// </summary>
        private GameObject RaycastPhysics()
        {
            GameObject previousFocusObject = HitObject;

            // If there is only one priority, don't prioritize
            if (RaycastLayerMasks.Length == 1)
            {
                IsGazingAtObject = Physics.Raycast(GazeOrigin, GazeNormal, out hitInfo, MaxGazeCollisionDistance, RaycastLayerMasks[0]);
            }
            else
            {
                // Raycast across all layers and prioritize
                RaycastHit? hit = UnityUtil.PrioritizeHits(Physics.RaycastAll(new Ray(GazeOrigin, GazeNormal), MaxGazeCollisionDistance, -1), RaycastLayerMasks);

                IsGazingAtObject = hit.HasValue;
                if (IsGazingAtObject)
                {
                    hitInfo = hit.Value;
                }
            }

            if (IsGazingAtObject)
            {
                //HitObject = HitInfo.collider.gameObject;
                HitPosition = HitInfo.point;
                lastHitDistance = HitInfo.distance;
            }
            else
            {
                HitObject = null;
                HitPosition = GazeOrigin + (GazeNormal * lastHitDistance);
            }
            return previousFocusObject;
        }

        /// <summary>
        /// Perform a Unity UI Raycast, compare with the latest 3D raycast, and overwrite the hit object info if the UI gets focus
        /// </summary>
        private void RaycastUnityUI()
        {
            IsGazingAtObject = false;
            if (UnityUIPointerEvent == null)
            {
                UnityUIPointerEvent = new PointerEventData(EventSystem.current);
            }
            if (!mainCam)
            {
                if (Player.instance)
                {
                    mainCam = Player.instance.hmdTransform.GetComponent<Camera>(); ;
                }
                return;
            }
            // 2D cursor position
            Vector2 cursorScreenPos = mainCam.WorldToScreenPoint(HitPosition);
            UnityUIPointerEvent.delta = (cursorScreenPos - UnityUIPointerEvent.position);
            UnityUIPointerEvent.position = cursorScreenPos;

            // Graphics raycast
            raycastResultList.Clear();
            EventSystem.current.RaycastAll(UnityUIPointerEvent, raycastResultList);
            RaycastResult uiRaycastResult = UnityUtil.FindClosestRaycastHitInLayermasks(raycastResultList, RaycastLayerMasks);
            UnityUIPointerEvent.pointerCurrentRaycast = uiRaycastResult;
            // If we have a raycast result, check if we need to overwrite the 3D raycast info
            if (uiRaycastResult.gameObject != null)
            {
                // Add the near clip distance since this is where the raycast is from
                float uiRaycastDistance = uiRaycastResult.distance + mainCam.nearClipPlane;
                //float uiRaycastDistance = uiRaycastResult.distance;// + Camera.main.nearClipPlane;

                bool superseded3DObject = false;
                if (IsGazingAtObject)
                {
                    // Check layer prioritization
                    if (RaycastLayerMasks.Length > 1)
                    {
                        // Get the index in the prioritized layer masks
                        int uiLayerIndex = UnityUtil.FindLayerListIndex(uiRaycastResult.gameObject.layer, RaycastLayerMasks);
                        int threeDLayerIndex = UnityUtil.FindLayerListIndex(hitInfo.collider.gameObject.layer, RaycastLayerMasks);

                        if (threeDLayerIndex > uiLayerIndex)
                        {
                            superseded3DObject = true;
                        }
                        else if (threeDLayerIndex == uiLayerIndex)
                        {
                            if (hitInfo.distance > uiRaycastDistance)
                            {
                                superseded3DObject = true;
                            }
                        }
                    }
                    else
                    {
                        if (hitInfo.distance > uiRaycastDistance)
                        {
                            superseded3DObject = true;
                        }
                    }
                }

                // Check if we need to overwrite the 3D raycast info
                if (!IsGazingAtObject || superseded3DObject)
                {
                    IsGazingAtObject = true;
                    Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(uiRaycastResult.screenPosition.x,
                        uiRaycastResult.screenPosition.y, uiRaycastDistance - mainCam.nearClipPlane));
                    helpRay.origin = uiLine.transform.position;
                    helpRay.direction = uiLine.transform.forward.normalized;
                    //worldPos = helpRay.GetPoint(uiRaycastDistance);//Camera.main.ScreenToWorldPoint(new Vector3(uiRaycastResult.screenPosition.x, uiRaycastResult.screenPosition.y, uiRaycastDistance));
                    hitInfo = new RaycastHit()
                    {
                        distance = uiRaycastDistance,
                        normal = -mainCam.transform.forward,
                        point = worldPos
                    };
                    HitObject = uiRaycastResult.gameObject;
                    HitPosition = HitInfo.point;
                    lastHitDistance = HitInfo.distance;

                    ///大于最大距离
                    if (Vector3.Distance(HitPosition, GazeTransform.position) > MaxGazeCollisionDistance)
                    {
                        IsGazingAtObject = false;
                        HitObject = null;
                        print(IsGazingAtObject);
                    }
                }
            }
            else
            {
                IsGazingAtObject = false;
                HitObject = null;
                //print(IsGazingAtObject);

            }
        }


        private void Awake()
        {
            hand = GetComponent<Hand>();
            uiLine = rayPoint.GetComponentInChildren<LineRenderer>();
            uiLine.enabled = false;
        }



        /// <summary>
        /// 辅助射线,辅助UI点击事件
        /// 1，没有拿到物体的时候才会出现
        /// 2，只有检测到UI层才会出现
        /// 3，只有在合适的距离才会出现
        /// </summary>
        public void UpdateRayPoint()
        {
            bool isShowLine = false;
            if (IsGazingAtObject)
            {
                isShowLine = true;
                uiLine.SetPosition(0, uiLine.transform.position);
                uiLine.SetPosition(1, HitPosition);
            }
            /// line 是否显示
            if (uiLine.enabled != isShowLine)
            {
                uiLine.enabled = isShowLine;
            }
        }









        /// <summary>
        /// 手柄控制UI事件
        /// </summary>
        /// <param name="closestInteractable"></param>
        public bool IsHitUI()
        {
            bool isHitUI = RaycastUnityUI(HitPosition);
            if (isHitUI)
            {
                ///获取当前触碰到的UI
                RaycastResult result = UnityUIPointerEvent.pointerCurrentRaycast;
                GameObject resObj = result.gameObject;
                ///当两次触碰不一致时
                if (HitObject != resObj)
                {
                    if (HitObject)
                    {
                        ExecuteEvents.ExecuteHierarchy(HitObject,
                    UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                        //Debug.Log("pointerExitHandler!!!"+"========="+ HitObject.name);
                        HandleUp();
                    }
                    HitObject = resObj;
                    ExecuteEvents.ExecuteHierarchy(HitObject,
                   UnityUIPointerEvent, ExecuteEvents.pointerEnterHandler);
                    //Debug.Log("pointerEnterHandler!!!" + "========" + HitObject.name);
                    //LanguageMgr.Instance.PlayEffectAudioByKey(0);
                }
                return true;
            }
            if (HitObject)
            {
                ExecuteEvents.ExecuteHierarchy(HitObject,
                UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
                HandleUp();
                ExecuteEvents.ExecuteHierarchy(HitObject, UnityUIPointerEvent, ExecuteEvents.endDragHandler);
            }
            UnityUIPointerEvent.delta = Vector2.zero;
            HitObject = null;
            return false;

        }
        public bool RaycastUnityUI(Vector3 point)
        {
            // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
            if (EventSystem.current == null)
            {
                Debug.LogError("缺少系统事件，请创建 EventSystem ！");
                return false;
            }
            if (UnityUIPointerEvent == null)
            {
                UnityUIPointerEvent = new PointerEventData(EventSystem.current);
            }
            if (!headCam)
            {
                headCam = Player.instance.hmdTransform.GetComponent<Camera>();
                return false;
            }
            Vector2 cursorScreenPos = headCam.WorldToScreenPoint(point);
            UnityUIPointerEvent.delta = (cursorScreenPos - UnityUIPointerEvent.position);
            UnityUIPointerEvent.position = cursorScreenPos;

            //JsonUtility
            // Graphics raycast
            raycastResultList.Clear();
            EventSystem.current.RaycastAll(UnityUIPointerEvent, raycastResultList);
            if (raycastResultList.Count > 0)
            {
                RaycastResult uiRaycastResult = UnityUtil.FindClosestRaycastHitInLayermasks(raycastResultList, RaycastLayerMasks);
                UnityUIPointerEvent.pointerCurrentRaycast = uiRaycastResult;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 执行按下事件
        /// </summary>
        public void HandleDown()
        {
            if (HitObject)
            {
                ExecuteEvents.ExecuteHierarchy(HitObject,
                       UnityUIPointerEvent, ExecuteEvents.pointerDownHandler);
            }
        }

        /// <summary>
        /// 执行抬起事件
        /// </summary>
        public void HandleUp()
        {
            if (HitObject)
            {
                ExecuteEvents.ExecuteHierarchy(HitObject,
                       UnityUIPointerEvent, ExecuteEvents.pointerUpHandler);
            }
        }
        /// <summary>
        /// 执行点击事件
        /// </summary>
        public void HandleClick()
        {
            if (HitObject)
            {
                ExecuteEvents.ExecuteHierarchy(HitObject,
                       UnityUIPointerEvent, ExecuteEvents.pointerClickHandler);
                Debug.Log("pointerClickHandler!!!");
                //LanguageMgr.Instance.PlayEffectAudioByKey(1);
            }
        }

    }
}