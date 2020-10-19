using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace BeinLab.Util
{
    public delegate void TimerHandler();

    public delegate void TimerArgsHandler(System.Object[] args);

    public class Timer
    {
        public TimerHandler Handler;           //无参的委托
        public TimerArgsHandler ArgsHandler;   //带参数的委托
        public float Frequency;               //时间间隔
        public int Repeats;                   //重复次数
        public System.Object[] Args;

        public float LastTickTime;

        public event Action OnComplete;        //计时器完成一次工作
        public event Action OnDestroy;        //计时器被销毁

        public Timer() { }

        /// <summary>
        /// 创建一个时间事件对象
        /// </summary>
        /// <param name="Handler">回调函数</param>
        /// <param name="ArgsHandler">带参数的回调函数</param>
        /// <param name="frequency">时间内执行</param>
        /// <param name="repeats">重复次数</param>
        /// <param name="Args">参数  可以任意的传不定数量，类型的参数</param>
        public Timer(TimerHandler Handler, TimerArgsHandler ArgsHandler, float frequency, int repeats, System.Object[] Args)
        {
            this.Handler = Handler;
            this.ArgsHandler = ArgsHandler;
            this.Frequency = frequency;
            this.Repeats = repeats == 0 ? 1 : repeats;
            this.Args = Args;
            this.LastTickTime = Time.time;
        }

        public void Notify()
        {
            if (Handler != null)
                Handler();
            if (ArgsHandler != null)
                ArgsHandler(Args);
            if (OnComplete != null)
            {
                OnComplete();
            }
        }

        /// <summary>
        /// 清理计时器，初始化参数  同时清理事件
        /// </summary>
        public void CleanUp()
        {
            Handler = null;
            ArgsHandler = null;
            Repeats = 1;
            Frequency = 0;
            if (OnDestroy != null)
            {
                OnDestroy();
            }
            OnDestroy = null;
            OnComplete = null;
        }
    }

    /// <summary>
    /// 计时器
    /// 添加一个计时事件
    /// 删除一个计时事件
    /// 更新计时事件
    /// </summary>
    public class TimerMgr : Singleton<TimerMgr>
    {
        private List<Timer> _Timers;//时间管理器
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            if (_Timers == null)
            {
                _Timers = new List<Timer>();
            }
            Application.runInBackground = true;
        }
        /// <summary>
        /// 创建一个简单的计时器
        /// </summary>
        /// <param name="callBack">回调函数</param>
        /// <param name="time">计时器时间</param>
        /// <param name="repeats">回调次数  小于0代表循环 大于0代表repeats次</param>
        public Timer CreateTimer(TimerHandler callBack, float time, int repeats = 1)
        {
            return Create(callBack, null, time, repeats);
        }

        public Timer CreateTimerWithArgs(TimerArgsHandler callBack, float time, int repeats, params System.Object[] args)
        {
            return Create(null, callBack, time, repeats, args);
        }

        private Timer Create(TimerHandler callBack, TimerArgsHandler callBackArgs, float time, int repeats, params System.Object[] args)
        {
            Timer timer = new Timer(callBack, callBackArgs, time, repeats, args);
            _Timers.Add(timer);
            return timer;
        }

        public Timer DestroyTimer(Timer timer)
        {
            if (timer != null)
            {
                _Timers.Remove(timer);
                timer.CleanUp();
                timer = null;
            }
            return timer;
        }
        public void ClearAll()
        {
            if (_Timers != null)
            {
                for (int i = 0; i < _Timers.Count; i++)
                {
                    _Timers[i].CleanUp();
                }
                _Timers.Clear();
            }
        }
        /// <summary>
        /// 固定更新检查更新的频率
        /// </summary>
        void Update()
        {
            if (_Timers!=null&&_Timers.Count != 0)
            {
                for (int i = _Timers.Count - 1; i >= 0; i--)
                {
                    if (i > _Timers.Count - 1) {
                        return;
                    }
                    Timer timer = _Timers[i];
                    float curTime = Time.time;
                    if (timer.Frequency + timer.LastTickTime > curTime)
                    {
                        continue;
                    }
                    timer.LastTickTime = curTime;
                    if (timer.Repeats-- == 0)
                    {//计时完成，可以删除了
                        DestroyTimer(timer);
                    }
                    else
                    {//触发计时
                        timer.Notify();
                    }
                }
            }
        }
    }
}