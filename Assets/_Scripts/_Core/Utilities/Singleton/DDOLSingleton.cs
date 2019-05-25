using UnityEngine;

public class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{

    protected static T _Instance = null;

    public static T Instance
    {

        get
        {
            if (null == _Instance)
            {

                GameObject go = GameObject.Find("DDOLGameObject");

                if (null == go)
                {

                    go = new GameObject("DDOLGameObject");

                    DontDestroyOnLoad(go);  //创建对象，过场不销毁

                }

                _Instance = go.AddComponent<T>();

            }
            return _Instance;

        }

    }

    //Raises the application quit event.

    private void OnApplicationQuit()
    {

        _Instance = null;

    }
}
