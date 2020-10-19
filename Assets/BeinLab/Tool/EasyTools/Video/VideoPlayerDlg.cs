using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace BeinLab.Util
{
    /// <summary>
    /// 视频播放器
    /// </summary>
    public class VideoPlayerDlg : Singleton<VideoPlayerDlg>
    {
        /// <summary>
        /// 是否强制性播放
        /// </summary>
        private bool isForcePlay = false;
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
        private Button closeBtn;
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
        private Transform loadingIcon;
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
        /// <summary>
        /// 初始化控制器
        /// </summary>
        private void Start()
        {
            InitPlayer();
        }
        private bool isInit = false;
        private void InitPlayer()
        {
            if (isInit) {
                return;
            }
            isInit = true;
            showRoot = UnityUtil.GetTypeChildByName<RectTransform>(gameObject, "ShowRoot");
            controlRoot = UnityUtil.GetChildByName(showRoot.gameObject, "ControlRoot");
            videoTexture = UnityUtil.GetTypeChildByName<RawImage>(gameObject, "VideoTexture");
            closeBtn = UnityUtil.GetTypeChildByName<Button>(controlRoot, "CloseBtn");
            playStateBtn = UnityUtil.GetTypeChildByName<Toggle>(controlRoot, "PlayStateBtn");
            videoSlider = UnityUtil.GetTypeChildByName<ISlider>(controlRoot, "VideoSlider");
            currentTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "CurrentTimeLabel");
            allTimeLabel = UnityUtil.GetTypeChildByName<Text>(videoSlider.gameObject, "AllTimeLabel");
            loadingIcon = UnityUtil.GetTypeChildByName<Transform>(gameObject, "LoadingIcon");

            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            playStateBtn.onValueChanged.AddListener(OnClickPlayBtn);
            videoTexture.GetComponent<Button>().onClick.AddListener(OnClickVideoTexture);
            videoSlider.OnSwipeOver += OnSwipeOver;
            videoSlider.OnSwipeStart += OnSwipeStart;
            closeBtn.onClick.AddListener(ClickCloseBtn);
            OnPlayVideo += OnPlayVideoStart;
            playStateBtn.isOn = false;
            controlRoot.SetActive(false);
            gameObject.SetActive(false);
            showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
            showRoot.localScale = Vector3.one;
            GetComponent<RectTransform>().sizeDelta = showRoot.sizeDelta;
            loadingIcon.gameObject.SetActive(false);
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
            if (VideoPlayer && videoPlayer.canSetSkipOnDrop)
            {
                lastFrame = (long)(VideoPlayer.frameCount * videoSlider.value);
                VideoPlayer.frame = lastFrame;
                int time = (int)(lastFrame / VideoPlayer.frameRate);
                TimeSpan ts = new TimeSpan(0, 0, time);
                currentTimeLabel.text = ts.ToString();
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
            videoTexture.texture = VideoPlayer.texture;
            controlRoot.SetActive(false);

            int time = (int)(VideoPlayer.frameCount / VideoPlayer.frameRate);
            TimeSpan ts = new TimeSpan(0, 0, time);
            allTimeLabel.text = ts.ToString();

            lastFrame = 0;
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
            if (!isOn)
            {
                PlayVideo();
            }
            else
            {
                PauseVideo();
            }
            controlRoot.SetActive(false);
            OnClickVideoTexture();
        }
        /// <summary>
        /// 当触碰到了屏幕
        /// </summary>
        private void OnClickVideoTexture()
        {
            ClearTimer();
            if (!IsForcePlay)
            {
                controlRoot.SetActive(!controlRoot.activeSelf);
                if (controlRoot.activeSelf)
                {
                    controlTimer = TimerMgr.Instance.CreateTimer(delegate ()
                    {
                        controlRoot.SetActive(false);
                    }, hideTime, 1);
                }
            }
            else
            {
                StopVideo();
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
        /// 是否强制播放
        /// 为true，代表不能使用控制器
        /// </summary>
        public bool IsForcePlay
        {
            get
            {
                return isForcePlay;
            }

            set
            {
                isForcePlay = value;
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
            InitPlayer();
            showRoot.sizeDelta = new Vector2(Screen.width, Screen.height);
            showRoot.localScale = Vector3.one;
            GetComponent<RectTransform>().sizeDelta = showRoot.sizeDelta;

            this.IsForcePlay = isForcePlay;
            gameObject.SetActive(true);
            if (VideoPlayer)
            {
                if (VideoPlayer.url == videoURL)
                {
                    if (!VideoPlayer.isPlaying)
                    {
                        playStateBtn.isOn = false;
                    }
                    return;
                }
                else
                {
                    StopVideo();
                }
            }
            StartCoroutine(CreateVideo(videoURL, isLoop));
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

            loadingIcon.gameObject.SetActive(true);

            while (!VideoPlayer.isPrepared)
            {
                yield return new WaitForFixedUpdate();
                loadingIcon.Rotate(loadingRote * Time.deltaTime);
            }
            loadingIcon.gameObject.SetActive(false);
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
            loadingIcon.gameObject.SetActive(true);
            while (!VideoPlayer.isPrepared)
            {
                yield return new WaitForFixedUpdate();
                loadingIcon.Rotate(loadingRote * Time.deltaTime);
            }
            loadingIcon.gameObject.SetActive(false);
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
            this.IsForcePlay = isForcePlay;
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
            }
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        public void PlayVideo()
        {
            if (VideoPlayer)
            {
                VideoPlayer.Play();
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
            if (controlRoot && controlRoot.activeSelf)
            {
                controlRoot.SetActive(false);
            }
            if (playStateBtn)
            {
                playStateBtn.isOn = false;
            }
            lastFrame = 0;
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

                if (Mathf.Abs(lastFrame - VideoPlayer.frame) > VideoPlayer.frameRate)
                {
                    if (VideoPlayer.frame < VideoPlayer.frameRate)
                    {
                        lastFrame = VideoPlayer.frame;
                    }
                    return;
                }

                int time = (int)(lastFrame / VideoPlayer.frameRate);
                TimeSpan ts = new TimeSpan(0, 0, time);
                currentTimeLabel.text = ts.ToString();
                videoSlider.value = (float)lastFrame / VideoPlayer.frameCount;
                if (loadingIcon.gameObject.activeSelf)
                {
                    loadingIcon.gameObject.SetActive(false);
                }
                lastFrame = VideoPlayer.frame;
            }
        }
    }
}