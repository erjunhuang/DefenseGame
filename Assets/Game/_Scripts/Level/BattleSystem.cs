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
using QGame.Core.StateMachine;
using QGame.Core.FightEnegin;
using Core.Health;
using QGame.Core.FightEnegin.Damage;
using AIBehavior;

namespace TargetDefense.Level
{
    public class BattleSystem 
    {
        private StateMachine<LevelState> levelState;
        private List<BattleOrder> battle_order;

        public event Action levelCompleted;
        public event Action levelFailed;
        public event Action<LevelState, LevelState> levelStateChanged;
        public bool isPlayGame
        {
            get { return (GetCurrentState() == LevelState.SpawningEnemies) || (GetCurrentState() == LevelState.AllEnemiesSpawned); }
        }
        public bool isGameOver
        {
            get { return (GetCurrentState() == LevelState.Win) || (GetCurrentState() == LevelState.Lose); }
        }
        public LevelState GetCurrentState()
        {
            return levelState.CurrentState;
        }

        private BattleField battleField;
        public BattleSystem(BattleField battleField)
        {
            this.battleField = battleField;
            battle_order = new List<BattleOrder>();
            levelState = new StateMachine<LevelState>();
            levelState.AddState(LevelState.Intro, null, null);
            levelState.AddState(LevelState.Building, null, null);
            levelState.AddState(LevelState.SpawningEnemies, UpdateGameing, null);
            levelState.AddState(LevelState.AllEnemiesSpawned, UpdateGameing, null);
            levelState.AddState(LevelState.Lose, null, Lose);
            levelState.AddState(LevelState.Win, null, Win);
            levelState.SetState(LevelState.Intro);
        }

        public void ChangeLevelState(LevelState newState)
        {
            if (GetCurrentState() == newState)
            {
                return;
            }
            LevelState oldState = GetCurrentState();
            levelState.SetState(newState);
            if (levelStateChanged != null)
            {
                levelStateChanged(oldState, GetCurrentState());
            }
        }

        public void Update()
        {   
            if(levelState != null)
            levelState.PerformAction();
        }
        void UpdateGameing() {
            BattleOrder entry;

            for (int i = 0; i < battle_order.Count; i++)
            {
                entry = battle_order[i];
               
                entry.Update();
            }

            for (int i = 0; i < battle_order.Count; i++)
            {
                entry = battle_order[i];
                if (entry.attackPhase == AttackPhase.Done)
                {
                    battle_order.Remove(entry);
                }
            }
        }
      
        void Lose()
        {
            SafelyCallLevelFailed();
        }
        void Win()
        {   
            SafelyCallLevelCompleted();
        }


        public void AddNormalAttackToBattleOrder(AttackData attackData ) {
            LevelAgent attacker = attackData.fsm.levelAgent;
            LevelAgent target = attackData.target.GetComponent<LevelAgent>();

            IAlignmentProvider alignment = attacker.configuration.alignmentProvider;

            List<BuffInfo> buffInfos = new List<BuffInfo>();
            //buffInfos.Add(new BuffInfo(eBuffType.damage, 10, 10f, "PoisonFX"));
            DamageInfo damageInfo = new DamageInfo(10,0, buffInfos, alignment);

            BattleOrder bo = new BattleOrder();
            bo.Construct(attacker,target, damageInfo);
            battle_order.Add(bo);
        }

        public void AddSkillAttackToBattleOrder(AttackData attackData) {

            LevelAgent attacker = attackData.fsm.levelAgent;
            LevelAgent target = attackData.target.GetComponent<LevelAgent>();

            List<LevelAgent> targets = new List<LevelAgent>();
            targets.Add(target);


            int damage;
            List<BuffInfo> buffInfos = new List<BuffInfo>();
            if (attackData.skillId == 2002)
            {
                damage = 0;
                buffInfos.Add(new BuffInfo(eBuffType.damage, 5, 3f, "PoisonFX"));
            }
            else if (attackData.skillId == 2003)
            {
                damage = 10;
            }
            else {
                damage = 1;
            }

            DamageInfo damageInfo = new DamageInfo(damage, 0, buffInfos, attacker.configuration.alignmentProvider);
           
            List<DamageInfo> skillDamages = new List<DamageInfo>();
            skillDamages.Add(damageInfo);

            BattleOrder bo = new BattleOrder();
            bo.Construct(attacker, targets, skillDamages, attackData.skillId);
            battle_order.Add(bo);
        }
        protected virtual void SafelyCallLevelFailed()
        {
            battle_order.Clear();
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