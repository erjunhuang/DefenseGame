using AIBehavior;
using Core.Utilities;
using GameModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Targets.Placement;
using UnityEngine;
using UnityEngine.AI;

public class LevelAgent : MonoBehaviour
{

    public AIBehaviors aIBehaviors { get; protected set; }
    public int currentLevel { get; protected set; }
    public int upgradeBranch { get; protected set; }
    public GameObject currentAgentLevel { get; protected set; }
    public bool isAtMaxLevel
    {
        get { return currentLevel == agentLevels.Count - 1; }
    }
    public Monster currentTargetLevelData
    {
        get { return agentLevels[currentLevel][upgradeBranch]; }
    }

    protected Dictionary<int, List<Monster>> agentLevels = new Dictionary<int, List<Monster>>();

    protected object[] args;
    protected virtual void Awake()
    {
        LazyLoad();
    }
    public virtual void Initialize(long monsterId, params object[] args)
    {
        LazyLoad();
        this.args = args;
        if (aIBehaviors == null) {
            aIBehaviors = GetComponent<AIBehaviors>();
        }
        Monster currentMonster = GameData.monsters[monsterId];

        List<Monster> monsters = new List<Monster>();
        monsters.Add(currentMonster);
        agentLevels.Add(currentMonster.Level, monsters);
        this.currentLevel = currentMonster.Level;

        if (currentMonster.levelIds != null)
        {
            foreach (int levelId in currentMonster.levelIds)
            {
                Monster monster = GameData.monsters[levelId];
                if (agentLevels.TryGetValue(monster.Level, out monsters))
                {
                    monsters.Add(monster);
                }
                else {
                    monsters = new List<Monster>();
                    monsters.Add(monster);
                    agentLevels.Add(monster.Level, monsters);
                }
            }
        }
        SetLevel(this.currentLevel);
    }

    protected virtual void LazyLoad() {

    }
    public virtual List<Monster> GetNextUpgradeAgents() {
        int nextLevel = currentLevel + 1;
        if (nextLevel <= agentLevels.Count)
        {
            return agentLevels[currentLevel + 1];
        }
        else {
            return null;
        }
    }
    public virtual int[] GetCurrentAgentSkill()
    {
        //foreach (int skill in agentLevels[currentLevel][upgradeBranch].SkillList)
        //{
        //    Debug.Log("skill:" + skill);
        //}
        return agentLevels[currentLevel][upgradeBranch].SkillList;
    }
    public virtual void UpgradeTarget()
    {
        if (isAtMaxLevel)
        {
            //return false;
        }
        SetLevel(currentLevel + 1);
        //return true;
    }
    public virtual bool UpgradeTowerToLevel(int level,int upgradeBranch)
    {
        if (level < 0 || isAtMaxLevel || level >= agentLevels.Count)
        {
            return false;
        }
        SetLevel(level, upgradeBranch);
        return true;
    }

    public virtual int GetCostForNextLevel()
    {
        if (isAtMaxLevel)
        {
            return -1;
        }
        return agentLevels[currentLevel + 1][upgradeBranch].Cost;
    }

    public int GetSellLevel()
    {
        //// sell for full price if waves haven't started yet
        //if (TargetDefense.Level.LevelManager.instance.levelState == LevelState.Building)
        //{
        //    int cost = 0;
        //    for (int i = 0; i <= level; i++)
        //    {
        //        cost += agentLevels[i].Cost;
        //    }

        //    return cost;
        //}
        return agentLevels[currentLevel][upgradeBranch].Sell;
    }

    public void Sell()
    {
        aIBehaviors.Remove();
    }
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
            Destroy(currentAgentLevel);
        }
         
        currentAgentLevel = Create();    
    }
    protected virtual GameObject Create()
    {
        return null;
    }

    protected virtual void OnDestroy()
    {
    }
}
