using BeinLab.FengYun.Modu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeinLab.FengYun.UI;
using BeinLab.Util;
using System;
using BeinLab.RS5.Gamer;
using BeinLab.RS5.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeinLab.FengYun.Modus
{
    /// <summary>
    /// 车型的配置列表
    /// </summary>
    [SerializeField]
    public class CarCellConf : CellConf
    {
        public GameDataConf dataConf;
        private Text sizeLabel;
        private Text processLabel;
        private RectTransform mask;
        private Toggle downLoadToggle;
        private List<AssetsVersionConf> assetVersionConfList;
        private bool isReqComplete = false;
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="cell"></param>
        public override void SetData(GameCell cell, int index)
        {
            base.SetData(cell, index);

            mask = UnityUtil.GetTypeChildByName<RectTransform>(cell.gameObject, "MaskBG");
            Text carNameLabel = UnityUtil.GetTypeChildByName<Text>(cell.gameObject, "CarNameLabel");
            carNameLabel.text = cellName;
            sizeLabel = UnityUtil.GetTypeChildByName<Text>(cell.gameObject, "SizeLabel");
            downLoadToggle = UnityUtil.GetTypeChildByName<Toggle>(cell.gameObject, "DownLoadToggle");
            processLabel = UnityUtil.GetTypeChildByName<Text>(cell.gameObject, "ProcessLabel");

            background.interactable = false;
            mask.sizeDelta = background.image.rectTransform.sizeDelta;
            downLoadToggle.isOn = true;
            downLoadToggle.gameObject.SetActive(false);
            processLabel.gameObject.SetActive(false);
            processLabel.text = "";
            sizeLabel.text = "";
            sizeLabel.gameObject.SetActive(false);
            //dataConf.ReqServerVersion(OnReadVersion);
            downLoadToggle.onValueChanged.AddListener((bool isOn) =>
            {
                if (!isOn)
                {
                    downLoadToggle.gameObject.SetActive(false);
                    DownData(cell, isOn);
                }
            });
            cell.StartCoroutine(CheckData());
        }
        /// <summary>
        /// 检测对应的资源是否完整的
        /// 如果完整，判断是否已经连接网络，如果已经连接网络，则申请服务器资源，如果没有联网直接检测完毕
        /// 如果不完整，判断是否已经联网，如果已经联网，则申请服务器资源检测更新，如果没有联网，则强制弹窗提示联网。
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckData()
        {
            ///如果本地完整
            if (false)
            //if (dataConf.isLocalComplete())
            {
                float watie = Time.time;
                while (!GameServerChecker.Instance.IsConnectServer && Time.time - watie < 0.5f)
                {
                    yield return new WaitForFixedUpdate();
                }
                isReqComplete = false;
                dataConf.ReqServerVersion(OnReadVersion);
                while (!isReqComplete)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                ///强制联网
                ///如果没有联网，强制弹窗
                while (!GameServerChecker.Instance.IsConnectServer)
                {
                    if (ServerCheckerDlg.Instance)
                    {
                        ServerCheckerDlg.Instance.ShowConnect();
                    }
                    yield return new WaitForFixedUpdate();
                }
                isReqComplete = false;
                dataConf.ReqServerVersion(OnReadVersion);
                while (!isReqComplete)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            yield return new WaitForFixedUpdate();
        }

        /// <summary>
        /// 获取更新的信息
        /// </summary>
        /// <param name="assetVersionConf"></param>
        private void OnReadVersion(List<AssetsVersionConf> assetVersionConf)
        {
            ///无更新内容
            if (assetVersionConf == null || assetVersionConf.Count < 1)
            {
                ///包体完整且最新了，解除封印
                //if (dataConf.isLocalComplete())
                if (false)
                {
                    background.interactable = true;
                    mask.sizeDelta = new Vector2(background.image.rectTransform.sizeDelta.x, 0);
                    downLoadToggle.gameObject.SetActive(false);
                    processLabel.gameObject.SetActive(false);
                    processLabel.text = "";
                    sizeLabel.text = "";
                    sizeLabel.gameObject.SetActive(false);
                }
                else
                {
                    sizeLabel.text = "网络资源错误，请连接后重试";
                }
            }
            else if (assetVersionConf != null && assetVersionConf.Count > 0)
            {
                background.interactable = false;
                mask.sizeDelta = background.image.rectTransform.sizeDelta;
                downLoadToggle.gameObject.SetActive(true);
                downLoadToggle.isOn = true;

                processLabel.gameObject.SetActive(true);
                processLabel.text = "";
                sizeLabel.text = UnityUtil.GetDataSize(UnityUtil.GetDataSize(assetVersionConf)) + " M";

                sizeLabel.gameObject.SetActive(true);
            }
            isReqComplete = true;
            this.assetVersionConfList = assetVersionConf;
        }
        /// <summary>
        /// 下载数据
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="isOn"></param>
        private void DownData(GameCell cell, bool isOn)
        {
            dataConf.DownData(this.assetVersionConfList, (float process) =>
            {
                processLabel.text = process.ToString("p2");
                mask.sizeDelta = new Vector2(background.image.rectTransform.sizeDelta.x, background.image.rectTransform.sizeDelta.x * (1 - process));
            }, () =>
            {
                background.interactable = true;
                mask.sizeDelta = new Vector2(background.image.rectTransform.sizeDelta.x, 0);
                downLoadToggle.gameObject.SetActive(false);
                processLabel.gameObject.SetActive(false);
                processLabel.text = "";
                sizeLabel.text = "";
                sizeLabel.gameObject.SetActive(false);
            });
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Bein_Conf/CarCellConf", false, 0)]
        static void CreateDynamicConf()
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                ScriptableObject bullet = ScriptableObject.CreateInstance<CarCellConf>();
                if (bullet)
                {
                    string confName = UnityUtil.TryGetName<CarCellConf>(path);
                    AssetDatabase.CreateAsset(bullet, confName);
                }
                else
                {
                    Debug.Log(typeof(CarCellConf) + " is null");
                }
            }
        }
#endif
    }
}