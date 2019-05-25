using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QGame.Core.Utils
{
    public delegate void TimerHandler(params object[] args);
    public class TimerCallback
    {
        /// <summary>
        /// 是否已经废弃
        /// </summary>
        public bool Disposed { get; set; }

        /// <summary>
        /// 隔多长时间触发一次
        /// </summary>
        public float Interval { get; set; }

        /// <summary>
        /// 下一次应该执行的时间
        /// </summary>
        public float NextTime { get; set; }

        /// <summary>
        /// 总共要执行的次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 已执行次数
        /// </summary>
        public int CurrentCount { get; set; }

        public TimerHandler Handler { get; private set; }

        public object[] Args { get; private set; }

        public TimerCallback(TimerHandler handler, float nextTime, float interval, int count, params object[] args)
        {
            this.Handler = handler;
            this.Args = args;
            this.NextTime = nextTime;
            this.Interval = interval;
            this.Count = count;

            this.Disposed = false;
            this.CurrentCount = 0;
        }

        public virtual void Dispose()
        {
            Disposed = true;
        }

        public void Call(float time)
        {
            if (NextTime > time || Disposed)
                return;

            Handler.Invoke(Args);

            CurrentCount++;

            if (Math.Abs(Interval) < 0.0001f || Count <= CurrentCount)
            {
                Disposed = true;
            }
            else
            {
                NextTime = time + Interval;
            }
        }
    }

    public static class Timer
    {
        private static readonly Dictionary<Int64, TimerCallback> _list = new Dictionary<long, TimerCallback>();

        private static long _currentTimerId = 0;

        private static long GenTimerId()
        {
            if (_currentTimerId < Int64.MaxValue)
                return ++_currentTimerId;

            throw new Exception("Timer Id Out Index");
        }

        public static long Add(TimerHandler handler, float delay, float interval = 0, int count = 0, params object[] args)
        {
            float nextTime = Time.time + delay;
            long timerId = GenTimerId();

            var callback = new TimerCallback(handler, nextTime, interval, count, args);

            _list.Add(timerId, callback);

            return timerId;
        }

        public static void Cancel(long timerId)
        {
            TimerCallback cb;
            if (_list.TryGetValue(timerId, out cb))
            {
                cb.Dispose();
            }
        }

        public static void CancelAll()
        {
            _list.Clear();
        }


        public static void Update()
        {
            if (_list.Count == 0)
                return;

            float now = Time.time;
            var temp = new List<long>();

            for (int i = 0; i < _list.Keys.Count; i++)
            {
                var key = _list.Keys.ElementAt(i);
                var callback = _list[key];
                callback.Call(now);
                if (callback.Disposed)
                    temp.Add(key);
            }


            foreach (var key in _list.Keys)
            {
                var callback = _list[key];
                callback.Call(now);
                if (callback.Disposed)
                    temp.Add(key);
            }

            foreach (var key in temp)
            {
                _list.Remove(key);
            }
        }
    }
}
