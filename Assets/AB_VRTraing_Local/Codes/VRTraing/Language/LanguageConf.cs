namespace BeinLab.VRTraing.Conf
{
    /// <summary>
    /// 存储语言的信息，语音的路径（相对路径）
    /// </summary>
    public class LanguageConf
    {
        private string priKey;
        private string message;
        private string audioPath;
        /// <summary>
        /// 文字的主键
        /// </summary>
        public string PriKey { get => priKey; set => priKey = value; }
        /// <summary>
        /// 文字的内容
        /// </summary>
        public string Message { get => message; set => message = value; }
        /// <summary>
        /// 对应的配音路径
        /// </summary>
        public string AudioPath { get => audioPath; set => audioPath = value; }
    }
}