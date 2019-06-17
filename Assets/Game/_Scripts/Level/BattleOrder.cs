using AIBehavior;
using Core.Health;
using GameModel;
using QGame.Core.Event;
using QGame.Core.FightEnegin;
using QGame.Core.FightEnegin.Damage;
using QGame.Core.StateMachine;
using System;
using System.Collections.Generic;
using TowerDefense.Game;
using UnityEngine;

namespace TargetDefense.Level
{
    public class BattleOrder
    {

        public LevelAgent attacker;

        //普通攻击
        public LevelAgent target;
        public DamageInfo damageInfo;

        //技能攻击
        public List<LevelAgent> targets;
        public List<DamageInfo> skillDamages;

        StateMachine<AttackPhase> attack_phase;
        public AttackPhase attackPhase
        {
            get
            {
                if (attack_phase == null)
                    return AttackPhase.Intro;
                return attack_phase.CurrentState;
            }
        }

        public void Update()
        {
            if (attack_phase != null)
                attack_phase.PerformAction();
        }

          
        public void Construct(LevelAgent attacker,LevelAgent target, DamageInfo damageInfo)
        {
            this.attacker = attacker;
            this.target = target;
            this.damageInfo = damageInfo;

            attack_phase = new StateMachine<AttackPhase>();
            attack_phase.AddState(AttackPhase.Attack, null, Attack);
            attack_phase.AddState(AttackPhase.Before, null, UpdateBefore);//前摇
            attack_phase.AddState(AttackPhase.After, null, UpdateAfter);//后摇
            attack_phase.AddState(AttackPhase.Done);
            attack_phase.SetState(AttackPhase.Attack);
        }
        private void Attack()
        {
            attack_phase.SetState(AttackPhase.Before);
        }

        private void UpdateBefore()
        {
            attack_phase.SetState(AttackPhase.After);
        }

        private void UpdateAfter()
        {
            if (target != null&& attacker!=null)
            {
                target.OnDamageTaken(this);

                //攻击完毕看看有没有buff要设置的
                for (int j = 0; j < this.damageInfo.buffInfos.Count; j++)
                {
                    target.AddBuff(BuffBase.GetBuff(damageInfo.buffInfos[j], this.attacker, target));
                }
            }

            attacker.AttackDone();
            attack_phase.SetState(AttackPhase.Done);
        }
        public SkillCfg skillcfg;
        public SkillManage skillManage;

        public void Construct(LevelAgent attacker, List<LevelAgent> targets, List<DamageInfo> skillDamages, long skillUniqueID, bool blSkillEdit = false) {
            this.attacker = attacker;
            this.targets = targets;
            this.skillDamages = skillDamages;
            skillManage =GameObject.Instantiate(Resources.Load<SkillManage>("Prefab/Skill/" + skillUniqueID));
            skillManage.OnComplete += OnComplete;

            attack_phase = new StateMachine<AttackPhase>();
            attack_phase.AddState(AttackPhase.Intro, null, SkillIntro);
            attack_phase.AddState(AttackPhase.Attack, null, SkillAttack);
            attack_phase.AddState(AttackPhase.Done);
            attack_phase.SetState(AttackPhase.Intro);
        }

        private void SkillIntro()
        {
            //准备阶段 类似动画啊啥的
            attack_phase.SetState(AttackPhase.Attack);
        }

        private void SkillAttack()
        {   
            //开始攻击 
            //清除空对象 因为你在等待放技能的时候对方对象可能死翘翘了 所以要干掉为空的对象 
            CleanupNullTransforms();
            if (this.targets.Count > 0 && this.attacker != null)
            {
                skillManage.Init(this.attacker, this.targets, this.skillDamages);
            }
            else {
                //攻击方死了 或者被攻击方翘辫子了
                attack_phase.SetState(AttackPhase.Done);
            }
        }

        void CleanupNullTransforms()
        {
            List<LevelAgent> tfmList = new List<LevelAgent>(this.targets);

            for (int i = 0; i < tfmList.Count; i++)
            {
                if (tfmList[i] == null)
                {
                    tfmList.RemoveAt(i);
                    i--;
                }
            }

            this.targets = tfmList;
        }

        private void OnComplete()
        {

            for (int i = 0; i < this.targets.Count; i++)
            {
                LevelAgent pc = this.targets[i];
                if (pc != null)
                {
                    DamageInfo skillDamageInfo = this.skillDamages[i];
                    if (pc.GetHealthValue() > 0 && skillDamageInfo.buffInfos.Count > 0)
                    {
                        for (int j = 0; j < skillDamageInfo.buffInfos.Count; j++)
                        {
                            pc.AddBuff(BuffBase.GetBuff(skillDamageInfo.buffInfos[j], this.attacker, pc));
                        }
                    }
                }
            }
            attacker.AttackDone();

            XEventBus.Instance.Post(EventId.BattleSkillEnd, new XEventArgs(this));

            attack_phase.SetState(AttackPhase.Done);
        }
    }
}