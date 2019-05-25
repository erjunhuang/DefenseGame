using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QGame.Core.Resource
{

    public class DownLoadManager : MonoBehaviour
    {
        // 载入最大并行链接
        public static int maxThread = 10;
        //是否开启模拟网络延迟
        public static bool netEmulation = false;
        //模拟的下载速度，单位为KB
        public static int netSpeed = 24;
        //是否开启日志
        public static bool showDebug = false;
        public static DownLoadManager instance = null;
        /// <summary>
        /// 加载错误列表
        /// </summary>
       // public static List<string> ErrorList = new List<string>();
        /// <summary>
        /// 完成加载的路径
        /// </summary>
        public static string loadingOver;
       

        // 当前队列
        private List<ResLoadNode> actArr = new List<ResLoadNode>();
        // 全部队列
        private List<ResLoadNode> nodeArr = new List<ResLoadNode>();
        
        //添加一个下载节点
        public void addNode(ResLoadNode node)
        {           
            if (actArr.Count < maxThread)
            {
                actArr.Add(node);
                execOne(node);
            }
            else
            {
                nodeArr.Add(node);
            }
        }
        //检查是否可以继续下载
        public void checkQueue()
        {
            if (nodeArr.Count > 0 && actArr.Count < maxThread)
            {
                ResLoadNode node = nodeArr[0];
                nodeArr.RemoveAt(0);
                actArr.Add(node);
                execOne(node);
            }
            if (showDebug && nodeArr.Count == 0 && actArr.Count == 0)
            {
                //Log.Info("*** All File Loaded ***", "DownloadManager");
            }
        }
        //开始下载一个
        private void execOne(ResLoadNode node)
        {
            StartCoroutine(downLoad(node));
        }
        //
        public IEnumerator downLoad(ResLoadNode node)
        {
            while (!Caching.ready)
            {
                yield return null;
            }

            if (showDebug)
            {
                //Log.Info("load :  " + node.path, "DownloadManager");
            }
            //如果是ab，则使用cache模式

            //老的加载方式
//            if (node.isABFile && GlobalData.systemSet.isCacheResource)
//            {
//#if UNITY_EDITOR
//                node.wwwRes = new WWW(node.path);
//#else
//            node.wwwRes = WWW.LoadFromCacheOrDownload(node.path, BuildVer.fileVer);
//#endif
//            }
//            else
           // int verNum = GameApp.GetVersionManager().GetVersionNum(assetBundleName);
           // if (!Caching.IsVersionCached(node.path,0))
            {
                node.wwwRes = new WWW(node.path);
                while (!node.wwwRes.isDone)
                {
                    // Log.Info("progress " + node.path + " : " + node.wwwRes.progress);
                    yield return null;
                }
                //报错
                if (!string.IsNullOrEmpty(node.wwwRes.error))
                {
                    //if (node.path.Contains("/PlatformAssets/"))
                    //    ErrorList.Add(node.path.Split(new string[] { "/PlatformAssets/" }, System.StringSplitOptions.None)[1]);
                    //else
                    //    if (node.path.Contains("/platformAssets/"))
                    //        ErrorList.Add(node.path.Split(new string[] { "/platformAssets/" }, System.StringSplitOptions.None)[1]);

                   // Log.Error("load error " + node.wwwRes.error);
                }
                else
                {
                    node.success = true;
                    //统一ab接口 
                    if (node.isABFile)
                    {                      
                        node.ab = node.wwwRes.assetBundle;
                       // Log.Info("down ::::::" + node.relaPath+" :: "+node.path);
                    }
                }
                onFinish(node);
            }
            //else
            //{
            //    Log.Info("IsVersionCached ::::::" + node.path);
            //    onFinish(node);
            //}
           
        }
        //下载完成
        private void onFinish(ResLoadNode node)
        {
            if (node == null || this == null || actArr == null)
            {
                return;
            }
            //loadingOver = node.relaPath;
            actArr.Remove(node);
            this.checkQueue();
            if (node.fn != null)
            {
                node.fn(node);
            }
        }
    }
}