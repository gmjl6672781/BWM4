using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace BeinLab.Conf
{
    public class VideoConf : ScriptableObject
    {
        /// <summary>
        /// 视频的url 可以是路径或者网络地址
        /// </summary>
        public string path;
        /// <summary>
        /// 视频源 视频对象或者URL
        /// </summary>
        public VideoSource videoSource;
        /// <summary>
        /// 视频分辨率,默认是1920,1080
        /// </summary>
        public Vector2 resolution = new Vector2(1920, 1080);
    }
}