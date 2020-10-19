using BeinLab.Util;
using UnityEngine;
namespace Karler.WarFire.UI
{
    /// <summary>
    /// 基础窗口
    /// 自适应框架：基于1920 * 1080分辨率下进行适配
    /// </summary>
    public class BaseDlg : MonoBehaviour
    {
        /// <summary>
        /// 是否自适应屏幕
        /// </summary>
        [Tooltip("是否自适应屏幕,基于1920 * 1080 分辨率")]
        public bool isAutoScreen = true;
        /// <summary>
        /// 背景是否全屏
        /// </summary>
        [Tooltip("背景是否需要全屏,非畸变缩放")]
        public bool isBGFulllScreen = true;
        [Tooltip("子物体是否需要全屏，畸变缩放")]
        public bool isUIFulllScreen = true;
        /// <summary>
        /// 标准分辨率
        /// </summary>
        //public static readonly Vector2 normalScreen = new Vector2(1920, 1080);
        private RectTransform dlgBG;
        private RectTransform uiRoot;
        private RectTransform uiRect;
        public Vector3 transPos = new Vector3(0.2f, 0.3f, 0.2f);

        public RectTransform DlgBG
        {
            get
            {
                return dlgBG;
            }

            set
            {
                dlgBG = value;
            }
        }

        public RectTransform UiRoot
        {
            get
            {
                return uiRoot;
            }

            set
            {
                uiRoot = value;
            }
        }

        private void Awake()
        {
            uiRect = GetComponent<RectTransform>();

            DlgBG = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "BG");
            UiRoot = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "UIRoot");

        }
        private int width;

        /// <summary>
        /// 
        /// </summary>
        public void SetDlgAspect(Vector2 size)
        {
            //uiRect = 
        }

        private void Start()
        {
            if (isAutoScreen)
            {
                uiRect.localScale = Vector3.one * UnityUtil.AspectZoomScreen(uiRect.sizeDelta);
            }
            if (DlgBG)
            {
                if (isBGFulllScreen)
                {
                    DlgBG.sizeDelta = UnityUtil.AspectScreen(uiRect.sizeDelta) / transform.localScale.x;
                    DlgBG.sizeDelta = new Vector2(Mathf.Ceil(DlgBG.sizeDelta.x), Mathf.Ceil(DlgBG.sizeDelta.y));
                }
            }
            if (UiRoot)
            {
                if (isUIFulllScreen)
                {
                    Vector2 extra = Vector2.zero;
                    if (GameDataMgr.Instance)
                    {
                        extra = GameDataMgr.Instance.extraUI;
                    }
                    UiRoot.sizeDelta = new Vector2(Mathf.Floor((Screen.width + extra.x) / transform.localScale.x),
                        Mathf.Ceil((Screen.height + extra.y) / transform.localScale.x));
                }
            }
            width = Screen.width;
        }

        public GameObject GetChild(GameObject root, string childName, bool isSeachChild = true)
        {
            return UnityUtil.GetChildByName(root, childName, isSeachChild);
        }

        public GameObject GetChild(string childName, bool isSeachChild = true)
        {
            return GetChild(gameObject, childName, isSeachChild);
        }

        public Component GetChildComponent<Component>(GameObject root, string childName, bool isSeachChild = true)
        {
            GameObject obj = GetChild(gameObject, childName, isSeachChild);
            if (obj)
            {
                return obj.GetComponent<Component>();
            }
            return default(Component);
        }
        public Component GetChildComponent<Component>(string childName, bool isSeachChild = true)
        {
            return GetChildComponent<Component>(gameObject, childName, isSeachChild);
        }
        /// <summary>
        /// 设置UI层的可视性
        /// </summary>
        /// <param name="isActive"></param>
        public void SetUIActive(bool isActive)
        {
            if (UiRoot && UiRoot.gameObject.activeSelf != isActive)
            {
                UiRoot.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 设置背景层的可视性
        /// </summary>
        /// <param name="isActive"></param>
        public void SetBGActive(bool isActive)
        {
            if (DlgBG && DlgBG.gameObject.activeSelf != isActive)
            {
                DlgBG.gameObject.SetActive(isActive);
            }
        }

        public void SetDlgActive(bool isActive)
        {
            if (gameObject.activeSelf != isActive)
            {
                gameObject.SetActive(isActive);
            }
        }
        private void Update()
        {
            if (Mathf.Abs(width - Screen.width) > 10)
            {
                width = Screen.width;
                Start();
            }
        }
    }
}