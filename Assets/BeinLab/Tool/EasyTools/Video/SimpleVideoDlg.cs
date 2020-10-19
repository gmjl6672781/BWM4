using BeinLab.Util;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class SimpleVideoDlg : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    /// <summary>
    /// 控制器的节点，默认不显示，当触摸屏幕时显示
    /// </summary>
    private GameObject controlRoot;
    /// <summary>
    /// 视频播放的面片
    /// </summary>
    private RawImage videoTexture;
    /// <summary>
    /// 显示Show Root
    /// </summary>
    private RectTransform showRoot;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    //private Button closeBtn;
    /// <summary>
    /// 暂停/播放按钮
    /// </summary>
    private Toggle playStateBtn;
    /// <summary>
    /// 视频进度条
    /// </summary>
    private ISlider videoSlider;
    /// <summary>
    /// 视频播放对象
    /// </summary>
    private VideoPlayer videoPlayer;
    /// <summary>
    /// 渲染的图片
    /// </summary>
    //private RenderTexture renderTexture;
    /// <summary>
    /// 默认自动隐藏的时间
    /// </summary>
    public float hideTime = 3f;
    /// <summary>
    /// 隐藏的计时器
    /// </summary>
    private Timer controlTimer;
    private Text currentTimeLabel;
    private Text allTimeLabel;
    /// <summary>
    /// 每隔一秒钟检测一次
    /// </summary>
    public float updateLabelTime = 0.1f;
    //private Transform loadingIcon;
    public Vector3 loadingRote = Vector3.forward * 90f;
    /// <summary>
    /// 当开始播放视频时
    /// </summary>
    public event Action OnPlayVideo;
    /// <summary>
    /// 当视频播放完毕时
    /// </summary>
    public event Action OnPlayVideoComplete;
    public event Action OnCloseVideo;
    /// <summary>
    /// 是否正在跳步
    /// </summary>
    private bool isJumping = false;
    private bool isInit;
    public string videoUrl;
    public static event Action ClickPlayVideo;
    private RectTransform parentRoot;
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = transform.parent.GetComponentsInParent<ScrollRect>();
        ClickPlayVideo += OnClickPlayVideo;
        InitPlayer();
        parentRoot = transform.parent.GetComponent<RectTransform>();
        StartCoroutine(TryPlayVideo());
    }

    private IEnumerator TryPlayVideo()
    {
        if (createVideoCoor != null)
        {
            StopCoroutine(createVideoCoor);
        }
        createVideoCoor = StartCoroutine(CreateVideo(videoUrl, true));
        while (!videoPlayer)
        {
            yield return new WaitForEndOfFrame();
        }
        videoPlayer.Pause();
        while (!videoPlayer.isPrepared)
        {
            videoPlayer.Pause();
            yield return new WaitForEndOfFrame();
        }
        while (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            yield return new WaitForEndOfFrame();
        }
        videoPlayer.Pause();
        videoPlayer.frame = (long)(VideoPlayer.frameRate);
        lastFrame = VideoPlayer.frame;
        controlRoot.SetActive(true);
        videoPlayer.Pause();
        playStateBtn.interactable = true;
        OnClickFullToggle(fullToggle.isOn);
        videoPlayer.Pause();
        playStateBtn.onValueChanged.AddListener(OnClickPlayBtn);
    }

    private void OnClickPlayVideo()
    {
        if (gameObject && isActiveAndEnabled)
        {
            if (videoPlayer && videoPlayer.isPlaying && playStateBtn.isOn)
            {
                playStateBtn.isOn = false;
            }
        }
    }

    private void InitPlayer()
    {
        if (isInit)
        {
            return;
        }
        isInit = true;
        showRoot = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "UIRoot");
        controlRoot = UnityUtil.GetChildByName(showRoot.gameObject, "ControlRoot");
        videoTexture = UnityUtil.GetTypeChildByName<RawImage>(gameObject, "VideoTexture");
        //closeBtn = UnityUtil.GetTypeChildByName<Button>(controlRoot, "CloseBtn");
        playStateBtn = UnityUtil.GetTypeChildByName<Toggle>(controlRoot, "PlayStateBtn");
        videoSlider = UnityUtil.GetTypeChildByName<ISlider>(controlRoot, "VideoSlider");
        currentTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "CurrentTimeLabel");
        allTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "AllTimeLabel");
        //loadingIcon = UnityUtil.GetTypeChildByName<Transform>(gameObject, "LoadingIcon");

        //GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

        videoTexture.GetComponent<Button>().onClick.AddListener(OnClickVideoTexture);
        videoSlider.OnSwipeOver += OnSwipeOver;
        videoSlider.OnSwipeStart += OnSwipeStart;
        //closeBtn.onClick.AddListener(ClickCloseBtn);
        OnPlayVideo += OnPlayVideoStart;
        //playStateBtn.isOn = false;
        controlRoot.SetActive(true);
        playStateBtn.interactable = false;
        fullToggle = UnityUtil.GetTypeChildByName<Toggle>(gameObject, "FullToggle");
        fullToggle.onValueChanged.AddListener(OnClickFullToggle);
        //showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
        //showRoot.localScale = Vector3.one;
        //GetComponent<RectTransform>().sizeDelta = showRoot.sizeDelta;
        //loadingIcon.gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击了全屏操作
    /// </summary>
    /// <param name="isOn"></param>
    private void OnClickFullToggle(bool isOn)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();
        ///全屏
        if (isOn)
        {
            transform.SetParent(transform.GetComponentInParent<Canvas>().transform);
            rt.anchoredPosition3D = Vector3.zero;
            Vector2 fullScreen = new Vector2(Screen.width, Screen.height);
            if (videoPlayer)
            {
                Vector2 v = new Vector2(1920, 1080);
                if (VideoPlayer.texture)
                {
                    v.x = VideoPlayer.texture.width;
                    v.y = VideoPlayer.texture.height;
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
            transform.SetParent(parentRoot);
            rt.anchoredPosition3D = Vector3.zero;
            rt.sizeDelta = new Vector2(1280, 720);
            if (videoTexture)
            {
                videoTexture.rectTransform.sizeDelta = rt.sizeDelta;
            }
            showRoot.sizeDelta = rt.sizeDelta;
            transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 开始滑动滑动条时
    /// </summary>
    private void OnSwipeStart()
    {
        ClearTimer();
        isJumping = true;
    }
    /// <summary>
    /// 结束滑动条时
    /// </summary>
    private void OnSwipeOver()
    {
        if (VideoPlayer)
        {
            lastFrame = (long)(VideoPlayer.frameCount * videoSlider.value);
            VideoPlayer.frame = lastFrame;
            int time = (int)(lastFrame / VideoPlayer.frameRate);
            TimeSpan ts = new TimeSpan(0, 0, time);
            currentTimeLabel.text = ts.ToString();
        }
        else
        {
            videoSlider.value = 0;
        }
        isJumping = false;
        controlRoot.SetActive(false);
        OnClickVideoTexture();
    }



    /// <summary>
    /// 当开始播放视频时
    /// </summary>
    private void OnPlayVideoStart()
    {
        videoTexture.color = Color.white;
        VideoPlayer.transform.SetParent(transform);
        //videoTexture.rectTransform.sizeDelta =new Vector2( VideoPlayer.texture.width, VideoPlayer.texture.height);
        videoTexture.texture = VideoPlayer.texture;
        videoTexture.rectTransform.sizeDelta = new Vector2(videoTexture.rectTransform.sizeDelta.x,
            videoTexture.rectTransform.sizeDelta.x / VideoPlayer.texture.width * VideoPlayer.texture.height);
        controlRoot.SetActive(false);

        int time = (int)(VideoPlayer.frameCount / VideoPlayer.frameRate);
        TimeSpan ts = new TimeSpan(0, 0, time);
        allTimeLabel.text = ts.ToString();

        lastFrame = VideoPlayer.frame;
    }
    /// <summary>
    /// 点击关闭按钮时
    /// </summary>
    private void ClickCloseBtn()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 当按下了播放/暂停键
    /// </summary>
    private void OnClickPlayBtn(bool isOn)
    {
        if (!VideoPlayer)
        {
            return;
        }
        if (!string.IsNullOrEmpty(videoUrl))
        {
            //if (!videoPlayer)
            //{
            //    PlayVideo(videoUrl);
            //    return;
            //}
            lastFrame = VideoPlayer.frame;
            if (isOn)
            {
                ClickPlayVideo();
                PlayVideo();
                controlRoot.SetActive(false);
                OnClickVideoTexture();
            }
            else
            {
                PauseVideo();
                ClearTimer();
                controlRoot.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 当触碰到了屏幕
    /// </summary>
    private void OnClickVideoTexture()
    {
        ClearTimer();
        if (controlRoot)
        {
            controlRoot.SetActive(!controlRoot.activeSelf);
        }
        if (controlRoot && controlRoot.activeSelf && videoPlayer.isPlaying)
        {
            controlTimer = TimerMgr.Instance.CreateTimer(delegate ()
            {
                if (controlRoot)
                {
                    controlRoot.SetActive(false);
                }
            }, hideTime, 1);
        }
    }
    /// <summary>
    /// 清除计时器
    /// </summary>
    private void ClearTimer()
    {
        if (controlTimer != null)
        {
            if (TimerMgr.Instance)
            {
                TimerMgr.Instance.DestroyTimer(controlTimer);
            }
            controlTimer = null;
        }
    }
    /// <summary>
    /// VideoPlayer的实例
    /// </summary>
    public VideoPlayer VideoPlayer
    {
        get
        {
            return videoPlayer;
        }

        set
        {
            videoPlayer = value;
        }
    }



    /// <summary>
    /// 支持播放在线视频，或者路径，文件等等视频源
    /// </summary>
    /// <param name="videoURL"></param>
    /// <param name="onVideoLoad"></param>
    /// <param name="size">视频分辨率</param>
    public void PlayVideo(string videoURL, bool isForcePlay = false, bool isLoop = true)
    {
        ClickPlayVideo();
        InitPlayer();
        //showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
        //showRoot.localScale = Vector3.one;
        //GetComponent<RectTransform>().sizeDelta = showRoot.sizeDelta;

        //gameObject.SetActive(true);
        if (VideoPlayer)
        {
            if (VideoPlayer.url == videoURL)
            {
                lastFrame = VideoPlayer.frame;
                if (!VideoPlayer.isPlaying)
                {
                    //playStateBtn.isOn = false;
                }
                return;
            }
            else
            {
                StopVideo();
            }
        }
        if (createVideoCoor != null)
        {
            StopCoroutine(createVideoCoor);
        }
        createVideoCoor = StartCoroutine(CreateVideo(videoURL, isLoop));
    }

    /// <summary>
    /// 通过路径videoURl加载
    /// </summary>
    /// <param name="videoURL"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private IEnumerator CreateVideo(string videoURL, bool isLoop)
    {
        VideoPlayer = new GameObject().AddComponent<VideoPlayer>();
        yield return VideoPlayer;

        VideoPlayer.source = VideoSource.Url;
        VideoPlayer.url = videoURL;
        videoTexture.color = Color.black;

        //loadingIcon.gameObject.SetActive(true);

        while (!VideoPlayer.isPrepared)
        {
            yield return new WaitForFixedUpdate();
            //loadingIcon.Rotate(loadingRote * Time.deltaTime);
        }
        //loadingIcon.gameObject.SetActive(false);
        //RenderTexture rt = new RenderTexture((int)VideoPlayer.texture.width, (int)VideoPlayer.texture.height, 0);
        //yield return rt;
        //VideoPlayer.targetTexture = rt;
        VideoPlayer.isLooping = isLoop;
        VideoPlayer.Play();

        if (OnPlayVideo != null)
        {
            OnPlayVideo();
        }

    }

    /// <summary>
    /// 通过视频对象Video模式加载
    /// </summary>
    /// <param name="videoClip"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private IEnumerator CreateVideo(VideoClip videoClip, bool isLoop)
    {
        showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
        GetComponent<RectTransform>().sizeDelta = showRoot.sizeDelta;

        VideoPlayer = new GameObject().AddComponent<VideoPlayer>();
        yield return VideoPlayer;

        VideoPlayer.source = VideoSource.VideoClip;
        VideoPlayer.clip = videoClip;
        videoTexture.color = Color.black;
        //loadingIcon.gameObject.SetActive(true);
        while (!VideoPlayer.isPrepared)
        {
            yield return new WaitForFixedUpdate();
            //loadingIcon.Rotate(loadingRote * Time.deltaTime);
        }
        //loadingIcon.gameObject.SetActive(false);
        //RenderTexture rt = new RenderTexture((int)VideoPlayer.texture.width, (int)VideoPlayer.texture.height, 0);
        //yield return rt;
        //VideoPlayer.targetTexture = rt;
        VideoPlayer.isLooping = isLoop;
        VideoPlayer.Play();
        if (OnPlayVideo != null)
        {
            OnPlayVideo();
        }
    }

    /// <summary>
    /// 支持播放加载到内存的视频对象
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="onVideoLoad"></param>
    /// <param name="positon"></param>
    public void PlayVideo(VideoClip clip, bool isForcePlay = false, bool isLoop = true)
    {
        if (VideoPlayer)
        {
            if (VideoPlayer.clip == clip)
            {
                if (!VideoPlayer.isPlaying)
                {
                    VideoPlayer.Play();
                }
                return;
            }
            else
            {
                StopVideo();
            }
        }
        StartCoroutine(CreateVideo(clip, isLoop));
    }
    /// <summary>
    /// 是否正在播放某个视频
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsPlay(string path)
    {
        if (VideoPlayer)
        {
            if (VideoPlayer.source == VideoSource.Url)
            {
                return VideoPlayer.url == path;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否正在播放某个视频
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public bool IsPlay(VideoClip clip)
    {
        if (VideoPlayer)
        {
            if (VideoPlayer.source == VideoSource.VideoClip)
            {
                return VideoPlayer.clip == clip;
            }
        }
        return false;
    }

    /// <summary>
    /// 停止视频,清除播放器同时隐藏对象
    /// </summary>
    public void StopVideo()
    {
        if (VideoPlayer)
        {
            ClearOldVideo();
        }
        gameObject.SetActive(false);
        if (OnPlayVideoComplete != null)
        {
            OnPlayVideoComplete();
        }
        if (OnCloseVideo != null)
        {
            OnCloseVideo();
        }
    }
    /// <summary>
    /// 关闭视频播放器
    /// </summary>
    public void CloseVideo()
    {
        gameObject.SetActive(false);
        StopVideo();
    }

    /// <summary>
    /// 暂停视频
    /// </summary>
    public void PauseVideo()
    {
        if (VideoPlayer)
        {
            VideoPlayer.Pause();
            lastFrame = VideoPlayer.frame;
        }
    }
    /// <summary>
    /// 播放视频
    /// </summary>
    public void PlayVideo()
    {
        if (VideoPlayer)
        {
            lastFrame = VideoPlayer.frame;
            VideoPlayer.Play();
            lastFrame = VideoPlayer.frame;
        }
    }

    /// <summary>
    /// 清除旧的视频
    /// </summary>
    private void ClearOldVideo()
    {
        if (VideoPlayer)
        {
            VideoPlayer.Stop();
            Destroy(VideoPlayer.gameObject);
            //Destroy(renderTexture);
            VideoPlayer = null;
        }
    }
    /// <summary>
    /// 当被隐藏时
    /// </summary>
    private void OnDisable()
    {
        StopVideo();
        ClearTimer();
        //if (controlRoot && controlRoot.activeSelf)
        //{
        //    controlRoot.SetActive(false);
        //}
        //if (playStateBtn)
        //{
        //    playStateBtn.isOn = false;
        //}
        lastFrame = 0;
    }
    private void OnDestroy()
    {
        ClickPlayVideo -= OnClickPlayVideo;
    }
    private long lastFrame;
    /// <summary>
    /// 同步进度条以及时间显示
    /// </summary>
    private void Update()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (VideoPlayer && VideoPlayer.isPrepared && VideoPlayer.isPlaying && !isJumping)
        {
            if (!VideoPlayer.isLooping && (long)VideoPlayer.frameCount / VideoPlayer.frameRate - VideoPlayer.frame / VideoPlayer.frameRate < 1)
            {
                if (OnPlayVideoComplete != null)
                {
                    OnPlayVideoComplete();
                }
                return;
            }

            if (Mathf.Abs(lastFrame - VideoPlayer.frame) > VideoPlayer.frameRate * 2)
            {
                if (VideoPlayer.frame < VideoPlayer.frameRate * 2)
                {
                    lastFrame = VideoPlayer.frame;
                }
                return;
            }

            int time = (int)(lastFrame / VideoPlayer.frameRate);
            TimeSpan ts = new TimeSpan(0, 0, time);
            currentTimeLabel.text = ts.ToString();
            videoSlider.value = (float)lastFrame / VideoPlayer.frameCount;
            //if (loadingIcon.gameObject.activeSelf)
            //{
            //    loadingIcon.gameObject.SetActive(false);
            //}
            lastFrame = VideoPlayer.frame;
        }
    }
    private ScrollRect[] scrollRect;
    private Toggle fullToggle;
    private Coroutine createVideoCoor;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!fullToggle.isOn)
        {
            for (int i = 0; i < scrollRect.Length; i++)
            {
                scrollRect[i].OnBeginDrag(eventData);
            }
        }
        if (VideoPlayer && VideoPlayer.isPlaying)
        {
            isJumping = true;
            lastFrame = VideoPlayer.frame;
        }
        ClearTimer();
        controlRoot.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!fullToggle.isOn)
        {
            for (int i = 0; i < scrollRect.Length; i++)
            {
                scrollRect[i].OnEndDrag(eventData);
            }
        }
        OnSwipeOver();
        if (controlRoot.activeSelf)
        {
            controlTimer = TimerMgr.Instance.CreateTimer(delegate ()
            {
                controlRoot.SetActive(false);
            }, hideTime, 1);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!fullToggle.isOn)
        {
            for (int i = 0; i < scrollRect.Length; i++)
            {
                scrollRect[i].OnDrag(eventData);
            }
        }
        ClearTimer();
        controlRoot.SetActive(true);
        isJumping = true;
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) * 10)
        {
            if (VideoPlayer && VideoPlayer.isPlaying)
            {
                //lastFrame += (long)(eventData.delta.x * Time.deltaTime *800);
                //lastFrame =(long) Mathf.Clamp(lastFrame,0, VideoPlayer.frameCount);
                videoSlider.value += (eventData.delta.x * Time.deltaTime * 0.03f);
                videoSlider.value = Mathf.Clamp(videoSlider.value, 0, 1);
                //VideoPlayer.frame = lastFrame;
                //videoSlider.value = VideoPlayer.frame * 1.0f / VideoPlayer.frameCount;

                lastFrame = (long)(VideoPlayer.frameCount * videoSlider.value);
                VideoPlayer.frame = lastFrame;

                int time = (int)(lastFrame / VideoPlayer.frameRate);
                TimeSpan ts = new TimeSpan(0, 0, time);
                currentTimeLabel.text = ts.ToString();
            }
        }
    }
}
