using ActionGameFramework.Health;
using AIBehavior;
using Core.Health;
using Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Arrow fly trajectory.
/// </summary>
public class BulletArrow : MonoBehaviour, IBullet
{
    // Maximum life time
    public float lifeTime = 3f;
    // Starting speed
    public float speed = 3f;
    // Constant acceleration
    public float speedUpOverTime = 0.5f;
    // If target is close than this distance - it will be hitted
    public float hitDistance = 0.2f;
    // Ballistic trajectory offset (in distance to target)
    public float ballisticOffset = 0.5f;
    // Do not rotate bullet during fly
    public bool freezeRotation = false;
	// This bullet don't deal damage to single target. Only AOE damage if it is
	public bool aoeDamageOnly = false;

    // From this position bullet was fired
    private Vector3 originPoint;
    // Aimed target
    private Transform targetable;
    // Last target's position
    private Vector3 myVirtualPosition;
    // Position on last frame
    private Vector3 myPreviousPosition;
    // Counter for acceleration calculation
    private float counter;
    // Image of this bullet
    private SpriteRenderer m_SpriteRenderer;

    private Damager m_Damager;

    private AOE m_AOE;

    public int GetDamage()
    {
      return  m_Damager.damage;
    }

    public virtual void Initialize(Transform targetable) {
        LazyLoad();
        counter = 0;
        m_SpriteRenderer.enabled = false;
        originPoint = myVirtualPosition = myPreviousPosition = transform.position;
        this.targetable = targetable;
    }
    private void Awake()
    {
        LazyLoad();
    }

    protected virtual void LazyLoad()
    {
        if (m_AOE == null) {
            m_AOE = GetComponent<AOE>();
        }
        if (m_SpriteRenderer == null)
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (m_Damager == null)
        {
            m_Damager = GetComponent<Damager>();
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void FixedUpdate ()
    {
        if (targetable == null) return;
        
        counter += Time.fixedDeltaTime;
        if (counter >= lifeTime)
        {
            counter = 0;
            Poolable.TryPool(gameObject);
        }
        // Add acceleration
        speed += Time.fixedDeltaTime * speedUpOverTime;

        // Calculate distance from firepoint to aim
        Vector3 originDistance = targetable.position - originPoint;
        // Calculate remaining distance
        Vector3 distanceToAim = targetable.position - (Vector3)myVirtualPosition;
        // Move towards aim
        myVirtualPosition = Vector3.Lerp(originPoint, targetable.position, counter * speed / originDistance.magnitude);
        // Add ballistic offset to trajectory
        transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);

        // Rotate bullet towards trajectory
        LookAtDirection2D((Vector3)transform.position - myPreviousPosition);

        myPreviousPosition = transform.position;

        m_SpriteRenderer.enabled = true;
        // Close enough to hit
        if (distanceToAim.magnitude <= hitDistance)
        {
            if (targetable != null)
            {
				// If bullet must deal damage to single target
				if (aoeDamageOnly == false)
				{
                    targetable.GetComponent<LevelAgent>().Damage(m_Damager.damage, targetable.transform.position, m_Damager.alignmentProvider);
				}
            }
            // Destroy bullet
            if (m_AOE != null) {
                m_AOE.OnRemove();
            }
            Poolable.TryPool(gameObject);
        }
    }

    /// <summary>
    /// Looks at direction2d.
    /// </summary>
    /// <param name="direction">Direction.</param>
    private void LookAtDirection2D(Vector3 direction)
    {
        if (freezeRotation == false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Adds ballistic offset to trajectory.
    /// </summary>
    /// <returns>The ballistic offset.</returns>
    /// <param name="originDistance">Origin distance.</param>
    /// <param name="distanceToAim">Distance to aim.</param>
    private Vector3 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            // Calculate sinus offset
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            // Add offset to trajectory
            return (Vector3)myVirtualPosition + (ballisticOffset * offset * Vector3.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }
}
