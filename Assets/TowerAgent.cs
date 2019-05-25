using AIBehavior;
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


    public override void Initialize(long monsterId, params object[] args) {
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
        long id = currentTargetLevelData.Id;
        GameObject agent = Instantiate(Resources.Load<GameObject>("Prefab/Monster/" + id), transform);

        Animator animator = agent.GetComponent<Animator>();
        CharacterAnimator characterAnimator = GetComponentInParent<CharacterAnimator>();
        AIBehaviors ai = GetComponent<AIBehaviors>();
        AIAnimationStates animationStates = GetComponentInParent<AIAnimationStates>();

        characterAnimator.anim = animator;

        //AI
        //BaseState baseState = ComponentHelper.AddComponentByName(ai.transform.Find("States").gameObject, "GotHitState") as BaseState;
        //baseState.name = "GotHitState";
        //GotHitState gotHitState = baseState as GotHitState;
        //gotHitState.hitStateDuration = 0;
        //gotHitState.returnToPreviousState = true;
        //gotHitState.animationStates[0] = animationStates.GetStateWithName("Hit");
        //ai.AddSubTrigger(gotHitState);

        //放置
        IPlacementArea targetArea = (IPlacementArea)args[0];
        IntVector2 destination = (IntVector2)args[1];
        UpdateTargetPos(targetArea, destination);

        //初始化
        UnityEngine.Object alignment = Resources.Load("Data/Alignment/TowerAlignment");
        //currentTargetLevelData.MaxHealth = 10000;
        ai.Initialize(currentTargetLevelData, alignment);
        return agent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (placementArea!=null) {
            placementArea.Clear(gridPosition, dimensions);
        }
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
