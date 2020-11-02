using BeinLab.CarShow.Modus;
using BeinLab.FengYun.Controller;
using BeinLab.VRTraing.Conf;
using BeinLab.VRTraing.Mgr;
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
namespace BeinLab.VRTraing
{
    /// <summary>
    /// 扳手的操作：
    /// 1，当接触到指定的螺栓的时候，扳手脱离手，同时状态记录为特殊catching
    /// 2，特殊catching时
    /// </summary>
    public class ToolBanShou : ToolBasic
    {
        /// <summary>
        /// 当前正在交互的螺栓
        /// </summary>
        private ToolLuoSuan curluoSuan;
        private Hand curHand;
        public Vector3 workPos = Vector3.up * 0.21f;
        public float maxFllowDis = 0.3f;
        private bool isWorking = false;
        public Vector3 workAngle = Vector3.right * 90;
        public GameObject turnDirection;
        public float turnDirHight;
        private GameObject turnDir;
        private bool startRotate = false;
        /// <summary>
        /// 跟随手的协程
        /// </summary>
        private Coroutine fllowCoroutine;
        private float lastAngle;
        /// <summary>
        /// 扳手要做的目标状态
        /// </summary>
        public LuoSuanState targetState = LuoSuanState.Open;
        private Transform fllowTarget;
        public float shockTime = 0.1f;
        private ToolConf taoTong;
        public DynamicConf dynamicConf;
        public GoalAutoPosition AotuBack;
        //private bool isConnectTaoTong;
        bool star = false;
        protected override void Awake()
        {
            base.Awake();
            transform.GetComponent<Rigidbody>().isKinematic = true;
            if (AotuBack!=null)
            {
                transform.SetParent(GameObject.Find(AotuBack.fatherName).transform);
                transform.localPosition = AotuBack.autoPosition_Local;
                transform.localRotation = Quaternion.Euler(AotuBack.autoRotation_Local);
            }
            
            
            
            //print(transform.localPosition);
            //print(transform.localRotation.x+","+ transform.localRotation.y + "," + transform.localRotation.z);
            transform.GetComponent<Rigidbody>().isKinematic = false;
            star = true;
        }

        //private void Update()
        //{
        //    if (star)
        //    {
        //        print(transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z);
        //        print(transform.localRotation.x + "," + transform.localRotation.y + "," + transform.localRotation.z);
        //    }
        //}

        /// <summary>
        /// 清除跟随的协程
        /// </summary>
        private void ClearCoroutine()
        {
            if (fllowCoroutine != null)
            {
                StopCoroutine(fllowCoroutine);
            }
        }
        /// <summary>
        /// 当得到一个套筒的时候
        /// 如何去掉套筒呢？
        /// </summary>
        /// <param name="taotong"></param>
        public void OnCatchTaoTong(ToolConf taotong)
        {
            this.taoTong = taotong;
            ///如果当前没有套上套筒，则凹槽开启
            ///如果已经套上了套筒，则凹槽关闭，不可接受其他套筒
            IsAoCaoActive = !this.taoTong;
        }

        /// <summary>
        /// 当接触到某个指定零件
        /// </summary>
        /// <param name="otherTool"></param>
        protected override void OnBeginHoverTool_(ToolConf otherTool)
        {
            //Debug.Log("当接触到某个指定零件OnBeginHoverTool_" + otherTool);
            base.OnBeginHoverTool_(otherTool);
            if (otherTool.toolBasic is ToolLuoSuan)
            {
                var tmpLuosuan = otherTool.toolBasic as ToolLuoSuan;
                ///没有安装套筒
                ///弹出框提示：棘轮扳手要安装套筒才能使用
                if (tmpLuosuan.taotong != taoTong)
                {
                    if (dynamicConf != null)
                        DynamicActionController.Instance.DoDynamic(dynamicConf);
                    return;
                }
                ClearCoroutine();
                curluoSuan = tmpLuosuan;
                turnDir = Instantiate(turnDirection);
                turnDir.transform.SetParent(otherTool.toolBasic.transform);
                turnDir.transform.localPosition = new Vector3(0, turnDirHight, 0);
                startRotate = true;
                if (curluoSuan.Direct == 1)
                {
                    turnDir.transform.localEulerAngles = new Vector3(90, 0, 0);
                    turnDir.transform.Rotate(Vector3.down, Space.Self);
                }
                else
                {
                    turnDir.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                StartCoroutine(RotateDir());
                SetToolCatch(false);
                SetToolKinematic(true);
                isWorking = true;

                if (catchHand != null)
                {
                    curHand = catchHand;
                    catchHand.DetachObject(gameObject);
                }
                ///设置触发的位置
                //transform.position = otherTool.toolBasic.transform.position + workPos;
                fllowCoroutine = StartCoroutine(FllowHand());
            }

            else
            {

            }
        }

        IEnumerator RotateDir()
        {
            while (startRotate)
            {
                yield return new WaitForSeconds(0.01F);
                if (curluoSuan)
                {
                    if (curluoSuan.Direct == 1)
                        turnDir.transform.Rotate(Vector3.down * 5, Space.World);
                    else
                        turnDir.transform.Rotate(Vector3.up * 5, Space.World);
                }
            }
        }
        /// <summary>
        /// 完成时改变螺栓的状态后，或者远离手柄后，自动将工具归位
        /// </summary>
        public void Complete()
        {
            if (turnDir != null)
            {
                startRotate = false;
                Destroy(turnDir);
            }
            if (fllowTarget)
            {
                fllowTarget.DetachChildren();
                Destroy(fllowTarget.gameObject);
            }
            if (curHand)
            {
                if (isWorking)
                {
                    isWorking = false;
                    SetToolCatch(true);
                    curHand.AttachObject(gameObject, GrabTypes.Pinch, toolConf.catchFlags);
                }
            }
            else
            {
                SetToolKinematic(true);
            }
            isWorking = false;
            curluoSuan = null;
        }

        /// <summary>
        /// 跟随的手的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator FllowHand()
        {
            //Debug.Log("跟随的手的协程FllowHand");
            if (fllowTarget)
            {
                fllowTarget.DetachChildren();
                Destroy(fllowTarget);
            }
            fllowTarget = new GameObject("BanShouFllow").transform;
            fllowTarget.position = curluoSuan.transform.position;
            fllowTarget.rotation = curluoSuan.transform.rotation;
            transform.SetParent(fllowTarget);
            transform.localPosition = workPos;
            transform.localEulerAngles = workAngle;
            yield return new WaitForFixedUpdate();
            float curShockTime = Time.time;
            while (curHand && isWorking && curluoSuan)
            {
                if (curluoSuan.CheckIsComplete(targetState))
                {
                    Complete();
                    break;
                }
                Vector3 pos = curHand.transform.position;
                pos.y = transform.position.y;
                Vector3 forward = transform.position - pos;
                fllowTarget.forward = forward;
                yield return new WaitForFixedUpdate();
                if (Vector3.Distance(curHand.transform.position, transform.position) > maxFllowDis)
                {
                    Complete();
                    break;
                }

                ///如果扳手的坐标累加，则扳手正在逆行，卸螺丝，否则为上螺丝
                if (lastAngle != transform.eulerAngles.y && Mathf.Abs(lastAngle - transform.transform.eulerAngles.y) < 90f)
                {
                    float deltAngle = 0.1f;
                    bool isChange = curluoSuan.AddForce(lastAngle - transform.eulerAngles.y, out deltAngle);
                    if (Time.time - curShockTime > shockTime
                        && isChange && Mathf.Abs(lastAngle - transform.eulerAngles.y) > 0.1f)
                    {
                        VRHandHelper.Instance.ShockHand(curHand, (ushort)(1500 * (1+deltAngle)));
                        curShockTime = Time.time;
                    }
                }
                lastAngle = transform.eulerAngles.y;
            }
            Complete();
        }



        /// <summary>
        /// 结束工具的交互状态
        /// </summary>
        /// <param name="otherTool"></param>
        //protected override void OnEndHoverTool_(ToolConf otherTool)
        //{
        //    base.OnEndHoverTool_(otherTool);
        //    if (otherTool.toolBasic is ToolLuoSuan)
        //    {
        //        curluoSuan = null;// otherTool.toolBasic as ToolLuoSuan;
        //    }
        //}
        /// <summary>
        /// 当拿起的时候
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnCatch_(Hand hand)
        {
            base.OnCatch_(hand);
            this.curHand = hand;
            SetToolHighlight(false);
        }
        /// <summary>
        /// 松手的时候，去掉引用
        /// </summary>
        /// <param name="hand"></param>
        protected override void OnDetach_(Hand hand)
        {
            base.OnDetach_(hand);
            if (!isWorking)
            {
                this.curHand = null;
            }
            if (taoTong)
            {
                taoTong.toolBasic.SetTrigger(true);
            }
        }
        protected override void OnPressUp_(Hand hand)
        {
            base.OnPressUp_(hand);
            if (isWorking)
            {
                Complete();
                this.curHand = null;
                ClearCoroutine();
            }
        }
        /// <summary>
        /// 扳手进入螺栓的时候
        /// </summary>
        /// <param name="otherTool"></param>
        protected override void OnEnterTool_(ToolConf otherTool)
        {
            base.OnEnterTool_(otherTool);
            if (otherTool.toolBasic is ToolLuoSuan)
            {
                ToolLuoSuan ls = otherTool.toolBasic as ToolLuoSuan;
                ///如果没有完成状态
                if (!ls.CheckIsComplete(targetState) && curluoSuan != ls)
                {
                    if (curluoSuan == null || curluoSuan.CheckIsComplete(targetState))
                    {
                        curluoSuan = otherTool.toolBasic as ToolLuoSuan;
                        OnBeginHoverTool_(otherTool);
                    }
                }
            }
        }
        /// <summary>
        /// 离开工具
        /// </summary>
        /// <param name="otherTool"></param>
        protected override void OnExitTool_(ToolConf otherTool)
        {
            base.OnExitTool_(otherTool);
            if (curluoSuan != null && otherTool.toolBasic == curluoSuan)
            {
                curluoSuan = null;
            }
        }

    }
}