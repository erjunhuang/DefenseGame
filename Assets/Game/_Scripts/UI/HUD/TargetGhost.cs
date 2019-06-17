using ActionGameFramework.Health;
using GameModel;
using QGame.Core.Config;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Targets;
using TargetDefense.UI.HUD;
using UnityEngine;

public class TargetGhost : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;
    public int towerId { get; private set; }
    public Collider ghostCollider { get; private set; }
    public MonsterCfg defaultLevel;

    public virtual void Initialize(int towerId)
    {
        spriteRenderer =transform.Find("Body").GetComponent<SpriteRenderer>();
        this.towerId = towerId;
        if (GameUIManager.instanceExists)
        {
            defaultLevel = ConfigService.Instance.MonsterCfgList.GetOne(towerId);
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
