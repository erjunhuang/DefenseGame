using QGame.Core.Scene.Scenes;
using UnityEngine;
using QGame.Core.Event;
using QGame.Core.Utils;
using QGame.Core.Resource;

namespace QGame.Core.Scene
{
    class SceneManager : MonoBehaviour
    {
       // private static Dictionary<SceneType, Func<object, SceneBase>> _sceneCreators;
       // private static SMSceneManager smsceneManager; 
        /// <summary>
        /// 当前激活的场景
        /// </summary>
        public static SceneBase currScene;

        public static SceneType currSceneType;
        //场景切换的时候 还没有真正的切换到场景 还在过度场景 判断失效所以加了这个
        public static SceneType realSceneType;

       // public static LoadingScene loadingScene;
        // private static SceneBase _nextScene;
        /// <summary>
        /// Fade 1渐  2Blinds 百页窗 5Ninja  6Cinema电影院  7Pixelate像素闪烁  8Tetris掉片片  9Tiles翻片片
        /// </summary>
        //private static string[] transitionName = new string[] { "Plain", "Fade", "Blinds", "Newspaper", "Cartoon", "Ninja", "Cinema", "Pixelate", "Tetris", "Tiles" };
        //private static string[] transitionPrefab = new string[] {"Transitions/SMLoadingTransition", "Transitions/SMFadeTransition", 
        //"Transitions/SMBlindsTransition", "Transitions/SMNewspaperTransition", "Transitions/SMCartoonTransition", 
        //"Transitions/SMNinjaTransition", "Transitions/SMCinemaTransition", "Transitions/SMPixelateTransition", 
        //"Transitions/SMTetrisTransition", "Transitions/SMTilesTransition"};

        void init()
        {
            //smsceneManager = new SMSceneManager(SMSceneConfigurationLoader.LoadActiveConfiguration("SceneConfig"));
            //smsceneManager.LevelProgress = new SMLevelProgress(smsceneManager.ConfigurationName);
           
            //StartScene ss = StartScene.Create(null);           
            //_sceneCreators = new Dictionary<SceneType, Func<object, SceneBase>>();
            //_sceneCreators[SceneType.Login] = LoginScene.Create;
            ////_sceneCreators[SceneType.Loading] = LoadingScene.Create;	
            //_sceneCreators[SceneType.City] = CityScene.Create;					
            //_sceneCreators[SceneType.Fight] = FightScene.Create;
            //_sceneCreators[SceneType.House] = HouseScene.Create;
            //_sceneCreators[SceneType.Fram] = FramScene.Create;
            //_sceneCreators[SceneType.Map] = MapScene.Create;
            //_sceneCreators[SceneType.TeamFight] = TeamFightScene.Create;
            //_sceneCreators[SceneType.War] = WarScene.Create;

           // loadingScene= LoadingScene.Create(null);	
            currScene=StartScene.Create(null);
            currScene.CreateView();
        }

        private static SceneBase getSceneBaseByType(SceneType type, object arg = null)
        {
            switch (type)
            {
                case SceneType.City:
                    return CityScene.Create(arg);
                case SceneType.Login:
                    return LoginScene.Create(arg);                  
               
            }
            Debug.LogError("getSceneBaseByType is null " + type.ToString());
            return null;
        }


        void Awake()
        {
            init();     
        }


        public static void EnterScene(SceneType type, object arg = null)
        {
            currSceneType = type;
            SceneBase scene = getSceneBaseByType(type,arg);//_sceneCreators[currSceneType](arg);
            //Resources.UnloadUnusedAssets();
            if (scene == null) return;
            if (currScene != null)
            {
                //XEventBus.Instance.Post(EventId.SceneExit, new XEventArgs(currScene.sceneType));                 
                currScene.OnDestroy();
                Timer.CancelAll();
                currScene = null;                
            }
            currScene = scene;         
           
            Timer.CancelAll();
            ResourceManager.UnloadRes();
            #if UNITY_EDITOR
            #else
                    //if (scene.scenePath.Length>0)
                    //    ResourceManager.LoadObject<GameObject>(scene.scenePath);               
            #endif
            Application.LoadLevel("Loading");
        }
    }
}
