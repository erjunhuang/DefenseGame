using Core.Economy;
using Core.Extensions;
using Core.Utilities;
using GameModel;
using LitJson;
using QGame.Core.Config;
using QGame.Core.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Economy;
using TargetDefense.Level;
using TargetDefense.Nodes;
using UnityEngine;
using UnityEngine.SceneManagement;
//PersistentSingleton<TBattleField>
public class BattleSceneBase<TBattleField> : Singleton<TBattleField> where TBattleField : BattleSceneBase<TBattleField>
{
    public List<Node> Nodes;

    public bool alwaysGainCurrency;
    public int startingCurrency;
    public Currency currency { get; protected set; }
    public CurrencyGainer currencyGainer;

    public BattleSystem battleSystem;
    public Faction enemyFaction;
    // Use this for initialization
    public virtual void InitializeBattleSystem()
    {
        XEventBus.Instance.Register(EventId.AddCurrency, AddCurrency);

        currency = new Currency(startingCurrency);
        currencyGainer.Initialize(currency);
    }
  
    protected virtual void Update()
    {
        if (battleSystem == null) return;
        battleSystem.Update();
        if (alwaysGainCurrency ||
            (!alwaysGainCurrency && battleSystem.isPlayGame))
        {
            currencyGainer.Tick(Time.deltaTime);
        }
    }

    private void AddCurrency(XEventArgs args)
    {
        int amount = args.GetData<int>(0);
        currency.AddCurrency(amount);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        XEventBus.Instance.UnRegister(EventId.AddCurrency, AddCurrency);
    }
 
}
