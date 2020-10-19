namespace BeinLab.Util
{
    public enum GameModel
    {
        /// <summary>
        /// 编辑器AR模式,需要锚点
        /// </summary>
        XRConf_EditorAR = 0,
        /// <summary>
        /// ARCore模式
        /// </summary>
        XRConf_ARCore = 1,
        /// <summary>
        /// ARKit模式
        /// </summary>
        XRConf_ARKit = 2,
        /// <summary>
        /// AR扫实车模式
        /// </summary>
        XRConf_ARScan = 4,
        /// <summary>
        /// 普通模式/VR模式
        /// </summary>
        XRConf_VR = 8,
        /// <summary>
        /// XR模式，包括ARCore，ARKit，VR（普通模式）
        /// </summary>
        XRConf_XR = 16
    }
}