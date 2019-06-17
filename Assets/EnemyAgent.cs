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
    public override void Initialize(int monsterId, params object[] args)
    {
        base.Initialize(monsterId,args);
    }
    protected override void LazyLoad()
    {
        base.LazyLoad();
    }

    protected override GameObject Create()
    {
        long id = currentTargetLevelData.monster.Id;
        GameObject agent = Instantiate(Resources.Load<GameObject>("Prefab/Monster/" + id), transform);

        LootDrop lootDrop = GetComponentInParent<LootDrop>();
        DamageTaker damageTaker = GetComponentInParent<DamageTaker>();
        UnitInfo unitInfo = GetComponentInParent<UnitInfo>();
        AIBehaviors ai = GetComponentInParent<AIBehaviors>();
        CharacterAnimator characterAnimator = GetComponentInParent<CharacterAnimator>();

        Node node = (Node)args[0];
        //OffensiveState offensiveState = ai.GetState<OffensiveState>();
        //offensiveState.currentNode = node;
        //offensiveState.GetTrigger<WithinDistanceTrigger>().center = transform;


        PatrolState patrolState = ai.GetState<PatrolState>();
        patrolState.SetPatrolPoints(node.transform.parent);
        patrolState.GetTrigger<WithinDistanceTrigger>().center = transform;

        //AttackState attackState = ai.GetState<AttackState>();
        //attackState.GetTrigger<BeyondDistanceTrigger>().center = transform;
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
        unitInfo.unitName = currentTargetLevelData.monster.Name;
        unitInfo.primaryText = currentTargetLevelData.monster.LootDropped.ToString();
        unitInfo.secondaryText = currentTargetLevelData.monster.PhyAttackMin + "-" + currentTargetLevelData.monster.PhyAttackMax;

        //属性
        lootDrop.lootDropped = currentTargetLevelData.monster.LootDropped;
        UnityEngine.Object alignment = Resources.Load("Data/Alignment/EnemyAlignment");

        if (this.configuration.alignment == null)
        {
            this.configuration.alignment = new SerializableIAlignmentProvider();
            this.configuration.alignment.unityObjectReference = alignment;
        }else{
            this.configuration.alignment.unityObjectReference = alignment;
        }
         
        //总开关
        damageTaker.Initialize();
        ai.Initialize();
        this.gameObject.name = id.ToString();
        return agent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
