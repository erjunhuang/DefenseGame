using ActionGameFramework.Health;
using AIBehavior;
using Core.Health;
using GameModel;
using QGame.Core.Event;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Towers;
using UnityEngine;

public class OnTowerAttack : MonoBehaviour {
    public void MeleeAttack(AttackData attackData) {
        ChackSkill(attackData);
        XEventBus.Instance.Post(EventId.BattleHurt, new XEventArgs(attackData));
    }

    public void RangedAttack(AttackData attackData)
    {
        ChackSkill(attackData);
        XEventBus.Instance.Post(EventId.BattleSkillHurt, new XEventArgs(attackData));
    }

    bool ChackSkill(AttackData attackData) {
        SkillCfg skillCfg = attackData.fsm.levelAgent.GetSkill();
        if (skillCfg != null)
        {
            attackData.skillId = skillCfg.Id;
            attackData.fsm.levelAgent.ResetSkill(skillCfg);
            return true;
        }
        return false;
    }
}
