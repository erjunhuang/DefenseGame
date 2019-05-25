using GameModel;
using QGame.Core.Event;
using TargetDefense.Level;
using TargetDefense.Targets;
using TargetDefense.Targets.Data;
using TargetDefense.UI.HUD;
using UnityEngine;
     
public class BuildSidebar : MonoBehaviour
{
    public TargetSpawnButton towerSpawnButton;
    protected virtual void Start()
    {
        foreach (int towerId in GameData.gameInfo.towersInfo)
        {
            TargetSpawnButton button = Instantiate(towerSpawnButton, transform);
            button.InitializeButton(towerId);
            button.buttonTapped += OnButtonTapped;
            button.draggedOff += OnButtonDraggedOff;
        }
    }

    void OnButtonTapped(long towerId)
    {
        var gameUI = GameUIManager.instance;
        if (gameUI.isBuilding)
        {
            gameUI.CancelGhostPlacement();
        }
        gameUI.SetToBuildMode(towerId);
    }

    void OnButtonDraggedOff(long towerId)
    {
        if (!GameUIManager.instance.isBuilding)
        {
            GameUIManager.instance.SetToDragMode(towerId);
        }
    }

    void OnDestroy()
    {
        TargetSpawnButton[] childButtons = GetComponentsInChildren<TargetSpawnButton>();

        foreach (TargetSpawnButton towerButton in childButtons)
        {
            towerButton.buttonTapped -= OnButtonTapped;
            towerButton.draggedOff -= OnButtonDraggedOff;
        }
    }


    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        XEventBus.Instance.Register(EventId.WaveStart, StartWaveButtonPressed);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        XEventBus.Instance.UnRegister(EventId.WaveStart, StartWaveButtonPressed);
    }

    public void StartWaveButtonPressed(XEventArgs args)
    {
        if (TargetDefense.Level.LevelManager.instanceExists)
        {
            TargetDefense.Level.LevelManager.instance.BuildingCompleted();
        }
    }

    public void AddCurrency(int amount)
    {
        if (TargetDefense.Level.LevelManager.instanceExists)
        {
            TargetDefense.Level.LevelManager.instance.currency.AddCurrency(amount);
        }
    }
}
