using UnityEngine;

namespace QGame.Core.Utils
{
    public class GameObjectUtils
    {
        public static Transform GetTransform(string path)
        {
            Transform returnObject = GameObject.Find(path).transform;
            if (returnObject == null)
            {
                Log.Error("{0}对象获取失败!", path);
            }
            return returnObject;
        }

        public static Transform GetTransform(string path, GameObject gameObj)
        {
            if (gameObj == null)
            {
                Log.Error("加载{0}对象时，指定父对象为空!", path);
                return null;
            }

            return GetTransform(path, gameObj.transform);
        }
        public static Transform GetTransform(string path, Transform transform)
        {
            if (transform == null)
            {
                Log.Error("加载{0}对象时，指定父对象为空!", path);
                return null;
            }
            Transform returnObject = transform.Find(path);
            if (returnObject == null)
            {
                Log.Error("{0}对象获取子对象失败!", transform.name);
            }
            return returnObject;
        }

        public static T GetComponent<T>(GameObject gameObj, string name) where T : Component
        {
            if (gameObj == null)
            {
                Log.Error("加载{0}对象时，指定父对象为空!", name);
                return null;
            }

            return GetComponent<T>(gameObj.transform, name);
        }

        public static T GetComponent<T>(Transform transform, string name) where T : Component
        {
            if (transform == null)
            {
                Log.Error("加载{0}对象时，指定父对象为空!", name);
                return null;
            }

            var obj = transform.Find(name);

            if (obj == null)
            {
                Log.Error("{0}对象获取子对象失败!", transform.name);
                return null;
            }

            return obj.GetComponent<T>();
        }


        public static T AddComponent<T>(Transform transform) where T : Component
        {
            if (transform == null)
            {
                Log.Error("用于添加组件的对象为空，不能添加组件！");
                return null;
            }
            T component = transform.gameObject.AddComponent<T>();
            if (component == null)
            {
                Log.Error("{0}对象添加组件失败!", transform.name);
            }
            return component;
        }

        public static void setChildLayer(GameObject go, int iLayer)
        {
            go.layer = iLayer;
            int i = 0;
            while (i < go.transform.childCount)
            {
                Transform child = go.transform.GetChild(i);

                if (child.childCount > 0)
                    setChildLayer(child.gameObject, iLayer);
                else
                    child.gameObject.layer = iLayer;
                i++;
            }

        }
    }
}
