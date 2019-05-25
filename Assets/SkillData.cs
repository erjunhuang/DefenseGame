using AIBehavior;
using GameModel;
using QGame.Core.FightEnegin.Damage;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public Skill skillInfo;

    public AIBehaviors attacker;
    public List<AIBehaviors> targets;
    public List<DamageInfo> skillDamages;

    public SkillData(AIBehaviors attacker, AIBehaviors target, Skill skillInfo)
    {
        this.skillInfo = skillInfo;

        this.attacker = attacker;

        List<AIBehaviors> targets = new List<AIBehaviors>();
        targets.Add(target);
        this.targets = targets;


        DamageInfo damager = new DamageInfo();
        damager.physicalDamage = attacker.monsterInfo.PhyAttackMax;
        damager.magicDamage = attacker.monsterInfo.MagicAttackMax;
        damager.alignmentProvider = attacker.configuration.alignmentProvider;
        damager.buffInfo = skillInfo.Buff;

        List<DamageInfo> skillDamages = new List<DamageInfo>();
        skillDamages.Add(damager);
        this.skillDamages = skillDamages;
    }

    public SkillData(AIBehaviors attacker, List<AIBehaviors> targets, List<DamageInfo> skillDamages, Skill skillInfo)
    {
        this.skillInfo = skillInfo;

        this.attacker = attacker;
        this.targets = targets;
        this.skillDamages = skillDamages;
    }
}