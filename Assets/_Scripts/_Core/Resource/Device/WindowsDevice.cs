using Assets.Editor;
using QGame.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QGame.Core.Device
{
    /// <summary>
    /// 原生功能Windows平台实现
    /// </summary>
    public class WindowsDevice : DeviceNative
    {
        /// <summary>
        /// //已导入的资源
        /// </summary>
        private Dictionary<string, AssetBundle> assets = new Dictionary<string, AssetBundle>();
       
        
        public WindowsDevice()
        {
            AssetPath ="file://" +  Application.streamingAssetsPath + "/";//
            DataPath = "file://" + Application.streamingAssetsPath + "/";
            ResPath = Application.streamingAssetsPath + "/res/";
            ResPathWWW = "file://" + ResPath;
            //Log.Info("WindowsDevice Create");
        }

        public override string ReadAssetText(string assetName)
        {          
            string path = AssetPath + assetName;
           // return File.ReadAllText(path);
            return FileUtils.getInstance().ReadFile(path);
        }


        public override bool CopyAsset(string assetName, string destPath)
        {
            string path = AssetPath + assetName;
            if (File.Exists(destPath))
                File.Delete(destPath);
            File.Copy(path, destPath);
            return true;            
        }

        public override bool Extract7z(string filePath, string outPath)
        {
            CompressTool ct = new CompressTool();
            ct.zipPath = filePath;
            ct.tempPath = outPath+"res/";
            ct.Extract();
            return true;
        }
        public override void ThreadExtract7z(string filePath, string outPath, ExtractCallback callback)
        {
            CompressTool ct = new CompressTool();
            ct.zipPath = filePath;
            ct.tempPath = outPath ;
            ct.Extract();
            callback(10, 10);
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
        //        assets.Add(md5Str, ab);
        //        return (T)ab.LoadAsset(originalFullPath);//resName != null ? resName : Path.GetFileNameWithoutExtension(path)ab.GetAllAssetNames()[0] Path.GetDirectoryName.GetFileName(apth);
        //    }
        //    else
        //    {
        //        Log.Info("Resources.Load:" + path);
        //        return (T)Resources.Load(parsePrefabPath(path));
        //    }
        //}

        //public override void LoadObjectWWW<T>(string resPath, ResLoadInfo.callback fn = null, object fnPara = null)
        //{
           
        //    string originalFullPath = "Assets/Resources_Bundle/" + resPath;
        //    string parseCommonPath = "Assets/Resources_Bundle/" + base.parseCommonAsset(resPath);
        //    string md5Str = HashUtils.MD5(parseCommonPath) + ".ab";
        //    string assetResPath = ResPath + md5Str;

        //    if (System.IO.File.Exists(assetResPath))
        //    {
        //        string[] dps = mainfest.GetAllDependencies(md5Str);
        //        string[] resPaths = new string[dps.Length + 1];
        //        if (dps != null && dps.Length > 0)
        //        {
        //            for (int i = 0; i < dps.Length; i++)
        //            {                       
        //                 resPaths[i] = ResPath + dps[i];
        //            }
        //        }
        //        resPaths[resPaths.Length - 1] = assetResPath;
        //        ResLoadInfo res = new ResLoadInfo();
        //        res.start(resPaths, fn, fnPara);


        //    }
        //    else
        //    {

        //    }

        //}



    }
}
