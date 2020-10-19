using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BeinLab.Util
{
    /// <summary>
    /// 视频播放器窗口
    /// </summary>
    public class VideoDlg : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private ScrollRect[] scrollRect;
        private RectTransform showRoot;
        private GameObject controlRoot;
        private RawImage videoTexture;
        private IToggle playStateBtn;
        private Toggle fullScreenToggle;
        private VideoPlayer moviePlayer;
        private Text currentTimeLabel;
        private Text allTimeLabel;
        private ISlider videoSlider;
        /// <summary>
        /// 是否正在滑动进度条
        /// </summary>
        private bool isSwipeSlider = false;
        /// <summary>
        /// 是否正在拖拽视频
        /// </summary>
        private bool isDragDlg = false;
        /// <summary>
        /// 视频URL
        /// </summary>
        public string videoURL;
        /// <summary>
        /// 视频片段
        /// </summary>
        public VideoClip videoClip;
        private Coroutine controlCoro;
        private RectTransform parentRoot;
        public static Vector2 normalScreen = new Vector2(1920, 1080);
        public static Vector2 normalVideoScreen = new Vector2(1280, 720);
        public float dragSpeed = 10f;
        public static event Action<VideoDlg> ClickPlayVideo;
        private Timer controlShowTimer;
        public float autoHideTime = 3;
        private float lastFrame = 0;
        private bool isVideoInit;

        private void Awake()
        {
            parentRoot = transform.parent.GetComponent<RectTransform>();
            scrollRect = transform.parent.GetComponentsInParent<ScrollRect>();
            showRoot = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "UIRoot");
            controlRoot = UnityUtil.GetChildByName(showRoot.gameObject, "ControlRoot");
            videoTexture = UnityUtil.GetTypeChildByName<RawImage>(gameObject, "VideoTexture");
            playStateBtn = UnityUtil.GetTypeChildByName<IToggle>(controlRoot, "PlayStateBtn");
            videoSlider = UnityUtil.GetTypeChildByName<ISlider>(controlRoot, "VideoSlider");
            currentTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "CurrentTimeLabel");
            allTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "AllTimeLabel");
            fullScreenToggle = UnityUtil.GetTypeChildByName<Toggle>(gameObject, "FullToggle");
        }
        // Start is called before the first frame update
        void Start()
        {
            ClickPlayVideo += OnClickPlayVideo;
            videoSlider.OnSwipeStart += OnSwipeStart;
            videoSlider.OnSwipeOver += OnSwipeOver;
            videoSlider.OnSwipe += OnSwipeSlider;
            videoSlider.onValueChanged.AddListener(OnValueChanged);
            videoTexture.GetComponent<Button>().onClick.AddListener(OnClickVideoTexture);
            playStateBtn.onValueChanged.AddListener(ControlVideoPlay);
            fullScreenToggle.onValueChanged.AddListener(FullScreenMovie);
            StartCoroutine(CreateVideo());
        }

        private void OnClickPlayVideo(VideoDlg dlg)
        {
            if (dlg != this && playStateBtn.isOn)
            {
                playStateBtn.isOn = false;
                playStateBtn.ShowTarget(false);
                ClearTimer();
                controlRoot.SetActive(true);
            }
        }
        private void OnDestroy()
        {
            ClickPlayVideo -= OnClickPlayVideo;
        }

        /// <summary>
        /// 通过路径videoURl加载
        /// </summary>
        /// <param name="videoURL"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private IEnumerator CreateVideo()
        {
            isVideoInit = false;
            if (moviePlayer)
            {
                moviePlayer.prepareCompleted -= OnPrepareCompleted;
                Destroy(moviePlayer.gameObject);
            }
            moviePlayer = new GameObject().AddComponent<VideoPlayer>();
            yield return moviePlayer;
            UnityUtil.SetParent(transform, moviePlayer.transform);
            videoTexture.color = Color.black;
            ///优先检测本地Clip
            if (videoClip)
            {
                moviePlayer.source = VideoSource.VideoClip;
                moviePlayer.clip = videoClip;
            }
            else if (!string.IsNullOrEmpty(videoURL))
            {

                string url =GameDataMgr.Instance.AssetPath+"/"+ GameDataMgr.Instance.ShowPath + "/" + videoURL.Trim();
                moviePlayer.source = VideoSource.Url;
                moviePlayer.url = url;
            }
            moviePlayer.isLooping = true;
            moviePlayer.prepareCompleted += OnPrepareCompleted;
        }
        private void ClearTimer()
        {
            if (controlShowTimer != null)
            {
                if (TimerMgr.Instance)
                {
                    controlShowTimer = TimerMgr.Instance.DestroyTimer(controlShowTimer);
                }
            }
        }


        /// <summary>
        /// 全屏切换
        /// </summary>
        /// <param name="isFull"></param>
        private void FullScreenMovie(bool isFull)
        {
            RectTransform rt = transform.GetComponent<RectTransform>();
            ///全屏
            if (isFull)
            {
                transform.SetParent(transform.GetComponentInParent<Canvas>().transform);
                rt.anchoredPosition3D = Vector3.zero;
                Vector2 fullScreen = new Vector2(Screen.width, Screen.height);
                if (moviePlayer && moviePlayer.isPrepared)
                {
                    Vector2 v = normalScreen;
                    if (moviePlayer.texture)
                    {
                        v.x = moviePlayer.texture.width;
                        v.y = moviePlayer.texture.height;
                    }
                    fullScreen.y = 1.0f * Screen.width / v.x * v.y;
                }
                if (videoTexture)
                {
                    videoTexture.rectTransform.sizeDelta = fullScreen;
                }
                showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
                transform.localScale = Vector3.one;
                rt.sizeDelta = new Vector2(Screen.width, Screen.height);
            }
            else
            {
                isSwipeSlider = false;
                transform.SetParent(parentRoot);
                rt.anchoredPosition3D = Vector3.zero;
                rt.sizeDelta = normalVideoScreen;
                if (videoTexture)
                {
                    videoTexture.rectTransform.sizeDelta = rt.sizeDelta;
                }
                showRoot.sizeDelta = rt.sizeDelta;
                transform.localScale = Vector3.one;
            }
        }

        private void ControlVideoPlay(bool isOn)
        {
            if (!isVideoInit)
            {
                ///播放
                if (isOn)
                {
                    if (!string.IsNullOrEmpty(videoURL) || videoClip != null)
                    {
                        print(string.IsNullOrEmpty(videoURL));
                        print(videoClip);
                        StartCoroutine(CreateVideo());
                    }
                    else
                    {
                        playStateBtn.isOn = true;
                        playStateBtn.isOn = false;
                    }
                }
                return;
            }
            if (controlCoro != null)
            {
                StopCoroutine(controlCoro);
            }
            controlCoro = StartCoroutine(ControlCoroVideoPlay(isOn));
            if (isOn)
            {
                ///播放了一个视频
                if (ClickPlayVideo != null)
                {
                    ClickPlayVideo(this);
                }
                AutoHideControlBtn();
            }
        }

        /// <summary>
        /// 自动隐藏控制按钮
        /// </summary>
        private void AutoHideControlBtn()
        {
            if (controlRoot && controlRoot.activeSelf && playStateBtn.isOn)
            {
                ClearTimer();
                controlShowTimer = TimerMgr.Instance.CreateTimer(delegate ()
                {
                    if (playStateBtn&&playStateBtn.isOn&& controlRoot)
                    {
                        controlRoot.SetActive(false);
                    }
                }, autoHideTime);
            }
        }

        private IEnumerator ControlCoroVideoPlay(bool isOn)
        {
            while (!moviePlayer || !moviePlayer.isPrepared)
            {
                yield return new WaitForFixedUpdate();
            }
            if (!isOn)
            {
                moviePlayer.Pause();
            }
            else
            {
                moviePlayer.Play();
            }
        }


        /// <summary>
        /// 当触碰播放界面时的事件
        /// </summary>
        private void OnClickVideoTexture()
        {
            if (!isDragDlg && !isSwipeSlider && controlRoot)
            {
                if (isVideoInit)
                {
                    controlRoot.SetActive(!controlRoot.activeSelf);
                }
            }
        }


        /// <summary>
        /// 当滑动条的值发生改变时
        /// 同步时间显示，仅同步时间显示，不做其他属性设置
        /// </summary>
        /// <param name="slideValue"></param>
        private void OnValueChanged(float slideValue)
        {
            if (moviePlayer && moviePlayer.isPrepared)
            {
                int time = (int)(slideValue * moviePlayer.frameCount / moviePlayer.frameRate);
                TimeSpan ts = new TimeSpan(0, 0, time);
                currentTimeLabel.text = ts.ToString();
            }
        }
        /// <summary>
        /// 滑动进度条，滑动进度条时，改变视频进度
        /// </summary>
        private void OnSwipeSlider()
        {
            isSwipeSlider = true;
            JumpToSwipe();
        }
        /// <summary>
        /// 视频的帧跳到进度条的位置
        /// </summary>
        private void JumpToSwipe()
        {
            //isSwipeSlider = true;
            if (moviePlayer && moviePlayer.isPrepared)
            {
                lastFrame = moviePlayer.frameCount * videoSlider.value;
                lastFrame = Mathf.Clamp(lastFrame, 0, moviePlayer.frameCount);
                moviePlayer.frame = (long)lastFrame;
            }
            else
            {
                videoSlider.value = 0;
            }
        }

        /// <summary>
        /// 当播放准备完毕时
        /// 暂停播放,只显示视频一帧的画面，静待播放
        /// </summary>
        /// <param name="source"></param>
        private void OnPrepareCompleted(VideoPlayer source)
        {
            videoTexture.color = Color.white;
            videoTexture.texture = moviePlayer.texture;
            videoTexture.rectTransform.sizeDelta = new Vector2(videoTexture.rectTransform.sizeDelta.x,
                videoTexture.rectTransform.sizeDelta.x / moviePlayer.texture.width * moviePlayer.texture.height);
            source.frame = (long)source.frameRate;
            if (!playStateBtn.isOn)
            {
                source.Pause();
            }
            else
            {
                source.Play();
                AutoHideControlBtn();
            }
            lastFrame = source.frame;
            int time = (int)(moviePlayer.frameCount / moviePlayer.frameRate);
            TimeSpan ts = new TimeSpan(0, 0, time);
            allTimeLabel.text = ts.ToString();
            isVideoInit = true;
        }

        /// <summary>
        /// 滑动结束
        /// </summary>
        private void OnSwipeOver()
        {
            JumpToSwipe();
            isSwipeSlider = false;
            AutoHideControlBtn();
        }
        /// <summary>
        /// 滑动开始
        /// </summary>
        private void OnSwipeStart()
        {
            isSwipeSlider = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isSwipeSlider && !isDragDlg && moviePlayer && moviePlayer.isPrepared && moviePlayer.isPlaying)
            {
                if (Mathf.Abs(lastFrame - moviePlayer.frame) > moviePlayer.frameRate * 2)
                {
                    if (moviePlayer.frame < moviePlayer.frameRate)
                    {
                        lastFrame = moviePlayer.frame;
                    }
                    return;
                }
                UpdateSlider(moviePlayer.frame);
            }
        }
        /// <summary>
        /// 滑动屏幕时，同步父级滑动事件
        /// </summary>
        /// <param name="eventData"></param>
        private void SwipeParentScroll(PointerEventData eventData, int dragType = 1)
        {
            if (!fullScreenToggle.isOn)
            {
                for (int i = 0; i < scrollRect.Length; i++)
                {
                    if (dragType == 0)
                    {
                        scrollRect[i].OnBeginDrag(eventData);
                    }
                    else if (dragType == 1)
                    {
                        scrollRect[i].OnDrag(eventData);
                    }
                    else if (dragType == 2)
                    {
                        scrollRect[i].OnEndDrag(eventData);
                    }
                }
            }
        }
        /// <summary>
        /// 更新进度条信息
        /// </summary>
        private void UpdateSlider(float frame)
        {
            int time = (int)(frame / moviePlayer.frameRate);
            TimeSpan ts = new TimeSpan(0, 0, time);
            currentTimeLabel.text = ts.ToString();
            videoSlider.value = (float)frame / moviePlayer.frameCount;
            lastFrame = frame;
        }

        /// <summary>
        /// 开始滑动屏幕
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragDlg = true;
            SwipeParentScroll(eventData, 0);
            lastFrame = moviePlayer.frame;
            if (controlRoot && !controlRoot.activeSelf)
            {
                ClearTimer();
                controlRoot.SetActive(true);
            }
        }
        /// <summary>
        /// 滑动屏幕时
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            isDragDlg = true;
            if (isSwipeSlider) return;
            SwipeParentScroll(eventData, 1);
            if (moviePlayer && moviePlayer.isPrepared && Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) * 10)
            {
                lastFrame += (eventData.delta.x * Time.deltaTime * moviePlayer.frameRate);
                lastFrame = Mathf.Clamp(lastFrame, 0, moviePlayer.frameCount);
                moviePlayer.frame = (long)lastFrame;
                UpdateSlider(lastFrame);
            }
        }
        /// <summary>
        /// 结束滑动屏幕
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            SwipeParentScroll(eventData, 2);
            isDragDlg = false;
            AutoHideControlBtn();
        }

    }
}