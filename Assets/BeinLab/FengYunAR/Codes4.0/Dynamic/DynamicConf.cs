using BeinLab.FengYun.Gamer;
using UnityEngine;
using BeinLab.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeinLab.CarShow.Modus
{
    /// <summary>
    /// 通用性动效配置
    /// 处理的对象是GameObject本身，ShowBody
    /// </summary>
    [SerializeField]
    public class DynamicConf : DynamicBase
    {
        /// <summary>
        /// 动效执行的对象
        /// 两种赋值方式，配置或者通过事件组进行赋值
        /// 指定式配置，代表此动效仅在对象存在时执行
        /// 非指定式配置，代表此动效将通过事件系统支配，事件系统将初始化对象，并管理动效的执行
        /// 指定式用来简化动效配置，例如配置一个简易的换色，销毁某个动效
        /// </summary>
        //[Tooltip("目标对象，动效的接收者")]
        //public GameDynamicConf gameDynamicConf;
        //[Tooltip("是否强制创建，当动效对象不存在时是否强制创建")]
        //public bool isForeceCreate = false;
        /// <summary>
        /// 动效的延时时长，0为不延时
        /// </summary>
        [Tooltip("延时时间")]
        public float delayTime;
        /// <summary>
        /// 是否在延时的时候隐藏对象
        /// </summary>
        [Tooltip("延时时是否隐藏对象")]
        public bool isHideOnDelay;

        [Tooltip("展示时间")]
        public float showTime;
        /// <summary>
        /// 是否自动删除
        /// 当动效播放完毕后是否删除
        /// 为True时在动效播放完毕后删除对象
        /// </summary>
        [Tooltip("动效完成之后是否删除此对象")]
        public bool isAutoDestroy;
        [Tooltip("动效完成之后，是否隐藏主体")]
        public bool isAutoHide;

        /// <summary>
        /// 是否为重复动效
        /// 不常用，请谨慎使用
        /// </summary>
        [Tooltip("是否循环播放")]
        public bool isLoop;


        [Tooltip("是否重置位置")]
        public bool isResetWorld;
        [Tooltip("位置")]
        public Vector3 position;
        [Tooltip("角度")]
        public Vector3 angle;
        [Tooltip("比例")]
        public Vector3 scale = Vector3.one;
        [Tooltip("当改变动效列表时是否删除此对象,当下一个动效组不包含此对象时是否删除此对象")]
        public bool isDelOnChangeList = true;
        public bool isStopOnChangeList = true;
        public bool isShowOnStop;
        public bool isLockChild = false;
        public bool isRandHideTime = false;
        public Vector2 randHideTime;
        /// <summary>
        /// 是否强制实例化
        /// </summary>
        public bool isForceInstance;

        /// <summary>
        /// 执行动效
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public virtual void DoDynamic(GameDynamicer gameDynamicer)
        {
            if (isResetWorld)
            {
                if (gameDynamicer.GetComponent<RectTransform>())
                {
                    gameDynamicer.GetComponent<RectTransform>().anchoredPosition3D = position;
                }
                else
                {
                    gameDynamicer.transform.localPosition = position;
                    gameDynamicer.ShowBody.transform.position = gameDynamicer.transform.position;
                }
                gameDynamicer.transform.localEulerAngles = angle;
                gameDynamicer.ShowBody.transform.eulerAngles = gameDynamicer.transform.eulerAngles;
                gameDynamicer.transform.localScale = Vector3.one;
                gameDynamicer.SetScale(scale);
            }
            if (isLockChild)
            {
                gameDynamicer.ShowBody.transform.localPosition = position;
                gameDynamicer.ShowBody.transform.localEulerAngles = angle;
                Debug.Log(isLockChild);
            }
        }
        /// <summary>
        /// 当此动效停止时要执行的事件
        /// </summary>
        /// <param name="gameDynamicer"></param>
        public virtual void OnStop(GameDynamicer gameDynamicer)
        {
            if (isShowOnStop)
            {
                if (!gameDynamicer.ShowBody.activeSelf)
                {
                    gameDynamicer.ShowBody.SetActive(true);
                }
            }
        }

        public virtual void Complete()
        {

        }
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Dynamic/DynamicConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<DynamicConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<DynamicConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(DynamicConf) + " is null");
                }
            }
        }

#endif
    }
}