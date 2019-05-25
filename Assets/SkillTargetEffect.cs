using ActionGameFramework.Health;
using AIBehavior;
using Core.Health;
using QGame.Core.FightEnegin.Damage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTargetEffect : SkillBase
{
    public bool blCopy = false;//不复制的话，只播放一个
    public override int AttachActor(AIBehaviors attacker, List<AIBehaviors> targets, List<DamageInfo> skillDamages)
    {
        base.AttachActor(attacker, targets, skillDamages);

        if (skill_Effect != null) {
            StartCoroutine(UpdateEffect());
        }
        StartCoroutine(UpdateAnimator());
        return 1;
    }

    IEnumerator UpdateEffect()
    {
        while (_currentTime < delayPlayTime)
        {
            yield return 0;
        }

        if (blCopy)
        {
            foreach (AIBehaviors pc in skillTargets)
            {
              GameObject effect =  Instantiate(skill_Effect, pc.transform);
            }
        }
        else
        {
            AIBehaviors pc = skillTargets[0];
            GameObject effect = Instantiate(skill_Effect, pc.transform);
        }
    }

    IEnumerator UpdateAnimator()
    {
        while (_currentTime < delayPlayTime)
        {
            yield return 0;
        }
        for (int j = 0; j < skillTargets.Count; j++)
        {
            AIBehaviors pc = skillTargets[j];
            int damage = skillDamages[j].physicalDamage;
            int heal = skillDamages[j].heal;
            AIBehaviors target_damageable = pc.GetComponent<AIBehaviors>();

            if (heal > 0)
            {
                target_damageable.Cure(heal, pc.position, skillDamages[j].alignmentProvider);
            }
            else {
                target_damageable.Damage(damage, pc.position, skillDamages[j].alignmentProvider);
            }
        }
    }
}
