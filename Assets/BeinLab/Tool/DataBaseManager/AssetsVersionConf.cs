using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BeinLab.Util
{
    /// <summary>
    /// 资源包。自动打包到
    /// </summary>
    public class AssetsVersionConf
    {
        private string priKey;
        /// <summary>
        /// 资源对应的MD5码
        /// </summary>
        private string mD5;
        private long size;
        /// <summary>
        /// 版本号
        /// </summary>
        private string version;
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
        public string MD5
        {
            get
            {
                return mD5;
            }

            set
            {
                mD5 = value;
            }
        }
        public long Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        public string Version
        {
            get
            {
                return version;
            }

            set
            {
                version = value;
            }
        }
    }
}