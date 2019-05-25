using ActionGameFramework.Health;
using GameModel;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Targets;
using TargetDefense.UI.HUD;
using UnityEngine;

public class TargetGhost : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    public long towerId { get; private set; }
    public Collider ghostCollider { get; private set; }
    public Monster defaultLevel;

    public virtual void Initialize(long towerId)
    {
        spriteRenderer =transform.Find("Body").GetComponent<SpriteRenderer>();
        this.towerId = towerId;
        if (GameUIManager.instanceExists)
        {
           defaultLevel = GameData.monsters[towerId];
           GameUIManager.instance.SetupRadiusVisualizer(defaultLevel, transform);
        }
        ghostCollider = GetComponent<Collider>();
        //spriteRenderer.sprite = controller.levels[controller.currentLevel].icon;
         
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show this ghost
    /// </summary>
    public virtual void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
