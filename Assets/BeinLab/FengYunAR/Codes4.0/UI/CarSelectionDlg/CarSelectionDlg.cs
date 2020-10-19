using BeinLab.FengYun.Controller;
using BeinLab.FengYun.Modus;
using BeinLab.FengYun.UI;
using BeinLab.Util;
using Karler.WarFire.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeinLab.UI
{
    public class CarSelectionDlg : MonoBehaviour
    {
        private CellListDlg cellListDlg;
        private GameObject scrollView;
        private Image LoadingView;
        // Start is called before the first frame update
        void Start()
        {
            BaseDlg dlg = GetComponent<BaseDlg>();
            scrollView = dlg.GetChild("Scroll View");
            LoadingView = dlg.GetChildComponent<Image>("LoadingView");
            cellListDlg = GetComponent<CellListDlg>();
            cellListDlg.OnClickGameCell += OnClickGameCell;
            LoadingView.gameObject.SetActive(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void OnClickGameCell(GameCell gameCell, CellConf cellConf, int index)
        {
            if (cellConf is SelectCellConf)
            {
                var select = cellConf as SelectCellConf;
                LoadingView.sprite = select.loadImg;
                scrollView.SetActive(false);
                LoadingView.gameObject.SetActive(true);
                if (select.loadList != null && select.loadList.Count > 0)
                {
                    GameDataMgr.Instance.isCanLoadScene = false;
                    DynamicActionController.Instance.isClearDataOnDestroy = false;
                    StartCoroutine(YuJiaZaiData(select.loadList));
                }
            }
        }

        private IEnumerator YuJiaZaiData(List<string> pLists)
        {
            yield return new WaitForFixedUpdate();
            int loadCount = 0;
            float lastProcess = 0;
            ProcessDlg.Instance.gameObject.SetActive(true);
            for (int i = 0; i < pLists.Count; i++)
            {
                yield return new WaitForFixedUpdate();
                bool isLoad = false;
                string path = pLists[i];
                GameAssetLoader.Instance.LoadObjectSyn(path, (UnityEngine.Object obj) =>
                {
                    isLoad = true;
#if UNITY_EDITOR
                    if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
                    {
                        Instantiate(obj);
                    }
#endif
                    loadCount++;
                }, (float process) =>
                {
                    if (ProcessDlg.Instance)
                    {
                        if (ProcessDlg.Instance.ProcessSlider.value > lastProcess)
                        {
                            ProcessDlg.Instance.UpdateProcess("资源准备中" + ((1 + process) *
                                (loadCount * 1f / pLists.Count)).ToString("P0"), (1 + process)
                                * (loadCount * 1f / pLists.Count));
                        }
                    }
                });
                while (!isLoad)
                {
                    yield return new WaitForFixedUpdate();
                    if (ProcessDlg.Instance)
                    {
                        if (ProcessDlg.Instance.ProcessSlider.value <= lastProcess && lastProcess < 1)
                        {
                            lastProcess += Time.deltaTime / 11f;
                            ProcessDlg.Instance.UpdateProcess("资源准备中" + lastProcess.ToString("P0"), lastProcess);
                        }
                    }
                }
            }
            GameDataMgr.Instance.isCanLoadScene = true;
        }
    }
}