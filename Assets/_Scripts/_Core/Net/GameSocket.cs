using QGame.Core.Event;
using QGame.Core.XProto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


namespace QGame.Core.Net
{
    public delegate void MessageCallback(int result, GameResponse resp);
    public delegate void MessageListener(int cmdId, int result, GameResponse resp);

    public class CallbackInfo
    {
        public int CmdId = 0;
        public int Sequence = 0;
        public DateTime SendTime = DateTime.Now;
        public DateTime TimeOut;
        public MessageCallback Callback;

        public CallbackInfo(int cmdId, int sequence, int timeOut, MessageCallback callback)
        {
            CmdId = cmdId;
            Sequence = sequence;
            Callback = callback;
            TimeOut = DateTime.Now.AddSeconds(timeOut);
        }
    }

    public class ListenerInfo
    {
        public int CmdId = 0;
        public MessageListener Listener;

        public ListenerInfo(int cmdId, MessageListener listener)
        {
            CmdId = cmdId;
            Listener = listener;
        }
    }

    public class GameSocket
    {
        private Socket mSocket = null;

        private Thread threadRecv = null;
        //private Thread threadCheck = null;

        private int sequence = 0;
        private int timeout = 12;
        private bool closeing = false;

        private IPEndPoint ipEndPort = null;

        List<CallbackInfo> callbacks = null;
        List<ListenerInfo> listeners = null;

        public static GameSocket Instance = new GameSocket();

        private Queue<GameResponse> queues = null;


        public GameSocket()
        {
            callbacks = new List<CallbackInfo>();
            listeners = new List<ListenerInfo>();
            queues = new Queue<GameResponse>();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public int GetSequence()
        {
            return ++sequence;
        }

        public bool Connected { get { return mSocket.Connected; } }

        public void Connect(string host, int port, int timeout)
        {
            try
            {
                var ips = Dns.GetHostAddresses(host);
                this.timeout = timeout;
                ipEndPort = new IPEndPoint(ips[0], port);

                Connect();
            }
            catch (Exception e)
            {
                //MessageTip.GetInstance().ShowMessage("连接服务器失败！请检查网络后重试！");
                Debug.Log("连接失败！");
                System.Console.Write("ERROR:" + e.Message);
            }
        }

        public void Connect()
        {
            if (mSocket.Connected)
                return;

            closeing = false;
            try
            {
                //mSocket.NoDelay = true;
                //SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                //socketAsyncEventArgs.Completed += OnComplete;
                //socketAsyncEventArgs.RemoteEndPoint = ipEndPort;
                //mSocket.ConnectAsync(socketAsyncEventArgs);
                 
                mSocket.Connect(ipEndPort);
            }
            catch (Exception e)
            {
                //LoginModule.GetInstance().ShowConnectPanel();
                Debug.Log("连接失败！"+ e);

                close();
                ReBuild();
                //MessageTip.GetInstance().ShowMessage("您已断开连接！");
                return;
            }

            threadRecv = new Thread(new ThreadStart(recvThread));
            threadRecv.Start();

            //XEventBus.Instance.Post(EventId.Connect);
            //threadCheck = new Thread(new ThreadStart(checkThread));
            //threadCheck.Start();
        }
        //private void OnComplete(object sender, SocketAsyncEventArgs e)
        //{
        //    Debug.Log("mSocket.Connected:" + mSocket.Connected);
        //}
        public void close()
        {
            closeing = true;
            if (threadRecv != null)
                threadRecv.Abort();
            mSocket.Close(0);
        }

        public void ReBuild()
        {
            sequence = 0;
            callbacks = new List<CallbackInfo>();
            listeners = new List<ListenerInfo>();
            queues = new Queue<GameResponse>();
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Send(GameRequest req, MessageCallback callback)
        {
            int retry = 0;

            while (!mSocket.Connected && retry < 5)
            {
                Connect();
                retry++;
                Thread.Sleep(200);
            }

            if (!mSocket.Connected)
            {
				//MessageTip.GetInstance().ShowMessage("连接服务器失败！请检查网络后重试！");
                return;
            }

            req.Sequence = GetSequence();
            if (callback != null)
            {
                callbacks.Add(new CallbackInfo(req.CmdId, req.Sequence, timeout, callback));
            }
			
			//Log.Debug("GameSocket:Send CmdId={0},Seq={1}", req.CmdId, req.Sequence);

            //写消息  
            mSocket.Send(req.Encode());
        }

        public void Send(int cmd, Message body, MessageCallback callback)
        {
            var req = new GameRequest();
            req.CmdId = cmd;
            req.Body = body;
            Send(req, callback);
        }

        private float diffTime;
        //private void checkThread()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(1000);

        //        CallbackInfo item = null;
        //        DateTime now = DateTime.Now;

        //        for (int i = callbacks.Count - 1; i >= 0; i--)
        //        {
        //            //超时
        //            item = callbacks[i];
        //            if (now >= item.TimeOut)
        //            {
        //                callbacks.Remove(item);
        //                var resp = new GameResponse(item.CmdId, item.Sequence, -99);
        //                //OnRecv(resp);
        //                MessageTip.GetInstance().ShowMessage("连接超时！");
        //                Log.Debug("GameSocket:Timeout CmdId={0},Seq={1},Result={2}", resp.CmdId, resp.Sequence, resp.Result);

        //                queues.Enqueue(resp);
        //            }
        //        }
        //    }
        //}

        private void recvThread()
        {
            int max_buffer = 1024 * 64;
            byte[] buffer = new byte[max_buffer];

            bool bReadedHead = false;
            int iTotalLen = 0;
            int iRecvLen = 0;
            bool nextPacket = true;
            short msgLen = 0;

            while (!closeing)
            {
                if (!mSocket.Connected)
                {
                    break;
                }

                if (nextPacket)
                {
                    //强断有异常
                    iRecvLen = mSocket.Receive(buffer, iTotalLen, max_buffer - iTotalLen, SocketFlags.None);
                    System.Console.WriteLine("Recv len={0}", iRecvLen);
                    Debug.Log("iRecvLen:"+ iRecvLen);
                    if (iRecvLen <= 0)
                    {
                        //网络断开
                        break;
                    }
                    iTotalLen += iRecvLen;
                }

                //先读包头
                if (!bReadedHead && iTotalLen > 3)
                {
                    msgLen = (short)((buffer[1] & 0xff) << 8);
                    msgLen += (short)(buffer[2] & 0xff);
                    bReadedHead = true;
                }
                if (bReadedHead && iTotalLen >= msgLen)
                {
                    Debug.Log("读取数据");
                    byte[] temp = new byte[msgLen - 3];
                    Array.Copy(buffer, 3, temp, 0, msgLen - 3);

                    var resp = GameResponse.Decode(temp);

                    iTotalLen -= msgLen;

                    if (iTotalLen > 0)
                        Array.Copy(buffer, msgLen, buffer, 0, iTotalLen);

                    bReadedHead = false;

                    //lock (lockObj)
                    {
                        queues.Enqueue(resp);
                    }
                    //OnRecv(resp);

                   // Log.Debug("GameSocket:Recv CmdId={0},Seq={1},Result={2}", resp.CmdId, resp.Sequence, resp.Result);

                    nextPacket = (iTotalLen == 0);
                }
                else
                {
                    nextPacket = true;
                }

                Thread.Sleep(10);
            }
        }

        private void checkTimeout()
        {
            CallbackInfo item = null;
            DateTime now = DateTime.Now;

            for (int i = callbacks.Count - 1; i >= 0; i--)
            {
                //超时
                item = callbacks[i];
                if (now >= item.TimeOut)
                {       
                    //callbacks.Remove(item);
                    var resp = new GameResponse(item.CmdId, item.Sequence, -99);
                    //Log.Debug("GameSocket:Timeout CmdId={0},Seq={1},Result={2}", resp.CmdId, resp.Sequence, resp.Result);
                    queues.Enqueue(resp);
                    //MessageTip.GetInstance().ShowMessage("连接超时！");
                }
            }

            if (diffTime <= 0f)
                diffTime = Time.time;
            //超时机制
            if (callbacks.Count > 0 && Time.time - diffTime < timeout)
            {
                if (Time.time - diffTime > 0.5f) {
                    // LayerManager.GetInstance().ShowLoadingMask();
                }

            }
            else
            {
                diffTime = 0;
                //LayerManager.GetInstance().CloseLoadingMask();
            }
        }

        public void Process()
        {
            while (queues.Count > 0)
            {
                GameResponse resp = queues.Dequeue();
                OnRecv(resp);
                //Threader.RunOnMainThread(() => OnRecv(resp));
            }

            checkTimeout();
        }

        private void OnRecv(GameResponse resp)
        {
            // System.Console.WriteLine("GameSocket Recv: {0}", resp.ToString());
            // Log.Debug("GameSocket Recv: {0}", resp.ToString());
            if (resp == null) return;
                 
            bool done = false;
            if (resp.Sequence > 0)
            {
                DateTime now = DateTime.Now;
                foreach (var item in callbacks)
                {
                    if (item.Sequence == resp.Sequence)
                    {
                       //double ms= (now - item.SendTime).TotalMilliseconds;
                       // if (ms > 1000)
                       //     MessageTip.GetInstance().ShowMessage("协议: CmdId=" + resp.CmdId + ",耗时 =" + ms + "ms");
                       // Log.Debug("Response: CmdId={0}, CostTime={1}ms", resp.CmdId, (now - item.SendTime).TotalMilliseconds);
                        done = true;
                        callbacks.Remove(item);
                        item.Callback(resp.Result, resp);
                        break;
                    }
                }
            }

            if (!done)
            {
                foreach (var item in listeners)
                {
                    if (item.CmdId == resp.CmdId)
                    {
                        done = true;
                        item.Listener(resp.CmdId, resp.Result, resp);
                    }
                }
            }

            //全局报错
            if (resp.Result != 0)
            {
                System.Console.Write("Cmd:" + resp.CmdId + ",Result:" + resp.Result);
                //XEventBus.Instance.Post(EventId.GameCommonError, resp.Result);
            }

            if (!done)
            {
                System.Console.WriteLine("GameClient未处理消息：{0}", resp);
            }
        }

        public void AddListener(int cmdId, MessageListener listener)
        {
            if (!ExistsListener(cmdId, listener))
            {
                listeners.Add(new ListenerInfo(cmdId, listener));
            }
        }

        public bool ExistsListener(int cmdId, MessageListener listener)
        {
            foreach (ListenerInfo item in listeners)
            {
                if (item.CmdId == cmdId && item.Listener == listener)
                    return true;
            }
            return false;
        }

        public void RemoveListener(int cmdId, MessageListener listener)
        {
            ListenerInfo item;
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                item = listeners[i];
                if (item.CmdId == cmdId && item.Listener == listener)
                    listeners.Remove(item);
            }
        }

        public void RemoveAllListener(int cmdId, MessageListener listener)
        {
            listeners.Clear();
        }
    }
}
