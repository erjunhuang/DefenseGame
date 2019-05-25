using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;
using UnityInput = UnityEngine.Input;
using State = TargetDefense.UI.HUD.GameUIManager.State;
using TargetDefense.UI.HUD;
using QGame.Core.Event;

public class DefenseKeyboardMouseInput : KeyboardMouseInput
{
    GameUIManager gameUIManager;
    protected override void OnEnable()
    {
        base.OnEnable();
        gameUIManager = GetComponent<GameUIManager>();
        if (!InputController.instanceExists)
        {
            Debug.LogError("[UI] Keyboard and Mouse UI requires InputController");
            return;
        }
        
        InputController controller = InputController.instance;

        controller.tapped += OnTap;
        controller.mouseMoved += OnMouseMoved;
    }

    /// <summary>
    /// Deregister input events
    /// </summary>
    protected override void OnDisable()
    {
        if (!InputController.instanceExists)
        {
            return;
        }

        InputController controller = InputController.instance;

        controller.tapped -= OnTap;
        controller.mouseMoved -= OnMouseMoved;
    }

    protected override void Update()
    {
        base.Update();

        // Escape handling
        if (UnityInput.GetKeyDown(KeyCode.Escape))
        {
            switch (gameUIManager.state)
            {
                case State.Normal:
                    if (gameUIManager.isTargetSelected)
                    {
                        gameUIManager.DeselectTarget();
                    }
                    else
                    {
                        gameUIManager.Pause();
                    }
                    break;
                case State.Building:
                    gameUIManager.DeselectTarget();
                    break;
            }
        }
    }

    /// <summary>
    /// Ghost follows pointer
    /// </summary>
    void OnMouseMoved(PointerInfo pointer)
    {
        // We only respond to mouse info
        var mouseInfo = pointer as MouseCursorInfo;

        if (mouseInfo != null&& !mouseInfo.startedOverUI)
        {
            if (gameUIManager.isBuilding)
            {
                gameUIManager.TryMoveGhost(mouseInfo);
            }
        }
    }

    /// <summary>
    /// Select towers or position ghosts
    /// </summary>
    void OnTap(PointerActionInfo pointer)
    {   
        // We only respond to mouse info
        var mouseInfo = pointer as MouseButtonInfo;

        if (mouseInfo != null && !mouseInfo.startedOverUI)
        {
            XEventBus.Instance.Post(EventId.UserClick);
            if (gameUIManager.isBuilding)
            {
                if (mouseInfo.mouseButtonId == 0) 
                {
                    //放置
                    gameUIManager.TryPlaceTarget(mouseInfo);
                }
                else // RMB cancels
                {
                    //取消动作
                    gameUIManager.CancelGhostPlacement();
                }
            }
            else {
                if (mouseInfo.mouseButtonId == 0) // LMB confirms
                {   
                    //尝试选择
                    gameUIManager.TrySelectTarget(mouseInfo);
                }
            }
        }
    }
}
