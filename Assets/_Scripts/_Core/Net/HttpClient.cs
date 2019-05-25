using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace QGame.Core.Net
{
    public delegate void HttpProgressHandler(HttpClient sender, HttpResponse response);
    public delegate void HttpCompletedHandler(HttpClient sender, Exception exception);

    public class HttpHeader
    {
        readonly Dictionary<string, List<string>> _headers = new Dictionary<string, List<string>>();

        public void AddHeader(string name, string value)
        {
            name = name.ToLower().Trim();
            value = value.Trim();
            if (!_headers.ContainsKey(name))
                _headers[name] = new List<string>();
            _headers[name].Add(value);
        }
        public void AddHeader(string line)
        {
            var split = line.IndexOf(':');
            if (split == -1)
                return;
            var parts = new string[2];
            parts[0] = line.Substring(0, split).Trim();
            parts[1] = line.Substring(split + 1).Trim();

            AddHeader(parts[0], parts[1]);
        }

        public void SetHeader(string name, string value)
        {
            name = name.ToLower().Trim();
            value = value.Trim();
            if (!_headers.ContainsKey(name))
                _headers[name] = new List<string>();
            _headers[name].Clear();
            _headers[name].TrimExcess();
            _headers[name].Add(value);
        }

        public List<string> GetHeaders()
        {
            var result = new List<string>();
            foreach (var name in _headers.Keys)
            {
                foreach (var value in _headers[name])
                {
                    result.Add(name + ": " + value);
                }
            }
            return result;
        }

        public List<string> GetHeaders(string name)
        {
            name = name.ToLower().Trim();
            if (!_headers.ContainsKey(name))
                return new List<string>();
            return _headers[name];
        }

        public string GetHeader(string name)
        {
            name = name.ToLower().Trim();
            if (!_headers.ContainsKey(name))
                return string.Empty;
            return _headers[name][_headers[name].Count - 1];
        }

        public void Clear()
        {
            _headers.Clear();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (string name in _headers.Keys)
            {
                foreach (string value in _headers[name])
                {
                    sb.AppendLine(name + ": " + value);
                }
            }

            return sb.ToString();
        }
    }

    public class HttpRequest
    {
        public string Method = "GET";
        private const string Protocol = "HTTP/1.1";
        public HttpHeader Header = new HttpHeader();

        public Uri Uri { get; set; }
        public byte[] PostData { get; set; }

        public HttpRequest(string method, string uri, byte[] data = null)
        {
            this.Method = method;
            this.Uri = new Uri(uri);
            this.PostData = data;
        }

        public void WriteToStream(Stream output)
        {
            Initialize();

            var stream = new BinaryWriter(output);

            stream.Write(Encoding.ASCII.GetBytes(Method.ToUpper() + " " + Uri.PathAndQuery + " " + Protocol));
            stream.Write(HttpClient.Eol);
            stream.Write(Encoding.ASCII.GetBytes(Header.ToString()));
            stream.Write(HttpClient.Eol);

            if (PostData != null && PostData.Length > 0)
            {
                stream.Write(PostData);
            }
        }

        private void Initialize()
        {
            Header.SetHeader("Host", Uri.Host);

            if (Header.GetHeader("Content-Type") == "")
            {
                Header.SetHeader("Content-Type", "application/x-www-form-urlencoded");
            }

            if (PostData != null && PostData.Length > 0)
            {
                Header.SetHeader("Content-Length", PostData.Length.ToString());
            }

            if (Header.GetHeader("User-Agent") == "")
            {
                Header.SetHeader("User-Agent", "UnityWeb/1.0");
            }

            if (Header.GetHeader("Connection") == "")
            {
                Header.SetHeader("Connection", "close");
            }

            // Basic Authorization
            if (!String.IsNullOrEmpty(Uri.UserInfo))
            {
                Header.SetHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Uri.UserInfo)));
            }
        }
    }

    public class HttpResponse
    {
        public static readonly int MinProgress = 512;
        public int Status { get; private set; }
        public string Message { get; private set; }

        public byte[] ResponseData { get; private set; }

        public bool IsFile { get; set; }
        public string FilePath { get; set; }

        public int ContentLength { get; private set; }
        public int ReceiveLength { get; private set; }

        public float Progress
        {
            get { return (float)ReceiveLength / ReceiveLength; }
        }

        public HttpHeader Header = new HttpHeader();

        public Action OnProgress { private get; set; }
        public string Text
        {
            get
            {
                if (ResponseData == null)
                    return "";
                return System.Text.Encoding.UTF8.GetString(ResponseData);
            }
        }

        public HttpResponse(Action onProgress)
        {
            OnProgress = onProgress;
        }

        protected string ReadLine(Stream stream)
        {
            var line = new List<byte>();
            while (true)
            {
                int c = stream.ReadByte();
                if (c == -1)
                {
                    throw new Exception("Unterminated Stream Encountered.");
                }
                if ((byte)c == HttpClient.Eol[1])
                    break;
                line.Add((byte)c);
            }
            var s = Encoding.ASCII.GetString(line.ToArray()).Trim();
            return s;
        }

        public void ReadFromStream(Stream input)
        {
            if (IsFile)
            {
                using (var output = File.Open(FilePath, FileMode.Create))
                {
                    ReadFromStream(input, output);
                }
            }
            else
            {
                using (var output = new MemoryStream())
                {
                    ReadFromStream(input, output);
                    ResponseData = output.ToArray();
                }
            }
        }

        private void ReadFromStream(Stream input, Stream output)
        {
            var line = ReadLine(input);
            var top = line.Split(new char[] { ' ' });
            int status = 200;

            if (!int.TryParse(top[1], out status))
                throw new Exception("Bad Status Code");

            Status = status;
            Message = string.Join(" ", top, 2, top.Length - 2);
            Header.Clear();

            while (true)
            {
                line = ReadLine(input);
                if (string.IsNullOrEmpty(line))
                    break;

                Header.AddHeader(line);
            }

            if (Header.GetHeader("transfer-encoding") == "chunked")
            {
                int len = 0;

                do
                {
                    line = ReadLine(input);
                    len = int.Parse(line, NumberStyles.AllowHexSpecifier);

                    ContentLength += len;

                    for (int i = 0; i < len; i++)
                    {
                        if (++ReceiveLength % MinProgress == 0 && OnProgress != null)
                            OnProgress();

                        output.WriteByte((byte)input.ReadByte());
                    }

                    input.ReadByte();
                    input.ReadByte();

                } while (len > 0);
            }
            else
            {
                try
                {
                    ContentLength = int.Parse(Header.GetHeader("TaskListContent-length"));
                }
                catch
                {
                    ContentLength = 0;
                }

                int b;

                while ((ContentLength == 0 || output.Length < ContentLength)
                          && (b = input.ReadByte()) != -1)
                {
                    if (++ReceiveLength % MinProgress == 0 && OnProgress != null)
                        OnProgress();

                    output.WriteByte((byte)b);
                }

                if (ContentLength > 0 && output.Length != ContentLength)
                {
                    throw new Exception("Response length does not match TaskListContent length");
                }
            }
        }
    }

    public class HttpClient
    {
        public static bool LogAllRequests = true;
        public static bool VerboseLogging = true;
        public static byte[] Eol = { (byte)'\r', (byte)'\n' };

        public HttpRequest Request { get; private set; }
        public HttpResponse Response { get; private set; }
        public bool IsDone { get; private set; }

        public int MaxRetryCount = 8;
        public bool Synchronous { get; set; }

        public event HttpProgressHandler ProgressEvent;
        public event HttpCompletedHandler CompletedEvent;

        private void SendRequest()
        {
            try
            {
                var retry = 0;
                while (++retry < MaxRetryCount)
                {
                    var client = new TcpClient();
                    client.Connect(Request.Uri.Host, Request.Uri.Port);
                    using (var stream = client.GetStream())
                    {
                        Request.WriteToStream(stream);
                        Response.ReadFromStream(stream);
                    }
                    client.Close();
                    switch (Response.Status)
                    {
                        case 307:
                        case 302:
                        case 301:
                            Request.Uri = new Uri(Response.Header.GetHeader("Location"));
                            continue;
                        default:
                            retry = MaxRetryCount;
                            break;
                    }
                }

                IsDone = true;
                OnCompleted(null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled Exception, aborting request.");
                Console.WriteLine(e);
                IsDone = true;
                OnCompleted(e);
            }

            if (LogAllRequests)
            {
                System.Console.WriteLine("NET: " + InfoString(VerboseLogging));
            }
        }

        private void Start()
        {
            if (Synchronous)
            {
                SendRequest();
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object t)
                {   
                    SendRequest();
                }));
            }
        }

        public void Download(string url, string path)
        {
            Request = new HttpRequest("GET", url, null);
            Response = new HttpResponse(onProgress);
            Response.IsFile = true;
            Response.FilePath = path;

            Start();
        }

        public void Post(string url, byte[] data)
        {
            Request = new HttpRequest("POST", url, data);
            Response = new HttpResponse(onProgress);

            Start();
        }
        public void Get(string url)
        {
            Request = new HttpRequest("GET", url, null);
            Request.Header.AddHeader("Accept", "text/html");
            Response = new HttpResponse(onProgress);

            Start();
        }

        private void onProgress()
        {
            if (!Synchronous && ProgressEvent != null)
            {
                ProgressEvent(this, Response);
            }
        }
        private void OnCompleted(Exception e)
        {
            if (Synchronous)
            {
                if (e != null)
                    throw e;
            }
            else if (CompletedEvent != null)
            {
                CompletedEvent(this, e);
            }
        }

        private static readonly string[] Sizes = { "B", "KB", "MB", "GB" };
        public string InfoString(bool verbose)
        {
            string status = IsDone && Response != null ? Response.Status.ToString() : "---";
            string message = IsDone && Response != null ? Response.Message : "Unknown";
            double size = IsDone && Response != null && Response != null ? Response.ContentLength : 0.0f;

            int order = 0;
            while (size >= 1024.0f && order + 1 < Sizes.Length)
            {
                ++order;
                size /= 1024.0f;
            }

            string sizeString = String.Format("{0:0.##}{1}", size, Sizes[order]);

            string result = Request.Uri.ToString() + " [ " + Request.Method.ToUpper() + " ] [ " + status + " " + message + " ] [ " + sizeString + " ] ";

            if (verbose && Response != null)
            {
                result += "\n\nRequest Headers:\n\n" + Request.Header.ToString();
                result += "\n\nResponse Headers:\n\n" + Response.Header.ToString();

                if (Response != null && Response.ResponseData != null)
                {
                    result += "\n\nResponse Body:\n" + System.Text.Encoding.UTF8.GetString(Response.ResponseData);
                }
            }

            return result;
        }
    }
}
