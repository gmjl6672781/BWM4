namespace BeinLab.Util
{
    public class ServerVisionConf
    {
        private string priKey;
        private string server;
        private string iosAppURL;
        private int sort;
        /// <summary>
        /// 当前APP的版本号
        /// </summary>
        private string iOSVersion;
        private string androidVersion;
        private string androidAppURL;
        private string gmsURL;
        /// <summary>
        /// 服务器名称
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
        /// 服务器地址
        /// </summary>
        public string Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }
        /// <summary>
        /// 访问优先级，值越小优先级越高
        /// </summary>
        public int Sort
        {
            get
            {
                return sort;
            }

            set
            {
                sort = value;
            }
        }

        public string IOSVersion
        {
            get { return iOSVersion; }
            set
            {
                iOSVersion = value;
            }
        }
        public string IOSAppURL
        {
            get { return iosAppURL; }
            set
            {
                iosAppURL = value;
            }
        }
        public string Androidversion
        {
            get { return androidVersion; }
            set
            {
                androidVersion = value;
            }
        }
        public string AndroidAppURL
        {
            get { return androidAppURL; }
            set
            {
                androidAppURL = value;
            }
        }
        public string GmsURL
        {
            get { return gmsURL; }
            set
            {
                gmsURL = value;
            }
        }
    }
}