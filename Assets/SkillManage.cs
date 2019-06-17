using ActionGameFramework.Health;
using AIBehavior;
using GameModel;
using QGame.Core.FightEnegin.Damage;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Game;
using UnityEngine;

public class SkillManage : MonoBehaviour {
    public string skillName;
    List<LevelAgent> skillTargets;
    private List<SkillBase> sbs;
    private float totalTime = 0f;
    public Action OnComplete;
    public void Init(LevelAgent attacker, List<LevelAgent> targets, List<DamageInfo> skillDamages)//, List<int> damages, bool blClearSkill = false, bool blJingHua = false
    {
        skillTargets = targets;
        sbs = new List<SkillBase>();
        int nCount = gameObject.transform.childCount;
         
        for (int i = 0; i < nCount; i++)
        {
            SkillBase skill = gameObject.transform.GetChild(i).gameObject.GetComponent<SkillBase>();
            if (skill != null)
            {
                skill.AttachActor(attacker, targets, skillDamages);
                sbs.Add(skill);
                //totalTime += skill.totalTime;

                skill.isOver += OnItemSkillOver;
            }
        }

        //StartCoroutine(UpdateSelf());
    }

    private void OnItemSkillOver(SkillBase skill)
    {
        sbs.Remove(skill);
        if (sbs.Count == 0) {
            BattleSkillCount();
        }
    }

    public void Init(LevelAgent attacker, List<LevelAgent> targets, SkillCfg skillInfo) {

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void BattleSkillCount()
    {
        if (OnComplete != null) {
            OnComplete();
        }
        Destroy(gameObject);
    }
    float _currentTime = 0;
    IEnumerator UpdateSelf()
    {
        while (_currentTime < totalTime)
        {
            _currentTime += GameManager.RealDeltaTime;
            yield return 0;
        }
        BattleSkillCount();
    }
}
