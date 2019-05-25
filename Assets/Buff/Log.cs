using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace QGame.Core.Utils
{
    public enum LogType : int
    {
        None,
        Debug,
        Info,
        Warning,
        Error,
        Exception,
    }
    public class Log
    {
        public static LogType Level = LogType.None;

        private static string outPath;
        private static StreamWriter writer;

        static Log()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            outPath = System.IO.Directory.GetCurrentDirectory() + "/" + "debug.log";
#elif UNITY_ANDROID
            outPath = Application.persistentDataPath + "/debug.log";
#endif
        }

        public static void Open()
        {
            writer = new StreamWriter(outPath, false, Encoding.UTF8);

            //运行时间
            writer.WriteLine("start time:{0}", DateTime.Now.ToString());
            //设备名称，及类型
            writer.WriteLine("{0},{1}", SystemInfo.deviceName, SystemInfo.deviceType);
            //操作系统，内存容量
            writer.WriteLine("{0},{1}", SystemInfo.operatingSystem, SystemInfo.systemMemorySize);
            //渲染设备及版本，显存信息
            writer.WriteLine("{0},{1},{2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVersion, SystemInfo.graphicsMemorySize);

            writer.Flush();

            Application.logMessageReceived += Application_logMessageReceived;


        }

        private static void Application_logMessageReceived(string condition, string stackTrace, UnityEngine.LogType type)
        {
            writer.WriteLine(condition);
            if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception || type == UnityEngine.LogType.Log)
            {
                writer.WriteLine(stackTrace);
            }
            writer.Flush();
        }


        public static void Debug(string format, params object[] args)
        {
            Loging(LogType.Debug, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            Loging(LogType.Info, format, args);
        }

        public static void Waring(string format, params object[] args)
        {
            Loging(LogType.Warning, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Loging(LogType.Error, format, args);
        }

        public static void Exception(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }

        private static void Loging(LogType type, string format, params object[] args)
        {
            if (type < Level)
                return;

            switch (type)
            {
                case LogType.Warning:
                    {
                        if (args == null || args.Length == 0)
                        {
                            UnityEngine.Debug.LogWarningFormat(format, args);
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning(format);
                        }

                        break;
                    }
                case LogType.Error:
                    {
                        if (args == null || args.Length == 0)
                        {
                            UnityEngine.Debug.LogErrorFormat(format, args);
                        }
                        else
                        {
                            UnityEngine.Debug.LogError(format);
                        }

                        break;
                    }
                default:
                    {
                        string message;
                        if (args == null || args.Length == 0)
                        {
                            message = string.Format("[{0}]{1}", type, format);
                        }
                        else
                        {
                            message = string.Format("[{0}]{1}", type, string.Format(format, args));
                        }
                        UnityEngine.Debug.Log(message);
                        break;
                    }
            }
        }
    }
}
