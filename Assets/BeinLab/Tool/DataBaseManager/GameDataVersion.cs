using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 游戏数据版本
    /// 本地创建，记录已下载的美术包，更新时间，版本号
    /// </summary>
    public class GameDataVersion
    {
        /// <summary>
        /// 检索标记,如果不存在或者空串，使用NULL代替
        /// </summary>
        private string priKey;
        /// <summary>
        /// 美术包名称，如果不存在或者空串，使用NULL代替
        /// </summary>
        private string artName;
        /// <summary>
        /// 美术包版本
        /// </summary>
        private string artVersion;
        /// <summary>
        /// 上次更新的时间
        /// </summary>
        private string updateTime;
        /// <summary>
        /// 本地是否已经下载完毕
        /// 下载完毕时更新此属性
        /// </summary>
        private bool isDownComplete;
        /// <summary>
        /// 额外的信息
        /// </summary>
        private string otherMsg;
        /// <summary>
        /// 检索标记
        /// </summary>
        public string PriKey
        {
            get
            {
                return priKey;
            }

            set
            {
                priKey = value;
            }
        }
        /// <summary>
        /// 美术包名称
        /// </summary>
        public string ArtName
        {
            get
            {
                return artName;
            }

            set
            {
                artName = value;
            }
        }
        /// <summary>
        /// 美术包版本
        /// </summary>
        public string ArtVersion
        {
            get
            {
                return artVersion;
            }

            set
            {
                artVersion = value;
            }
        }
        /// <summary>
        /// 上次更新的时间
        /// </summary>
        public string UpdateTime
        {
            get
            {
                return updateTime;
            }

            set
            {
                updateTime = value;
            }
        }
        /// <summary>
        /// 本地是否已经下载完毕
        /// 下载完毕时更新此属性
        /// </summary>
        public bool IsDownComplete
        {
            get
            {
                return isDownComplete;
            }

            set
            {
                isDownComplete = value;
            }
        }
        /// <summary>
        /// 额外的扩展信息，例如MD值之类的鬼东西
        /// </summary>
        public string OtherMsg
        {
            get
            {
                return otherMsg;
            }

            set
            {
                otherMsg = value;
            }
        }
    }
}