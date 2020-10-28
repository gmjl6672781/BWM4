using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.FengYun.UI;
using BeinLab.RS5.Mgr;
using BeinLab.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.FengYun.Gamer
{
    /// <summary>
    /// 动效处理器
    /// 单个对象的动效处理器，每个动效对象必备的组件
    /// 处理动效组
    /// 处理单纯的动效，不考虑其他资源加载等元素
    /// 动效处理必须是已经存在的对象
    /// 当前能否同时执行一组动效？如果不能，如果让轮子转动的同时，车向前运动？
    /// 如果能同时执行，则如何协调不同对象的属性控制
    /// </summary>
    public class GameDynamicer : MonoBehaviour
    {
        /// <summary>
        /// 展示的对象
        /// </summary>
        private GameObject showBody;
        private bool isInit;
        private GameDynamicConf gameDynamic;
        /// <summary>
        /// 动效组协程
        /// </summary>
        private Coroutine dynGroupCoroutine;
        private Coroutine dynSingleCoroutine;
        private Dictionary<DynamicConf, Timer> dynamicLoop = new Dictionary<DynamicConf, Timer>();
        private DynamicGroupConf dynGroup;
        private DynamicConf singleDynamic;
        private Timer fllowTimer;
        public event Action OnDynDestroy;

        private void Start()
        {
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.ChangeDynamicActionList += OnChangeDynamicActionList;
                DynamicActionController.Instance.OnDoAction += OnReSetPosition;
            }
            if (gameDynamic)
            {
                if (gameDynamic.isRoot)
                {
                    transform.SetParent(null);
                }
                if (gameDynamic.isLookCamera)
                {
                    LookCamera();
                }
                if (gameDynamic.parentDynamicer)
                {
                    FllowDynamicer();
                }
            }
            OnChangeVolume(AudioDlg.curVolume);
            AudioDlg.OnChangeVolume += OnChangeVolume;
        }
        private void ClearFllowTimer()
        {
            if (fllowTimer != null)
            {
                fllowTimer = TimerMgr.Instance.DestroyTimer(fllowTimer);
            }
        }
        private void FllowDynamicer()
        {
            ClearFllowTimer();
            fllowTimer = TimerMgr.Instance.CreateTimer(delegate ()
            {
                bool isCreate = false;
                var p = DynamicActionController.Instance.GetOrCreateGameDynamicer(gameDynamic.parentDynamicer, out isCreate);
                if (p)
                {
                    if (gameDynamic.isAllFllow)
                    {
                        transform.SetParent(p.transform);
                    }
                    else
                    {
                        ShowBody.transform.SetParent(p.transform);
                    }
                    //UnityUtil.SetParent(p.transform, ShowBody.transform);
                }
            }, 0.1f);
        }
        
        private void OnChangeVolume(float volume)
        {
            if (showBody && showBody.GetComponent<AudioSource>())
            {
                if (gameDynamic && gameDynamic.isListen)
                    showBody.GetComponent<AudioSource>().volume = volume;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LookCamera()
        {
            Transform target = Camera.main.transform;
            if (!target)
            {
                target = Player.instance.hmdTransform;
            }
            if (!target && XRController.Instance && XRController.Instance.ARCamera)
            {
                target = XRController.Instance.ARCamera.transform;
            }
            if (target)
            {
                UnityUtil.LookAtV(transform, target, gameDynamic.forward);
            }
        }

        public float Scale
        {
            get
            {
                return transform.lossyScale.x;
            }
        }

        public GameObject ShowBody
        {
            get { return showBody; }
            set
            {
                showBody = value;
            }
        }

        /// <summary>
        /// 当高层事件重置时
        /// </summary>
        private void OnReSetPosition(ActionConf actionConf)
        {
            if (actionConf.actionType == ActionType.XRState && actionConf.action == "Reset")
            {
                if (DynamicActionController.Instance)
                {
                    DynamicActionController.Instance.OnDoAction -= OnReSetPosition;
                }
                if (gameDynamic && gameDynamic.isDelOnReset)
                {
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// 当改变动效列表时
        /// 判断是否移除此动效对象
        /// 一种是处理动效组
        /// 一种是针对单个动效
        /// </summary>
        /// <param name="obj"></param>
        private void OnChangeDynamicActionList(DynamicActionListConf obj)
        {
            if (obj)
            {
                if (dynGroup != null)
                {
                    if (dynGroupCoroutine != null)
                    {
                        StopCoroutine(dynGroupCoroutine);
                    }
                    dynGroup.OnStop(this);
                    if (!obj.IsHaveGameDynamicConf(gameDynamic) && dynGroup.isDelOnChangeList)
                    {
                        Destroy(gameObject);
                    }
                }
                if (singleDynamic != null)
                {
                    if (dynSingleCoroutine != null)
                    {
                        StopCoroutine(dynSingleCoroutine);
                    }
                    singleDynamic.OnStop(this);
                    if (!obj.IsHaveGameDynamicConf(gameDynamic) && singleDynamic.isDelOnChangeList)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        public void SetScale(Vector3 scale)
        {
            if (ShowBody)
            {
                ShowBody.transform.localScale = scale;
            }
        }

        /// <summary>
        /// 执行动效组
        /// </summary>
        /// <param name="dynGroup"></param>
        public void SetData(DynamicGroupConf dynGroup)
        {
            this.dynGroup = dynGroup;
            if (dynGroupCoroutine != null)
            {
                StopCoroutine(dynGroupCoroutine);
            }
            DestroyLoopDynamic();
            if (gameObject.activeSelf)
            {
                dynGroupCoroutine = StartCoroutine(DoDynamicGroup(dynGroup));
            }
        }
        private void Update()
        {
            if (gameDynamic && gameDynamic.isUpdate && Camera.main)
            {
                LookCamera();
            }
        }
        public event Action OnFixedUpdate;
        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
            {
                OnFixedUpdate();
            }
        }

        private IEnumerator DoDynamicGroup(DynamicGroupConf dynGroup)
        {
            ///等待初始化完成
            //while (!isInit)
            //{
            //    yield return new WaitForFixedUpdate();
            //}
            int loop = dynGroup.loop;
            do
            {
                loop--;
                DestroyLoopDynamic();
                if (gameDynamic.parentDynamicer)
                {
                    FllowDynamicer();
                }
                for (int i = 0; i < dynGroup.dynamicListConf.Count; i++)
                {
                    DynamicConf dynConf = dynGroup.dynamicListConf[i];
                    while (!isInit)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    float delayTime = dynConf.delayTime;
                    if (dynConf.isRandHideTime)
                    {
                        delayTime = UnityUtil.GetRandValue(dynConf.randHideTime);
                    }
                    if (delayTime > 0)
                    {
                        if (dynConf.isHideOnDelay)
                        {
                            ShowBody.SetActive(false);
                        }

                        yield return new WaitForSeconds(delayTime);
                        if (!ShowBody.activeSelf)
                        {
                            ShowBody.SetActive(true);
                        }
                    }
                    DoDynamic(dynConf, (DynamicConf dynamic) =>
                    {

                        if (dynamic.isAutoDestroy)
                        {
                            StopAllCoroutines();
                            Destroy(gameObject);
                        }
                        else if (dynamic.isAutoHide)
                        {
                            ShowBody.SetActive(false);
                        }
                    });
                }
                yield return new WaitForSeconds(dynGroup.showTime);
            } while (loop != 0);
        }

        private void DestroyLoopDynamic()
        {
            if (dynamicLoop != null)
            {
                foreach (var item in dynamicLoop)
                {
                    if (TimerMgr.Instance)
                    {
                        TimerMgr.Instance.DestroyTimer(item.Value);
                    }
                }
                dynamicLoop.Clear();
            }
        }

        private void OnDestroy()
        {
            if (OnDynDestroy != null)
            {
                OnDynDestroy();
            }
            ClearFllowTimer();
            DestroyLoopDynamic();
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.RemoveDynamicGamer(gameDynamic);
            }
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.ChangeDynamicActionList -= OnChangeDynamicActionList;
            }
            if (DynamicActionController.Instance)
            {
                DynamicActionController.Instance.OnDoAction -= OnReSetPosition;
            }
            AudioDlg.OnChangeVolume -= OnChangeVolume;
            if (ShowBody)
            {
                Destroy(ShowBody);
            }
        }

        public void DestroyDynamicer()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        /// <summary>
        /// 执行动效
        /// </summary>
        /// <param name="dynConf"></param>
        /// <param name="OnComplete"></param>
        public void DoDynamic(DynamicConf dynConf, Action<DynamicConf> OnComplete)
        {
            dynConf.DoDynamic(this);
            if (dynamicLoop != null && dynamicLoop.ContainsKey(dynConf))
            {
                TimerMgr.Instance.DestroyTimer(dynamicLoop[dynConf]);
                dynamicLoop.Remove(dynConf);
            }
            Timer timer = TimerMgr.Instance.CreateTimer(delegate ()
             {
                 dynConf.Complete();
                 if (OnComplete != null)
                 {
                     OnComplete(dynConf);
                 }
                 if (dynConf.isLoop)
                 {
                     DoDynamic(dynConf, OnComplete);
                 }
             }, dynConf.showTime);
            if (dynamicLoop != null)
            {
                dynamicLoop.Add(dynConf, timer);
            }
        }

        /// <summary>
        /// 执行某个动效
        /// </summary>
        /// <param name="dynConf"></param>
        public void SetData(DynamicConf dynConf)
        {
            singleDynamic = dynConf;
            if (dynSingleCoroutine != null)
            {
                StopCoroutine(dynSingleCoroutine);
            }
            DestroyLoopDynamic();
            if (gameObject.activeSelf)
            {
                dynSingleCoroutine = StartCoroutine(DoSingleDynamic(dynConf));
            }
        }
        /// <summary>
        /// 执行单独的
        /// </summary>
        /// <param name="dynConf"></param>
        /// <returns></returns>
        private IEnumerator DoSingleDynamic(DynamicConf dynConf)
        {
            while (!isInit)
            {
                yield return new WaitForFixedUpdate();
            }
            float delayTime = dynConf.delayTime;
            if (dynConf.isRandHideTime)
            {
                delayTime = UnityUtil.GetRandValue(dynConf.randHideTime);
            }
            if (delayTime > 0)
            {
                //print(dynConf);

                if (dynConf.isHideOnDelay)
                {
                    if (ShowBody)
                    {
                        ShowBody.SetActive(false);
                    }
                    {
                        //print(dynConf);
                    }
                }

                yield return new WaitForSeconds(delayTime);
                if (!ShowBody)
                {
                    print(dynConf);
                }
                if (!ShowBody.activeSelf)
                {
                    ShowBody.SetActive(true);
                }
            }
            DoDynamic(dynConf, (DynamicConf dynamic) =>
            {
                if (dynamic.isAutoDestroy)
                {
                    StopAllCoroutines();
                    Destroy(gameObject);
                }
                else if (dynamic.isAutoHide)
                {
                    ShowBody.SetActive(false);
                }
            });
        }

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="gameDynamic"></param>
        public void InitData(GameDynamicConf gameDynamic)
        {
            this.gameDynamic = gameDynamic;
            if (!gameDynamic.localPrefab)
            {
                //float curTime = Time.realtimeSinceStartup;
                //var obj = GameAssetLoader.Instance.LoadObject(gameDynamic.prefabPath);
                //print(gameDynamic.name+"--"+ (Time.realtimeSinceStartup- curTime));
                if (false)
                {
                    GameAssetLoader.Instance.LoadObjectSyn(gameDynamic.prefabPath, (UnityEngine.Object obj) =>
                    {
                        if (obj)
                        {
                            ShowBody = Instantiate(obj) as GameObject;
                            //ShowBody.SetActive(false);
                            obj = null;
                            //GameAssetLoader.Instance.UnLoadObjectSyn(gameDynamic.prefabPath, false);
                            if (gameDynamic.isRoot)
                            {
                                transform.SetParent(null);
                            }
                            UnityUtil.SetParent(transform, ShowBody.transform);
                            isInit = true;
                        }
                    });
                }
                else
                {
                    string prefabPath = gameDynamic.prefabPath;
                    var obj = GameAssetLoader.Instance.LoadObject(gameDynamic.prefabPath);
                    if (obj)
                    {
                        ShowBody = Instantiate(obj) as GameObject;
                        //ShowBody.SetActive(false);

                        obj = null;
                        //GameAssetLoader.Instance.UnLoadObjectSyn(gameDynamic.prefabPath, false);
                        if (gameDynamic.isRoot)
                        {
                            transform.SetParent(null);
                        }
                        UnityUtil.SetParent(transform, ShowBody.transform);
                    }
                    isInit = true;
                }
                //if (obj)
                //{
                //    ShowBody = Instantiate(obj) as GameObject;
                //    obj = null;
                //    //GameAssetLoader.Instance.UnLoadObjectSyn(gameDynamic.prefabPath, false);
                //    if (gameDynamic.isRoot)
                //    {
                //        transform.SetParent(null);
                //    }

                //    UnityUtil.SetParent(transform, ShowBody.transform);

                //}

            }
            else
            {
                var obj = gameDynamic.localPrefab;
                if (obj)
                {
                    ShowBody = Instantiate(obj) as GameObject;
                    //GameAssetLoader.Instance.UnLoadObjectSyn(gameDynamic.prefabPath, false);
                    //UnityUtil.SetParent(transform, ShowBody.transform);
                    if (gameDynamic.isRoot)
                    {
                        transform.SetParent(null);
                    }
                    UnityUtil.SetParent(transform, ShowBody.transform);
                }
                isInit = true;
            }
            if (gameDynamic)
            {
                if (gameDynamic.isRoot)
                {
                    transform.SetParent(null);
                }
                if (gameDynamic.isLookCamera)
                {
                    LookCamera();
                }
                if (gameDynamic.parentDynamicer)
                {
                    FllowDynamicer();
                }
            }
        }
    }
}