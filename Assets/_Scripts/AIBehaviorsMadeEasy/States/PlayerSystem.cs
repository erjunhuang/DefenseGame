using System;
using GameModel;
using QGame.Core.Event;
using QGame.Core.FightEnegin;
using QGame.Core.StateMachine;
using UnityEngine;

namespace AIBehavior
{
    public class PlayerSystem
    {
        public StateMachine<eAttackState> attack_state;

         
        private AIBehaviors fsm;
        private AttackState attackState;

        public PlayerSystem(AttackState attackState, AIBehaviors fsm)
        {
            this.attackState = attackState;
            this.fsm = fsm;
             
            attack_state = new StateMachine<eAttackState>();
            attack_state.AddState(eAttackState.Ready, null, Ready);//准备攻击状态
            attack_state.AddState(eAttackState.Fighting);//正在攻击中
            attack_state.AddState(eAttackState.SelectTarget, SelectTarget, null);//选择目标,寻到路径后直接抢第一个格子，避免走路很丑
            attack_state.AddState(eAttackState.Done, null, Done);//刚完成攻击，需要cd
            attack_state.SetState(eAttackState.SelectTarget);
        }
        public void PerformAction()
        {
            attack_state.PerformAction();
        }

        public void ChangeState(eAttackState state)
        {
            attack_state.SetState(state);
        }

        private void Ready()
        {
            if(attackState.IsCanAttack())
            {
                AttackData attackData = new AttackData(fsm, attackState.target, attackState.rangedAttackSkillId);

                SkillCfg skillCfg = attackData.fsm.levelAgent.GetSkill();
                if (skillCfg != null)
                {
                    //技能 没有就是普通攻击
                    attackData.skillId = skillCfg.Id;
                    attackData.fsm.levelAgent.ResetSkill(skillCfg);

                    XEventBus.Instance.Post(EventId.BattleSkillHurt, new XEventArgs(attackData));
                }
                else {
                    if (attackState.attackType == AttackState.AttackType.Melee)
                    {
                        XEventBus.Instance.Post(EventId.BattleHurt, new XEventArgs(attackData));
                    }
                    else
                    {
                        XEventBus.Instance.Post(EventId.BattleSkillHurt, new XEventArgs(attackData));
                    }
                }
                 
                attackState.Attack(fsm, attackState.target);
                attackState.ResetCoolDownTime();

                ChangeState(eAttackState.Fighting);
            }
            else {
                ChangeState(eAttackState.SelectTarget);
            }
        }

        private void SelectTarget()
        {
            if (attackState.target != null)
            {
                float sqrDistanceThreshold = attackState.attackRange * attackState.attackRange;
                Vector3 targetDir = attackState.target.transform.position - fsm.transform.position;
                if (targetDir.sqrMagnitude <= sqrDistanceThreshold)
                {
                    ChangeState(eAttackState.Ready);
                }
            }
        }


        private void Done()
        {
            ChangeState(eAttackState.SelectTarget);
        }
    }
}