using System;
using System.Collections;
using UnityEngine;
//using UnityEngine.XR.ARFoundation;

namespace BeinLab.Util
{
    /// <summary>
    /// 检测当前机型是否支持XR：ARCore，HuaweiAREngine，ARKit
    /// </summary>
    public class CheckXRGamer : Singleton<CheckXRGamer>
    {
        /// <summary>
        /// 获取到检测的结果
        /// </summary>
        public event Action<bool> OnCheckSupport;
        /// <summary>
        /// 编辑器支持
        /// </summary>
        public bool isSupportEditor = true;
        /// <summary>
        /// 是否已经检测
        /// </summary>
        private static bool isHadCheck = false;
        /// <summary>
        /// XR的支持结果
        /// </summary>
        public static bool supportAR = false;
        /// <summary>
        /// 是否使用华为引擎
        /// </summary>
        public bool isHuaweiAREngine = false;
        public static bool IsHadCheck
        {
            get
            {
                return isHadCheck;
            }

            set
            {
                isHadCheck = value;
            }
        }
        /// <summary>
        /// 监听程序启动的事件，当程序启动时开始判断
        /// </summary>
        // Use this for initialization
        void Start()
        {
            if (isHadCheck)
            {
                Destroy(gameObject);
            }
            else
            {
                GameSceneManager.Instance.OnEnterSystem += StartCheckXR;
            }
        }
        /// <summary>
        /// 开启检测XR
        /// </summary>
        public void StartCheckXR()
        {
            StartCoroutine(CheckXRSupport());
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameSceneManager.Instance.OnEnterSystem -= StartCheckXR;
        }
        /// <summary>
        /// 检测当前机型是否支持
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckXRSupport()
        {
            yield return new WaitForEndOfFrame();
            bool isSupport = false;
#if UNITY_EDITOR
            isSupport = isSupportEditor;
#else
            isSupport = true;
            //if ((ARSession.state == ARSessionState.None) ||
            //(ARSession.state == ARSessionState.CheckingAvailability))
            //{
            //    yield return ARSession.CheckAvailability();
            //}

            //if (ARSession.state == ARSessionState.Unsupported)
            //{
            //    // Start some fallback experience for unsupported devices
            //    isSupport = false;
            //}
            //else
            //{
            //    isSupport = true;
            //}
            //#elif UNITY_IOS
            //            int iOSGen = (int)UnityEngine.iOS.Device.generation;
            //            if (iOSGen >= 25 && iOSGen <= 42 && iOSGen != 28 && iOSGen != 33 && iOSGen != 34 && iOSGen != 36 && iOSGen != 30)
            //            {
            //                isSupport = true;
            //            }
            //#elif UNITY_ANDROID
            //            yield return new WaitForEndOfFrame();

            //            if (isHuaweiAREngine)
            //            {
            //                isSupport = true;
            //            }
            //            else
            //            {
            //                AsyncTask<ApkAvailabilityStatus> checkTask = Session.CheckApkAvailability();
            //                CustomYieldInstruction customYield = checkTask.WaitForCompletion();
            //                yield return customYield;
            //                ApkAvailabilityStatus result = checkTask.Result;

            //                if (result == ApkAvailabilityStatus.SupportedInstalled)
            //                {
            //                    while (Session.Status == SessionStatus.Initializing)
            //                    {
            //                        yield return new WaitForEndOfFrame();
            //                    }
            //                    if (Session.Status != SessionStatus.ErrorApkNotAvailable &&
            //                        Session.Status != SessionStatus.ErrorPermissionNotGranted &&
            //                        Session.Status != SessionStatus.FatalError ||
            //                        Session.Status != SessionStatus.None)
            //                    {
            //                        isSupport = true;
            //                    }
            //                }
            //            }
#endif
            supportAR = isSupport;
            IsHadCheck = true;
            if (OnCheckSupport != null)
            {
                OnCheckSupport(isSupport);
            }
            Destroy(gameObject);
        }
    }
}