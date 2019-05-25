using ActionGameFramework.Health;
using Core.Health;
using QGame.Core.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Area Of Effect damage on destroing.
/// </summary>
public class AOE : MonoBehaviour
{
	// Percent of AOE damage in part of IBullet damage. 0f = 0%, 1f = 100%
	public float aoeDamageRate = 1f;
    // Area radius
    public float radius = 0.3f;
    // Explosion prefab
    public GameObject explosion;
    // Explosion visual duration
    public float explosionDuration = 1f;

	// IBullet component of this gameObject to get the damage amount
	private IBullet bullet;
    // Scene is closed now. Forbidden to create new objects on destroy
    private bool isQuitting;


    /// <summary>
    /// The alignment of the damager
    /// </summary>
    public SerializableIAlignmentProvider alignment;

    /// <summary>
    /// Gets the alignment of the damager
    /// </summary>
    public IAlignmentProvider alignmentProvider
    {
        get { return alignment != null ? alignment.GetInterface() : null; }
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
	{
		bullet = GetComponent<IBullet>();
		Debug.Assert(bullet != null, "Wrong initial settings");
	}

    /// <summary>
    /// Raises the enable event.
    /// </summary>
    void OnEnable()
    {
        XEventBus.Instance.Register(EventId.SceneQuit, SceneQuit);
    }

    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        XEventBus.Instance.UnRegister(EventId.SceneQuit, SceneQuit);
    }

    /// <summary>
    /// Raises the application quit event.
    /// </summary>
    void OnApplicationQuit()
    {
        isQuitting = true;
    }
    public virtual void OnRemove() {
        // If scene is in progress
        if (isQuitting == false)
        {
            // Find all colliders in specified radius
            //Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
            Collider[] cols = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider col in cols)
            {
                // If target can receive damage
                Targetable damageTaker = col.gameObject.GetComponent<Targetable>();
                if (damageTaker != null)
                {
                    // Target takes damage equal bullet damage * AOE Damage Rate
                    damageTaker.TakeDamage((int)(Mathf.Ceil(aoeDamageRate * (float)bullet.GetDamage())), col.transform.position, alignmentProvider);
                }
            }
            if (explosion != null)
            {
                // Create explosion visual effect
                Destroy(Instantiate<GameObject>(explosion, transform.position, transform.rotation), explosionDuration);
            }
        }
    }
    /// <summary>
    /// Raises on scene quit.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="param">Parameter.</param>
    private void SceneQuit(XEventArgs args)
    {
        isQuitting = true;
    }
}
