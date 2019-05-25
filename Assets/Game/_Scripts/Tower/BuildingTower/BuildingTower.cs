using ActionGameFramework.Health;
using Core.Utilities;
using QGame.Core.Event;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Targets.Placement;
using UnityEngine;

/// <summary>
/// Tower building and operation.
/// </summary>
public class BuildingTower : MonoBehaviour
{
    // Prefab for building tree
    public GameObject buildingTreePrefab;

    public GameObject buildingPlace;

    public Transform defendPoint;
    // User interface manager
    private UiManager uiManager;
    // Level UI canvas for building tree display
    private Canvas canvas;
    // Collider of this tower
    private Collider2D bodyCollider;
     
    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        XEventBus.Instance.Register(EventId.GamePaused, GamePaused);
        XEventBus.Instance.Register(EventId.UserClick, UserClick);
        XEventBus.Instance.Register(EventId.UserUiClick, UserClick);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {

        XEventBus.Instance.UnRegister(EventId.GamePaused, GamePaused);
        XEventBus.Instance.UnRegister(EventId.UserClick, UserClick);
        XEventBus.Instance.UnRegister(EventId.UserUiClick, UserClick);
    }

    /// <summary>
    /// Atart this instance.
    /// </summary>
    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
        // This canvas wiil use to place building tree UI
        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas canv in canvases)
        {
            if (canv.CompareTag("LevelUI"))
            {
                canvas = canv;
                break;
            }
        }
        bodyCollider = GetComponent<Collider2D>();
        Debug.Assert(uiManager && canvas && bodyCollider, "Wrong initial parameters");

    }

    /// <summary>
    /// Opens the building tree.
    /// </summary>
    private void OpenBuildingTree()
    {
        if (buildingTreePrefab != null)
        {
            // Create building tree
           GameObject activeBuildingTree = Instantiate<GameObject>(buildingTreePrefab, canvas.transform);
            // Set it over the tower
            activeBuildingTree.transform.position = Camera.main.WorldToScreenPoint(transform.position);

            // Disable tower raycast
            bodyCollider.enabled = false;
        }
    }

    /// <summary>
    /// Closes the building tree.
    /// </summary>
    private void CloseBuildingTree()
    {
      
    }

    /// <summary>
    /// Builds the tower.
    /// </summary>
    /// <param name="towerPrefab">Tower prefab.</param>
    public void BuildTower(GameObject towerPrefab)
    {
        // Close active building tree
        CloseBuildingTree();
        Tower target = towerPrefab.GetComponent<Tower>();
        int cost = target.currentTargetLevelData.Cost;
        bool successfulPurchase = TargetDefense.Level.LevelManager.instance.currency.TryPurchase(cost);
        if(successfulPurchase)
        {
            SingleTowerPlacementArea placementArea = Instantiate(buildingPlace).GetComponent<SingleTowerPlacementArea>();
            placementArea.transform.parent = transform.parent;
            placementArea.transform.position = transform.position;

            placementArea.transform.Find("DefendPoint").transform.position = defendPoint.position;

            Tower createdTarget = Instantiate(target);
            createdTarget.Initialize(placementArea, IntVector2.zero);
            createdTarget.transform.parent = placementArea.transform;

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Disable tower raycast and close building tree on game pause.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void GamePaused(XEventArgs args)
    {
        string param = args.GetData<string>(1);
        if (param == bool.TrueString) // Paused
        {
            CloseBuildingTree();
            bodyCollider.enabled = false;
        }
        else // Unpaused
        {
            bodyCollider.enabled = true;
        }
    }

    /// <summary>
    /// On user click.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void UserClick(XEventArgs args)
    {
        GameObject obj = args.GetData<GameObject>();
        if (obj == gameObject) // This tower is clicked
        {
            // Show attack range
            ShowRange(true);
           
            // Open building tree if it is not
            OpenBuildingTree();
        }
        else // Other click
        {
            // Hide attack range
            ShowRange(false);
            // Close active building tree
            CloseBuildingTree();
        }
    }

    /// <summary>
    /// Display tower's attack range.
    /// </summary>
    /// <param name="condition">If set to <c>true</c> condition.</param>
    private void ShowRange(bool condition)
    {
    }
}
