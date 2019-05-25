using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;

namespace QGame.Core.Resource
{

    public enum NodeCacheType : int
    {
        NO_CACHE = 0,
        NO_FREE
    }
    public class ResLoadNodeMgr:ClassSingleton<ResLoadNodeMgr>
    {
        public Dictionary<string, ResLoadNode> _resLoadNodes = new Dictionary<string, ResLoadNode>();


        public ResLoadNode GetNode(string path)
        {
            ResLoadNode node = null;
            _resLoadNodes.TryGetValue(path, out node);
            return node;
        }

        public void RemoveNode(string path)
        {
            if (_resLoadNodes.ContainsKey(path))
            {
                _resLoadNodes.Remove(path);
            }            
        }

        public void AddNode(string path, ResLoadNode node)
        {
            if (!_resLoadNodes.ContainsKey(path))
            {
                _resLoadNodes.Add(path, node);
            }
        }

    }


    public class ResLoadNode
    {

        private static ResLoadNodeMgr GetMgr()
        {
            return ResLoadNodeMgr.Instance;
        }
        public static bool CreateOrGetNode(string path, out ResLoadNode node,string key)
        {

            node = GetMgr().GetNode(path);
            if (node == null)
            {
                GetMgr().RemoveNode(path);
                node = new ResLoadNode();
                GetMgr().AddNode(path, node);
                //Debug.LogError(key + " create node retain " + path);
                return true;
            }
            else
            {
                //Debug.LogError(key + " add node retain " + path);
                node.Retain();
            }
            return false;
        }

        //下载路径
        public string path;
        //测试路径
        public string relaPath;
        public bool success = false;
        //
        public loadCallBack fn;
        public delegate void loadCallBack(ResLoadNode node);

        //public WWW www;
        public bool isABFile = true;
        //只对ab有效
        public AssetBundle ab;
        //对其他类型文件有效
        public WWW wwwRes;

        public int _refCount = 1;

        //引用计数
        public void Retain()
        {
            _refCount++;
        }

        public void Release(string key)
        {
            _refCount--;
            //Debug.LogError(key + " remove node retain " + path);
            if (_refCount == 0)
            {
                if (wwwRes != null && wwwRes.error == null)
                {             
                    wwwRes.Dispose();
                }
                if (this.ab != null)      //释放节点
                {
                    ab.Unload(false);
                   // this.ab = null;
                    if (ab!=null)
                    {
                        Debug.LogError(this.relaPath+" NOOOOOOOOOOOOO");
                    }
                }

                GetMgr().RemoveNode(this.path);
//#if UNITY_EDITOR
               // DebugLog.Logf("unload:{0}", this.path);
//#endif
            }
        }
    }
}
