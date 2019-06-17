using ActionGameFramework.Health;
using Core.Health;
using QGame.Core.Event;
using System;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Nodes;
using UnityEngine;

public class Faction
{
    public List<DamageableBehaviour> participants = null;
    private BattleField battleScene;
    public int factionID = 0;

    public void Initialize(int factionID, BattleField battleScene) {
        if (null == participants)
            participants = new List<DamageableBehaviour>();
        else
            participants.Clear();
        this.battleScene = battleScene;
        this.factionID = factionID;
    }

    public EnemyAgent SpawnEnemyCharacter(PlayInfo playInfo ,int nodeIndex) {
        EnemyAgent enemyAgent = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Game/Enemy")).GetComponent<EnemyAgent>();
        Node node = battleScene.Nodes[nodeIndex];
        object[] myObjArray = { node };
        enemyAgent.transform.position = node.transform.position;
        enemyAgent.transform.rotation = node.transform.rotation;

        enemyAgent.Initialize(playInfo.monster.Id, myObjArray);
        enemyAgent.removed += DecrementNumberOfEnemies;
         
        participants.Add(enemyAgent);
        return null;
    }

    public virtual void DecrementNumberOfEnemies(DamageableBehaviour character)
    {
        for (int i = 0; i < participants.Count; i++) {
            if (participants[i] == character) {
                participants.Remove(participants[i]);
            }
        }
        if (participants.Count <= 0 && battleScene.battleSystem.GetCurrentState() == LevelState.AllEnemiesSpawned)
        {
            XEventBus.Instance.Post(EventId.GameResult, new XEventArgs(LevelState.Win));
        }
    }
}