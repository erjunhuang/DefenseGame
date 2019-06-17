using System;
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
    private void Awake()
    {
        XEventBus.Instance.Register(EventId.SpawningEnemies, SpawningEnemies);
    }

    private void SpawningEnemies(XEventArgs args)
    {
        foreach (int towerId in GameData.gameInfo.towersInfo)
        {
            TargetSpawnButton button = Instantiate(towerSpawnButton, transform);
            button.InitializeButton(towerId);
            button.buttonTapped += OnButtonTapped;
            button.draggedOff += OnButtonDraggedOff;
        }
    }

    void OnButtonTapped(int towerId)
    {
        var gameUI = GameUIManager.instance;
        if (gameUI.isBuilding)
        {
            gameUI.CancelGhostPlacement();
        }
        gameUI.SetToBuildMode(towerId);
    }

    void OnButtonDraggedOff(int towerId)
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

        XEventBus.Instance.UnRegister(EventId.SpawningEnemies, SpawningEnemies);
    }
 
}
