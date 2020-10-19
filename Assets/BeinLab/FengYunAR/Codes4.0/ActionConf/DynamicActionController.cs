using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Gamer;
using BeinLab.Util;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeinLab.FengYun.Controller
{
    /// <summary>
    /// 动效播放器
    /// 处理动效事件，初始化节点，创建展示动效对象，执行动效组
    /// 播放动效组，执行动效组
    /// 播放动效，执行动效
    /// 当动效事件改变时如何清除当前执行中的动效？
    /// 获取当前事件组，如果动效组的目标对象不包含当前的对象，则删除？
    /// </summary>
    public class DynamicActionController : Singleton<DynamicActionController>
    {
        /// <summary>
        /// 当修改了动效展示组
        /// </summary>
        public event Action<DynamicActionListConf> ChangeDynamicActionList;
        /// <summary>
        /// 动效对象列表
        /// 确定使用不为空
        /// </summary>
        private Dictionary<GameDynamicConf, GameDynamicer> gameDynamicMap = new Dictionary<GameDynamicConf, GameDynamicer>();
        /// <summary>
        /// 当前的主动效事件列表
        /// </summary>
        private DynamicActionListConf dynamicList;
        private Dictionary<Material, Material> mateMap;
        private Dictionary<DynamicActionListConf, Coroutine> dynamicActionListMap = new Dictionary<DynamicActionListConf, Coroutine>();
        /// <summary>
        /// 一个简易事件的配置
        /// </summary>
        public event Action<ActionConf> OnDoAction;
        private ActionConf currentAction;
        public bool isClearDataOnDestroy = true;
        private void Start()
        {
            OnDoAction += OnDoActionEvent;
        }

        private void OnDoActionEvent(ActionConf obj)
        {
            currentAction = obj;
            if (obj)
            {
                if (obj.actionType == ActionType.LoadScene)
                {
                    StartCoroutine(GameDataMgr.Instance.LoadGameScene(obj.action));
                }
            }
        }

        public void ClearCache()
        {
            StopAllCoroutines();
            gameDynamicMap.Clear();
            dynamicActionListMap.Clear();
            ResetMate();
            dynamicList = null;
            if (GameAssetLoader.Instance && isClearDataOnDestroy)
            {
                GameAssetLoader.Instance.ClearBundles();
            }
        }
        /// <summary>
        /// 执行某个事件
        /// </summary>
        /// <param name="action"></param>
        public void DoActionList(List<ActionConf> action)
        {
            for (int i = 0; i < action.Count; i++)
            {
                DoAction(action[i]);
            }
        }
        public void DoActionList(List<string> action)
        {
            for (int i = 0; i < action.Count; i++)
            {
                DoAction(action[i]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionPath"></param>
        public void DoAction(string actionPath)
        {
            var obj = GameAssetLoader.Instance.LoadObject(actionPath);
            if (obj)
            {
                DoAction(obj as ActionConf);
            }
            //GameAssetLoader.Instance.LoadObjectSyn(actionPath, (UnityEngine.Object obj) =>
            //{
            //    if (obj)
            //    {
            //        DoAction(obj as ActionConf);
            //    }
            //});
        }
        public void AutoDoAction(string action)
        {
            //GameAssetLoader.Instance.LoadObjectSyn(action, (UnityEngine.Object obj) =>
            //{
            //    if (obj)
            //    {
            //        if (obj is DynamicConf)
            //        {
            //            DoDynamic(obj as DynamicConf);
            //        }
            //        else if (obj is DynamicActionListConf)
            //        {
            //            DoDynamicActionList(obj as DynamicActionListConf);
            //        }
            //        else if (obj is DynamicActionConf)
            //        {
            //            DoDynamicAction(obj as DynamicActionConf);
            //        }
            //        else if (obj is DynamicGroupConf)
            //        {
            //            DynamicGroupConf dg = obj as DynamicGroupConf;
            //            GameObject node = GameNoder.Instance.GetNode(dg.gameDynamicConf.nodeConf, dg.isForeceCreate);

            //            GameDynamicer gder = DoDynamicGroup(dg);
            //            //if (gder && node)
            //            //{
            //            //    gder.transform.SetParent(node.transform);
            //            //    gder.transform.localPosition = Vector3.zero;
            //            //    gder.transform.localRotation = Quaternion.identity;
            //            //    gder.transform.localScale = Vector3.one;
            //            //}
            //        }
            //    }
            //});
            var obj = GameAssetLoader.Instance.LoadObject(action);
            if (obj)
            {
                if (obj is DynamicConf)
                {
                    DoDynamic(obj as DynamicConf);
                }
                else if (obj is DynamicActionListConf)
                {
                    DoDynamicActionList(obj as DynamicActionListConf);
                }
                else if (obj is DynamicActionConf)
                {
                    DoDynamicAction(obj as DynamicActionConf);
                }
                else if (obj is DynamicGroupConf)
                {
                    DynamicGroupConf dg = obj as DynamicGroupConf;
                    GameObject node = GameNoder.Instance.GetNode(dg.gameDynamicConf.nodeConf, dg.isForeceCreate);

                    GameDynamicer gder = DoDynamicGroup(dg);
                    //if (gder && node)
                    //{
                    //    gder.transform.SetParent(node.transform);
                    //    gder.transform.localPosition = Vector3.zero;
                    //    gder.transform.localRotation = Quaternion.identity;
                    //    gder.transform.localScale = Vector3.one;
                    //}
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionPath"></param>
        public void DoAction(string actionPath, ActionType actionType)
        {
            //print(actionPath);
            //var obj = GameAssetLoader.Instance.LoadObject(actionPath);
            if (actionType == ActionType.Dynamic || actionType == ActionType.DynamicAction ||
                actionType == ActionType.DynamicActionList || actionType == ActionType.DynamicGroup)
            {
                var obj = GameAssetLoader.Instance.LoadObject(actionPath);
                //GameAssetLoader.Instance.LoadObjectSyn(actionPath, (UnityEngine.Object obj) =>
                //{
                if (obj)
                {
                    if (actionType == ActionType.Dynamic)
                    {
                        if (obj)
                        {
                            DoDynamic(obj as DynamicConf);
                        }
                    }
                    else if (actionType == ActionType.DynamicActionList)
                    {
                        if (obj)
                        {
                            DoDynamicActionList(obj as DynamicActionListConf);
                        }
                    }
                    else if (actionType == ActionType.DynamicAction)
                    {
                        if (obj)
                        {
                            DoDynamicAction(obj as DynamicActionConf);
                        }
                    }
                    else if (actionType == ActionType.DynamicGroup)
                    {
                        if (obj is DynamicGroupConf)
                        {
                            DynamicGroupConf dg = obj as DynamicGroupConf;
                            GameObject node = GameNoder.Instance.GetNode(dg.gameDynamicConf.nodeConf, dg.isForeceCreate);
                            GameDynamicer gder = DoDynamicGroup(dg);
                            //if (gder && node)
                            //{
                            //    gder.transform.SetParent(node.transform);
                            //    gder.transform.localPosition = Vector3.zero;
                            //    gder.transform.localRotation = Quaternion.identity;
                            //    gder.transform.localScale = Vector3.one;
                            //}
                        }
                    }
                }
                else
                {
                    Debug.LogError(actionPath);
                }
                //});
            }
        }
        /// <summary>
        /// 执行某个事件
        /// </summary>
        /// <param name="action"></param>
        public void DoAction(ActionConf action)
        {
            if (action)
            {
                DoAction(action.action, action.actionType);

                ///发出动效事件  先说再做
                if (OnDoAction != null)
                {
                    OnDoAction(action);
                }
            }
        }
        /// <summary>
        /// 执行多个动效事件
        /// </summary>
        /// <param name="dynamicList"></param>
        public void DoDynamicActionList(DynamicActionListConf dynamicList)
        {
            if (dynamicList != null)
            {

                ///当主动效事件启动时判断切换主动效组
                ///非主动效组不会切断主动效的播放
                if (this.dynamicList != dynamicList && dynamicList.isMainAction)
                {
                    if (this.dynamicList != null && dynamicActionListMap.ContainsKey(this.dynamicList))
                    {
                        StopCoroutine(dynamicActionListMap[this.dynamicList]);
                        dynamicActionListMap.Remove(dynamicList);
                    }
                    this.dynamicList = dynamicList;
                }
                if (ChangeDynamicActionList != null)
                {
                    ChangeDynamicActionList(dynamicList);
                }
                if (dynamicActionListMap.ContainsKey(dynamicList))
                {
                    StopCoroutine(dynamicActionListMap[dynamicList]);
                    dynamicActionListMap.Remove(dynamicList);
                }
                Coroutine dynamicActionListCoroutine = StartCoroutine(DoDynamicActionListConf(dynamicList));
                if (!dynamicActionListMap.ContainsKey(dynamicList))
                {
                    dynamicActionListMap.Add(dynamicList, dynamicActionListCoroutine);
                }
            }
            else
            {
                Debug.LogError(dynamicList + " is Null");
            }
        }
        /// <summary>
        /// 同时执行多个ActionList
        /// </summary>
        /// <param name="dynamicList"></param>
        /// <returns></returns>
        private IEnumerator DoDynamicActionListConf(DynamicActionListConf dynamicList)
        {
            int loop = dynamicList.loop;
            do
            {
                for (int i = 0; i < dynamicList.dynamicBaseList.Count; i++)
                {
                    DoDynamicAction(dynamicList.dynamicBaseList[i]);
                }
                yield return new WaitForSeconds(dynamicList.showTime);
                loop--;
            } while (loop != 0);
            dynamicActionListMap.Remove(dynamicList);
        }

        /// <summary>
        /// 当前播放的列表里是否存在某个游戏对象
        /// </summary>
        /// <param name="gameDyn"></param>
        /// <returns></returns>
        public bool IsGamerStayList(GameDynamicConf gameDyn)
        {
            if (dynamicList != null)
            {
                return dynamicList.IsHaveGameDynamicConf(gameDyn);
            }
            return false;
        }


        /// <summary>
        /// 处理动效事件，初始化节点，创建展示动效对象，执行动效组
        /// </summary>
        /// <param name="dynActionConf"></param>
        public void DoDynamicAction(DynamicBase dynBase)
        {
            if (dynBase)
            {

                if (dynBase is DynamicActionConf)
                {
                    var dynActionConf = dynBase as DynamicActionConf;
                    if (dynActionConf != null)
                    {
                        ///获取或者创建展示节点

                        //GameObject node = GameNoder.Instance.GetNode(dynActionConf.nodeConf, dynActionConf.isForeceCreate);
                        if (dynActionConf.dynamicGroup)
                        {
                            dynActionConf.dynamicGroup.isForeceCreate = dynActionConf.isForeceCreate;
                            dynActionConf.dynamicGroup.gameDynamicConf = dynActionConf.gameDynamicConf;
                            dynActionConf.gameDynamicConf.nodeConf = dynActionConf.nodeConf;

                            GameDynamicer dyn = DoDynamicGroup(dynActionConf.dynamicGroup);
                            //if (dyn)
                            //{
                            //    dyn.transform.SetParent(node.transform);
                            //    //dyn.transform.localPosition = Vector3.zero;
                            //    //dyn.transform.localRotation = Quaternion.identity;
                            //    //dyn.transform.localScale = Vector3.one;
                            //}
                        }
                        else
                        {

                        }
                    }
                }
                else if (dynBase is DynamicConf)
                {
                    DoDynamic(dynBase as DynamicConf);
                }
                else if (dynBase is DynamicGroupConf)
                {
                    DoDynamicGroup(dynBase as DynamicGroupConf);
                }
            }
        }

        /// <summary>
        /// 播放动效组，执行动效组
        /// </summary>
        /// <param name="dynGroup"></param>
        public GameDynamicer DoDynamicGroup(DynamicGroupConf dynGroup)
        {
            if (dynGroup != null)
            {
                GameObject node = GameNoder.Instance.GetNode(dynGroup.gameDynamicConf.nodeConf, dynGroup.isForeceCreate);
                bool isCreate = false;
                GameDynamicer dynamicer = GetOrCreateGameDynamicer(dynGroup.gameDynamicConf, out isCreate, dynGroup.isForeceCreate);
                if (dynamicer != null)
                {
                    dynamicer.transform.SetParent(node.transform);
                    if (isCreate)
                    {
                        dynamicer.transform.localPosition = Vector3.zero;
                        dynamicer.transform.localScale = Vector3.one;
                        dynamicer.transform.rotation = Quaternion.identity;
                    }
                    dynamicer.SetData(dynGroup);
                    return dynamicer;
                }
            }
            return null;
        }
        public void DoDynamics(List<DynamicConf> dynConfs)
        {
            foreach (var item in dynConfs)
            {
                DoDynamic(item);
            }
        }
        /// <summary>
        ///播放动效，执行动效  播放单个动效,并将设置安排好节点
        /// </summary>
        /// <param name="dynConf"></param>
        public GameDynamicer DoDynamic(DynamicConf dynConf)
        {
            if (dynConf != null)
            {
                bool isDo = false;
                if (dynConf.isListenAction)
                {
                    if (dynConf.listenAction != null && dynConf.listenAction.Contains(currentAction))
                    {
                        isDo = true;
                    }
                    if (dynConf.unListenAction != null && dynConf.unListenAction.Contains(currentAction))
                    {
                        isDo = false;
                    }
                }
                else
                {
                    isDo = true;
                }
                if (!isDo)
                {
                    return null;
                }
                GameObject node = null;
                if (dynConf.gameDynamicConf.nodeConf && GameNoder.Instance)
                {
                    node = GameNoder.Instance.GetNode(dynConf.gameDynamicConf.nodeConf, dynConf.isForeceCreate);
                }
                bool isCreate = false;
                GameDynamicer dynamicer = GetOrCreateGameDynamicer(dynConf.gameDynamicConf,
                    out isCreate, dynConf.isForeceCreate, dynConf.isForceInstance);
                if (dynamicer && node)
                {
                    dynamicer.transform.SetParent(node.transform);
                    dynamicer.transform.localPosition = Vector3.zero;
                    dynamicer.transform.localRotation = Quaternion.identity;
                    dynamicer.transform.localScale = Vector3.one;
                    dynamicer.SetData(dynConf);
                }
                return dynamicer;
            }
            return null;
        }
        public GameDynamicer GetGameDynamicerByName(string dynName)
        {
            foreach (var item in gameDynamicMap)
            {
                if (item.Value.name == dynName)
                {
                    return item.Value;
                }
            }
            return null;
        }
        /// <summary>
        /// 创建一个动效对象
        /// </summary>
        public GameDynamicer GetOrCreateGameDynamicer(GameDynamicConf gameDynamic,
            out bool isCreate, bool isForeceCreate = false, bool isForceInstance = false)
        {
            isCreate = false;
            GameDynamicer dynamicer = null;
            if (gameDynamicMap.ContainsKey(gameDynamic) && !isForceInstance)
            {
                dynamicer = gameDynamicMap[gameDynamic];
            }
            else if (!dynamicer && isForeceCreate || isForceInstance)
            {
                if (gameDynamic.nodeConf.nodeType != NodeType.World)
                {
                    dynamicer = new GameObject(gameDynamic.dynamicName,
                        typeof(RectTransform)).AddComponent<GameDynamicer>();
                    dynamicer.gameObject.layer = LayerMask.NameToLayer("UI");
                }
                else
                {
                    var tmp = new GameObject(gameDynamic.dynamicName);
                    dynamicer = tmp.AddComponent<GameDynamicer>();
                }
                dynamicer.InitData(gameDynamic);
                if (!gameDynamicMap.ContainsKey(gameDynamic))
                {
                    gameDynamicMap.Add(gameDynamic, dynamicer);
                }
                isCreate = true;
            }
            return dynamicer;
        }

        /// <summary>
        /// 移除动效对象
        /// </summary>
        /// <param name="gameDynamic"></param>
        public void RemoveDynamicGamer(GameDynamicConf gameDynamic)
        {
            if (gameDynamic && gameDynamicMap.ContainsKey(gameDynamic))
            {
                gameDynamicMap.Remove(gameDynamic);
            }
        }

        /// <summary>
        /// 备份材质
        /// </summary>
        /// <param name="mater"></param>
        public void CopyMater(Material mater)
        {
            if (mater)
            {
                if (mateMap == null)
                {
                    mateMap = new Dictionary<Material, Material>();
                }
                if (!mateMap.ContainsKey(mater))
                {
                    Material materCopy = Instantiate(mater);
                    materCopy.shader = mater.shader;
                    materCopy.CopyPropertiesFromMaterial(mater);
                    mateMap.Add(mater, materCopy);
                }
            }
        }
        public void ChangeMaterial(Material mater, Material otherMate)
        {
            mater.shader = otherMate.shader;
            mater.CopyPropertiesFromMaterial(otherMate);
        }
        public void ResetMate(Material mater)
        {
            if (mateMap != null)
            {
                if (mateMap.ContainsKey(mater))
                {
                    ChangeMaterial(mater, mateMap[mater]);
                }
            }
        }
        /// <summary>
        /// 重置所有已改变的材质
        /// </summary>
        public void ResetMate()
        {
            if (mateMap != null)
            {
                foreach (var item in mateMap)
                {
                    item.Key.DOKill();
                    ChangeMaterial(item.Key, item.Value);
                }
                mateMap.Clear();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearCache();

        }
    }
}