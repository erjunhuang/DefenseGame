using Core.Input;
using QGame.Core.Event;
using TargetDefense.UI.HUD;
using UnityEngine;

namespace TowerDefense.Input
{
    public class DefenseTouchInput : TouchInput
    {
        GameUIManager gameUIManager;
        protected override void OnEnable()
        {
            base.OnEnable();
            gameUIManager = GetComponent<GameUIManager>();
            if (InputController.instanceExists)
            {
                InputController.instance.tapped += OnTap;
                InputController.instance.dragged += OnDrag;
            }
        }

        /// <summary>
        /// Deregister input events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (InputController.instanceExists)
            {
                InputController.instance.tapped -= OnTap;
                InputController.instance.dragged -= OnDrag;
            }
        }

        /// <summary>
        /// Hide UI 
        /// </summary>
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// Decay flick
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Called on tap,
        /// calls confirmation of tower placement
        /// </summary>
        protected virtual void OnTap(PointerActionInfo pointer)
        {
            XEventBus.Instance.Post(EventId.UserClick);
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null && !touchInfo.startedOverUI)
            {
                if (!gameUIManager.isBuilding) {
                    gameUIManager.TrySelectTarget(touchInfo);
                }
            }
        }

        protected override void OnDrag(PointerActionInfo pointer)
        {
            base.OnDrag(pointer);
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null && !touchInfo.startedOverUI)
            {
                if (gameUIManager.isBuilding)
                {
                    gameUIManager.TryMoveGhost(touchInfo);
                }
            }
        }
        protected override void OnRelease(PointerActionInfo pointer)
        {
            base.OnRelease(pointer);
            var touchInfo = pointer as TouchInfo;
            if (touchInfo != null && !touchInfo.startedOverUI)
            {
                if (gameUIManager.isBuilding)
                {
                    gameUIManager.CancelGhostPlacement();
                }
            }
        }
         
    }
}