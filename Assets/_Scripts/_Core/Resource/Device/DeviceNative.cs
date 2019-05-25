// ------------------------------------------------------------------
// Description : 平台原生功能抽象类
// Author      : sunlu
// Date        : 2015.01.05
// Histories   :
// ------------------------------------------------------------------

using QGame.Core.Resource;
using QGame.Utils;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace QGame.Core.Device
{
    /// <summary>
    /// 平台原生功能抽象类
    /// </summary>
    public abstract class DeviceNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ExtractCallback(int total, int current);       

        public AssetBundleManifest mainfest;
        /// <summary>
        /// 原始路径 APK中资源文件路径(asset)streamingAssetsPath
        /// </summary>
        public string AssetPath { get; protected set; }

        /// <summary>
        /// 目标路径 
        /// </summary>
        public string DataPath { get; protected set; }
        /// <summary>
        /// 目标路径 存放最终解压资源的目录
        /// </summary>
        public string ResPath { get; protected set; }
        /// <summary>
        /// 用于www读取的 目标路径 存放最终解压资源的目录
        /// </summary>
        public string ResPathWWW { get; protected set; }
        public string WebVersionUrl = "http://192.168.1.100/ver.dat";
        public string WebApkUrl = "http://192.168.1.100/cygame157.apk";
        /// <summary>
        /// 解压7zip文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outPath"></param>
        /// <returns></returns>
        public abstract bool Extract7z(string filePath, string outPath);

        /// <summary>
        /// 解压7zip文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="outPath"></param>
        /// <returns></returns>
        public abstract void ThreadExtract7z(string filePath, string outPath, ExtractCallback callback);

        /// <summary>
        /// 读原始assets中文本内容
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public abstract string ReadAssetText(string assetName);

        /// <summary>
        /// 复制assets目录下资源到指定目录
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="destPath"></param>
        /// <returns></returns>
        public abstract bool CopyAsset(string assetName, string destPath);

        /// <summary>
        /// UintyPlayer发送消息，用来测试
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
       // public abstract void UnitySendMessage(string arg0, string arg1, string arg2);

        public virtual void Install(string filePath)
        {

        }

        public virtual string GetMemInfo()
        {
            return string.Empty;
        }
       // public abstract string getFullResFileName(string pathName);
        //public abstract T LoadObject<T>(string path,bool blUnLoad=false) where T : UnityEngine.Object;//,string resName=null
        public Dictionary<string, ResLoadInfo> directoryResLoadInfo = new Dictionary<string, ResLoadInfo>();
        public void RemoveDirResLoadInfoByKey(string key)
        {
            directoryResLoadInfo.Remove(key);
        }
        public virtual void LoadObjectWWW(string resPath, ResLoadInfo.callback fn = null, object fnPara = null)
        {
            string originalFullPath = "Assets/Resources_Bundle/" + resPath;
            string parseCommonPath = "Assets/Resources_Bundle/" + parseCommonAsset(resPath);
            string md5Str = HashUtils.MD5(parseCommonPath) + ".ab";
            string assetResPath = ResPath +  md5Str;
            if (directoryResLoadInfo.ContainsKey(ResPathWWW + md5Str))
            {
               ResLoadInfo rli = directoryResLoadInfo[ResPathWWW + md5Str];
               if (rli.success)
               {
                   rli.currentPrefabName = Path.GetFileNameWithoutExtension(resPath);
                   fn(rli, fnPara);

               }
               else
                   rli.append(Path.GetFileNameWithoutExtension(resPath),fn, fnPara );
                return;
            }
            if (true)//System.IO.File.Exists(assetResPath)
            {
               // Log.Info("LoadObjectWWW :" + Path.GetFileNameWithoutExtension(resPath)+ " " + assetResPath);
                string[] dps = mainfest.GetAllDependencies(md5Str);

                string[] resPaths = new string[dps.Length + 1];
                if (dps != null && dps.Length > 0)
                {
                    for (int i = 0; i < dps.Length; i++)
                    {
                        resPaths[i] = ResPathWWW + dps[i];
                    }
                }
                resPaths[resPaths.Length - 1] = ResPathWWW + md5Str;// assetResPath;
                ResLoadInfo res = new ResLoadInfo();
                res.isCache = parseCommonAssetCache(resPath);
                directoryResLoadInfo.Add(ResPathWWW + md5Str, res);
                res.start(Path.GetFileNameWithoutExtension(resPath), resPaths, fn, fnPara, resPath);
            }
            else
            {
                //Log.Error("LoadObjectWWW: " + parseCommonPath + "  " + md5Str + " not find file!");
                ResLoadInfo res = new ResLoadInfo();
                fn(res,null);
            }
        }
        
        public AssetBundle LoadBundle(string path)
        {
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            return ab;
        }

        /// <summary>
        /// 加载依赖文件(暂时借用回调)
        /// </summary>
        public virtual void buildMainFest(ExtractCallback ecb)
        {
            //#if UNITY_EDITOR           
            //ecb(1, 1);
            //#else            
            ResLoadInfo resLoadInfo = new ResLoadInfo();
            string[] ss = new string[1];
            ss[0]=ResPathWWW + "res";
            Debug.Log("Extract" + ss[0]);
            resLoadInfo.start("AssetBundleManifest",ss,(ResLoadInfo res, object param) =>
            {
                mainfest =res.LoadAsset<AssetBundleManifest>();
                //Log.Info(mainfest == null ? "mainfest加载失败" : "mainfest加载成功");
                ecb(1, 1);
            });
        }
         


        public void printPath()
        {
            string ap = Application.streamingAssetsPath;
            string pdp = Application.persistentDataPath;
            string cd = Directory.GetCurrentDirectory();
            string dp = Application.dataPath;
            string tcp = Application.temporaryCachePath;
            UnityEngine.Debug.LogFormat(" Application.streamingAssetsPath:{0}\r\n Application.persistentDataPath:{1}\r\n  Directory.GetCurrentDirectory:{2} \r\n Application.dataPath:{3} \r\n  Application.temporaryCachePath:{4}", ap, pdp, cd, dp, tcp);
        }
        //string[] commonPaths = new string[] { "UIResources/Common/Others/", "UIResources/Common/Flag/", "UIResources/Common/Icon/", "UIResources/Common/Pic/", "UIResources/Common/Quality/", "UIResources/Common/Quality/", "UIResources/Common/" };

        public bool parseCommonAssetCache(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (dir.StartsWith("UIResources/Common"))
                return true;
            //if (dir.StartsWith("Music"))
            //    return true;

                return false;
        }
        /// <summary>
        /// 解析公共资源,某些公共资源打包在一起了
        /// </summary>
        public string parseCommonAsset(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (dir.StartsWith("UIResources/Common"))
                return dir;
            return path;

        }

        /// <summary>
        /// 去掉.prefab后缀
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        public string parsePrefabPath(string strPath)
        {
            string ext=Path.GetExtension(strPath).ToLower();
            if (ext==".prefab")
            {
                return strPath.Substring(0, strPath.Length - 7);
            }
            return strPath;
        }


        //public virtual void UnLoadAllBundle()
        //{           
        //}

        public virtual void UnLoadAllBundleFalse()
        {
            //foreach (ResLoadInfo rli in directoryResLoadInfo.Values)
            //{
            //    if (rli.success)
            //    {
            //        rli.unload();
            //    }
                 
            //}
            //int icount=directoryResLoadInfo.Values.Count;
            //for (int i = icount; i >= 0;i-- )
            //{
            //    directoryResLoadInfo.Clear();
            //}        

            //Debug.Log(" directoryResLoadInfo:: " + DeviceFactory.Instance.directoryResLoadInfo.Count);
            //foreach (ResLoadInfo rli in DeviceFactory.Instance.directoryResLoadInfo.Values)
            //{
            //    Debug.Log(rli.key + " :: " + rli.currentPrefabName);
            //}
            //ResLoadNodeMgr rln = SingletonHolder<ResLoadNodeMgr>.getInstance();
            //Debug.Log("resLoadNodes" + " :: " + rln._resLoadNodes.Count);
            //foreach (ResLoadNode rli in rln._resLoadNodes.Values)
            //{
            //    Debug.Log(rli.path + " :: " + rli._refCount);
            //} 
        }

        public virtual void UnLoadBundle(string md5str)
        {
        }

    }
}