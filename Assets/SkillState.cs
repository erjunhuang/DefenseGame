using ActionGameFramework.Health;
using AIBehavior;
using GameModel;
using QGame.Core.FightEnegin;
using QGame.Core.FightEnegin.Damage;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Nodes;
using UnityEditor;
using UnityEngine;

public class SkillState : BaseState
{
    Transform target = null;
    public bool findVisibleTargetsOnly = false;
    protected override void Action(AIBehaviors fsm)
    {

    }

    protected override void Init(AIBehaviors fsm)
    {
        target = GetTarget(fsm);
        if (target != null)
        {
            AIBehaviors targetable = target.GetComponent<AIBehaviors>();
            AISkillState aiSkillState = skillStatesComponent.GetStateNoCoolingTime();


            Skill skillInfo = GameData.skills[aiSkillState.Id];
            aiSkillState.currentCoolDown = skillInfo.CoolDown;
 

            SkillData skillData = new SkillData(fsm, targetable, skillInfo);
            fsm.PlaySkill(skillData);

            Debug.Log(fsm.name);
            fsm.ChangeActiveState(fsm.previousState);
        }
        else
        {
            fsm.ChangeActiveState(fsm.previousState);
        }


    }
    protected Transform GetTarget(AIBehaviors fsm)
    {
        if (findVisibleTargetsOnly)
        {
            return fsm.GetClosestPlayerWithinSight(objectFinder.GetTransforms(SearchType.Enemy));
        }
        else
        {
            return fsm.GetClosestPlayer(objectFinder.GetTransforms(SearchType.Enemy));
        }
    }
    protected override bool Reason(AIBehaviors fsm)
    {
        return true;
    }

    protected override void StateEnded(AIBehaviors fsm)
    {
    }
}
