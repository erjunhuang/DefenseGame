namespace QGame.Core.Scene.Scenes
{
    /// <summary>
    /// 主城
    /// </summary>
    class CityScene:SceneBase
    {
        public override SceneType sceneType { get { return SceneType.City; } }
        public CityScene(object arg)
        {
            this.arg = arg;
            scenePath = "Scenes/Art_Assets/zhucheng/Scene/zhucheng.unity";
            sceneName = "zhucheng";
        }
        ///// <summary>
        ///// 从战场回来先不显示主城，提高响应速度
        ///// </summary>
        //private bool blFromFight=false;
        public static SceneBase Create(object arg)
        {
            return new CityScene(arg);
        }

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
