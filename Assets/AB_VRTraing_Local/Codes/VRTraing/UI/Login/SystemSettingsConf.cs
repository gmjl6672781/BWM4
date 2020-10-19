namespace BeinLab.Conf
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class SystemSettingsConf
    {
        private string priKey;
        private string serverURL;
        private int languageIndex = 0;
        private int normalScore = 70;
        private string loginUrl = "https://testjoylearning.bmw.com.cn:9500/auth/login";
        /// <summary>
        /// 申请个人信息的接口
        /// </summary>
        private string reqUserUrl = "https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/personalDetails";
        /// <summary>
        /// 发送考试结果的接口
        /// </summary>
        private string sendResultUrl = "https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/examination";
        /// <summary>
        /// 查看历史记录的接口
        /// </summary>
        private string reqHistroyUrl = "https://testjoylearning.bmw.com.cn:9500/learning/otherinfo/vr/history";
        private int size = 4;
        private bool isFoceLogin = true;
        private string userName="ta.ta";
        private string token= "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9" +
            ".eyJzdWIiOiI0IiwiaWF0IjoxNTc4NjMzODEzLCJleHAiOjE1NzkyMzg2MTN9" +
            ".bz5QApLg1VJYmyZh01QNApuvelQbnbNqo5GjCc8KcpCSsZAwS8AgZgCFNlYfYGQLh_WXFuuXj9c9vGIGmXD_jQ";
        private string vrName= "技术培训_新能源_⾼压安全强化及G38PHEV电池模组拆装";
        public string PriKey { get => priKey; set => priKey = value; }
        public string ServerURL { get => serverURL; set => serverURL = value; }
        public int LanguageIndex { get => languageIndex; set => languageIndex = value; }
        public int NormalScore { get => normalScore; set => normalScore = value; }
        public string LoginUrl { get => loginUrl; set => loginUrl = value; }
        public string ReqUserUrl { get => reqUserUrl; set => reqUserUrl = value; }
        public string SendResultUrl { get => sendResultUrl; set => sendResultUrl = value; }
        public string ReqHistroyUrl { get => reqHistroyUrl; set => reqHistroyUrl = value; }
        public int Size { get => size; set => size = value; }
        public bool IsFoceLogin { get => isFoceLogin; set => isFoceLogin = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Token { get => token; set => token = value; }
        public string VrName { get => vrName; set => vrName = value; }
    }
}