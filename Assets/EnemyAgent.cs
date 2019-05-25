using AIBehavior;
using Core.Health;
using Core.Utilities;
using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Economy;
using TargetDefense.Level;
using TargetDefense.Nodes;
using TargetDefense.Targets.Placement;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : LevelAgent
{
    private LevelManager m_LevelManager;
    protected Node node;
    public override void Initialize(long monsterId, params object[] args)
    {
        base.Initialize(monsterId,args);
        if (this.tag == "Enemy")
        {
            m_LevelManager.IncrementNumberOfEnemies();
        }
    }
    protected override void LazyLoad()
    {
        base.LazyLoad();
        if (m_LevelManager == null)
        {
            m_LevelManager = LevelManager.instance;
        }
    }
    protected override GameObject Create()
    {
        long id = currentTargetLevelData.Id;
        GameObject agent = Instantiate(Resources.Load<GameObject>("Prefab/Monster/" + id), transform);

        LootDrop lootDrop = GetComponentInParent<LootDrop>();
        DamageTaker damageTaker = GetComponentInParent<DamageTaker>();
        UnitInfo unitInfo = GetComponentInParent<UnitInfo>();
        AIBehaviors ai = GetComponentInParent<AIBehaviors>();
        CharacterAnimator characterAnimator = GetComponentInParent<CharacterAnimator>();

        AIPatrolState aiPatrolState = ai.GetState<AIPatrolState>();
        aiPatrolState.currentNode = (Node)args[0];

        //动画
        Animator animator = agent.GetComponent<Animator>();
        RuntimeAnimatorController runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Model/" + id + "/Animation/Controller");
        if (runtimeAnimatorController != null)
        {
            animator.runtimeAnimatorController = runtimeAnimatorController;
        }
        characterAnimator.anim = animator;

        damageTaker.healthBar = agent.transform.Find("HealthBar");
        damageTaker.sprite = GetComponentInChildren<SpriteRenderer>();
         

        //信息
        unitInfo.unitName = currentTargetLevelData.Name;
        unitInfo.primaryText = currentTargetLevelData.LootDropped.ToString();
        unitInfo.secondaryText = currentTargetLevelData.PhyAttackMin + "-" + currentTargetLevelData.PhyAttackMax;

        //属性
        UnityEngine.Object alignment = Resources.Load("Data/Alignment/EnemyAlignment");
        lootDrop.lootDropped = currentTargetLevelData.LootDropped;
        //currentTargetLevelData.MaxHealth = 500;
        //总开关
        damageTaker.Initialize();

        ai.Initialize(currentTargetLevelData, alignment);
        return agent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (this.tag == "Enemy")
        {
            m_LevelManager.DecrementNumberOfEnemies();
        }
    }
}
