using UnityEngine;
namespace QGame.Core.Scene.Scenes
{
    /// <summary>
    /// 加载场景
    /// </summary>
    class LoadingScene : SceneBase
    {
        public override SceneType sceneType { get { return SceneType.Loading; } }

        public LoadingScene(object arg)
        {
            this.arg = arg;
        }
        public static LoadingScene Create(object arg)
        {
            return new LoadingScene(arg);
        }
       
        public override void CreateView()
        {
            base.CreateView();

            //GameObject loadingPanel= (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/Login/LoadingPanel"));
            //loadingPanel.transform.SetParent(canvasView.MainLayer.transform, false);
            //LoadingPanel lp = loadingPanel.GetComponent<LoadingPanel>();
            //lp.StartLoad();
        }

    }
}

  
