using AIBehavior;
using Core.Health;
using Core.Utilities;
using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Targets.Placement;
using UnityEngine;
using UnityEngine.AI;

public class TowerAgent : LevelAgent
{
    public IntVector2 dimensions;
    public IntVector2 gridPosition { get; private set; }
    public IPlacementArea placementArea { get; private set; }

    public override void Initialize(int monsterId, params object[] args) {
        base.Initialize(monsterId, args);
    }

    //给塔用的
    public virtual void UpdateTargetPos(IPlacementArea targetArea=null, IntVector2 destination = default(IntVector2))
    {

        if (placementArea != null)
        {
            placementArea.Clear(gridPosition, dimensions);
        }
        placementArea = targetArea;
        gridPosition = destination;

        if (targetArea != null)
        {
            transform.position = placementArea.GridToWorld(destination, dimensions);
            transform.rotation = placementArea.transform.rotation;
            targetArea.Occupy(destination, dimensions);
            targetArea.SetController(transform);
        }
    }

    protected override GameObject Create()
    {


        
        IPlacementArea targetArea = (IPlacementArea)args[0];
        IntVector2 destination = (IntVector2)args[1];
         

        long id = currentTargetLevelData.monster.Id;
        GameObject agent = Instantiate(Resources.Load<GameObject>("Prefab/Monster/" + id), transform);

        Animator animator = agent.GetComponent<Animator>();
        CharacterAnimator characterAnimator = GetComponentInParent<CharacterAnimator>();
        AIBehaviors ai = GetComponent<AIBehaviors>();
        AIAnimationStates animationStates = GetComponentInParent<AIAnimationStates>();

        characterAnimator.anim = animator;


        PatrolState patrolState = ai.GetState<PatrolState>();
        Transform[] transforms = new Transform[2];
        transforms[0] = targetArea.transform;
        transforms[1] = targetArea.transform;
        patrolState.SetPatrolPoints(transforms);
        patrolState.GetTrigger<WithinDistanceTrigger>().center = transform;
        //IdleState ldleState = ai.GetState<IdleState>();
        //ldleState.currentNode = targetArea.transform;
        //ldleState.GetTrigger<WithinDistanceTrigger>().center = targetArea.transform;

        AttackState attackState = ai.GetState<AttackState>();
        attackState.GetTrigger<BeyondDistanceTrigger>().center = targetArea.transform;

        //AI
        //BaseState baseState = ComponentHelper.AddComponentByName(ai.transform.Find("States").gameObject, "GotHitState") as BaseState;
        //baseState.name = "GotHitState";
        //GotHitState gotHitState = baseState as GotHitState;
        //gotHitState.hitStateDuration = 0;
        //gotHitState.returnToPreviousState = true;
        //gotHitState.animationStates[0] = animationStates.GetStateWithName("Hit");
        //ai.AddSubTrigger(gotHitState);

        //初始化
        UnityEngine.Object alignment = Resources.Load("Data/Alignment/TowerAlignment");

        if (this.configuration.alignment == null)
        {
            this.configuration.alignment = new SerializableIAlignmentProvider();
            this.configuration.alignment.unityObjectReference = alignment;
        }
        else {
            this.configuration.alignment.unityObjectReference = alignment;
        }

        ai.Initialize();
        //放置
        UpdateTargetPos(targetArea, destination);
        return agent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (placementArea!=null) {
            placementArea.Clear(gridPosition, dimensions);
        }
    }
}
