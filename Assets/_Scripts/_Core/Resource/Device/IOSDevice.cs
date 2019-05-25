
using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using QGame.Utils;
using System.IO;
using QGame.Core.Utils;
using QGame.Core.Resource;


namespace QGame.Core.Device
{
    /// <summary>
    /// 原生功能IOS平台实现
    /// </summary>
    public class IOSDevice : DeviceNative
    {
        [DllImport("qgame", EntryPoint = "extract", CallingConvention = CallingConvention.Cdecl)]
        extern static int Extract(string filePath, string outPath, IntPtr callback);
        private Dictionary<string, AssetBundle> assets = new Dictionary<string, AssetBundle>();
        public IOSDevice()
        {
            //Log.Waring("IOSDevice init");
            AssetPath = Application.streamingAssetsPath + "/";
            DataPath = Application.persistentDataPath + "/";
            ResPath = Application.persistentDataPath + "/res/";
            //printPath();
        }

        public override bool Extract7z(string filePath, string outPath)
        {
            try
            {
                return Extract(filePath, outPath, IntPtr.Zero) == 0;
            }
            catch (Exception e)
            {
                //Log.Error("Extract7z Error=" + e.Message);
            }
            return false;
        }

        private static T Call<T>(string apiName, params object[] args)
        {
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass player = new AndroidJavaClass("com.chuangyue.core.DeviceUtils"))
                {
                    return player.CallStatic<T>(apiName, args);
                }
            }
            catch (Exception ex)
            {
                Log.Error("{0} Error={1}", apiName, ex.Message);
            }
#endif
            return default(T);
        }

        private static void Call(string apiName, params object[] args)
        {
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass player = new AndroidJavaClass("com.chuangyue.core.DeviceUtils"))
                {
                    player.CallStatic(apiName, args);
                }
            }
            catch (Exception ex)
            {
                Log.Error("{0} Error={1}", apiName, ex.Message);
            }
#endif
        }




        public override void ThreadExtract7z(string filePath, string outPath, ExtractCallback callback)
        {
            var thread = new Thread(new ThreadStart(delegate()
            {
                IntPtr ptr = Marshal.GetFunctionPointerForDelegate(callback);
                int ret = Extract(filePath, outPath, ptr);

                if (ret != 0)
                    callback(-1, 0);
            }));

            thread.Start();
        }

        public override string ReadAssetText(string assetName)
        {
            return Call<string>("readAssetText", assetName);
        }

        public override bool CopyAsset(string assetName, string destPath)
        {
            return Call<bool>("copyAsset", assetName, destPath);
        }


        public override void Install(string filePath)
        {
            //  Call("installApk", filePath);
        }

        public override string GetMemInfo()
        {
            StringBuilder sb = new StringBuilder();

#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass debug = new AndroidJavaClass("android.os.Debug"))
                {
                    sb.AppendFormat("{0}/{1}M", debug.CallStatic<long>("getNativeHeapAllocatedSize") >> 20, debug.CallStatic<long>("getNativeHeapSize") >> 20);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Get MemInfo Error={0}", ex.Message);
            }
#endif

            return sb.ToString();
        }


        //public override T LoadObject<T>(string path, bool blUnLoad = false)
        //{
        //    if (mainfest == null)
        //    {
        //        Log.Info("ResPath:: " + ResPath + "res");
        //        AssetBundle mainab = this.LoadBundle(ResPath + "res");
        //        if (mainab)
        //        {
        //            mainfest = (AssetBundleManifest)mainab.LoadAsset("AssetBundleManifest");
        //        }
        //        else
        //        {
        //            Log.Error("mainfest null");
        //        }
        //    }
        //    string originalFullPath = "Assets/Resources_Bundle/" + path;
        //    string parseCommonPath = "Assets/Resources_Bundle/" + base.parseCommonAsset(path);
        //    string md5Str = HashUtils.MD5(parseCommonPath) + ".ab";
        //    string assetResPath = ResPath + md5Str;

        //    if (assets.ContainsKey(md5Str))
        //    {
        //        return (T)assets[md5Str].LoadAsset(originalFullPath);
        //    }

        //    if (System.IO.File.Exists(assetResPath))
        //    {

        //        string[] dps = mainfest.GetAllDependencies(md5Str);
        //        if (dps != null && dps.Length > 0)
        //        {
        //            //AssetBundle[] abs = new AssetBundle[dps.Length];
        //            for (int i = 0; i < dps.Length; i++)
        //            {
        //                if (!assets.ContainsKey(dps[i]))
        //                {
        //                    AssetBundle _ab = LoadBundle(ResPath + dps[i]);
        //                    assets.Add(dps[i], _ab);
        //                }
        //            }
        //        }
        //        AssetBundle ab = LoadBundle(assetResPath);
        //        if (ab != null)
        //        {
        //            assets.Add(md5Str, ab);
        //            return (T)ab.LoadAsset(originalFullPath);
        //        }
        //        else
        //        {
        //            Log.Error("加载 " + path + "失败！");
        //            return null;
        //        }

        //    }
        //    else
        //    {
        //        return (T)Resources.Load(parsePrefabPath(path));
        //    }
        //}

     

    }
}