
using QGame.Core.Event;
using UnityEngine;
namespace QGame.Core.Scene.Scenes
{
    /// <summary>
    /// 启动场景
    /// </summary>
    class StartScene:SceneBase
    {
        public override SceneType sceneType { get { return SceneType.Start; } }

        public StartScene(object arg)
        {
            this.arg = arg;
        }
        public static StartScene Create(object arg)
        {
            return new StartScene(arg);
        }
       private GameObject LoadingPanel;
       public override void CreateView()
       {
           base.CreateView();
       }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
