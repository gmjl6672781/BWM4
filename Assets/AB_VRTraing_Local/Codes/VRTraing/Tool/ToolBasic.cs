using BeinLab.Util;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using DG.Tweening;
using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BeinLab.VRTraing
{
    /// <summary>
    /// 工具基本功能，若要扩展功能，可继承此类
    /// </summary>
    public class ToolBasic : ToolInteractable
    {
        public ToolConf toolConf;
        [HideInInspector]
        public Hand catchHand;
        [HideInInspector]
        public Hand pressHand;
        protected Rigidbody mRigidbody;
        protected VelocityEstimator velocityEstimator;
        protected ToolHighLight toolHighLight;
        protected Interactable interactable;
        protected bool isComplete = false;

        public event Action<Hand, ToolConf> OnCatch;
        public event Action<Hand, ToolConf> OnCatching;
        public event Action<Hand, ToolConf> OnDetach;
        public event Action<Hand, ToolConf> OnHoverBegin;
        public event Action<Hand, ToolConf> OnHover;
        public event Action<Hand, ToolConf> OnHoverEnd;
        public event Action<Hand, ToolConf> OnPress;
        public event Action<Hand, ToolConf> OnPressUp;
        public event Action<Hand, ToolConf> OnPressDown;
        public event Action<ToolConf> OnEnterTool;
        public event Action<ToolConf> OnExitTool;
        public event Action<PutTooConf> OnPutAoCao;
        public float aocaoTime = 0.618f;
        /// <summary>
        /// 新增与指定工具的交互
        /// </summary>
        public event Action<ToolConf> OnBeginHoverTool;
        public event Action<ToolConf> OnEndHoverTool;
        private float catchTime = 0;
        private float inAoCaoTime = -10;
        public float deltAoCaoTime = 1;
        /// <summary>
        /// 是否可以放置物体
        /// 默认激活，凹槽一方的设置
        /// </summary>
        private bool isAoCaoActive = true;

        public bool IsAoCaoActive { get => isAoCaoActive; set => isAoCaoActive = value; }
        private bool isCanPutToAoCao = true;
        protected override void Awake()
        {
            inAoCaoTime = -10;
            //Debug.Log("Awake==================" + this.name);
            base.Awake();
            InitComponent();
            InitToolModel();
            InitTool();

        }

        //private void Update()
        //{
        //    if (gameObject.name == "GaoYaXianShu1")
        //        Debug.Log("ToolBasic");
        //}

        public void ToolAwake()
        {
            Awake();
            SetToolTaskInitInToolBasic();
            if (toolConf.isSetInitPose)
            {
                transform.localPosition = toolConf.InitPosition;
                transform.localEulerAngles = toolConf.InitAngle;
            }
            if (toolConf && toolConf.putList.Count > 0)
            {
                toolConf.indexAoCao = 0;
                Start();
            }
        }

    protected virtual void Start()
    {
        if (toolConf && toolConf.putList.Count > 0)
        {
            OnEnterTool += OnEnterAoCaoTool;
            if (toolConf.indexAoCao >= 0 && toolConf.indexAoCao < toolConf.putList.Count)
            {
                PutToAoCao(toolConf.putList[toolConf.indexAoCao], true);
            }
            isCanPutToAoCao = toolConf.canPutAoCao;
        }
    }

        private void OnEnterAoCaoTool(ToolConf obj)
        {
            //print(obj);
            if (Time.time - catchTime < aocaoTime)
            {
                return;
            }
            for (int i = 0; i < toolConf.putList.Count; i++)
            {
                if (toolConf.putList[i].triggerTool == obj)
                {
                    PutToAoCao(toolConf.putList[i]);
                    break;
                }
            }
        }

        
        /// <summary>
        /// 放到凹槽中去
        /// </summary>
        /// <param name="putTooConf"></param>
        public void PutToAoCao(PutTooConf putTooConf, bool isMoveNow = false)
        {
            //Debug.Log("============================================================PutToAoCao" + putTooConf);
            //Debug.LogFormat("Time.time:{0}, inAoCaoTime :{1}, deltAoCaoTime:{2}===" , Time.time,inAoCaoTime,deltAoCaoTime);
            if (Time.time - inAoCaoTime < deltAoCaoTime)
            {
                return;
            }
            //Debug.Log("putTooConf.triggerTool==" + putTooConf.triggerTool);
            //Debug.Log("isCanPutToAoCao==" + isCanPutToAoCao);
            //Debug.Log("putTooConf.triggerTool.toolBasic.IsAoCaoActive==" + putTooConf.triggerTool.toolBasic.IsAoCaoActive);
            ///当凹槽开启同时本身可以去凹槽的时候
            if (putTooConf.triggerTool && isCanPutToAoCao && putTooConf.triggerTool.toolBasic.IsAoCaoActive)
            {
                inAoCaoTime = Time.time;
                SetToolCatch(false);
                SetToolKinematic(true);
                if (catchHand)
                {
                    catchHand.DetachObject(gameObject);
                }
                try
                {
                    transform.DOKill();
                    Transform oldParent = transform.parent;
                    transform.SetParent(putTooConf.triggerTool.toolBasic.transform);
                    Tweener tweener = transform.DOLocalMove(putTooConf.putPos, isMoveNow ? 0 : putTooConf.doTime);
                    //Debug.Log("----------------------------------------PutToAoCao--"+ putTooConf);
                    //transform.position = putTooConf.triggerTool.toolBasic.transform.TransformPoint(putTooConf.putPos);

                    putTooConf.triggerTool.toolBasic.SetHoverTool(toolConf);
                    transform.localEulerAngles = putTooConf.putAngle;
                    //transform.DOLocalRotate(putTooConf.putAngle, putTooConf.doTime, RotateMode.FastBeyond360);
                    tweener.onComplete += () =>
                    {
                        OnPutAoCao?.Invoke(putTooConf);
                        if (putTooConf.isReSetParent)
                        {
                            transform.SetParent(oldParent);
                        }
                        if (putTooConf.isCanCatchOnPut)
                        {
                            SetToolCatch(true);
                        };

                        //OnPutAoCao?.Invoke(putTooConf);
                    };
                }
                catch (Exception x)
                { }
                

                
            }
        }

        #region 碰撞体触发器
        /// <summary>
        /// 与其他工具发生交互
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("OnTriggerEnter");
            ToolBasic otherTool = GetTypeParent<ToolBasic>(other.gameObject, true);
            if (otherTool)
                OnEnterTool_(otherTool.toolConf);

        }

        private void OnTriggerExit(Collider other)
        {
            //Debug.Log("OnTriggerExit");
            ToolBasic otherTool = GetTypeParent<ToolBasic>(other.gameObject, true);
            if (otherTool)
                OnExitTool_(otherTool.toolConf);

        }
        #endregion

        private void InitComponent()
        {
            mRigidbody = GetComponent<Rigidbody>();
            toolHighLight = GetComponent<ToolHighLight>();
            interactable = GetComponent<Interactable>();
            throwable = GetComponent<Throwable>();
            velocityEstimator = GetComponent<VelocityEstimator>();
        }


        private void InitToolModel()
        {
            if (toolConf)
            {
                if (toolConf.toolModel != null)
                {
                    var goTool = Instantiate<GameObject>(toolConf.toolModel, transform);
                    goTool.name = toolConf.toolModel.name;
                    goTool.transform.localPosition = Vector3.zero;
                    goTool.transform.eulerAngles = Vector3.zero;
                }
            }
        }

        #region 交互功能
        protected override void OnCatch_(Hand hand)
        {
            catchTime = Time.time;
            //Debug.Log("OnCatch");
            catchHand = hand;

            if (toolConf)
            {
                ////Debug.Log("OnCatch------"+ toolConf);
                if (OnCatch != null)
                    OnCatch(hand, toolConf);
                if (toolConf.isSetCatchHighlight)
                    SetToolHighlight(toolConf.isCatchHighlight);
            }

            SetCatchPose(hand);
            SetTrigger(true);
            SetToolKinematic(false);
        }

        protected override void OnCatching_(Hand hand)
        {
            //Debug.Log("OnCatching");
            if (toolConf)
            {
                if (OnCatching != null)
                    OnCatching(hand, toolConf);
                VRHandHelper.Instance.ShockHand(hand, (ushort)(500));
            }

        }

        protected override void OnDetach_(Hand hand)
        {
            //Debug.Log("OnDetach");
            catchHand = null;
            if (toolConf)
            {
                if (OnDetach != null)
                {
                    OnDetach(hand, toolConf);
                }
            }
            SetTrigger(false);
        }

        protected override void OnHoverBegin_(Hand hand)
        {
            //Debug.Log("OnHoverBegin");

            if (toolConf)
            {
                if (OnHoverBegin != null)
                    OnHoverBegin(hand, toolConf);
            }
        }

        protected override void OnHover_(Hand hand)
        {
            //Debug.Log("OnHover");

            if (toolConf)
            {
                if (OnHover != null)
                    OnHover(hand, toolConf);
            }
        }

        protected override void OnHoverEnd_(Hand hand)
        {
            ////Debug.Log("OnHoverEnd");

            if (toolConf)
            {
                if (OnHoverEnd != null)
                    OnHoverEnd(hand, toolConf);
            }

        }

        protected override void OnPress_(Hand hand)
        {
            //Debug.Log("OnPress");

            pressHand = hand;
            if (toolConf)
            {
                if (OnPress != null)
                    OnPress(hand, toolConf);
            }
        }

        protected override void OnPressDown_(Hand hand)
        {
            //Debug.Log("OnPressDown");

            if (toolConf)
            {
                OnPressDown?.Invoke(hand, toolConf);
                //if (OnPressDown != null)
                //    OnPressDown(hand, toolConf);
            }
        }

        protected override void OnPressUp_(Hand hand)
        {
            //Debug.Log("OnPressUp");

            pressHand = null;
            if (toolConf)
            {
                if (OnPressUp != null)
                    OnPressUp(hand, toolConf);
            }

        }


        //当进入可以匹配的工具
        protected virtual void OnEnterTool_(ToolConf otherTool)
        {
            //if (toolConf && otherTool)
            //    Debug.LogFormat("{0} OnEnter {1}", toolConf.toolName, otherTool.toolName);
            if (toolConf)
            {
                if (OnEnterTool != null)
                    OnEnterTool(otherTool);
            }

            //进行位置设置,进行高亮设置
            if (toolConf && toolConf.isSetTriggerPose)
            {
                mRigidbody.isKinematic = true;
                transform.DOMove(toolConf.triggerPositon, 0.3f);
                transform.DORotate(toolConf.triggerAngle, 0.3f);
            }
        }

        //离开可以匹配的工具
        protected virtual void OnExitTool_(ToolConf otherTool)
        {
            //Debug.Log("OnExitTool");
            if (toolConf)
            {
                if (OnExitTool != null)
                    OnExitTool(otherTool);
            }
            if (toolConf && otherTool)
                /*Debug.LogFormat("{0} OnExit {1}", toolConf.toolName, otherTool.toolName)*/;
        }

        //某个交互工具进入
        protected virtual void OnBeginHoverTool_(ToolConf otherTool)
        {
            OnBeginHoverTool?.Invoke(otherTool);
            //Debug.LogFormat("{0} OnBeginHoverTool_ {1}", toolConf.toolName, otherTool.toolName);
        }
        //某个交互工具离开
        protected virtual void OnEndHoverTool_(ToolConf otherTool)
        {
            OnEndHoverTool?.Invoke(otherTool);
            //Debug.LogFormat("{0} OnEndHoverTool_ {1}", toolConf.toolName, otherTool.toolName);
        }

        #endregion

        #region 任务功能,监听任务

        //任务未开始时设置
        public virtual void InitTool()
        {
            //Debug.Log(string.Format("{0} InitTool", toolConf.toolName));
            if (toolConf)
            {
                toolConf.toolBasic = this;

                if (mRigidbody)
                {
                    mRigidbody.mass = toolConf.mass;
                    mRigidbody.drag = toolConf.drag;
                    mRigidbody.angularDrag = toolConf.angularDrag;
                }


                if (throwable && toolConf)
                    throwable.attachmentFlags = toolConf.catchFlags;
            }
        }

        /// <summary>
        /// 设置工具最初状态
        public virtual void OnTaskInit(TaskConf taskConf)
        {

        }

        public virtual void OnTaskStart(TaskConf taskConf)
        {

        }

        public virtual void OnTaskDoing(TaskConf taskConf)
        {

        }

        public virtual void OnTaskEnd(TaskConf taskConf)
        {

        }
        #endregion


        #region 功能性方法

        protected T GetTypeParent<T>(GameObject goSlef, bool recursive = true)
        {
            var result = goSlef.GetComponent<T>();
            if (result != null)
                return result;

            if (recursive)
            {
                var trParent = goSlef.transform.parent;
                if (trParent)
                    return GetTypeParent<T>(trParent.gameObject, true);
            }

            return default(T);
        }
        public void SetHoverTool(ToolConf otherTool)
        {
            this.OnBeginHoverTool_(otherTool);
        }
        public void SetHoverExitTool(ToolConf otherTool)
        {
            this.OnEndHoverTool_(otherTool);
        }

        protected void SetCatchPose(Hand hand)
        {
            Transform objectAttachmentPoint = UnityUtil.GetTypeChildByName<Transform>(hand.gameObject, "ObjectAttachmentPoint");
            //需要进行抓取姿态设置
            if (toolConf.catchFlags.HasFlag(Hand.AttachmentFlags.SnapOnAttach))
            {
                objectAttachmentPoint.localPosition = toolConf.handPosition;
                objectAttachmentPoint.localEulerAngles = toolConf.handAngle;
            }
        }

        private void SetToolTaskInitInToolBasic()
        {
            SetToolHighlight(false);
            ToolTaskInfo toolTaskInfo = TaskManager.Instance.GetToolInitInfo(toolConf);

            if (toolTaskInfo.isSetPose && catchHand == null)
            {
                transform.localPosition = toolTaskInfo.position;
                transform.localEulerAngles = toolTaskInfo.angle;
                //Debug.Log("______物品："+transform.name+"重置位置："+ toolTaskInfo.position+" _________________________ "+ transform.localPosition);
            }
            //print(name);
            if (toolTaskInfo.isSetCanCatch)
                SetToolCatch(toolTaskInfo.isCanCatch);
            if (toolTaskInfo.isSetKinematic)
                SetToolKinematic(toolTaskInfo.isKinematic);
            if (toolTaskInfo.isSetHighlight)
                SetToolHighlight(toolTaskInfo.isHighlight);
            if (toolTaskInfo.isSetHide)
            {
                gameObject.SetActive(!toolTaskInfo.isHide);
            }
            if (toolTaskInfo.isSetScaleSize)
                transform.localScale = toolTaskInfo.scaleSize;

            if (toolTaskInfo.isSetCanHover && interactable)
            {
                isCanHover = toolTaskInfo.isCanHover;
                interactable.highlightOnHover = isCanHover;
            }
            if (toolTaskInfo.indexAoCao >= 0 && toolTaskInfo.indexAoCao < toolConf.putList.Count)
            {
                PutToAoCao(toolConf.putList[toolTaskInfo.indexAoCao], true);
            }
        }

        public void SetToolTaskInit(ToolTaskConf toolTaskConf)
        {
            //Debug.Log("-----------toolTaskConf：" + toolTaskConf.name);
            SetToolHighlight(false);
            ToolTaskInfo toolTaskInfo = TaskManager.Instance.GetToolInitInfo(toolConf);
           
            if(toolTaskInfo.isSetPose && catchHand == null)
            {
                transform.localPosition = toolTaskInfo.position;
                transform.localEulerAngles = toolTaskInfo.angle;
                //Debug.Log("______物品："+transform.name+"重置位置："+ toolTaskInfo.position+" _________________________ "+ transform.localPosition);
            }
            
            //print(name);
            if (toolTaskInfo.isSetCanCatch)
                SetToolCatch(toolTaskInfo.isCanCatch);
            if (toolTaskInfo.isSetKinematic)
                SetToolKinematic(toolTaskInfo.isKinematic);
            if (toolTaskInfo.isSetHighlight)
                SetToolHighlight(toolTaskInfo.isHighlight);
            if (toolTaskInfo.isSetHide)
            {
                try {
                    gameObject.SetActive(!toolTaskInfo.isHide);
                }
                catch (Exception x) { }
            }
            if (toolTaskInfo.isSetScaleSize)
                transform.localScale = toolTaskInfo.scaleSize;

            if (toolTaskInfo.isSetCanHover && interactable)
            {
                isCanHover = toolTaskInfo.isCanHover;
                interactable.highlightOnHover = isCanHover;
            }
            if (toolTaskInfo.indexAoCao >= 0 && toolTaskInfo.indexAoCao < toolConf.putList.Count)
            {
                PutToAoCao(toolConf.putList[toolTaskInfo.indexAoCao], true);
            }
        }

        public void SetToolTaskStart(ToolTaskConf toolTaskConf)
        {
            //Debug.Log("SetToolTaskStart---" + TaskManager.Instance.CurrentTask + "-----" + toolTaskConf);
            SetToolHighlight(false);
            if (toolTaskConf)
            {
                if (toolTaskConf.isSetStartPose && catchHand == null)
                {
                    transform.localPosition = toolTaskConf.StartPosition;
                    transform.localEulerAngles = toolTaskConf.StartAngle;
                    //Debug.Log("transform.localPosition---" + transform.localPosition);
                    //Debug.Log("transform.localEulerAngles---" + transform.localEulerAngles);
                }
                if (toolTaskConf.isSetStartCollider)
                {
                    SetTrigger(toolTaskConf.isStartCollider);
                }

                if (toolTaskConf.isSetStartCanCatch)
                    SetToolCatch(toolTaskConf.isStartCanCatch);
                if (toolTaskConf.isSetStartKinematic)
                    SetToolKinematic(toolTaskConf.isStartKinematic);
                if (toolTaskConf.isSetStartHighlight)
                    SetToolHighlight(toolTaskConf.isStartHighlight);
                if (toolTaskConf.isSetStartHide)
                    gameObject.SetActive(!toolTaskConf.isStartHide);
                if (toolTaskConf.isSetStartScaleSize)
                    transform.localScale = toolTaskConf.startScaleSize;

                if (toolTaskConf.isSetStartCanHover && interactable)
                {
                    isCanHover = toolTaskConf.isStartCanHover;
                    interactable.highlightOnHover = isCanHover;
                }

                if (toolTaskConf.aocaoStartIndex >= 0 && toolTaskConf.aocaoStartIndex < toolConf.putList.Count)
                {
                    //print("........................." + toolTaskConf.aocaoStartIndex);
                    PutToAoCao(toolConf.putList[toolTaskConf.aocaoStartIndex], true);
                }
                if (toolTaskConf.isSetAoCaoStart)
                {
                    isCanPutToAoCao = toolTaskConf.isAoCaoStart;
                }
            }
        }

        public void SetToolTaskEnd(ToolTaskConf toolTaskConf)
        {
            //Debug.Log("SetToolTaskEnd---" + TaskManager.Instance.CurrentTask + "-----" + toolTaskConf);
            if (toolTaskConf)
            {
                if (toolTaskConf.isSetEndPose && catchHand == null)
                {
                    transform.localPosition = toolTaskConf.EndPosition;
                    transform.localEulerAngles = toolTaskConf.EndAngle;
                    //Debug.Log("transform.localPosition---" + transform.localPosition);
                    //Debug.Log("transform.localEulerAngles---" + transform.localEulerAngles);
                }
                if (toolTaskConf.isSetEndCollider)
                {
                    SetTrigger(toolTaskConf.isEndCollider);
                }

                if (toolTaskConf.isSetEndCanCatch)
                    SetToolCatch(toolTaskConf.isEndCanCatch);
                if (toolTaskConf.isSetEndKinematic)
                    SetToolKinematic(toolTaskConf.isEndKinematic);
                if (toolTaskConf.isSetEndHighlight)
                    SetToolHighlight(toolTaskConf.isEndHighlight);
                if (toolTaskConf.isSetEndHide)
                    gameObject.SetActive(!toolTaskConf.isEndHide);
                if (toolTaskConf.isSetEndScaleSize)
                    transform.localScale = toolTaskConf.endScaleSize;

                if (toolTaskConf.isSetEndCanHover && interactable)
                {
                    isCanHover = toolTaskConf.isEndCanHover;
                    interactable.highlightOnHover = isCanHover;
                }
                if (toolTaskConf.aocaoEndIndex >= 0 && toolTaskConf.aocaoEndIndex < toolConf.putList.Count)
                {
                    PutToAoCao(toolConf.putList[toolTaskConf.aocaoEndIndex], true);
                }
                if (toolTaskConf.isSetAoCaoEnd)
                {
                    isCanPutToAoCao = toolTaskConf.isAoCaoEnd;
                }
            }
            ///任务结束后 关闭高亮
            SetToolHighlight(false);
        }

        //
        public void SetTrigger(bool isTirgger)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            //设置碰撞体
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = isTirgger;
            }
        }
        public void ToggleCollider(bool isOn)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            //设置碰撞体
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = isOn;
            }
        }

        public void SetToolHover(bool isCanHover)
        {
            if (interactable)
            {
                interactable.highlightOnHover = isCanHover;
                this.isCanHover = isCanHover;
            }
        }

        //设置工具物理属性
        public void SetToolCatch(bool isCanCatch)
        {
            //设置碰撞体
            //Collider[] colliders = GetComponentsInChildren<Collider>();
            //for (int i = 0; i < colliders.Length; i++)
            //{
            //   colliders[i].enabled = isPhysics;
            //}

            //设置刚体

            //if (mRigidbody)
            //    mRigidbody.isKinematic = !isPhysics;
            this.isCanCatch = isCanCatch;

            if (throwable)
            {
                if (isCanCatch)
                    throwable.attachmentFlags = toolConf.catchFlags;
                else
                    throwable.attachmentFlags = Hand.AttachmentFlags.TurnOnKinematic;
            }

            if (!isCanCatch && velocityEstimator)
                velocityEstimator.FinishEstimatingVelocity();
        }

        public void SetToolKinematic(bool isKinematic)
        {
            if (mRigidbody)
                mRigidbody.isKinematic = isKinematic;
            //SetTrigger(isKinematic);
        }

        public void SetToolHighlight(bool isHighlight)
        {
            if (toolHighLight)
            {
                if (isHighlight && !catchHand)
                {
                    toolHighLight.ShowToolLight();
                }
                else
                    toolHighLight.HideToolLight();
            }
        }
        #endregion

    }

}
