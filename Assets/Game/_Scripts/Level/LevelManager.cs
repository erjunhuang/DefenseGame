using Core.Economy;
using Core.Utilities;
using LitJson;
using GameModel;
using QGame.Core.Event;
using System;
using System.Collections.Generic;
using TargetDefense.Economy;
using TargetDefense.Nodes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TargetDefense.Level
{
    public class LevelManager : Singleton<LevelManager>
    {
        public LevelIntro intro;
        public int startingCurrency;
        public bool alwaysGainCurrency;
        public Currency currency { get; protected set; }
        public CurrencyGainer currencyGainer;
         

        public WaveManager waveManager { get; protected set; }
        public int numberOfEnemies;

        public LevelState levelState { get; protected set; }

        public bool isGameOver
        {
            get { return (levelState == LevelState.Win) || (levelState == LevelState.Lose); }
        }
        public event Action levelCompleted;
        public event Action levelFailed;
        public event Action<LevelState, LevelState> levelStateChanged;
        public event Action<int> numberOfEnemiesChanged;

        // UI scene. Load on level start
        public string levelUiSceneName;
        // User interface manager
        private UiManager uiManager;

        private int beforeLooseCounter;
        protected override void Awake()
        {
            base.Awake();
            Initialize();
            waveManager = GetComponent<WaveManager>();
            waveManager.spawningCompleted += OnSpawningCompleted;

            levelState = LevelState.Intro;
            numberOfEnemies = 0;

            currency = new Currency(startingCurrency);
            currencyGainer.Initialize(currency);

            if (intro != null)
            {
                intro.introCompleted += IntroCompleted;
            }
            else
            {
                IntroCompleted();
            }
            // Load UI scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelUiSceneName, LoadSceneMode.Additive);
        }
        public List<Node> Nodes;

        void Initialize() {

            TextAsset monsterConfig = Resources.Load<TextAsset>("Config/Monster");
            foreach (string str in monsterConfig.text.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                try
                {
                    string str2 = str.Trim();
                    if (str2 == "")
                    {
                        continue;
                    }
                    Monster jsonData = JsonMapper.ToObject<Monster>(str2.Trim());
                    GameData.monsters.Add(jsonData.Id, jsonData);
                }
                catch (Exception e)
                {
                    throw new Exception($"parser json fail: {str}", e);
                }
            }

            TextAsset levelConfig = Resources.Load<TextAsset>("Config/Level");
            foreach (string str in levelConfig.text.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                try
                {
                    string str2 = str.Trim();
                    if (str2 == "")
                    {
                        continue;
                    }
                    GameModel.Level jsonData = JsonMapper.ToObject<GameModel.Level>(str2.Trim());
                    GameData.levels.Add(jsonData.Id, jsonData);
                }
                catch (Exception e)
                {
                    throw new Exception($"parser json fail: {str}", e);
                }
            }

            TextAsset skillConfig = Resources.Load<TextAsset>("Config/Skill");
            foreach (string str in skillConfig.text.Split(new[] { "\n" }, StringSplitOptions.None))
            {
                try
                {
                    string str2 = str.Trim();
                    if (str2 == "")
                    {
                        continue;
                    }
                    GameModel.Skill jsonData = JsonMapper.ToObject<GameModel.Skill>(str2.Trim());
                    GameData.skills.Add(jsonData.Id, jsonData);
                }
                catch (Exception e)
                {
                    throw new Exception($"parser json fail: {str}", e);
                }
            }

            GameData.gameInfo.currentLevels = 100011;
            GameData.gameInfo.towersInfo.Add(2001);

            WaveInfo waveInfo = new WaveInfo();
            int[] createMonsterInfo = GameData.levels[GameData.gameInfo.currentLevels].CreateMonster;
            for (int i = 0; i < createMonsterInfo.Length; i += 3)
            {
                SpawnInstructionInfo spawnInstructionInfo = new SpawnInstructionInfo();
                spawnInstructionInfo.agentId = createMonsterInfo[i];
                spawnInstructionInfo.delayToSpawn = createMonsterInfo[i + 1];
                spawnInstructionInfo.startingNode = createMonsterInfo[i + 2];
                waveInfo.SpawnInstructions.Add(spawnInstructionInfo);
            }
            GameData.levelInfo.waveInfos.Add(waveInfo);

        }
        /// <summary>
        /// Raises the enable event.
        /// </summary>
        void OnEnable()
        {
            XEventBus.Instance.Register(EventId.Captured, Captured);
        }

        /// <summary>
        /// Raises the disable event.
        /// </summary>
        void OnDisable()
        {
           // EventManager.StopListening("Captured", Captured);
            XEventBus.Instance.UnRegister(EventId.Captured, Captured);
        }

        /// <summary>
        /// Enemy reached capture point.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="param">Parameter.</param>
        private void Captured(XEventArgs args)
        {
            if (beforeLooseCounter > 0)
            {
                beforeLooseCounter--;
                uiManager.SetDefeatAttempts(beforeLooseCounter);
                if (beforeLooseCounter <= 0)
                {
                    ChangeLevelState(LevelState.Lose);
                    uiManager.GoToDefeatMenu();
                }
            }
        }

        private void Start()
        {
            uiManager = FindObjectOfType<UiManager>();
            Debug.Assert(uiManager, "Wrong initial parameters");
            beforeLooseCounter = 1;
            uiManager.SetDefeatAttempts(beforeLooseCounter);
        }
        protected virtual void Update()
        {
            if (alwaysGainCurrency ||
                (!alwaysGainCurrency && levelState != LevelState.Building && levelState != LevelState.Intro))
            {
                currencyGainer.Tick(Time.deltaTime);
            }
        }

        public virtual void DecrementNumberOfEnemies()
        {
            numberOfEnemies--;
            SafelyCallNumberOfEnemiesChanged();
            if (numberOfEnemies < 0)
            {
                Debug.LogError("[LEVEL] There should never be a negative number of enemies. Something broke!");
                numberOfEnemies = 0;
            }

            if (numberOfEnemies == 0 && levelState == LevelState.AllEnemiesSpawned)
            {   
                ChangeLevelState(LevelState.Win);
                uiManager.GoToVictoryMenu();
            }
        }

        public virtual void IncrementNumberOfEnemies()
        {
            numberOfEnemies++;
            SafelyCallNumberOfEnemiesChanged();
        }

        protected virtual void SafelyCallNumberOfEnemiesChanged()
        {
            if (numberOfEnemiesChanged != null)
            {
                numberOfEnemiesChanged(numberOfEnemies);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (waveManager != null)
            {
                waveManager.spawningCompleted -= OnSpawningCompleted;
            }
            if (intro != null)
            {
                intro.introCompleted -= IntroCompleted;
            }
        }
         
        public virtual void BuildingCompleted()
        {
            ChangeLevelState(LevelState.SpawningEnemies);
        }

        protected virtual void IntroCompleted()
        {
            ChangeLevelState(LevelState.Building);
        }

        protected virtual void OnSpawningCompleted()
        {
            ChangeLevelState(LevelState.AllEnemiesSpawned);
        }

        protected virtual void ChangeLevelState(LevelState newState)
        {
            // If the state hasn't changed then return
            if (levelState == newState)
            {
                return;
            }

            LevelState oldState = levelState;
            levelState = newState;
            if (levelStateChanged != null)
            {
                levelStateChanged(oldState, newState);
            }

            switch (newState)
            {
                case LevelState.SpawningEnemies:
                    waveManager.StartWaves();
                    break;
                case LevelState.AllEnemiesSpawned:
                    // Win immediately if all enemies are already dead
                    if (numberOfEnemies == 0)
                    {
                        ChangeLevelState(LevelState.Win);
                    }
                    break;
                case LevelState.Lose:
                    SafelyCallLevelFailed();
                    break;
                case LevelState.Win:
                    SafelyCallLevelCompleted();
                    break;
            }
        }


        protected virtual void SafelyCallLevelFailed()
        {
            if (levelFailed != null)
            {
                levelFailed();
            }
        }

        protected virtual void SafelyCallLevelCompleted()
        {
            if (levelCompleted != null)
            {
                levelCompleted();
            }
        }
    }
}