using AIBehavior;
using QGame.Utils;
using UnityEngine;

namespace QGame.Core.FightEnegin.Damage.Buff
{
    /// <summary>
    /// 伤害buff
    /// </summary>
    public class DamgeBuff : BuffBase
    {
        private float healStepTime = 1f;
        private float currentStepTime = 0;
        public int stepBlood;
        public DamgeBuff(eBuffType buffType, AIBehaviors attacker, AIBehaviors target)
        {
            this.buffType = buffType;
            this.attacker = attacker;
            this.target = target;

            InitData();
            //根据玩家数值运算
            int allValue = this.buffInfo.AllValue * attacker.monsterInfo.PhyAttackMax;
            stepBlood = allValue / (int)(durationTime / healStepTime);
        }

        void InitData()
        {
            this.buffInfo = new BuffInfo();
            this.buffInfo.buffID = 0;
            this.buffInfo.targetHeroUniqueID = 0;
            this.buffInfo.nnc = null;

            this.buffInfo.AllValue = 30;
            this.buffInfo.durationTime = 10;
            this.buffInfo.buffType = buffType;
            this.buffInfo.buffResName = "PoisonFX";

             
            healStepTime = 1f;
            this.blDebuff = true;
            durationTime = this.buffInfo.durationTime;
            buffIconUrl = "19";
            buffEffectName = this.buffInfo.buffResName;

            currentStepTime = healStepTime;
        }
        override public bool onEnert()
        {
            //刷新
            if (this.target.list_buff.ContainsKey(buffType))
            {
                InitData();
                DamgeBuff hb = (DamgeBuff)this.target.list_buff[eBuffType.damage];
                hb.durationTime = this.durationTime;
                hb.buffInfo = this.buffInfo;
                int allValue = this.buffInfo.AllValue * attacker.monsterInfo.PhyAttackMax;
                hb.stepBlood = allValue / (int)(durationTime / healStepTime);
                return false;
            }
            return base.onEnert();
        }
        override public void onUpdate()
        {
            currentStepTime += Time.deltaTime;
            if (currentStepTime >= healStepTime && buffInfo.AllValue > 0)
            {
                this.target.Damage(stepBlood, this.target.transform.position, this.attacker.configuration.alignmentProvider);
                currentStepTime = 0;
                buffInfo.AllValue -= stepBlood;

                if (buffEffectDelayPlay != null)
                {
                    buffEffectDelayPlay.SetKaping(false);
                }
            }
        }

        public override void onExit()
        {
            if (buffInfo.AllValue > 0)
            {
                this.target.Damage(stepBlood, this.target.transform.position, this.attacker.configuration.alignmentProvider);
            }
            base.onExit();
        }
    }
}
