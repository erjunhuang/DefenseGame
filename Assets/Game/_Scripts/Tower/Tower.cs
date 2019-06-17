using ActionGameFramework.Health;
using AIBehavior;
using Core.Utilities;
using GameModel;
using QGame.Core.Event;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Targets.Placement;
using UnityEngine;

/// <summary>
/// Tower building and operation.
/// </summary>
public class Tower : Targetable
{
    public string targetName;
    public int currentLevel { get; protected set; }
    public TargetLevel currentTargetLevel { get; protected set; }
    public bool isAtMaxLevel
    {
        get { return currentLevel == towerLevels.Count - 1; }
    }
    //public TargetGhost targetGhostPrefab
    //{
    //    get { return levels[currentLevel].targetGhostPrefab; }
    //}
    public MonsterCfg currentTargetLevelData
    {
        get { return towerLevels[currentLevel]; }
    }

    public IntVector2 dimensions;

    public IntVector2 gridPosition { get; private set; }
    public IPlacementArea placementArea { get; private set; }

    public List<MonsterCfg> towerLevels = new List<MonsterCfg>();

    public virtual void Initialize(IPlacementArea targetArea, IntVector2 destination)
    {
        UpdateTargetPos(targetArea, destination);
        SetLevel(0);
        //if (TargetDefense.Level.BattleField.instanceExists)
        //{
        //    TargetDefense.Level.BattleField.instance.levelStateChanged += OnLevelStateChanged;
        //}
    }

    public virtual void UpdateTargetPos(IPlacementArea targetArea, IntVector2 destination)
    {

        if (placementArea != null)
        {
            placementArea.Clear(gridPosition, dimensions);
        }
        placementArea = targetArea;
        gridPosition = destination;

        if (targetArea != null)
        {
            transform.position = placementArea.GridToWorld(destination, dimensions);
            transform.rotation = placementArea.transform.rotation;
            targetArea.Occupy(destination, dimensions);
            targetArea.SetController(transform);
        }
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public int GetCostForNextLevel()
    {
        if (isAtMaxLevel)
        {
            return -1;
        }
        return towerLevels[currentLevel + 1].Cost;
    }

    public void KillTower()
    {
        // Invoke base kill method
        Kill();
    }

    public int GetSellLevel()
    {
        return GetSellLevel(currentLevel);
    }
    public int GetSellLevel(int level)
    {
        // sell for full price if waves haven't started yet
        //if (TargetDefense.Level.BattleSystem.instance.levelState == LevelState.Building)
        //{
        //    int cost = 0;
        //    for (int i = 0; i <= level; i++)
        //    {
        //        cost += towerLevels[i].Cost;
        //    }

        //    return cost;
        //}
        return towerLevels[currentLevel].Sell;
    }
    public virtual bool UpgradeTarget()
    {
        if (isAtMaxLevel)
        {
            return false;
        }
        SetLevel(currentLevel + 1);
        return true;
    }

    public virtual bool DowngradeTower()
    {
        if (currentLevel == 0)
        {
            return false;
        }
        SetLevel(currentLevel - 1);
        return true;
    }

    public virtual bool UpgradeTowerToLevel(int level)
    {
        if (level < 0 || isAtMaxLevel || level >= towerLevels.Count)
        {
            return false;
        }
        SetLevel(level);
        return true;
    }

    public void Sell()
    {
        Remove();
    }

    public override void Remove()
    {
        base.Remove();
        placementArea.Clear(gridPosition, dimensions);
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        //if (TargetDefense.Level.BattleSystem.instanceExists)
        //{
        //    TargetDefense.Level.BattleSystem.instance.levelStateChanged -= OnLevelStateChanged;
        //}
    }

    protected void SetLevel(int level)
    {
        if (level < 0 || level >= towerLevels.Count)
        {
            return;
        }
        currentLevel = level;
        if (currentTargetLevel != null)
        {
            Destroy(currentTargetLevel.gameObject);
        }

         

        long levelId = towerLevels[currentLevel].Id;
        GameObject towerLevel = Instantiate(Resources.Load<GameObject>("Prefab/Monster/" + levelId),transform);
        currentTargetLevel = towerLevel.AddComponent<TargetLevel>();

        //AiStateManager aiStateManager = towerLevel.AddComponent<AiStateManager>();
        //AiStateIdle aiStateIdle = towerLevel.AddComponent<AiStateIdle>();

        //aiStateManager.AddState(aiStateIdle);
        //aiStateManager.SetDefaultState(aiStateIdle);
        //currentTargetLevel.Initialize(this, -1, configuration.alignmentProvider);

        //AIBehaviors ai = towerLevel.GetComponent<AIBehaviors>();
        //ai.Initialize();

        ScaleHealth();

        //// disable affectors
        //LevelState levelState = BattleSystem.instance.levelState;
        //bool initialise = levelState == LevelState.SpawningEnemies || levelState == LevelState.AllEnemiesSpawned;
        //currentTargetLevel.SetAffectorState(initialise);
    }

    protected virtual void ScaleHealth()
    {
        configuration.SetMaxHealth(currentTargetLevelData.MaxHealth);
        if (currentLevel == 0)
        {
            configuration.SetHealth(currentTargetLevelData.MaxHealth);
        }
        else
        {
            int currentHealth = Mathf.FloorToInt(configuration.normalisedHealth * currentTargetLevelData.MaxHealth);
            configuration.SetHealth(currentHealth);
        }

        UnitInfo unitInfo = GetComponentInParent<UnitInfo>();
        unitInfo.unitName = currentTargetLevelData.Name;
        unitInfo.primaryText = currentTargetLevelData.Description.ToString();
        unitInfo.secondaryText = currentTargetLevelData.PhyAttackMin + "-" + currentTargetLevelData.PhyAttackMax;
    }
    protected virtual void OnLevelStateChanged(LevelState previous, LevelState current)
    {
        bool initialise = current == LevelState.SpawningEnemies || current == LevelState.AllEnemiesSpawned;
        currentTargetLevel.SetAffectorState(initialise);
    }
}
