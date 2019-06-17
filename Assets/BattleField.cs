using AIBehavior;
using Core.Utilities;
using QGame.Core.Config;
using QGame.Core.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Nodes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleField : BattleSceneBase<BattleField>
{
    public WaveManager waveManager { get; protected set; }
    private LevelIntro intro;
    public override void InitializeBattleSystem()
    {
        base.InitializeBattleSystem();

        //配置加载 如果有大厅之后最好放在大厅登陆成功
        ConfigService.Instance.Initialize();

        GameData.gameInfo.currentLevels = 100011;
        GameData.gameInfo.towersInfo.Add(2001);


        XEventBus.Instance.Register(EventId.GameResult, GameResult);
        XEventBus.Instance.Register(EventId.BattleHurt, NormalHurt);
        XEventBus.Instance.Register(EventId.BattleSkillHurt, SkillHurt);
        XEventBus.Instance.Register(EventId.BattleSkillEnd, BattleSkillEnd);
        XEventBus.Instance.Register(EventId.SpawningEnemies, SpawningEnemies);

        battleSystem = new BattleSystem(this);

        enemyFaction = new Faction();
        enemyFaction.Initialize(1, this);

        waveManager = GetComponent<WaveManager>();
        waveManager.spawningCompleted += OnSpawningCompleted;

        intro = GetComponent<LevelIntro>();
        if (intro != null)
        {
            intro.introCompleted += IntroCompleted;
        }
        else
        {
            IntroCompleted();
        }
    }
    public void SpawnAgent(PlayInfo playInfo, int nodeIndex)
    {
        enemyFaction.SpawnEnemyCharacter(playInfo, nodeIndex);
    }

    protected virtual void IntroCompleted()
    {
        battleSystem.ChangeLevelState(LevelState.Building);

        XEventBus.Instance.Post(EventId.SpawningEnemies);
    }

    public virtual void SpawningEnemies(XEventArgs args)
    {
        waveManager.StartWaves();
        battleSystem.ChangeLevelState(LevelState.SpawningEnemies);
    }
    
    protected virtual void OnSpawningCompleted()
    {
        battleSystem.ChangeLevelState(LevelState.AllEnemiesSpawned);
    }

    private void GameResult(XEventArgs args)
    {
        battleSystem.ChangeLevelState(args.GetData<LevelState>(0));
    }

    void NormalHurt(XEventArgs args)
    {
        battleSystem.AddNormalAttackToBattleOrder(args.GetData<AttackData>(0));
    }

    void SkillHurt(XEventArgs args)
    {
        battleSystem.AddSkillAttackToBattleOrder(args.GetData<AttackData>(0));
    }

    private void BattleSkillEnd(XEventArgs args)
    {
        //Debug.Log("技能释放完毕");
    }

    protected override void Update()
    {
        base.Update();
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

        XEventBus.Instance.UnRegister(EventId.GameResult, GameResult);
        XEventBus.Instance.UnRegister(EventId.BattleHurt, NormalHurt);
        XEventBus.Instance.UnRegister(EventId.BattleSkillHurt, SkillHurt);
        XEventBus.Instance.UnRegister(EventId.BattleSkillEnd, BattleSkillEnd);
        XEventBus.Instance.UnRegister(EventId.SpawningEnemies, SpawningEnemies);
    }

   
}
