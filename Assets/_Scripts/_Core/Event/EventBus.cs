
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QGame.Core.Event
{
    public class XEventArgs
    {
        private object[] args;

        public XEventArgs()
        {
        }
        public XEventArgs(params object[] args)
        {
            this.args = args;
        }

        public T GetData<T>(int idx = 0)
        {
            if (args != null && args.Length > idx && args[idx] is T)
                return (T)args[idx];

            return default(T);
        }
    }

    public delegate void XEventHandler(XEventArgs args);

    public class XSubscriber
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; private set; }

        public XEventHandler Handler { get; private set; }

        public XSubscriber(XEventHandler handler, int priority = 0)
        {
            Priority = priority;
            Handler = handler;
        }
    }

    public class XSubscriberList
    {
        private readonly List<XSubscriber> _items = new List<XSubscriber>();

        public void Add(XEventHandler handler, int priority)
        {
            _items.Add(new XSubscriber(handler, priority));
            _items.Sort(Sort);
        }

        public bool Remove(XEventHandler handler)
        {
            _items.RemoveAll(item => item.Handler.Equals(handler));
            return _items.Count == 0;
        }

        public bool Contains(XEventHandler handler)
        {
            return _items.Exists(item => item.Handler.Equals(handler));
        }

        public void Call(XEventArgs args)
        {
            for (var i = _items.Count - 1; i >= 0; i--)
            {
                try
                {
                    _items[i].Handler.Invoke(args);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Event Call Exception:" + ex.Message);
                    throw;
                }
            }
        }

        public static int Sort(XSubscriber p1, XSubscriber p2)
        {
            return p1.Priority.CompareTo(p2.Priority);
        }
    }

    public class XEventBus
    {
        public static XEventBus Instance = new XEventBus();

        readonly Dictionary<EventId, XSubscriberList> _events = new Dictionary<EventId, XSubscriberList>();

        public void Register(EventId id, XEventHandler handler, int priority = 0)
        {
            if (IsRegistered(id, handler))
                return;

            XSubscriberList list = null;

            if (!_events.ContainsKey(id))
            {
                list = new XSubscriberList();
                _events.Add(id, list);
            }
            else
            {
                list = _events[id];
            }

            list.Add(handler, priority);
        }

        public bool IsRegistered(EventId id, XEventHandler handler)
        {
            if (handler == null)
                return true;

            if (!_events.ContainsKey(id))
                return false;

            return _events[id].Contains(handler);
        }

        public void UnRegister(EventId id, XEventHandler handler)
        {
            if (!_events.ContainsKey(id))
                return;

            var list = _events[id];

            if (list.Remove(handler))
                _events.Remove(id);
        }

        public void Post(EventId id, XEventArgs args)
        {
            if (!_events.ContainsKey(id))
                return;

            var list = _events[id];

            list.Call(args);
        }

        public void Post(EventId id, params object[] args)
        {
            Post(id, new XEventArgs(args));
        }
    }
}
