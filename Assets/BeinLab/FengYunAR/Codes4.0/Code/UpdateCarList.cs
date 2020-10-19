using BeinLab.FengYun.Modu;
using BeinLab.FengYun.Modus;
using BeinLab.FengYun.UI;
using BeinLab.RS5.Gamer;
using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCarList : MonoBehaviour
{
    public static string SceneName = "CheckScene";
    public GameDataConf gameData;
    private bool isPublicFile;
    private bool isNeedDownLoad;
    private List<AssetsVersionConf> updateList = null;
    private float curProcess = 0;
    private bool isDown = false;

    public static bool isPublicComplete = false;

    private Image mengBan;
    private Button button;
    private Text num;
    private Text carName;
    private GameObject cutOffTip;
    private float fullHeight;

    private float assetsSize;

    public List<Sprite> buttonPic;

    private Button thisButton;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<GameCell>())
        {
            thisButton = GetComponent<Button>();
            thisButton.interactable = false;
            SelectCellConf selectCellConf = (SelectCellConf)GetComponent<GameCell>().cellConf;
            gameData = selectCellConf.dataConf;

            mengBan = UnityUtil.GetTypeChildByName<Image>(gameObject, "MengBan");
            button = UnityUtil.GetTypeChildByName<Button>(gameObject, "Button");
            button.GetComponent<Image>().sprite = buttonPic[0];
            button.onClick.AddListener(CarTypeDown);
            num = UnityUtil.GetChildByName(gameObject, "Num").GetComponent<Text>();
            num.gameObject.SetActive(false);
            carName = UnityUtil.GetTypeChildByName<Text>(gameObject, "Text");
            carName.text = GetComponentInParent<GameCell>().cellConf.cellName;

            cutOffTip = UnityUtil.GetChildByName(gameObject, "CutOffTip");
            isPublicFile = false;
            fullHeight = mengBan.rectTransform.sizeDelta.y;
        }
        else
            isPublicFile = true;
#if UNITY_EDITOR
        if (GameDataMgr.Instance.buildConf.buildType == BulidType.Editor)
        {
            if (GetComponent<GameCell>())
                OnUpdateComplete();
        }
        if (GameDataMgr.Instance.buildConf.buildType == BulidType.LocalPhone || GameDataMgr.Instance.buildConf.buildType == BulidType.Stream)
        {
            if (SceneRecord.lastSceneName == SceneName)
            {
                StartCoroutine(StartRun());
            }
            else
            {
                OnUpdateComplete();
            }
        }
#else
        if (GameDataMgr.Instance.buildConf.buildType == BulidType.Stream)
        {
            if (GetComponent<GameCell>())
                OnUpdateComplete();
        }
        else
       {
             if (SceneRecord.lastSceneName == SceneName)
            {
                StartCoroutine(StartRun());
            }
            else
            {
                OnUpdateComplete();
            }
        }
#endif
    }

    private IEnumerator StartRun()
    {
        if (!isPublicFile)
        {
            while (!isPublicComplete)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        if (isPublicFile || (!isPublicFile && isPublicComplete))
        {
            if (gameData != null)
            {
                //yield return 0;
                //if (gameData.isLocalComplete())
                //{
                //    if (CheckNetwork())
                //    {
                //        StartCoroutine(CheckData(gameData));
                //        while (!isCheckDataOver)
                //        {
                //            yield return new WaitForFixedUpdate();
                //        }
                //        if (isNeedDownLoad)
                //        {
                //            if (isPublicFile)
                //                PublicDown();
                //            //else
                //            //    button.onClick.AddListener(CarTypeDown);
                //        }
                //        else
                //        {
                //            if (isPublicFile)
                //                isPublicComplete = true;
                //            else
                //                OnUpdateComplete();
                //        }
                //    }
                //    else
                //    {
                //        if (isPublicFile)
                //            isPublicComplete = true;
                //        else
                //            OnUpdateComplete();
                //    }
                //}
                //else
                //{
                //    if (CheckNetwork())
                //    {
                //        StartCoroutine(CheckData(gameData));
                //        while (!isCheckDataOver)
                //        {
                //            yield return new WaitForFixedUpdate();
                //        }
                //        if (isPublicFile)
                //            PublicDown();
                //        //else
                //        //    button.onClick.AddListener(CarTypeDown);
                //    }
                //    else
                //    {
                //        ProcessDlg.Instance.gameObject.SetActive(true);
                //        ProcessDlg.Instance.UpdateProcess("需要下载美术资源，请连接网络。。。", 0);
                //        while (!CheckNetwork())
                //        {
                //            yield return new WaitForFixedUpdate();
                //        }
                //        StartCoroutine(CheckData(gameData));
                //        while (!isCheckDataOver)
                //        {
                //            yield return new WaitForFixedUpdate();
                //        }
                //        if (isPublicFile)
                //            PublicDown();
                //        //else
                //        //    button.onClick.AddListener(CarTypeDown);
                //    }
                //}
                if (!CheckNetwork())
                {
                    //if (gameData.isLocalComplete())
                    //{
                    //    OnUpdateComplete();
                    //}
                    if (false)
                    {
                        OnUpdateComplete();
                    }
                    else
                    {
                        if (isPublicFile && ProcessDlg.Instance)
                        {
                            yield return ProcessDlg.Instance;
                            ProcessDlg.Instance.gameObject.SetActive(true);
                            ProcessDlg.Instance.UpdateProcess("需要下载美术资源，请连接网络。。。", 0);
                        }
                        while (!CheckNetwork())
                        {
                            yield return new WaitForFixedUpdate();
                        }
                        StartCoroutine(StartRun());
                        //print("break");
                        yield break;
                    }
                }
                else
                {
                    StartCoroutine(CheckData(gameData));
                    while (!isCheckDataOver)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    print(gameData.name + "---" + isNeedDownLoad + "---");
                    if (!isNeedDownLoad)
                    {
                        OnUpdateComplete();
                    }
                    else
                    {
                        if (isPublicFile)
                        {
                            PublicDown();
                            //isPublicComplete = true;
                        }
                        else
                        {
                            string st = GetComponentInParent<GameCell>().cellConf.cellName + "|" + assetsSize.ToString("f1") + "M";
                            print(st);
                            carName.text = UnityUtil.SplitToLine(st);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator CheckAndDownData()
    {
        StartCoroutine(CheckData(gameData));
        while (!isCheckDataOver)
        {
            yield return new WaitForFixedUpdate();
        }
        PublicDown();
    }

    private void OnUpdateComplete()
    {
        if (isPublicFile)
        {
            isPublicComplete = true;
        }
        else
        {
            if (mengBan)
            {
                mengBan.gameObject.SetActive(false);
                button.gameObject.SetActive(false);
                carName.gameObject.SetActive(false);
                thisButton.interactable = true;
            }
        }
        isNeedDownLoad = false;
    }

    private void PublicDown()
    {
        if (isNeedDownLoad)
        {
            StartCoroutine(DownLoadData((float process) =>
            {
                if (process > curProcess)
                {
                    curProcess = process;
                    if ((curProcess).ToString("p0") != "100%")
                    {
                        ProcessDlg.Instance.UpdateProcess("正在下载资源" + (curProcess).ToString("p0"), curProcess);
                    }
                    else
                    {
                        if ((process).ToString("p0") != "100%")
                            ProcessDlg.Instance.UpdateProcess("解压中请稍后" + (process).ToString("p0"), process);
                        else
                            ProcessDlg.Instance.gameObject.SetActive(false);
                    }
                }
            }, () => { isDown = true; }));
        }
        OnUpdateComplete();
    }

    private void CarTypeDown()
    {
        if (CheckNetwork())
        {
            if (isNeedDownLoad)
            {
                button.GetComponent<Image>().sprite = buttonPic[1];
                StartCoroutine(DownLoadData((float process) =>
                {
                    if (process > curProcess)
                    {
                        curProcess = process;
                        if ((curProcess).ToString("p0") != "100%")
                        {
                            num.gameObject.SetActive(true);
                            num.text = (curProcess * 100).ToString("f1") + "%";
                            Vector2 size = mengBan.rectTransform.sizeDelta;
                            size.y = fullHeight * (1.0f - curProcess);
                            mengBan.rectTransform.sizeDelta = size;
                        }
                        else
                        {
                            OnUpdateComplete();
                        }
                    }
                }, () => { isDown = true; }));
            }
        }
        else
        {
            StartCoroutine(show());
        }
    }

    IEnumerator show()
    {
        float time = Time.time;
        cutOffTip.SetActive(true);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (CheckNetwork() || Time.time - time >= 2)
            {
                cutOffTip.SetActive(false);
                break;
            }
        }
    }

    /// <summary>
    /// 检查是否联网
    /// </summary>
    /// <returns></returns>
    public bool CheckNetwork()
    {
        return GameNetChecker.Instance.NetState != NetworkReachability.NotReachable;
        //if (GameNetChecker.Instance.NetState == NetworkReachability.ReachableViaCarrierDataNetwork)
        //{
        //    return true;
        //}
        //else if (GameNetChecker.Instance.NetState == NetworkReachability.ReachableViaLocalAreaNetwork)
        //{
        //    return true;
        //}
        //else
        //    return false;
    }

    private bool isCheckDataOver;
    IEnumerator CheckData(GameDataConf conf)
    {
        if (conf)
        {
            isCheckDataOver = false;
            bool isReq = false;
            List<AssetsVersionConf> versionList = null;
            while (!GameServerChecker.Instance.IsConnectServer)
            {
                yield return new WaitForFixedUpdate();
            }
            if (!isReq)
            {
                conf.ReqServerVersion((List<AssetsVersionConf> list) =>
                {
                    isReq = true;
                    versionList = list;
                });
            }
            while (!isReq)
            {
                yield return new WaitForFixedUpdate();
            }
            //conf.GetLocalMD5();
            //updateList = conf.ComparisonVersion(versionList);
            isNeedDownLoad = false;
            if (updateList == null || updateList.Count < 1)
            {
                ///无需更新数据
                isNeedDownLoad = false;
            }
            else
            {
                //print("本次更新" + updateList.Count);
                //ProcessDlg.Instance.UpdateProcess("本次更新" + updateList.Count + "个文件", 0);
                //ProcessDlg.Instance.gameObject.SetActive(true);
                assetsSize = UnityUtil.GetDataSize(UnityUtil.GetDataSize(updateList));
                isNeedDownLoad = true;
            }
            isCheckDataOver = true;
        }
    }

    IEnumerator DownLoadData(Action<float> updateProcess, Action CompleteUpdate)
    {
        if (isNeedDownLoad)
            gameData.DownData(GameServerChecker.Instance.GameServer, gameData.dataPath, updateList, updateProcess, () =>
            {
                isNeedDownLoad = false;
            });
        while (isNeedDownLoad)
        {
            yield return new WaitForFixedUpdate();
        }
    }
}
