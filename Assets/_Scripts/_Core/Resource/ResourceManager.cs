using System;
using System.Collections.Generic;
using QGame.Core.Device;
using QGame.Core.Utils;
using QGame.Utils;
using System.IO;
using UnityEngine;


namespace QGame.Core.Resource
{
    class ResourceManager
    {
        public static string GetMd5FileName(string file)
        {
            string path = "";
            file = GetFileName(file);
            int index = file.IndexOf("/", System.StringComparison.Ordinal);
            if (index > 0)
                path = file.Substring(0, index + 1);

            return path + HashUtils.MD5(file);
        }

        private static string GetFileName(string path)
        {
            int index = path.LastIndexOf(".", System.StringComparison.Ordinal);
            return index >= 0 ? path.Remove(index) : path;
        }

        //public static T LoadObject<T>(string resPath) where T : Object
        //{
        //    if (Path.GetExtension(resPath) == "")
        //    {
        //        resPath += ".prefab";
        //    }

        //    bool blUnLoad = false;
        //    T t = DeviceFactory.Instance.LoadObject<T>(resPath, blUnLoad);
        //    return t;
        //}

        public static void LoadObjectByWWW(string resPath,ResLoadInfo.callback fn = null, object fnPara = null) 
        {
            if (Path.GetExtension(resPath) == "")
            {
                resPath += ".prefab";
            }          
            DeviceFactory.Instance.LoadObjectWWW(resPath, fn,fnPara);
        }

        public static Dictionary<string,GameObject> SaveDictionary = new Dictionary<string, GameObject>();
        public static void LoadObjectByWWW(string resPath, Action<GameObject> ac, bool isSave = true)
        {
            if (Path.GetExtension(resPath) == "")
            {
                resPath += ".prefab";
            }

            if (SaveDictionary.ContainsKey(resPath))
            {
                GameObject go = SaveDictionary[resPath];
                ac(go);
            }
            else
            {
                DeviceFactory.Instance.LoadObjectWWW(resPath, delegate(ResLoadInfo res, object param)
                {
                    GameObject go = res.LoadAsset<GameObject>();
                    ac(go);

                    if (isSave && !(SaveDictionary.ContainsKey(resPath)))
                    {
                        SaveDictionary.Add(resPath,go);
                    }
                });
            }
        }

        public static Dictionary<string, AudioClip> SaveClip = new Dictionary<string, AudioClip>();
        public static void LoadObjectByWWW(string resPath, Action<AudioClip> ac, bool isSave = true)
        {
            if (Path.GetExtension(resPath) == "")
            {
                resPath += ".prefab";
            }

            if (SaveClip.ContainsKey(resPath))
            {
                AudioClip go = SaveClip[resPath];
                ac(go);
            }
            else
            {
                DeviceFactory.Instance.LoadObjectWWW(resPath, delegate(ResLoadInfo res, object param)
                {
                    AudioClip go = res.LoadAsset<AudioClip>();
                    ac(go);

                    if (isSave)
                    {
                        SaveClip.Add(resPath, go);
                    }
                });
            }
        }

        ///// <summary>
        ///// 直接实例化要加载的对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="resPath"></param>
        ///// <returns></returns>
        //public static T InstantiateGameObject<T>(string resPath) where T : Object
        //{
        //    T t = LoadObject<T>(resPath);
        //    if (t != null)
        //        return GameObject.Instantiate(t);
        //    else
        //    {
        //        Debug.LogError(resPath + " is null");
        //        return null;
        //    }   
        //}
       
        public static void UnloadRes()
        {
            DeviceFactory.Instance.UnLoadAllBundleFalse();
            Resources.UnloadUnusedAssets();
        }
    }
}
