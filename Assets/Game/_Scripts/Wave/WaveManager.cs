using System;
using System.Collections.Generic;
using Core.Extensions;
using GameModel;
using QGame.Core.Config;
using QGame.Core.Event;
using UnityEngine;

namespace TargetDefense.Level
{
	/// <summary>
	/// WaveManager - handles wave initialisation and completion
	/// </summary>
	public class WaveManager : MonoBehaviour
	{
        /// <summary>
        /// Current wave being used
        /// </summary>
        protected int m_CurrentIndex;

		/// <summary>
		/// Whether the WaveManager starts waves on Awake - defaulted to null since the BattleSystem should call this function
		/// </summary>
		public bool startWavesOnAwake;

		/// <summary>
		/// The waves to run in order
		/// </summary>
		[Tooltip("Specify this list in order")]
		public List<Wave> waves = new List<Wave>();

        /// <summary>
        /// The current wave number
        /// </summary>
        public int waveNumber
		{
			get { return m_CurrentIndex + 1; }
		}

		/// <summary>
		/// The total number of waves
		/// </summary>
		public int totalWaves
		{
			get { return waves.Count; }
		}

        public float waveProgress
        {
            get
            {
                if (waves == null || waves.Count <= m_CurrentIndex)
                {
                    return 0;
                }
                return waves[m_CurrentIndex].progress;
            }
        }

		/// <summary>
		/// Called when all waves are finished
		/// </summary>
		public event Action spawningCompleted;

		/// <summary>
		/// Starts the waves
		/// </summary>
		public virtual void StartWaves()
		{
            GameModel.MapCfg mapCfg = ConfigService.Instance.MapCfgList.GetOne(GameData.gameInfo.currentLevels);
            int[] createMonsterInfo = mapCfg.CreateMonster;
            int count = createMonsterInfo.Length;
            for (int i = 0; i < count; i++) {
                GameObject waveObj = new GameObject();
                waveObj.transform.parent = transform;
                waveObj.name = "Wave" + i;
                TimedWave timedWave = waveObj.AddComponent<TimedWave>();

                WaveCfg waveCfg = ConfigService.Instance.WaveCfgList.GetOne(createMonsterInfo[i]);
                timedWave.timeToNextWave = waveCfg.timeToNextWave;
                timedWave.spawnInstructions = new List<SpawnInstructionInfo>();
                for (int j = 0; j < waveCfg.monsterInfo.Length; j ++)
                {
                    SpawnCfg spawnCfg = ConfigService.Instance.SpawnCfgList.GetOne(waveCfg.monsterInfo[j]);
                    SpawnInstructionInfo spawnInstructionInfo = new SpawnInstructionInfo();
                    spawnInstructionInfo.agentId = spawnCfg.agentId;
                    spawnInstructionInfo.delayToSpawn = spawnCfg.delayToSpawn;
                    spawnInstructionInfo.startingNode = spawnCfg.startingNode;

                    timedWave.spawnInstructions.Add(spawnInstructionInfo);
                }
                
                waves.Add(timedWave);
            }


            if (waves.Count > 0)
			{
				InitCurrentWave();
			}
			else
			{
				Debug.LogWarning("[LEVEL] No Waves on wave manager. Calling spawningCompleted");
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// Inits the first wave
		/// </summary>
		protected virtual void Awake()
		{
			if (startWavesOnAwake)
			{
				StartWaves();
			}
        }

		/// <summary>
		/// Sets up the next wave
		/// </summary>
		protected virtual void NextWave()
		{
            waves[m_CurrentIndex].waveCompleted -= NextWave;
			if (waves.Next(ref m_CurrentIndex))
			{
				InitCurrentWave();
			}
			else
			{
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// Initialize the current wave
		/// </summary>
		protected virtual void InitCurrentWave()
		{
			Wave wave = waves[m_CurrentIndex];
			wave.waveCompleted += NextWave;
			wave.Init();

            TimedWave timedWave = waves[waveNumber - 1] as TimedWave;
            float timeToNextWave = timedWave.timeToNextWave;
            int wavesCount = waves.Count;
            XEventBus.Instance.Post(EventId.WaveChanged,new XEventArgs(waveNumber, timeToNextWave, wavesCount));
        }

		/// <summary>
		/// Calls spawningCompleted event
		/// </summary>
		protected virtual void SafelyCallSpawningCompleted()
		{
			if (spawningCompleted != null)
			{
				spawningCompleted();
			}
		}
	}
}