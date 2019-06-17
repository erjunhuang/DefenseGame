using AIBehavior;
using Core.Health;
using QGame.Core.FightEnegin.Damage.Buff;
using QGame.Core.Resource;
using QGame.Utils;
using System.Collections.Generic;
using UnityEngine;
namespace QGame.Core.FightEnegin.Damage
{

    public class BuffBase
    {
        public int id;
        /// <summary>
        /// 
        /// </summary>
        public eBuffType buffType;
        /// <summary>
        /// buff是否合并
        /// </summary>
        //public bool blMerge=true;
        /// <summary>
        /// 持续时间 buff时间 在卡屏时要停止
        /// </summary>
        public float durationTime = 5;
        public string buffIconUrl;
        protected BuffInfo buffInfo;
        protected LevelAgent target;
        protected LevelAgent attacker;
        public EffectDelayPlay buffEffectDelayPlay;
        /// <summary>
        /// 是否是减益效果
        /// </summary>
        public bool blDebuff=false;
        /// <summary>
        /// buff特效路径
        /// </summary>
        protected string buffEffectName=string.Empty;
        public static BuffBase GetBuff(BuffInfo buffInfo, LevelAgent attacker, LevelAgent target)
        {   
            //if (buffType == null)
            //    return null;
            //if (buffInfo.nnc != null)
            //{
            //}
            switch (buffInfo.buffType)
            {
                case eBuffType.heal:
                    {
                        return new  HealBuff(buffInfo, attacker , target);
                    }
                case eBuffType.damage:
                    {
                        return new DamgeBuff(buffInfo, attacker, target);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        virtual public bool onEnert()
        {
           // Debug.Log("onEnert " + buffEffectName);
            if (buffEffectName.Length>0)
            {
                ResourceManager.LoadObjectByWWW(buffEffectName, (ResLoadInfo res, object param) =>
                {
                    GameObject obj = res.LoadAsset<GameObject>();
                    
                    if (obj != null)
                    {
                        buffEffectDelayPlay = FightDefin.LoadEffect(obj);
                        if (buffEffectDelayPlay != null)
                        {
                            buffEffectDelayPlay.gameObject.transform.parent = this.target.transform;
                            FightDefin.TransformIdentity(buffEffectDelayPlay.gameObject.transform);
                        }
                    }
                    else
                    {
                        Debug.LogError("buff加载失败" + buffEffectName.ToString());
                    }

                });

            }        
            return true;
        }

        virtual public void onExit()
        {
            if (buffEffectDelayPlay!=null)
                GameObject.DestroyImmediate(buffEffectDelayPlay.gameObject);
        }

        virtual public void onUpdate()
        { 
        }

        /// <summary>
        /// 某些buff在普通攻击下会有特效
        /// </summary>
        virtual public void onNormalAttack(AIBehaviors fobAttack)
        {
        }
    }

    public class DamageInfo 
    {
        /// <summary>
        /// The amount of damage this damager does
        /// </summary>
        public int damage;
        public int heal;
        public List<BuffInfo> buffInfos;
        public IAlignmentProvider alignment;
        public DamageInfo()
        {
        }
        public DamageInfo(int damage, int heal, List<BuffInfo> buffInfos, IAlignmentProvider alignment) {
            this.damage = damage;
            this.heal = heal;
            this.buffInfos = buffInfos;
            this.alignment = alignment;
        }
    }

    public class BuffInfo
    {
        /// <summary>
        /// 技能id
        /// </summary>
        public int buffID;
        /// <summary>
        /// 相同的buff要合并?
        /// </summary>
        public eBuffType buffType;

        /// <summary>
        /// 持续时间 buff时间 在卡屏时要停止,单位为秒
        /// </summary>
        public float durationTime = 5;

       // public GameObject buffEffect;

        public int targetHeroUniqueID;

        public int AllValue;//持续伤害或者持续回复的总量
        public NumericalNumberChange nnc;
        /// <summary>
        /// 特效资源路径名
        /// </summary>
        public string buffResName = string.Empty;
        public BuffInfo()
        {
        }
        public BuffInfo(eBuffType buffType, int AllValue = 0,float durationTime = 0,string buffResName ="", NumericalNumberChange nnc = null,int buffID=0, int targetHeroUniqueID=0)
        {
            this.buffType = buffType;
            this.AllValue = AllValue;
            this.durationTime = durationTime;
            this.buffResName = buffResName;
            this.nnc = nnc;

            this.buffID = buffID;
            this.targetHeroUniqueID = targetHeroUniqueID;
        }
    }

    /// <summary>
    /// 数值有正负，正为增益  负为减益 百分比数值 50代表 50%
    /// </summary>
    /// 
    public class NumericalNumberChange
    {
        /// <summary>
        /// 攻击
        /// </summary>
        public int attack = 0;
        /// <summary>
        /// 防御
        /// </summary>
        public int armor = 0;
        /// <summary>
        /// 行动力
        /// </summary>
        public int speed = 0;
        /// <summary>
        /// 一骑当先值
        /// </summary>
        public int yjdx = 0;
        /// <summary>
        /// 体力
        /// </summary>
        public int Vit = 0;
        /// <summary>
        /// 伤害吸收
        /// </summary>
        public int Absorb = 0;
        public NumericalNumberChange add(NumericalNumberChange nnc)
        {
            this.attack += nnc.attack;
            this.armor += nnc.armor;
            this.speed += nnc.speed;
            this.yjdx += nnc.yjdx;
            return this;
        }

        public NumericalNumberChange subtract(NumericalNumberChange nnc)
        {
            this.attack -= nnc.attack;
            this.armor -= nnc.armor;
            this.speed -= nnc.speed;
            this.yjdx -= nnc.yjdx;
            return this;
        }
    }
}
