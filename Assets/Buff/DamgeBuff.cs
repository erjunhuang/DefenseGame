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
        public DamgeBuff(BuffInfo buffInfo, LevelAgent attacker, LevelAgent target)
        {
            this.buffInfo = buffInfo;
            this.attacker = attacker;
            this.target = target;

            this.buffType = buffInfo.buffType;

            InitData();
            //根据玩家数值运算
            int allValue = this.buffInfo.AllValue;
            stepBlood = allValue / (int)(durationTime / healStepTime);
        }

        void InitData()
        { 
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
                int allValue = this.buffInfo.AllValue * attacker.currentTargetLevelData.monster.PhyAttackMax;
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
