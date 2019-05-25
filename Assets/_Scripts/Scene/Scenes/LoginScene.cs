namespace QGame.Core.Scene.Scenes
{
    /// <summary>
    /// 登录场景
    /// </summary>
    class LoginScene : SceneBase
    {
        public override SceneType sceneType { get { return SceneType.Login; } }

        public LoginScene(object arg)
        {
            this.arg = arg;
        }
        public static LoginScene Create(object arg)
        {          
            return new LoginScene(arg);
        }
      
        public override void CreateView()
        {
            base.CreateView();
        }

    }
}
