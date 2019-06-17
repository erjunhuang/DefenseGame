using ActionGameFramework.Health;
using AIBehavior;
using Core.Health;
using Core.Utilities;
using ETModel;
using GameModel;
using QGame.Core.Config;
using QGame.Core.FightEnegin;
using QGame.Core.FightEnegin.Damage;
using QGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TargetDefense.Level;
using TargetDefense.Targets.Placement;
using UnityEngine;
using UnityEngine.AI;

public class LevelAgent : Targetable
{
    public long InstanceId;
    /// <summary>
    /// AI控制器
    /// </summary>
    public AIBehaviors aIBehaviors { get; protected set; }
    /// <summary>
    /// 当前等级索引
    /// </summary>
    public int currentLevel { get; protected set; }
    /// <summary>
    /// 当前等级的分支索引 比如1级升2级 2级有两个方向 0代表第一个方向 1代表第二个方向
    /// </summary>
    public int upgradeBranch { get; protected set; }
    /// <summary>
    /// 当前对象
    /// </summary>
    public GameObject currentAgentLevel { get; protected set; }
    /// <summary>
    /// 是否是最大等级
    /// </summary>
    public bool isAtMaxLevel
    {
        get { return currentLevel == agentLevels.Count - 1; }
    }
    /// <summary>
    /// 当前对象的数据
    /// </summary>
    public PlayInfo currentTargetLevelData
    {
        get { return agentLevels[currentLevel][upgradeBranch]; }
    }
    /// <summary>
    /// 等级列表
    /// </summary>
    protected Dictionary<int, List<PlayInfo>> agentLevels = new Dictionary<int, List<PlayInfo>>();


    public class CoolDown {
        public float recharge_skill_timer;
        public SkillCfg skillCfg;
        public CoolDown(float recharge_skill_timer, SkillCfg skillCfg) {
            this.skillCfg = skillCfg;
            this.recharge_skill_timer = recharge_skill_timer;
        }
    }
    /// <summary>
    /// 技能集合
    /// </summary>
    public Dictionary<long, CoolDown> list_skill = new Dictionary<long, CoolDown>();

    /// <summary>
    /// 数据来源
    /// </summary>
    protected object[] args;
    protected override void Awake()
    {
        base.Awake();
        LazyLoad();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    public virtual void Initialize(int monsterId, params object[] args)
    {
        LazyLoad();
        this.args = args;

        agentLevels.Clear();

        MonsterCfg currentMonster = ConfigService.Instance.MonsterCfgList.GetOne(monsterId);

        List<PlayInfo> playInfo = new List<PlayInfo>();
        playInfo.Add(new PlayInfo(currentMonster));
        agentLevels.Add(currentMonster.Level, playInfo);
        this.currentLevel = currentMonster.Level;

        if (currentMonster.levelIds != null)
        {
            foreach (int levelId in currentMonster.levelIds)
            {
                MonsterCfg monster = ConfigService.Instance.MonsterCfgList.GetOne(levelId);
                if (agentLevels.TryGetValue(monster.Level, out playInfo))
                {
                    playInfo.Add(new PlayInfo(monster));
                }
                else {
                    playInfo = new List<PlayInfo>();
                    playInfo.Add(new PlayInfo(monster));
                    agentLevels.Add(monster.Level, playInfo);
                }
            }
        }
        SetLevel(this.currentLevel);
    }
    /// <summary>
    /// 加载
    /// </summary>
    protected virtual void LazyLoad() {
        if (aIBehaviors == null)
        {
            aIBehaviors = GetComponent<AIBehaviors>();
        }
    }
    /// <summary>
    /// 显示
    /// </summary>
    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }
    /// <summary>
    /// 隐藏
    /// </summary>
    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取升级下一阶段可能出现的所有对象
    /// </summary>
    public virtual List<PlayInfo> GetNextUpgradeAgents() {
        int nextLevel = currentLevel + 1;
        if (nextLevel <= agentLevels.Count)
        {
            return agentLevels[currentLevel + 1];
        }
        else {
            return null;
        }
    }
    /// <summary>
    /// 获取对象的技能
    /// </summary>
    public virtual int[] GetCurrentAgentSkill()
    {
        return agentLevels[currentLevel][upgradeBranch].monster.SkillList;
    }
    /// <summary>
    /// 升一级
    /// </summary>
    public virtual bool UpgradeTarget()
    {
        if (isAtMaxLevel)
        {
            return false;
        }
        SetLevel(currentLevel + 1);
        return true;
    }
    /// <summary>
    /// 升级到指定的等级
    /// </summary>
    public virtual bool UpgradeTowerToLevel(int level,int upgradeBranch)
    {
        if (level < 0 || isAtMaxLevel || level >= agentLevels.Count)
        {
            return false;
        }
        SetLevel(level, upgradeBranch);
        return true;
    }
    /// <summary>
    /// 得到下一等级的升级价格
    /// </summary>
    public virtual int GetCostForNextLevel()
    {
        if (isAtMaxLevel)
        {
            return -1;
        }
        return agentLevels[currentLevel + 1][upgradeBranch].monster.Cost;
    }
    /// <summary>
    /// 得到出售的价格
    /// </summary>
    public int GetSellLevel()
    {
        return agentLevels[currentLevel][upgradeBranch].monster.Sell;
    }
    /// <summary>
    /// 出售
    /// </summary>
    public void Sell()
    {
        Remove();
    }
    /// <summary>
    /// 设置相应等级的对象
    /// </summary>
    protected virtual void SetLevel(int level,int upgradeBranch = 0)
    {
        if (upgradeBranch > agentLevels[currentLevel].Count) {
            return;
        }
        if (level < 0 || level >= agentLevels.Count)
        {
            return;
        }
        this.currentLevel = level;
        this.upgradeBranch = upgradeBranch;

        if (currentAgentLevel != null)
        {   
            RemoveAllBuff();
            Destroy(currentAgentLevel);
        }
         
        currentAgentLevel = Create();

        list_skill.Clear();

        //foreach (int skillId in currentTargetLevelData.monster.SkillList)
        //{
        //    list_skill.Add(skillId, new CoolDown(0, GameData.skills[skillId]));
        //}

        list_skill.Add(2002, new CoolDown(0, ConfigService.Instance.SkillCfgList.GetOne(2002)));
        list_skill.Add(2003, new CoolDown(0, ConfigService.Instance.SkillCfgList.GetOne(2003)));
        //测试
        this.configuration.SetMaxHealth(currentTargetLevelData.monster.MaxHealth);
        this.configuration.SetHealth(currentTargetLevelData.monster.MaxHealth);

        InstanceId = IdGenerater.GenerateInstanceId();
    }
    /// <summary>
    /// 创建对象
    /// </summary>
    protected virtual GameObject Create()
    {
        return null;
    }
    //-----------------------------------------------------------------------------------

    /// <summary>
    /// buff集合
    /// </summary>
    public Dictionary<eBuffType, BuffBase> list_buff = new Dictionary<eBuffType, BuffBase>();


    /// <summary>
    /// 获取当前血量
    /// </summary>
    public float GetHealthValue()
    {
        return this.configuration.currentHealth;
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public override void Remove()
    {
        base.Remove();
        RemoveAllBuff();
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        
    }

    /// <summary>
    /// 普攻受击伤害
    /// </summary>
    /// <param name="battleOrder"></param>
    public  void OnDamageTaken(BattleOrder battleOrder)
    {
        if (this.isDead)
            return;

        //类似 闪避 免疫功能的实现
        //if (blMissGongBing && (battleOrder.attacker.playInfo.armyID / 10) % 10 == 3)
        //{
        //    //AddHUDText("免疫", eScoreFlashType.Common);
        //    AddHUDText("Icon/Skill/SkillHudTag/免疫", eScoreFlashType.SkillUp);
        //    return;
        //}
        if (Damage(battleOrder.damageInfo.damage, battleOrder.target.position, battleOrder.damageInfo.alignment)) {
            battleOrder.target = null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="blKapingKey">是否技能打击的关键目标（决定卡屏结束时间）</param>
    /// <param name="hurtHP"></param>
    /// <param name="blJS">true 为技能的最后一击</param>
    public void OnSkillDamageTaken(int damageValue, Vector3 damagePoint, IAlignmentProvider alignment,
        bool blJS = false, GameObject hitEffect = null, string targetTag = "")
    {
        if (this.isDead)return;

        if (hitEffect != null)
        {
            EffectDelayPlay edp = FightDefin.LoadSkill(hitEffect, this, eSkillDirection.orignal, false, false);
            edp.m_blIgnoreScaleTime = true;
        }
        //最后一击
        if (blJS && targetTag != "")
        {
           // AddHUDText(targetTag, eScoreFlashType.SkillUp);
        }
        Damage(damageValue, damagePoint, alignment);
    }

    /// <summary>
    /// 伤害
    /// </summary>
    public bool Damage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignment)
    {
        // We don't want to enable this state if the FSM is dead
        //GotHitState gotHitState = GetState<GotHitState>();

        //if ( gotHitState != null && gotHitState.CanGetHit(this) )
        //{
        //SubtractHealthValue(totalDamage);
         
        //if ( gotHitState.CoolDownFinished() )
        //{
        //	ChangeActiveState(gotHitState);
        //}
        //}

        if (alignment.CanHarm(this.configuration.alignmentProvider))
        {
            this.TakeDamage(damageValue, damagePoint, alignment);
        }
        else
        {
            Debug.Log("不可伤害,因为是友方单位");
        }
        return this.isDead;
    }
    /// <summary>
    /// 治疗
    /// </summary>
    public void Cure(float addCureValue, Vector3 damagePoint, IAlignmentProvider alignment)
    {
        if (alignment.IsFriend(this.configuration.alignmentProvider))
        {
            this.TakeCure(addCureValue, damagePoint, alignment);
        }
        else {
            Debug.Log("不可治疗,因为是敌方单位");
        }
    }

    /// <summary>
    /// buff控制
    /// </summary>
    private eBuffType[] keys;
    bool blDelete = false;
    public void Update()
    {
        if (Time.timeScale > 0)
        {
            blDelete = false;
            //处理buff
            if (list_buff != null && list_buff.Count > 0)
            {
                if (keys == null)
                {
                    keys = new eBuffType[list_buff.Count];
                    list_buff.Keys.CopyTo(keys, 0);
                }

                foreach (eBuffType buffType in keys)
                {
                    BuffBase bb = list_buff[buffType];
                    bb.onUpdate();
                    bb.durationTime -= Time.deltaTime;
                    if (bb.durationTime <= 0)
                    {
                        blDelete = true;
                        list_buff.Remove(bb.buffType);
                        bb.onExit();
                    }
                }
                if (blDelete)
                {
                    if (list_buff.Count > 0)
                    {
                        keys = new eBuffType[list_buff.Count];
                        list_buff.Keys.CopyTo(keys, 0);
                    }
                    else
                    {
                        keys = null;
                    }
                }
            }

            //技能处理
            if (list_skill != null && list_skill.Count > 0)
            {
                //new CoolDown(x.Value.recharge_skill_timer + Time.deltaTime, GameData.skills[x.Key])
                list_skill = list_skill.ToDictionary(x => x.Key, x => GetCoolDown(x.Value,Time.deltaTime));
            }
        }
    }

    public CoolDown GetCoolDown(CoolDown coolDown, float deltaTime) {
        
        float recharge_skill_timer = coolDown.recharge_skill_timer + deltaTime;
        if (recharge_skill_timer >= coolDown.skillCfg.CoolDown) {
            recharge_skill_timer = coolDown.skillCfg.CoolDown;
        }
        coolDown.recharge_skill_timer = recharge_skill_timer;
        return coolDown;
    }
    public SkillCfg GetSkill() {
        foreach (long skillId in list_skill.Keys)
        {
            CoolDown coolDown = list_skill[skillId];
            if (coolDown.recharge_skill_timer >= coolDown.skillCfg.CoolDown)
            {
                return coolDown.skillCfg;
            }
        }
        return null;
    }

    public void ResetSkill(SkillCfg skillCfg) {
        foreach (long skillId in list_skill.Keys)
        {
            CoolDown coolDown = list_skill[skillId];
            if (skillCfg == coolDown.skillCfg)
            {
                coolDown.recharge_skill_timer = 0;
            }
        }
    }


    /// <summary>
    /// 攻击完成
    /// </summary>
    public  void AttackDone()
    {
        //if (aIBehaviors.currentState.ToString() == "Attack")
        //{
        //    AttackState attackState = (AttackState)aIBehaviors.currentState;
        //    attackState.ChangeState(eAttackState.Done);
        //}
    }

    /// <summary>
    /// 移除所有buff
    /// </summary>
    private void RemoveAllBuff()
    {
        if (list_buff != null)
        {
            foreach (BuffBase bi in list_buff.Values)
            {
                bi.onExit();
            }
            list_buff.Clear();
        }
    }
    /// <summary>
    /// 净化buff
    /// </summary>
    public void JingHua()
    {
        foreach (eBuffType buffType in keys)
        {
            BuffBase bb = list_buff[buffType];
            if (bb.blDebuff)
            {
                bb.onExit();
                list_buff.Remove(bb.buffType);
            }
        }
    }
    /// <summary>
    /// 添加buff
    /// </summary>
    public void AddBuff(BuffBase buff)
    {
        if (this.configuration.currentHealth <= 0)
            return;
        if (buff == null)
            return;
        if (buff.onEnert()) {
            list_buff.Add(buff.buffType, buff);
        }
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void MoveAgentWithVector(Transform target, float targetSpeed, float rotationSpeed)
    {
        Vector3 targetPoint = target.position;
        transform.position = Vector3.MoveTowards(transform.position, target.position, targetSpeed * 0.1f * Time.fixedDeltaTime);
    }
}
