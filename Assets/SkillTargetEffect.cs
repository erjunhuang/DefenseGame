using Core.Health;
using QGame.Core.FightEnegin.Damage;
using QGame.Core.Utils;
using QGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTargetEffect :SkillBase
{   
    public bool isAOE = false;
    public float radius = 1f;

    public bool blCopy;//不复制的话，只播放一个

    public bool isUseDelay = false;
    public override int AttachActor(LevelAgent attacker, List<LevelAgent> targets, List<DamageInfo> skillDamages)
    {
        base.AttachActor(attacker, targets, skillDamages);
        if (isUseDelay) {
            StartCoroutine(DelayUpdateEffect());
            StartCoroutine(DelayPlay());
        }
        return 1;
    }
    IEnumerator DelayUpdateEffect()
    {
        while (_currentTime < delayPlayTime)
        {
            yield return 0;
        }
        LoadSkill();
    }

    IEnumerator DelayPlay()
    {
        while (_currentTime < delayPlayTime)
        {
            yield return 0;
        }
        DamageEnemy();
    }

    public  void LoadSkill() {
        if (skill_Effect == null) return;
        if (blCopy)
        {
            foreach (LevelAgent pc in targets)
            {
                if (pc != null)
                {
                    EffectDelayPlay edp = FightDefin.LoadSkill(skill_Effect, pc);
                    if (edp != null)
                    {
                        GameObjectUtils.setChildLayer(edp.gameObject, FightDefin.skillLayer);
                        effects.Add(edp);
                    }
                }
            }
        }
        else
        {
            if (targets[0] != null)
            {
                EffectDelayPlay edp = FightDefin.LoadSkill(skill_Effect, targets[0]);
                if (edp != null)
                {
                    GameObjectUtils.setChildLayer(edp.gameObject, FightDefin.skillLayer);
                    effects.Add(edp);
                }
            }
        }
    }
    public void DamageEnemy() {
        for (int j = 0; j < targets.Count; j++)
        {
            LevelAgent pc = targets[j];
            if (pc != null)
            {
                int damage = skillDamages[j].damage;
                int heal = skillDamages[j].heal;
                IAlignmentProvider alignment = skillDamages[j].alignment;
                if (heal > 0)
                {
                    pc.Cure(heal, pc.position, alignment);
                }
                else
                {
                    if (isAOE)
                    {
                        IsAOE(pc, damage, alignment);
                    }
                    else
                    {
                        pc.OnSkillDamageTaken(damage, pc.position, alignment, true, null, "");
                    }
                }
            }
        }

        if (isOver != null)
        {
            isOver(this);
        }
    }
  

    void IsAOE(LevelAgent target,int damage, IAlignmentProvider alignment) {
        Collider[] cols = Physics.OverlapSphere(target.position, radius);
        foreach (Collider col in cols)
        {
            // If target can receive damage
            LevelAgent pc = col.gameObject.GetComponent<LevelAgent>();
            if (pc != null&& target.configuration.alignmentProvider.IsFriend(pc.configuration.alignmentProvider))
            {

                Vector3 targetDir = pc.transform.position - target.transform.position;
           
                float proportion  = 1-targetDir.sqrMagnitude / (radius* radius);
                int finalDamage = (int)(damage* proportion);

                pc.OnSkillDamageTaken(finalDamage, pc.position, alignment, true, null, "");
            }
        }
    }
 }
