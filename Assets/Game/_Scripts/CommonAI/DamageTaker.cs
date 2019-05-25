using ActionGameFramework.Health;
using Core.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This target can receive damage.
/// </summary>
public class DamageTaker : MonoBehaviour
{
    // Damage visual effect duration
    public float damageDisplayTime = 0.2f;
    // Helth bar object
    public Transform healthBar;
    // Image of this object
    public SpriteRenderer sprite;
    // Visualisation of hit or heal is in progress
	private bool coroutineInProgress;
	// Original width of health bar (full hp)
    private float originHealthBarWidth;

    private DamageableBehaviour m_DamageableBehaviour;
    /// <summary>
    /// Awake this instance.
    /// </summary>
    public virtual void Initialize()
    {
        if (m_DamageableBehaviour == null)
        {
            m_DamageableBehaviour = GetComponent<DamageableBehaviour>();
        }
        m_DamageableBehaviour.configuration.healthChanged += OnHealthChanged;
        originHealthBarWidth = healthBar.localScale.x;
    }

    private void OnDestroy()
    {
        m_DamageableBehaviour.configuration.healthChanged -= OnHealthChanged;
    }
    public void OnHealthChanged(HealthChangeInfo healthChangeInfo) {
        UpdateHealthBar(healthChangeInfo);
        StartCoroutine(DisplayDamage());
    }

    /// <summary>
    /// Updates the health bar width.
    /// </summary>
    private void UpdateHealthBar(HealthChangeInfo healthChangeInfo)
    {
        Damageable damageable = healthChangeInfo.damageable;
        float healthBarWidth = originHealthBarWidth * damageable.currentHealth / damageable.startingHealth;
        healthBar.localScale = new Vector2(healthBarWidth, healthBar.localScale.y);
    }

    /// <summary>
    /// Damage visualisation.
    /// </summary>
    /// <returns>The damage.</returns>
    IEnumerator DisplayDamage()
    {
        if (coroutineInProgress) yield return null;

        coroutineInProgress = true;
        Color originColor = sprite.color;
        float counter;
        // Set color to black and return to origin color over time
		for (counter = 0f; counter < damageDisplayTime; counter += Time.fixedDeltaTime)
        {
            sprite.color = Color.Lerp(originColor, Color.black, Mathf.PingPong(counter, damageDisplayTime));
			yield return new WaitForFixedUpdate();
        }
        sprite.color = originColor;
        coroutineInProgress = false;
    }
}
