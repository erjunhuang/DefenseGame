using ActionGameFramework.Health;
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
public class WorldBulletArrow : MonoBehaviour, IBullet
{
    // Damage amount
	[HideInInspector] int damage = 1;
  
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
    /// Set damage amount for this bullet.
    /// </summary>
    /// <param name="damage">Damage.</param>
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

	/// <summary>
	/// Get damage amount for this bullet.
	/// </summary>
	/// <returns>The damage.</returns>
	public int GetDamage()
	{
		return damage;
	}

    private void Awake()
    {
    }
    private Transform target;

    private Vector3 speed = Vector3.zero;
    private float time;
    private float g = -10.0f;
    public void Fire(Transform target)
    {
        this.target = target;
        time = 1f;
        speed.x = (target.position.x - transform.position.x) / time;
        speed.z = (target.position.z - transform.position.z) / time;
        speed.y = (target.position.y - transform.position.y) / time - g * time / 2; // 竖直方向的位移方程是： y = vt-gt^2/2

        Destroy(gameObject,2f);
    }
    private float delaTime;
    private Vector3 G = Vector3.zero;
    private Vector3 currentAngle = Vector3.zero;
    void FixedUpdate ()
    {
        delaTime += Time.deltaTime;
        if (delaTime > time)
        {
            return;
        }
        G.y = g * delaTime;

        transform.localPosition += (G + speed) * Time.deltaTime;

        Vector2 v = G + speed;
        currentAngle.z = Angle(new Vector3(1, 0, 0), new Vector3(v.x, v.y, 0));
        transform.localEulerAngles = currentAngle;

        //Vector3 movePos = Vector3.MoveTowards(transform.position, this.target.transform.position, 1f * Time.fixedDeltaTime);
        //transform.position = movePos;
    }

    public  float Angle(Vector3 from, Vector3 to)
    {
        if (Vector3.Cross(from, to).z > 0)
        {
            return Vector2.Angle(from, to);
        }
        else
        {
            return -Vector2.Angle(from, to);
        }
    }

    public void Initialize(Transform target)
    {
        throw new NotImplementedException();
    }
}
