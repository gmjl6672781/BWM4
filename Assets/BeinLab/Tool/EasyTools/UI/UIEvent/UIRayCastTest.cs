using BeinLab.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRayCastTest : MonoBehaviour
{
    private LineRenderer line;
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
    public bool ShouldSendUnityUiEvents { get { return UnityUIPointerEvent != null && EventSystem.current != null; } }
    /// <summary>
    /// Cached results of racast results.
    /// </summary>
    private List<RaycastResult> raycastResultList = new List<RaycastResult>();
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponentInChildren<LineRenderer>();
        if (IsGazingAtObject)
        {
            line.enabled = true;
            line.SetPosition(1, line.transform.InverseTransformPoint(HitPosition));
        }
        else
        {
            line.enabled = false;
        }
        FocusedObjectChanged += FocusedChanged;
    }
    private void FocusedChanged(GameObject previousObject, GameObject newObject)
    {
        if (previousObject != null)
        {
            //ExecuteEvents.ExecuteHierarchy(previousObject, null, OnFocusExitEventHandler);
            if (ShouldSendUnityUiEvents)
            {
                ExecuteEvents.ExecuteHierarchy(previousObject, UnityUIPointerEvent, ExecuteEvents.pointerExitHandler);
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
    // Update is called once per frame
    void Update()
    {
        UpdateGazeInfo();
        GameObject previousFocusObject = RaycastPhysics();

        // If we have a unity event system, perform graphics raycasts as well to support Unity UI interactions
        if (EventSystem.current != null)
        {
            // NOTE: We need to do this AFTER we set the HitPosition and HitObject since we need to use HitPosition to perform the correct 2D UI Raycast.
            RaycastUnityUI();
        }

        // Dispatch changed event if focus is different
        if (previousFocusObject != HitObject && FocusedObjectChanged != null)
        {
            FocusedObjectChanged(previousFocusObject, HitObject);
        }

        if (IsGazingAtObject)
        {
            line.enabled = true;
            line.SetPosition(1, line.transform.InverseTransformPoint(HitPosition));
        }
        else
        {
            line.enabled = false;
        }
    }
    private void UpdateGazeInfo()
    {
        Vector3 newGazeOrigin = GazeTransform.position;
        Vector3 newGazeNormal = GazeTransform.forward;
        GazeOrigin = newGazeOrigin;
        GazeNormal = newGazeNormal;
    }
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
            RaycastHit? hit = PrioritizeHits(Physics.RaycastAll(new Ray(GazeOrigin, GazeNormal), MaxGazeCollisionDistance, -1));

            IsGazingAtObject = hit.HasValue;
            if (IsGazingAtObject)
            {
                hitInfo = hit.Value;
            }
        }

        if (IsGazingAtObject)
        {
            HitObject = HitInfo.collider.gameObject;
            HitPosition = HitInfo.point;
            lastHitDistance = HitInfo.distance;
        }
        else
        {
            HitObject = null;
            //HitPosition = GazeOrigin + (GazeNormal * lastHitDistance);
            HitPosition = GazeOrigin + (GazeNormal * MaxGazeCollisionDistance);
        }
        return previousFocusObject;
    }

    private void RaycastUnityUI()
    {
        if (UnityUIPointerEvent == null)
        {
            UnityUIPointerEvent = new PointerEventData(EventSystem.current);
        }

        // 2D cursor position
        Vector2 cursorScreenPos = Camera.main.WorldToScreenPoint(HitPosition);
        UnityUIPointerEvent.delta = cursorScreenPos - UnityUIPointerEvent.position;
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
            float uiRaycastDistance = uiRaycastResult.distance + Camera.main.nearClipPlane;
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
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(uiRaycastResult.screenPosition.x, uiRaycastResult.screenPosition.y, uiRaycastDistance));
                hitInfo = new RaycastHit()
                {
                    distance = uiRaycastDistance,
                    normal = -Camera.main.transform.forward,
                    point = worldPos
                };

                HitObject = uiRaycastResult.gameObject;
                HitPosition = HitInfo.point;
                lastHitDistance = HitInfo.distance;
            }
            if (Vector3.Distance(HitPosition, transform.position) > MaxGazeCollisionDistance)
            {
                IsGazingAtObject = false;
                HitObject = null;
            }
        }
    }

    #region Helpers

    /// <summary>
    /// Find the closest raycast hit in the list of RaycastResults that is also included in the LayerMask list.  
    /// </summary>
    /// <param name="candidates">List of RaycastResults from a Unity UI raycast</param>
    /// <param name="layerMaskList">List of layers to support</param>
    /// <returns>RaycastResult if hit, or an empty RaycastResult if nothing was hit</returns>
    private RaycastResult FindClosestRaycastHitInLayermasks(List<RaycastResult> candidates, LayerMask[] layerMaskList)
    {
        int combinedLayerMask = 0;
        for (int i = 0; i < layerMaskList.Length; i++)
        {
            combinedLayerMask = combinedLayerMask | layerMaskList[i].value;
        }

        RaycastResult? minHit = null;
        for (var i = 0; i < candidates.Count; ++i)
        {
            if (candidates[i].gameObject == null || !IsLayerInLayerMask(candidates[i].gameObject.layer, combinedLayerMask))
            {
                continue;
            }
            if (minHit == null || candidates[i].distance < minHit.Value.distance)
            {
                minHit = candidates[i];
            }
        }

        return minHit ?? new RaycastResult();
    }

    /// <summary>
    /// Look through the layerMaskList and find the index in that list for which the supplied layer is part of
    /// </summary>
    /// <param name="layer">Layer to search for</param>
    /// <param name="layerMaskList">List of LayerMasks to search</param>
    /// <returns>LayerMaskList index, or -1 for not found</returns>
    private int FindLayerListIndex(int layer, LayerMask[] layerMaskList)
    {
        for (int i = 0; i < layerMaskList.Length; i++)
        {
            if (IsLayerInLayerMask(layer, layerMaskList[i].value))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsLayerInLayerMask(int layer, int layerMask)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    private RaycastHit? PrioritizeHits(RaycastHit[] hits)
    {
        if (hits.Length == 0)
        {
            return null;
        }

        // Return the minimum distance hit within the first layer that has hits.
        // In other words, sort all hit objects first by layerMask, then by distance.
        for (int layerMaskIdx = 0; layerMaskIdx < RaycastLayerMasks.Length; layerMaskIdx++)
        {
            RaycastHit? minHit = null;

            for (int hitIdx = 0; hitIdx < hits.Length; hitIdx++)
            {
                RaycastHit hit = hits[hitIdx];
                if (IsLayerInLayerMask(hit.transform.gameObject.layer, RaycastLayerMasks[layerMaskIdx]) &&
                    (minHit == null || hit.distance < minHit.Value.distance))
                {
                    minHit = hit;
                }
            }

            if (minHit != null)
            {
                return minHit;
            }
        }

        return null;
    }

    #endregion Helpers
}
