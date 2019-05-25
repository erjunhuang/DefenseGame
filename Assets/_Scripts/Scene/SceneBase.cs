using UnityEngine;

namespace QGame.Core.Scene
{
    abstract class SceneBase
    { 
        /// <summary>
        /// 场景类型
        /// </summary>
        public abstract SceneType sceneType { get; }
        public string scenePath = "";
        public string sceneName = "CommomScene";
        public object arg;
        public SceneBase()
        {

        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        public virtual void CreateView()
        {
            SceneManager.realSceneType = SceneManager.currSceneType;
            RenderSettings.fog = false;
        }
      
        public virtual void OnDestroy()
        {
           
        }
    }
}
