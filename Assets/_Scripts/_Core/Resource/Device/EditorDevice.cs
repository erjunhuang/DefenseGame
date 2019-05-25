// ------------------------------------------------------------------
// Description : 原生功能Editor平台实现
// Author      : sunlu
// Date        : 2015.01.05
// Histories   :
// ------------------------------------------------------------------

using QGame.Core.Utils;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using QGame.Utils;
using QGame.Core.Resource;
using System.Threading;
using System.Runtime.InteropServices;


namespace QGame.Core.Device
{
    /// <summary>
    /// 原生功能Editor平台实现
    /// </summary>
    public class EditorDevice : DeviceNative
    {
      // string AssetPathwww;
        public EditorDevice()
        {
            AssetPath =  Application.streamingAssetsPath + "/";
            DataPath = "file://" + Directory.GetCurrentDirectory() + "/Assets/Data/";
            DataPath = DataPath.Replace("\\", "/");
            ResPath = "Assets/Resources_Bundle/";
            ResPathWWW = "file://" + Application.streamingAssetsPath + "/res/";
           // printPath();
           // string path = "D:/unity_work/qgame/dev/client/cygame/Assets/Data/config/HeroCfg.dat";//DataPath+"/config/CorpsWishCfg.dat";

           // WWW www = new WWW(path);
           //// Log.Info(path);
           // while (!www.isDone)
           // {

           // }
           // if (string.IsNullOrEmpty(www.error))
           // {
           //     Debug.Log("niceeeeee");
           // }
           // else
           // {
           //     Debug.Log(www.error);
           // }
        }

        public override bool Extract7z(string filePath, string outPath)
        {
            Log("EditorNative:Extract7z");
            return true;
        }

        public override void ThreadExtract7z(string filePath, string outPath, ExtractCallback callback)
        {
            callback(10,10);
        
        }

        public override string ReadAssetText(string assetName)
        {
            Log("EditorNative:ReadAssetText");
            string path = AssetPath + assetName;
            //FileStream fs= File.OpenRead(path); 
            StringBuilder sb=new StringBuilder();
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[fs.Length];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    sb.Append(temp.GetString(b));
                }
            }
            return sb.ToString();
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
        public override void buildMainFest(ExtractCallback ecb)
        {
            ecb(1, 1);
        }

        private void Log(string message)
        {
            //DebugEx.Debug(message);
        }
        public override string GetMemInfo()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                
               // Process process = Process.GetCurrentProcess();

             //   sb.AppendFormat("{0}/{1}", process.WorkingSet64,process.PrivateMemorySize64);
               // sb.AppendFormat("使用={0},", process.WorkingSet64 );
               // sb.AppendFormat("剩余={0}", process.VirtualMemorySize64 );
            }
            catch// (System.Exception ex)
            {
                //Logger.Error("Get MemInfo Error={0}", ex.Message);
            }

            return sb.ToString();
        }


//        public override T LoadObject<T>(string path, bool blUnLoad = false)
//        {
//            string resPath = ResPath + path;
//            string r = Directory.GetCurrentDirectory() + "/" + resPath;
//            if (System.IO.File.Exists(r))
//            {
//#if UNITY_EDITOR
//                return AssetDatabase.LoadAssetAtPath<T>(resPath);
//#else
//                 return (T)Resources.Load(parsePrefabPath(path));
//#endif

//            }
//            else
//            {
//                return (T)Resources.Load(parsePrefabPath(path));
//            }

//        }


        public override void LoadObjectWWW(string resPath, ResLoadInfo.callback fn = null, object fnPara = null)
        {
            string path = ResPath + resPath;
            string r = Directory.GetCurrentDirectory() + "/" + path;
            if (System.IO.File.Exists(r))
            {
                #if UNITY_EDITOR
                ResLoadInfo rli=new ResLoadInfo();
                rli.goEditor = AssetDatabase.LoadAssetAtPath<Object>(path);
                rli.success = true;
                fn(rli, fnPara);
#endif
                return;

            }
            else
            {
               // Debug.Log("LoadObjectWWW no file: " + resPath);
                ResLoadInfo rli = new ResLoadInfo();
                rli.goEditor = Resources.Load(parsePrefabPath(path));
                rli.success = false;
                fn(rli, fnPara);
             
            }

            //string originalFullPath = "Assets/Resources_Bundle/" + resPath;
            //string parseCommonPath = "Assets/Resources_Bundle/" + parseCommonAsset(resPath);
            //string md5Str = HashUtils.MD5(parseCommonPath) + ".ab";
            //string assetResPath = Application.streamingAssetsPath + "/res/" + md5Str;

            //if (System.IO.File.Exists(assetResPath))
            //{
            //    string[] dps = mainfest.GetAllDependencies(md5Str);

            //    string[] resPaths = new string[dps.Length + 1];
            //    if (dps != null && dps.Length > 0)
            //    {
            //        for (int i = 0; i < dps.Length; i++)
            //        {
            //            resPaths[i] = ResPathWWW + dps[i];
            //        }
            //    }
            //    resPaths[resPaths.Length - 1] = ResPathWWW + md5Str;// assetResPath;
            //    ResLoadInfo res = new ResLoadInfo();
            //    res.start(Path.GetFileNameWithoutExtension(resPath), resPaths, fn, fnPara);
            //}
            //else
            //{
            //}
        }



    }
}