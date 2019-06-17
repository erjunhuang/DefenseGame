using System;
using System.Collections.Generic;
using Core.Health;
using QGame.Core.FightEnegin.Damage;
using QGame.Utils;
using UnityEngine;

public class SkillBase: MonoBehaviour
{
    public float delayPlayTime = 0;
    protected LevelAgent attacker;
    protected List<LevelAgent> targets;
    protected List<DamageInfo> skillDamages;
    protected float _currentTime = 0;
    public GameObject skill_Effect;
    protected List<EffectDelayPlay> effects;

    public Action<SkillBase> isOver;
    public virtual int AttachActor(LevelAgent attacker, List<LevelAgent> targets, List<DamageInfo> skillDamages)
    {
        effects = new List<EffectDelayPlay>();
        this.attacker = attacker;
        this.targets = targets;
        this.skillDamages = skillDamages;
        _currentTime = 0;
        return 0;
    }

    protected virtual void Update()
    {
         _currentTime += Time.deltaTime;
    }
}