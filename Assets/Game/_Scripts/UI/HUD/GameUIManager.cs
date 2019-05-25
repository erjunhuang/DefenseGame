using Core.Input;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Core.Utilities;
using JetBrains.Annotations;
using TargetDefense.Targets.Placement;
using Core.Health;
using TargetDefense.Targets;
using TargetDefense.Level;
using System.Collections.Generic;
using GameModel;
using QGame.Core.Event;

namespace TargetDefense.UI.HUD
{
    public struct UIPointer
    {
        public PointerInfo pointer;
        public Ray ray;
        public RaycastHit? raycast;
        public bool overUI;
    }

    public class GameUIManager : Singleton<GameUIManager>
    {
        public enum State
        {
            Normal,
            Building,
            Paused,
            GameOver,
            BuildingWithDrag
        }

        public List<SingleTowerPlacementArea> IPlacementAreas;

        public State state { get; private set; }

        public LayerMask placementAreaMask;
        public LayerMask ghostWorldPlacementMask;

        public RadiusVisualizerController radiusVisualizerController;

        public Action<State, State> stateChanged;

        private TowerAgent currentSelectedTarget;
        public Action<TowerAgent> selectionChanged;



        private TargetGhost m_CurrentGhost;

        IPlacementArea m_CurrentArea;
        IntVector2 m_GridPosition;
        bool m_GhostPlacementPossible;

        public bool isTargetSelected
        {
            get { return currentSelectedTarget != null; }
        }

        public bool isBuilding
        {
            get
            {
                return state == State.Building || state == State.BuildingWithDrag;
            }
        }


        // Use this for initialization
        void Start()
        {
            //测试
            SetState(State.Normal);
        }
        public void TrySelectTarget(PointerInfo info) {

            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to select towers outside of Normal state");
            }
            UIPointer uiPointer = WrapPointer(info);
            if (uiPointer.overUI)
            {
                return;
            }
            RaycastHit hit;
            if (Physics.Raycast(uiPointer.ray, out hit, float.MaxValue))
            {
                switch (hit.collider.tag) {
                    case "Tower":
                    case "Enemy":
                        XEventBus.Instance.Post(EventId.UserClick, new XEventArgs(hit.collider.gameObject));
                        TowerAgent tower = hit.collider.GetComponent<TowerAgent>();
                        if (tower != null) {
                            SelectTarget(tower);
                        }
                        break;
                }
            }
        }

        public void SelectTarget(TowerAgent target)
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to select whilst not in a normal state");
            }
            DeselectTarget();
            currentSelectedTarget = target;
            if (currentSelectedTarget != null)
            {
                currentSelectedTarget.aIBehaviors.removed += OnTargetDied;
            }
            Monster monster = target.currentTargetLevelData;
            radiusVisualizerController.SetupRadiusVisualizers(monster,target.transform);
            if (selectionChanged != null)
            {
                selectionChanged(target);
            }
        }
        public void UpgradeSelectedTarget()
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to upgrade whilst not in Normal state");
            }
            if (currentSelectedTarget == null)
            {
                throw new InvalidOperationException("Selected Target is null");
            }
            if (currentSelectedTarget.isAtMaxLevel)
            {
                return;
            }
            int upgradeCost = currentSelectedTarget.GetCostForNextLevel();
            bool successfulUpgrade = TargetDefense.Level.LevelManager.instance.currency.TryPurchase(upgradeCost);
            if (successfulUpgrade) {
                currentSelectedTarget.UpgradeTarget();
            }
             
            DeselectTarget();
        }

        public void SellSelectedTarget()
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to sell tower whilst not in Normal state");
            }
            if (currentSelectedTarget == null)
            {
                throw new InvalidOperationException("Selected Target is null");
            }
            int sellValue = currentSelectedTarget.GetSellLevel();
            if (TargetDefense.Level.LevelManager.instanceExists && sellValue > 0)
            {
                TargetDefense.Level.LevelManager.instance.currency.AddCurrency(sellValue);
                currentSelectedTarget.Sell();
            }
            DeselectTarget();
        }

        public void DeselectTarget(bool isRemoveSelectTarget = false)
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to deselect tower whilst not in Normal state");
            }

            if (currentSelectedTarget != null)
            {
                currentSelectedTarget.aIBehaviors.removed -= OnTargetDied;
                currentSelectedTarget.Show();
                if (isRemoveSelectTarget) {
                    currentSelectedTarget.Sell();
                }
            }

            currentSelectedTarget = null;
            if (selectionChanged != null)
            {
                selectionChanged(null);
            }

            radiusVisualizerController.HideRadiusVisualizers();
        }


        protected void OnTargetDied(DamageableBehaviour targetable)
        {
            DeselectTarget();
        }

        public void GameOver()
        {
            SetState(State.GameOver);
        }

        public void Pause()
        {
            SetState(State.Paused);
        }

        public void Unpause()
        {
            SetState(State.Normal);
        }

        void SetState(State newState)
        {
            if (state == newState)
            {
                return;
            }
            State oldState = state;
            if (oldState == State.Paused || oldState == State.GameOver)
            {
                Time.timeScale = 1f;
            }

            switch (newState)
            {
                case State.Normal:
                    break;
                case State.Building:
                    break;
                case State.BuildingWithDrag:
                    break;
                case State.Paused:
                case State.GameOver:
                    if (oldState == State.Building)
                    {
                        CancelGhostPlacement();
                    }
                    Time.timeScale = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newState", newState, null);
            }
            state = newState;
            if (stateChanged != null)
            {
                stateChanged(oldState, state);
            }
        }

        protected UIPointer WrapPointer(PointerInfo pointerInfo)
        {
            return new UIPointer
            {
                overUI = IsOverUI(pointerInfo),
                pointer = pointerInfo,
                ray = Camera.main.ScreenPointToRay(pointerInfo.currentPosition)
            };
        }

        protected bool IsOverUI(PointerInfo pointerInfo)
        {
            int pointerId;
            EventSystem currentEventSystem = EventSystem.current;

            // Pointer id is negative for mouse, positive for touch
            var cursorInfo = pointerInfo as MouseCursorInfo;
            var mbInfo = pointerInfo as MouseButtonInfo;
            var touchInfo = pointerInfo as TouchInfo;

            if (cursorInfo != null)
            {
                pointerId = PointerInputModule.kMouseLeftId;
            }
            else if (mbInfo != null)
            {
                // LMB is 0, but kMouseLeftID = -1;
                pointerId = -mbInfo.mouseButtonId - 1;
            }
            else if (touchInfo != null)
            {
                pointerId = touchInfo.touchId;
            }
            else
            {
                throw new ArgumentException("Passed pointerInfo is not a TouchInfo or MouseCursorInfo", "pointerInfo");
            }

            return currentEventSystem.IsPointerOverGameObject(pointerId);
        }


        //设置Ghost
        public void SetToDragMode([NotNull] long towerId)
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to enter drag mode when not in Normal mode");
            }

            if (m_CurrentGhost != null)
            {
                // Destroy current ghost
                CancelGhostPlacement();
            }
            SetUpGhostTarget(towerId);
            SetState(State.BuildingWithDrag);
        }
         
        public void SetToBuildMode([NotNull] long towerId)
        {
            if (state != State.Normal)
            {
                throw new InvalidOperationException("Trying to enter Build mode when not in Normal mode");
            }

            if (m_CurrentGhost != null)
            {
                // Destroy current ghost
                CancelGhostPlacement();
            }
            SetUpGhostTarget(towerId);
            SetState(State.Building);
        }

        void SetUpGhostTarget([NotNull] long towerId)
        {
            //if (towerId == null)
            //{
            //    throw new ArgumentNullException("towerId");
            //}
            m_CurrentGhost = Instantiate<GameObject>(Resources.Load<GameObject>("Prefab/Game/Target_Ghost")).GetComponent<TargetGhost>();
            m_CurrentGhost.Initialize(towerId);
            m_CurrentGhost.Hide();
        }

        public void SetupRadiusVisualizer(Monster target, Transform ghost = null)
        {
            radiusVisualizerController.SetupRadiusVisualizers(target, ghost);
        }

        /// <summary>
        /// Hides the radius visualizer
        /// </summary>
        public void HideRadiusVisualizer()
        {
            radiusVisualizerController.HideRadiusVisualizers();
        }

        public void CancelGhostPlacement(bool isRemoveSelectTarget = false)
        {
            if (!isBuilding)
            {
                throw new InvalidOperationException("Can't cancel out of ghost placement when not in the building state.");
            }
            Destroy(m_CurrentGhost.gameObject);
            m_CurrentGhost = null;
            SetState(State.Normal);
            DeselectTarget(isRemoveSelectTarget);
        }
        //移动Ghost
        public void TryMoveGhost(PointerInfo pointerInfo, bool hideWhenInvalid = false)
        {
            
            if (m_CurrentGhost == null)
            {
                throw new InvalidOperationException("Trying to move the Target ghost when we don't have one");
            }
            UIPointer pointer = WrapPointer(pointerInfo);
            // Do nothing if we're over UI
            if (pointer.overUI && hideWhenInvalid)
            {
                m_CurrentGhost.Hide();
                return;
            }
            MoveGhost(pointer, hideWhenInvalid);
        }
        protected void MoveGhost(UIPointer pointer, bool hideWhenInvalid = false)
        {
            if (m_CurrentGhost == null || !isBuilding)
            {
                throw new InvalidOperationException(
                    "Trying to position a Target ghost while the UI is not currently in the building state.");
            }

            PlacementAreaRaycast(ref pointer);
            if (pointer.raycast != null)
            {
                MoveGhostWithRaycastHit(pointer.raycast.Value);
            }
            else
            {
                MoveGhostOntoWorld(pointer.ray, hideWhenInvalid);
            }
        }

        protected void PlacementAreaRaycast(ref UIPointer pointer)
        {
            pointer.raycast = null;

            if (pointer.overUI)
            {
                return;
            }
            //RaycastHit2D hit = Physics2D.Raycast(pointer.ray.origin, Vector2.zero, float.MaxValue, placementAreaMask);
            //if (hit)
            //{
            //    pointer.raycast = hit;
            //}

            RaycastHit hit;
            if (Physics.Raycast(pointer.ray, out hit, float.MaxValue, placementAreaMask))
            {
                pointer.raycast = hit;
            }
        }

        protected virtual void MoveGhostWithRaycastHit(RaycastHit raycast)
        {

            //m_CurrentArea = raycast.collider.GetComponent<BuildingPlaceDisplayMappingBind>().GetBindGameObject().GetComponent<IPlacementArea>();
            m_CurrentArea = raycast.collider.GetComponent<IPlacementArea>();
            if (m_CurrentArea == null)
            {
                Debug.LogError("There is not an IPlacementArea attached to the collider found on the m_PlacementAreaMask");
                return;
            }
           
            m_GridPosition = m_CurrentArea.WorldToGrid(raycast.point);
            TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition);

            m_CurrentGhost.Show();
            m_GhostPlacementPossible = fits == TowerFitStatus.Fits && IsValidPurchase();
            if (m_GhostPlacementPossible)
            {
                //print("可以放置");
            }
            m_CurrentGhost.transform.position = raycast.point;
        }

        public bool IsValidPurchase()
        {
            if (!isBuilding)
            {
                throw new InvalidOperationException("Trying to check ghost position when not in a build mode");
            }
            if (m_CurrentGhost == null)
            {
                return false;
            }
            if (m_CurrentArea == null)
            {
                return false;
            }
            return TargetDefense.Level.LevelManager.instance.currency.CanAfford(m_CurrentGhost.defaultLevel.Cost);
        }

        protected virtual void MoveGhostOntoWorld(Ray ray, bool hideWhenInvalid)
        {
            if (!hideWhenInvalid)
            {
                //RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, float.MaxValue, ghostWorldPlacementMask);
                //RaycastHit hit;
                //if (Physics.Raycast(ray.origin,out hit,float.MaxValue, ghostWorldPlacementMask)) {
                //    m_CurrentGhost.Show();
                //    m_CurrentGhost.transform.position = hit.point;
                //}
                RaycastHit hit;
                // check against all layers that the ghost can be on
                Physics.SphereCast(ray, 1, out hit, float.MaxValue, ghostWorldPlacementMask);
                if (hit.collider == null)
                {
                    return;
                }
                m_CurrentGhost.Show();
                m_CurrentGhost.transform.position = hit.point;

            }
            else
            {
                m_CurrentGhost.Hide();
            }
        }

        //根据Ghost放置Target

        public void TryPlaceTarget(PointerInfo pointerInfo)
        {
            UIPointer pointer = WrapPointer(pointerInfo);

            // Do nothing if we're over UI
            if (pointer.overUI)
            {
                return;
            }
            BuyTarget(pointer);
        }

        public void BuyTarget(UIPointer pointer)
        {
            if (!isBuilding)
            {
                throw new InvalidOperationException("Trying to buy towers when not in a Build Mode");
            }
            if (m_CurrentGhost == null)
            {
                return;
            }
            PlacementAreaRaycast(ref pointer);
            if (!pointer.raycast.HasValue || pointer.raycast.Value.collider == null)
            {
                CancelGhostPlacement();
                return;
            }

            
            PlaceTargetGhost(pointer);
        }

        public bool IsGhostAtValidPosition()
        {
            if (!isBuilding)
            {
                throw new InvalidOperationException("Trying to check ghost position when not in a build mode");
            }
            if (m_CurrentGhost == null)
            {
                return false;
            }
            if (m_CurrentArea == null)
            {
                return false;
            }
            TowerFitStatus fits = m_CurrentArea.Fits(m_GridPosition);
            return fits == TowerFitStatus.Fits;
        }

        protected void PlaceTargetGhost(UIPointer pointer)
        {
            if (m_CurrentGhost == null || !isBuilding)
            {
                throw new InvalidOperationException(
                    "Trying to position a Target ghost while the UI is not currently in a building state.");
            }

            MoveGhost(pointer);

            if (m_CurrentArea != null)
            {
                bool  isFits = IsGhostAtValidPosition();

                if (isFits)
                {
                    if (currentSelectedTarget)
                    {
                        currentSelectedTarget.UpdateTargetPos(m_CurrentArea, m_GridPosition);
                        currentSelectedTarget.Show();
                        CancelGhostPlacement();
                    }
                    else
                    {
                        int cost = m_CurrentGhost.defaultLevel.Cost;
                        bool successfulPurchase = TargetDefense.Level.LevelManager.instance.currency.TryPurchase(cost);
                        if (successfulPurchase)
                        {
                            GameObject createdTarget = Instantiate(Resources.Load<GameObject>("Prefab/Game/Tower"), m_CurrentArea.transform);

                            TowerAgent tower = createdTarget.GetComponent<TowerAgent>();
                            object[] myObjArray = {m_CurrentArea,m_GridPosition};
                            tower.Initialize(m_CurrentGhost.towerId, myObjArray);

                            CancelGhostPlacement();
                        }
                        else
                        {
                            KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "金币不足" });
                            CancelGhostPlacement();
                        }
                    }
                }
                else {
                    if(!currentSelectedTarget) return;
                    currentSelectedTarget.UpdateTargetPos(m_CurrentArea, m_GridPosition);
                    currentSelectedTarget.Show();
                    CancelGhostPlacement();

                    Tower target = m_CurrentArea.GetController().GetComponent<Tower>();
                    if (target != currentSelectedTarget)
                    {
                        if (target.currentLevel == currentSelectedTarget.currentLevel)
                        {
                            if (!target.isAtMaxLevel)
                            {
                                int upgradeCost = target.GetCostForNextLevel();
                                bool successfulUpgrade = TargetDefense.Level.LevelManager.instance.currency.TryPurchase(upgradeCost);
                                if (successfulUpgrade)
                                {
                                    target.UpgradeTarget();
                                    CancelGhostPlacement(true);
                                }
                                else
                                {
                                    KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "没钱升什么级" });
                                    CancelGhostPlacement();
                                }
                            }
                            else
                            {
                                KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "炮台已经是最高等级了" });
                            }
                        }
                        else
                        {
                            CancelGhostPlacement();
                            KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "不是同一级别的炮台" });
                        }
                    }
                    else
                    {
                        CancelGhostPlacement();
                    }
                }
            }
        }

        //点击按钮直接放置Target
        public void OnTryPlaceTargetClick()
        {
            //foreach (SingleTowerPlacementArea placementArea in IPlacementAreas)
            //{
            //    TowerFitStatus fits = placementArea.Fits(IntVector2.zero, IntVector2.zero);
            //    if (fits == TowerFitStatus.Fits)
            //    {
            //        Tower target = GameData.TowerLibrary.configurations[0];
            //        int cost = target.purchaseCost;
            //        bool successfulPurchase = TargetDefense.Level.LevelManager.instance.currency.TryPurchase(cost);
            //        if (successfulPurchase)
            //        {
            //            Tower createdTarget = Instantiate(target);
            //            createdTarget.Initialize(placementArea, IntVector2.zero);
            //            createdTarget.transform.parent = placementArea.transform;
            //        }
            //        else
            //        {
            //            KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "没钱放置炮台了" });
            //        }
            //        return;
            //    }
            //}

            //KindlyReminderUI.instance.ShowTip(new ShowTipParams { msg = "没地盘了" });
        }
    }
}
