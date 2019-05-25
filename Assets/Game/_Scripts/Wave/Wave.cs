using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Utilities;
using TargetDefense.Nodes;
using UnityEngine;
using TargetDefense.Economy;
using AIBehavior;

namespace TargetDefense.Level
{
	/// <summary>
	/// A Wave is a TimedBehaviour, that uses the RepeatingTimer to spawn enemies
	/// </summary>
	public class Wave : TimedBehaviour
	{
		/// <summary>
		/// A list of instructions on how to spawn enemies
		/// </summary>
		public List<SpawnInstructionInfo> spawnInstructions = new List<SpawnInstructionInfo>();

		/// <summary>
		/// The index of the current enemy to spawn
		/// </summary>
		protected int m_CurrentIndex;

		/// <summary>
		/// The RepeatingTimer used to spawn enemies
		/// </summary>
		protected RepeatingTimer m_SpawnTimer;

		/// <summary>
		/// The event that is fired when a Wave is completed
		/// </summary>
		public event Action waveCompleted;

		public virtual float progress
		{
			get { return (float) (m_CurrentIndex) / spawnInstructions.Count; }
		}

		/// <summary>
		/// Initializes the Wave
		/// </summary>
		public virtual void Init()
		{
			// If the wave is empty then warn the level designer and fire complete event
			if (spawnInstructions.Count == 0)
			{
				Debug.LogWarning("[LEVEL] Empty Wave");
				SafelyBroadcastWaveCompletedEvent();
				return;
			}
            m_SpawnTimer = new RepeatingTimer(spawnInstructions[0].delayToSpawn, SpawnCurrent);
			StartTimer(m_SpawnTimer);
        }

		/// <summary>
		/// Handles spawning the current agent and sets up the next agent for spawning
		/// </summary>
		protected virtual void SpawnCurrent()
		{
			Spawn();
			if (!TrySetupNextSpawn())
			{
				SafelyBroadcastWaveCompletedEvent();
				// this is required so wave progress is still accurate
				m_CurrentIndex = spawnInstructions.Count;
				StopTimer(m_SpawnTimer);
			}
		}

		/// <summary>
		/// Spawns the current agent
		/// </summary>
		protected void Spawn()
		{
            SpawnInstructionInfo spawnInstruction = spawnInstructions[m_CurrentIndex];
			SpawnAgent(spawnInstruction);
		}

		/// <summary>
		/// Tries to setup the next spawn
		/// </summary>
		/// <returns>true if there is another spawn instruction, false if not</returns>
		protected bool TrySetupNextSpawn()
		{
			bool hasNext = spawnInstructions.Next(ref m_CurrentIndex);
			if (hasNext)
			{
                SpawnInstructionInfo nextSpawnInstruction = spawnInstructions[m_CurrentIndex];
				if (nextSpawnInstruction.delayToSpawn <= 0f)
				{
					SpawnCurrent();
				}
				else
				{
					m_SpawnTimer.SetTime(nextSpawnInstruction.delayToSpawn);
				}
			}

			return hasNext;
		}
 
        /// <summary>
        /// Spawns the agent
        /// </summary>
        /// <param name="agentConfig">The agent to spawn</param>
        /// <param name="node">The starting node that the agent uses</param>
        protected virtual void SpawnAgent(SpawnInstructionInfo  spawnInstructionInfo)
		{
            int agentId = spawnInstructionInfo.agentId;
            int nodeIndex = spawnInstructionInfo.startingNode;


            Node node = LevelManager.instance.Nodes[nodeIndex];

            //var poolable = Poolable.TryGetPoolable<Poolable>(agentConfig.agentPrefab);
            //if (poolable == null)
            //{
            //	return;
            //}

            GameObject poolable = Instantiate(Resources.Load<GameObject>("Prefab/Game/Enemy"));
            poolable.transform.position = node.transform.position;
            poolable.transform.rotation = node.transform.rotation;
 

            EnemyAgent enemyAgent = poolable.GetComponent<EnemyAgent>();
            object[] myObjArray = { node };
            enemyAgent.Initialize(agentId, myObjArray);
        }

        /// <summary>
        /// Launch the waveCompleted event
        /// </summary>
        protected void SafelyBroadcastWaveCompletedEvent()
		{
			if (waveCompleted != null)
			{
				waveCompleted();
			}
		}
	}
}