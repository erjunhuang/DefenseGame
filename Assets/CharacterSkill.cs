using ActionGameFramework.Health;
using Core.Health;
using QGame.Core.FightEnegin;
using QGame.Core.FightEnegin.Damage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AIBehavior
{
    public class CharacterSkill : MonoBehaviour
    {
        private SkillData skillState;
        private Dictionary<int, int[]> buffs;

        public void OnSkillState(SkillData skillState)
        {
            //buffs = new Dictionary<int, BuffInfo>();
            this.skillState = skillState;
            buffs = new Dictionary<int, int[]>();
            for (int i = 0; i < skillState.skillDamages.Count; i++)
            {
                if (skillState.skillDamages[i].buffInfo!=null)
                    buffs.Add(i, skillState.skillDamages[i].buffInfo);
            }


            SkillManage skillManage = Instantiate(Resources.Load<SkillManage>("Prefab/Skill/" + skillState.skillInfo.Id));
            skillManage.Init(skillState.attacker, skillState.targets, skillState.skillDamages);
            skillManage.OnComplete += OnComplete;
        }

        private void OnComplete()
        { 
            foreach (int key in buffs.Keys)
            {
                AIBehaviors pc = skillState.targets[key];
                foreach (int buffInfo in buffs[key]) {
                    pc.AddBuff(BuffBase.GetBuff((eBuffType)buffInfo, skillState.attacker, pc));
                }
            }
        }

        private void Update()
        {
        }
    }
}