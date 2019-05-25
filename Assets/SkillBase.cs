using System;
using System.Collections.Generic;
using ActionGameFramework.Health;
using AIBehavior;
using QGame.Core.FightEnegin.Damage;
using TowerDefense.Game;
using UnityEngine;

public class SkillBase: MonoBehaviour
{
    public float delayPlayTime = 0;
    protected AIBehaviors skillActor;
    protected List<AIBehaviors> skillTargets;
    protected List<DamageInfo> skillDamages;
    public GameObject skill_Effect;
    protected float _currentTime = 0;

    public float kapingTime;
    public virtual int AttachActor(AIBehaviors attacker, List<AIBehaviors> targets, List<DamageInfo> skillDamages)
    {
        this.skillActor = attacker;
        this.skillTargets = targets;
        this.skillDamages = skillDamages;
        return 0;
    }

    void Update()
    {
        _currentTime += GameManager.RealDeltaTime;
    }
}