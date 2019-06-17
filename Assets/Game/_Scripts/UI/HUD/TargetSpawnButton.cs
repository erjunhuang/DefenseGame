using GameModel;
using QGame.Core.Config;
using System;
using TargetDefense.Targets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A button controller for spawning towers
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TargetSpawnButton : MonoBehaviour, IDragHandler
{
    /// <summary>
    /// The text attached to the button
    /// </summary>
    public Text buttonText;

    public Image towerIcon;

    public Button buyButton;

    public Image energyIcon;

    public Color energyDefaultColor;

    public Color energyInvalidColor;

    public event Action<int> buttonTapped;

    public event Action<int> draggedOff;


    int towerId;

    RectTransform m_RectTransform;

 

    public void InitializeButton(int towerId)
    {
        this.towerId = towerId;
        MonsterCfg monster = ConfigService.Instance.MonsterCfgList.GetOne(towerId);
        buttonText.text = monster.Cost.ToString();
        UpdateButton();
    }

    /// <summary>
    /// Cache the rect transform
    /// </summary>
    protected virtual void Awake()
    {
        m_RectTransform = (RectTransform)transform;
    }

    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    protected virtual void OnDestroy()
    {
    }

    /// <summary>
    /// The click for when the button is tapped
    /// </summary>
    public void OnClick()
    {
        if (buttonTapped != null)
        {
            buttonTapped(towerId);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(m_RectTransform, eventData.position))
        {
            if (draggedOff != null)
            {
                draggedOff(towerId);
            }
        }
    }

    /// <summary>
    /// Update the button's button state based on cost
    /// </summary>
    void UpdateButton()
    {
        buyButton.interactable = true;
        energyIcon.color = energyDefaultColor;
    }
}
