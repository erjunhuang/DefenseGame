using QGame.Core.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.UI.HUD;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI icon in towers building tree.
/// </summary>
public class TowerUI : MonoBehaviour
{
    private Button upgradeButton, sell,closeBtn;
    private TowerAgent myTower;
    private RectTransform panelRectTransform;
    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
        this.gameObject.SetActive(false);
        upgradeButton = transform.Find("Buttons/Update").GetComponent<Button>();
        sell = transform.Find("Buttons/Sell").GetComponent<Button>();
        closeBtn = transform.Find("CloseBG").GetComponent<Button>();
        panelRectTransform = transform.Find("Buttons").GetComponent<RectTransform>();
        upgradeButton.GetComponent<EventTriggerListener>().onClick += OnUpgradeClick;
        sell.GetComponent<EventTriggerListener>().onClick += OnSellClick;
        closeBtn.GetComponent<EventTriggerListener>().onClick += OnCloseBtnClick;
        if (GameUIManager.instanceExists)
        {
            GameUIManager.instance.selectionChanged += OnUISelectionChanged;
            GameUIManager.instance.stateChanged += OnGameUIStateChanged;
        }

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        AdjustPosition();
    }

    protected void AdjustPosition()
    {
        if (myTower == null)
        {
            return;
        }
        Vector3 point = Camera.main.WorldToScreenPoint(myTower.transform.position);
        point.z = 0;
        panelRectTransform.transform.position = point;
    }

    public virtual void Show(TowerAgent target)
    {
        if (target == null)
        {
            return;
        }
        myTower = target;
        AdjustPosition();
        this.gameObject.SetActive(true);

        int sellValue = myTower.GetSellLevel();
   

        if (upgradeButton != null)
        {
            upgradeButton.interactable =
                TargetDefense.Level.LevelManager.instance.currency.CanAfford(myTower.GetCostForNextLevel());
            bool maxLevel = target.isAtMaxLevel;
            upgradeButton.gameObject.SetActive(!maxLevel);
            if (!maxLevel)
            {
                //Debug.Log(target.GetNextUpgradeAgents()[0].UpgradeDescription.ToUpper());
            }
        }

        TargetDefense.Level.LevelManager.instance.currency.currencyChanged += OnCurrencyChanged;
    }

    void OnCurrencyChanged()
    {
        if (myTower != null && upgradeButton != null)
        {
            upgradeButton.interactable =
                TargetDefense.Level.LevelManager.instance.currency.CanAfford(myTower.GetCostForNextLevel());
        }
    }

    protected virtual void OnDisable()
    {
        if (TargetDefense.Level.LevelManager.instanceExists)
        {
            TargetDefense.Level.LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
        }
    }
    public virtual void Hide()
    {
        myTower = null;
        if (GameUIManager.instanceExists)
        {
            GameUIManager.instance.HideRadiusVisualizer();
        }
        this.gameObject.SetActive(false);
        TargetDefense.Level.LevelManager.instance.currency.currencyChanged -= OnCurrencyChanged;
    }
    void OnGameUIStateChanged(GameUIManager.State oldState, GameUIManager.State newState)
    {
        if (newState == GameUIManager.State.GameOver)
        {
            Hide();
        }
    }
    protected virtual void OnUISelectionChanged(TowerAgent target)
    {
        if (target != null)
        {
            Show(target);
        }
        else
        {
            Hide();
        }
    }
    private void OnDestroy()
    {
        upgradeButton.GetComponent<EventTriggerListener>().onClick -= OnUpgradeClick;
        sell.GetComponent<EventTriggerListener>().onClick -= OnSellClick;
        closeBtn.GetComponent<EventTriggerListener>().onClick -= OnCloseBtnClick;
        if (GameUIManager.instanceExists)
        {
            GameUIManager.instance.selectionChanged -= OnUISelectionChanged;
            GameUIManager.instance.stateChanged -= OnGameUIStateChanged;
        }
    }

    private void OnUpgradeClick(PointerEventData eventData, GameObject go)
    {
        GameUIManager.instance.UpgradeSelectedTarget();
    }

    private void OnSellClick(PointerEventData eventData, GameObject go)
    {
        GameUIManager.instance.SellSelectedTarget();
    }

    private void OnCloseBtnClick(PointerEventData eventData, GameObject go)
    {
        GameUIManager.instance.DeselectTarget();
    }
}
