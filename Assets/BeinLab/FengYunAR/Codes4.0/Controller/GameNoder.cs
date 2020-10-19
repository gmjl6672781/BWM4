using BeinLab.FengYun.UI;
using BeinLab.RS5.Mgr;
using BeinLab.VRTraing.Mgr;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;

namespace BeinLab.Util
{
    /// <summary>
    /// 游戏节点汇总，动态加载的所有资源将存放于此
    /// 节点创建，节点销毁，节点查找。支持分号创建或者查找节点
    /// 游戏节点索引，可以配置的节点控制器
    /// 主要分两部分，一个是UI Canvas坐标，一个是世界坐标的节点，并且可以自动配置和添加
    /// 动态创建，监听
    /// </summary>
    public class GameNoder : Singleton<GameNoder>
    {
        /// <summary>
        /// UI层的对象配置
        /// </summary>
        private GameObject gameCanvas;
        /// <summary>
        /// 世界坐标
        /// </summary>
        private GameObject gameWorld;
        /// <summary>
        /// 世界UI坐标
        /// </summary>
        private GameObject worldCanvas;
        private Transform root;
        private bool isCanTransfer = true;
        private Transform defPos;
        public CameraClearFlags flags = CameraClearFlags.Color;
        /// <summary>
        /// 
        /// </summary>
        public GameObject GameCanvas
        {
            get
            {
                return gameCanvas;
            }

            set
            {
                gameCanvas = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public GameObject GameWorld
        {
            get
            {
                return gameWorld;
            }

            set
            {
                gameWorld = value;
            }
        }

        public GameObject WorldCanvas
        {
            get
            {
                return worldCanvas;
            }

            set
            {
                worldCanvas = value;
            }
        }

        public Transform Root
        {
            get { return root; }
            set
            {
                root = value;
            }
        }
        public bool IsCanTransfer
        {
            get { return isCanTransfer; }
            set
            {
                isCanTransfer = value;
            }
        }

        public Transform DefPos { get => defPos; set => defPos = value; }

        private float lastDis;
        private float pressDownTime;
        private Vector3 worldScale;
        private List<Touch> touchList;
        private List<Touch> pressTouchList;
        /// <summary>
        ///
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            Root = UnityUtil.GetTypeChildByName<Transform>(gameObject, "NodeRoot");
            GameCanvas = UnityUtil.GetChildByName(Root.gameObject, "Canvas");
            GameWorld = UnityUtil.GetChildByName(Root.gameObject, "World");
            WorldCanvas = UnityUtil.GetChildByName(GameWorld, "WorldCanvas");
            DefPos = UnityUtil.GetTypeChildByName<Transform>(gameObject, "DefultPos");
#if UNITY_EDITOR
            var cam = GameObject.FindObjectOfType<Camera>();
            if (cam)
            {
                if (flags != CameraClearFlags.Nothing)
                {
                    cam.clearFlags = flags;
                }
            }
#endif

        }
        private void Start()
        {
            if (VRHandHelper.Instance)
            {
                VRHandHelper.Instance.OnVRHandActive += OnVRHandActive;
            }
        }
        private void OnVRHandActive()
        {
            WorldCanvas.GetComponent<Canvas>().worldCamera = Player.instance.hmdTransform.GetComponent<Camera>();
        }

        public void ReSetRoot()
        {
            //ResetNoder();
            //SetShowRoot(Vector3.zero, Vector3.zero, Vector3.one);
        }
        /// <summary>
        /// 重置所有位置
        /// </summary>
        public void ResetNoder()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            //XRController.Instance.ARCamera.transform.parent.position = Vector3.zero;
            //XRController.Instance.ARCamera.transform.parent.rotation = Quaternion.identity;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="v"></param>
        public void ZoomShowModel(float v)
        {
            Vector3 scale = worldScale;
            scale.x += v * Time.deltaTime;
            scale.x = Mathf.Clamp(scale.x, XRController.Instance.minScale, XRController.Instance.maxScale);
            scale.z = scale.y = scale.x;
            worldScale = scale;

            GameNoder.Instance.Root.localScale = worldScale;
            //MsgDlg
            ReturnDlg.Instance.ShowMsg((scale.x).ToString("p0"));
        }

        /// <summary>
        /// 缩放 WorldAnchorZoom
        /// </summary>
        public void WorldAnchorRote()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                pressDownTime = Time.time;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Time.time - pressDownTime < 0.2f)
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    if (XRController.Instance.ARCamera)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(XRController.Instance.ARCamera.ScreenPointToRay(Input.mousePosition), out hit))
                        {
                            ColliderButton cb = hit.collider.GetComponent<ColliderButton>();
                            if (cb)
                            {
                                cb.OnClickCollider();
                            }
                        }
                    }
                }
            }
            if (Input.GetMouseButton(0) && UnityUtil.IsInSceneCenter(Input.mousePosition))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                float y = -Input.GetAxis("Mouse X") * Time.deltaTime * XRController.Instance.roteSpeed;

                y *= 100f;
                GameNoder.Instance.Root.Rotate(new Vector3(0, y, 0));
            }
#else

            if (Input.touchCount >= 1)
            {
                int roteCount = 0;
                Touch touch = Input.GetTouch(0);
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (UnityUtil.IsInSceneCenter(Input.GetTouch(i).position) && Input.GetTouch(i).phase == TouchPhase.Moved)
                    {
                        roteCount++;
                        //isCanRote = true;
                        touch = Input.GetTouch(i);
                    }
                    else
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                            {
                                continue;
                            }
                            pressDownTime = Time.time;
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended || Input.GetTouch(i).phase == TouchPhase.Canceled)
                        {
                            if (Time.time - pressDownTime < 0.2f)
                            {
                                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                                {
                                    continue;
                                }
                                if (XRController.Instance.ARCamera)
                                {
                                    RaycastHit hit;
                                    if (Physics.Raycast(XRController.Instance.ARCamera.ScreenPointToRay(Input.GetTouch(i).position), out hit))
                                    {
                                        ColliderButton cb = hit.collider.GetComponent<ColliderButton>();
                                        if (cb)
                                        {
                                            cb.OnClickCollider();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (roteCount != 1)
                {
                    return;
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPos = touch.deltaPosition;
                    GameNoder.Instance.Root.Rotate(-Vector3.up * deltaPos.x * XRController.Instance.roteSpeed * Time.deltaTime, Space.World);//绕Y轴进行旋转
                }
            }
#endif
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public bool WorldAnchorZoom()
        {
            if (!IsCanTransfer)
            {
                return IsCanTransfer;
            }
            bool isCanRote = true;
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                ZoomShowModel(Input.GetAxis("Mouse ScrollWheel") * XRController.Instance.zoomSpeed * 10);
            }
            if (Input.touchCount >= 2)
            {
                int moveCount = 0;
                if (touchList == null)
                {
                    touchList = new List<Touch>();
                }
                if (pressTouchList == null)
                {
                    pressTouchList = new List<Touch>();
                }
                touchList.Clear();
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (UnityUtil.IsInSceneCenter(Input.GetTouch(i).position) && Input.GetTouch(i).phase == TouchPhase.Moved)
                    {
                        ///选取前两个滑动的手指作为缩放参考
                        if (touchList.Count < 2)
                        {
                            touchList.Add(Input.GetTouch(i));
                        }
                        moveCount++;
                    }
                }
                if (moveCount != 2)
                {
                    lastDis = -1;
                    return isCanRote;
                }
                Touch touch1 = touchList[0];
                Touch touch2 = touchList[1];
                ///现在的大小
                float curDis = Vector2.Distance(touch1.position, touch2.position);
                if (lastDis <= 0)
                {
                    lastDis = curDis;
                }
                if (Mathf.Abs(curDis - lastDis) > 0)
                {
                    ZoomShowModel((curDis - lastDis) * XRController.Instance.zoomSpeed * Time.deltaTime);
                }
                lastDis = curDis;
                isCanRote = false;
            }
            else
            {
                lastDis = -1;
            }
            return isCanRote;
        }

        public void SetShowRoot(Vector3 position, Vector3 angle, Vector3 scale, bool isLookAtCamera = false)
        {
            Root.localPosition = position;
            Root.localEulerAngles = angle;
            Root.localScale = scale;
            worldScale = scale;
            ///看向相机
            if (isLookAtCamera)
            {
                UnityUtil.LookAtV(Root, XRController.Instance.ARCamera.transform);
            }
        }


        /// <summary>
        /// 当不存在节点时是否创建节点
        /// </summary>
        /// <param name="nodeConf"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public GameObject GetNode(NodeConf nodeConf, bool isCreate = false)
        {
            GameObject node = null;
            GameObject parent = nodeConf.nodeType == NodeType.World ?
                GameWorld : (nodeConf.nodeType == NodeType.UICanvas ? GameCanvas : WorldCanvas);
            string[] names = nodeConf.nodeName.Split('/', '|', ',');
            for (int i = 0; i < names.Length; i++)
            {
                node = UnityUtil.GetChildByName(parent, names[i], false);
                if (node)
                {
                    parent = node;
                }
                else if (isCreate)
                {
                    if (nodeConf.nodeType == NodeType.World)
                    {
                        node = new GameObject(names[i]);
                    }
                    else
                    {
                        node = new GameObject(names[i], typeof(RectTransform));
                    }
                    if (nodeConf.nodeType != NodeType.World)
                    {
                        node.layer = LayerMask.NameToLayer("UI");
                    }
                    node.transform.SetParent(parent.transform);
                    node.transform.localPosition = Vector3.zero;
                    node.transform.localEulerAngles = Vector3.zero;
                    node.transform.localScale = Vector3.one;
                    parent = node;
                }
                else
                {
                    break;
                }
            }
            if (node)
            {
                node.transform.localPosition = nodeConf.position;
                node.transform.localEulerAngles = nodeConf.angle;
                node.transform.localScale = nodeConf.scale;
            }
            return node;
        }

        public GameObject GetNode(NodeType nodeType, string nodeName)
        {
            GameObject node = null;
            GameObject parent = nodeType == NodeType.World ?
                GameWorld : (nodeType == NodeType.UICanvas ? GameCanvas : WorldCanvas);
            string[] names = nodeName.Split('/', '|', ',');
            for (int i = 0; i < names.Length; i++)
            {
                node = UnityUtil.GetChildByName(parent, names[i], false);
                if (node)
                {
                    parent = node;
                }
                else
                {
                    break;
                }
            }
            return node;
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeConf"></param>
        public void DelNode(NodeConf nodeConf)
        {
            GameObject node = GetNode(nodeConf);
            if (node)
            {
                Destroy(node);
            }
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="nodeConf"></param>
        public void DelNode(NodeType nodeType, string nodeName)
        {
            GameObject node = GetNode(nodeType, nodeName);
            if (node)
            {
                Destroy(node);
            }
        }

    }
}