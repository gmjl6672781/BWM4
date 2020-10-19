using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace BeinLab.Util
{
    /// <summary>
    /// 视频播放器
    /// 同时最多只支持一个视频的播放
    /// </summary>
    public class GameVideoPlayerController : Singleton<GameVideoPlayerController>
    {
        /// <summary>
        /// 视频播放对象
        /// </summary>
        private VideoPlayer videoPlayer;
        /// <summary>
        /// 渲染的图片
        /// </summary>
        //private RenderTexture renderTexture;

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
        public void PlayVideo(string videoURL, Action<VideoPlayer> onVideoLoad, Vector2 size)
        {
            if (VideoPlayer)
            {
                if (VideoPlayer.url == videoURL)
                {
                    if (!VideoPlayer.isPlaying)
                    {
                        VideoPlayer.Play();
                    }
                    if (onVideoLoad != null)
                    {
                        onVideoLoad(VideoPlayer);
                        return;
                    }
                }
                else
                {
                    StopVideo();
                }
            }
            StartCoroutine(CreateVideo(videoURL, onVideoLoad, size));
        }

        private IEnumerator CreateVideo(string videoURL, Action<VideoPlayer> onVideoLoad, Vector2 size)
        {
            RenderTexture rt = new RenderTexture((int)size.x, (int)size.y, 0);
            yield return rt;
            VideoPlayer = new GameObject().AddComponent<VideoPlayer>();
            yield return VideoPlayer;
            VideoPlayer.source = VideoSource.Url;
            VideoPlayer.targetTexture = rt;
            VideoPlayer.url = videoURL;
            if (onVideoLoad != null)
            {
                onVideoLoad(VideoPlayer);
            }
        }

        private IEnumerator CreateVideo(VideoClip videoClip, Action<VideoPlayer> onVideoLoad, Vector2 size)
        {
            RenderTexture rt = new RenderTexture((int)size.x, (int)size.y, 0);
            yield return rt;
            VideoPlayer = new GameObject().AddComponent<VideoPlayer>();
            yield return VideoPlayer;
            VideoPlayer.source = VideoSource.VideoClip;
            VideoPlayer.targetTexture = rt;
            VideoPlayer.clip = videoClip;
            if (onVideoLoad != null)
            {
                onVideoLoad(VideoPlayer);
            }
        }

        /// <summary>
        /// 支持播放加载到内存的视频对象
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="onVideoLoad"></param>
        /// <param name="positon"></param>
        public void PlayVideo(VideoClip clip, Action<VideoPlayer> onVideoLoad, Vector2 size)
        {
            if (VideoPlayer)
            {
                if (VideoPlayer.clip == clip)
                {
                    if (!VideoPlayer.isPlaying)
                    {
                        VideoPlayer.Play();
                    }
                    if (onVideoLoad != null)
                    {
                        onVideoLoad(VideoPlayer);
                        return;
                    }
                }
                else
                {
                    StopVideo();
                }
            }
            StartCoroutine(CreateVideo(clip, onVideoLoad, size));
        }
        /// <summary>
        /// 
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
        /// 
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
        /// 停止视频
        /// </summary>
        public void StopVideo()
        {
            if (VideoPlayer)
            {
                ClearOldVideo();
            }
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

    }
}