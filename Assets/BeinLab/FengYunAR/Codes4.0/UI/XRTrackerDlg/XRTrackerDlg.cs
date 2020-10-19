using BeinLab.RS5.Mgr;
using BeinLab.Util;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;

namespace BeinLab.UI
{
    /// <summary>
    /// XR 的锚点定位的窗口
    /// </summary>
    public class XRTrackerDlg : MonoBehaviour
    {
        /// <summary>
        /// 进度滑块
        /// </summary>
        private Slider pointSlider;
        //private ARPointCloud pointCloud;
        private Text label;
        /// <summary>
        /// 默认100个特征点是最佳定位地点
        /// </summary>
        public float pointCount = 100f;
        private void Awake()
        {
            pointSlider = UnityUtil.GetTypeChildByName<Slider>(gameObject, "PointSlider");
            label = UnityUtil.GetTypeChildByName<Text>(pointSlider.gameObject, "Label");
            if (XRController.Instance)
            {
                //pointCloud = XRController.Instance.ARCamera.transform.parent
                //    .GetComponentInChildren<ARPointCloud>();
            }
            pointSlider.minValue = 0;
            pointSlider.maxValue = pointCount;
        }

        private void OnEnable()
        {
            //if (pointCloud)
            //{
            //    pointCloud.updated += OnCloudUpdate;
            //}
        }
        //private void OnCloudUpdate(ARPointCloudUpdatedEventArgs updateEvent)
        //{
        //    int curCount = pointCloud.positions.Length;
        //    pointSlider.value = Mathf.Clamp(curCount, 0, pointCount);
        //    label.text = curCount.ToString();
        //}
        //private void OnDisable()
        //{
        //    if (pointCloud)
        //    {
        //        pointCloud.updated -= OnCloudUpdate;
        //    }
        //}
    }
}