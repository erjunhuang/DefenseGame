using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using QGame.Core.Utils;
using System.IO;
using QGame.Core.Device;
using System;

namespace QGame.Core.Resource
{
    public class ResLoadInfo
    {
        public delegate void callback(ResLoadInfo res, object param);

        //原始列表
        public string[] pathArr;
        private List<callback> fns;
        private List<object> fnParas;
        public bool isCache=false;
        private int fileCount;
        public bool success = false;
        public Dictionary<string, ResLoadNode> nodeDict = new Dictionary<string, ResLoadNode>();
        
        public ResLoadNode content;
        /// <summary>
        /// 最后一个是主包
        /// </summary>
        public string key;
        private List<string> prefabNames;
        public string currentPrefabName;
        //测试路径
        public string relaPath;
        /// <summary>
        /// 在开发环境下的外部传入对象
        /// </summary>
        public UnityEngine.Object goEditor;
        public void start(string prefabName, string[] pathArr, callback fn = null, object fnPara = null, string ss="")
        {
            fns = new List<callback>();
            fnParas = new List<object>();
            prefabNames = new List<string>();
            this.success = false;
           // this.dataRoot = dataRoot;
            this.pathArr = pathArr;
            this.fns.Add(fn);
            this.fnParas.Add(fnPara);
            currentPrefabName = prefabName;
            this.prefabNames.Add(prefabName);
            this.key = pathArr[pathArr.Length - 1];
            this.relaPath = ss;
            this.loadAll();
        }

        public void append(string prefabName,callback fn = null, object fnPara = null)
        {
            this.fns.Add(fn);
            this.fnParas.Add(fnPara);
            this.prefabNames.Add(prefabName);
        }
        //开始下载
        private void loadAll()
        {
            this.fileCount = this.pathArr.Length;
          
            for (int i = 0; i < pathArr.Length; i++)
            {
                ResLoadNode node = null;
                bool isCraete = ResLoadNode.CreateOrGetNode(pathArr[i], out node,key);//引用计数+1
              
                if (!isCraete && node.success)//已经加载完毕的了
                {
                    onNodeLoaded(node);
                }
                else if (!isCraete && !string.IsNullOrEmpty(node.path) && node.fn != null)//正在加载的
                {
                    node.fn += onNodeLoaded;
                }
                else//刚创建出来的
                { 
                    //路径
                    node.path = pathArr[i];
                    nodeDict[pathArr[i]] = node;//node.relaPath
                    node.relaPath = pathArr[i];                    
                    //加载完毕回调函数
                    node.fn = onNodeLoaded;
                    DownLoadManager.instance.addNode(node);
                }
            }
        }
        private void onNodeLoaded(ResLoadNode node)
        {
            if (this == null || this.Equals(null))
            {                
                //Log.Error("onNodeLoaded().this");
                return;
            }
            if (nodeDict == null)
            {
                //Log.Error("onNodeLoaded().nodeDict");
                return;
            }           
            this.fileCount--;
            if (key == node.path)
            {
                content = node;
            }
            if (fileCount <= 0)
            {
                this.success = true;
                for (int i = 0; i < fns.Count;i++ )
                {
                    callback fn = fns[i];    
                    currentPrefabName=prefabNames[i];
                    fn(this, fnParas[i]);
                }
                fns.Clear();
                fnParas.Clear();
                if (!isCache)
                {
                    unload();
                    DeviceFactory.Instance.RemoveDirResLoadInfoByKey(key);
                   // Debug.Log("remove " + key + "      " + currentPrefabName);
                }
                else
                {
                   // Debug.LogError("cache "+key+"      "+currentPrefabName+"PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
                }
             
            }
        }

        public T LoadAsset<T>(string loadPrefabName="") where T : UnityEngine.Object
        {
            if (content!=null)
            {
                try
                {
                    T t = content.wwwRes.assetBundle.LoadAsset<T>(loadPrefabName==""?currentPrefabName:loadPrefabName);              
                    return t;
                }
                catch(Exception e)
                {
                    //Log.Error(loadPrefabName+"加载失败"+e.Message.ToString());
                    return null;
                }             
            }
            else
            {
                if (goEditor!=null)
                    return goEditor as T;
                return null;
            }             
        }

        public T LoadAssetAndInstantiate<T>() where T : UnityEngine.Object
        {
            if (content != null)
            {
                T t = content.wwwRes.assetBundle.LoadAsset<T>(currentPrefabName);               
                return GameObject.Instantiate<T>(t);
            }
            else
            {
                if (goEditor != null)
                    return GameObject.Instantiate<T>(goEditor as T);
                return null;
            }
        }

        //释放assets object
        public void unload(bool unloadUsedObjects = false)
        {
            foreach (ResLoadNode node in this.nodeDict.Values)
            {
                node.Release(key);               
            }
            nodeDict.Clear();
        }
    }
}