// ------------------------------------------------------------------
// Description : 原生功能Editor平台实现
// Author      : sunlu
// Date        : 2015.01.05
// Histories   :
// ------------------------------------------------------------------

using QGame.Core.Utils;
using System.Diagnostics;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using QGame.Utils;
using System.Collections.Generic;
using QGame.Core.Resource;


namespace QGame.Core.Device
{
    /// <summary>
    /// 原生功能Editor平台实现
    /// </summary>
    public class EditorTestAssets : DeviceNative
    {
        private Dictionary<string, AssetBundle> assets = new Dictionary<string, AssetBundle>();  
        public EditorTestAssets()
        {
            AssetPath = Application.streamingAssetsPath + "/";
            //DataPath = Directory.GetCurrentDirectory() + "/../assetbundle_window/";
            //ResPath = Directory.GetCurrentDirectory() + "/../assetbundle_window/res/";//"Assets/Resources_Bundle/";//Directory.GetCurrentDirectory() + 

            DataPath = Application.streamingAssetsPath + "/";
            ResPath = Application.streamingAssetsPath + "/res/";
            ResPathWWW = "file://" + ResPath;
            printPath();
        }

        public override bool Extract7z(string filePath, string outPath)
        {
            Log("EditorNative:Extract7z");
            return true;
        }

        public override void ThreadExtract7z(string filePath, string outPath, ExtractCallback callback)
        {
            callback(10, 10);
        }

        public override string ReadAssetText(string assetName)
        {
            Log("EditorNative:ReadAssetText");
            string path = AssetPath + assetName;
            return FileUtils.getInstance().ReadFile(path);
            //return File.ReadAllText(path);
        }

        public override bool CopyAsset(string assetName, string destPath)
        {
            Log("EditorNative:CopyAsset");

            //string path = AssetPath + assetName;

            //if (File.Exists(destPath))
            //    File.Delete(destPath);

            //File.Copy(path, destPath);
            return true;
        }

        //public override void UnitySendMessage(string arg0, string arg1, string arg2)
        //{
        //    Log("EditorNative:UnitySendMessage");
        //}

        private void Log(string message)
        {
            //DebugEx.Debug(message);
        }
        public override string GetMemInfo()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //Process process = Process.GetCurrentProcess();
                //sb.AppendFormat("{0}/{1}", process.WorkingSet64, process.PrivateMemorySize64);
            }
            catch// (System.Exception ex)
            {
                //Logger.Error("Get MemInfo Error={0}", ex.Message);
            }

            return sb.ToString();
        }


        //public override T LoadObject<T>(string path, bool blUnLoad = false)
        //{
        //    if (mainfest == null)
        //    {
        //       // Log.Info("ResPath:: " + ResPath + "res");
        //        AssetBundle mainab = this.LoadBundle(ResPath + "res");
        //        if (mainab)
        //        {
        //            mainfest = (AssetBundleManifest)mainab.LoadAsset("AssetBundleManifest");
        //        }
        //        else
        //        {
        //          //  log.Error("mainfest null");
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
        //           // Log.Error("加载 " + path + "失败！");
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