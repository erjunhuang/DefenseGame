using Core.Data;
using Core.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Game
{
    /// <summary>
    /// Game Manager - a persistent single that handles persistence, and level lists, etc.
    /// This should be initialized when the game starts.
    /// </summary>
    public class GameManager : GameManagerBase<GameManager, GameDataStore>
    {
 
        /// <summary>
        /// Scriptable object for list of levels
        /// </summary>
        public LevelList levelList;

        public static float RealDeltaTime;
        /// <summary>
        /// Set sleep timeout to never sleep
        /// </summary>
        protected override void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            base.Awake();

            //测试
            GetPlayerInfo();
            RealDeltaTime = Time.deltaTime;
        }

        private void Update()
        {
            RealDeltaTime = Time.deltaTime;
        }
        /// <summary>
        /// Method used for completing the level
        /// </summary>
        /// <param name="levelId">The levelId to mark as complete</param>
        /// <param name="starsEarned"></param>
        public void CompleteLevel(string levelId, int starsEarned)
        {
            if (!levelList.ContainsKey(levelId))
            {
                Debug.LogWarningFormat("[GAME] Cannot complete level with id = {0}. Not in level list", levelId);
                return;
            }

            m_DataStore.CompleteLevel(levelId, starsEarned);
            SaveData();
        }

        /// <summary>
        /// Gets the id for the current level
        /// </summary>
        public LevelItem GetLevelForCurrentScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            return levelList.GetLevelByScene(sceneName);
        }

        /// <summary>
        /// Determines if a specific level is completed
        /// </summary>
        /// <param name="levelId">The level ID to check</param>
        /// <returns>true if the level is completed</returns>
        public bool IsLevelCompleted(string levelId)
        {
            if (!levelList.ContainsKey(levelId))
            {
                Debug.LogWarningFormat("[GAME] Cannot check if level with id = {0} is completed. Not in level list", levelId);
                return false;
            }

            return m_DataStore.IsLevelCompleted(levelId);
        }

        /// <summary>
        /// Gets the stars earned on a given level
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public int GetStarsForLevel(string levelId)
        {
            if (!levelList.ContainsKey(levelId))
            {
                Debug.LogWarningFormat("[GAME] Cannot check if level with id = {0} is completed. Not in level list", levelId);
                return 0;
            }

            return m_DataStore.GetNumberOfStarForLevel(levelId);
        }


        public  void SetPlayerInfo(PlayInfoSaveData playInfoSaveData)
        {
            m_DataStore.SetPlayerInfo(playInfoSaveData);
            SaveData();
        }

        public PlayInfoSaveData GetPlayerInfo()
        {
            //测试 给了默认的数据
            LevelItem level = GameManager.instance.GetLevelForCurrentScene();
            CompleteLevel(level.id, 3);
            SetPlayerInfo(new PlayInfoSaveData(10086,"Ditto1",1, m_DataStore.completedLevels));
            PlayInfoSaveData playInfoSaveData = m_DataStore.GetPlayerInfo();

            Debug.Log("playInfoSaveData:" + playInfoSaveData.playerName);
            return playInfoSaveData;
        }

    }
}