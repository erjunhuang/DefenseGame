using UnityEngine;

namespace ActionGameFramework.Health
{
	/// <summary>
	/// Damage trigger - a trigger based implementation of Damage zone
	/// </summary>
	[RequireComponent(typeof(Collider2D))]
	public class DamageTrigger : DamageZone
	{
		/// <summary>
		/// On entering the trigger see that the collider has a Damager component and if so make the damageableBehaviour take damage
		/// </summary>
		/// <param name="triggeredCollider">The collider that entered the trigger</param>
		protected void OnTriggerEnter2D(Collider2D triggeredCollider)
		{
			var damager = triggeredCollider.GetComponent<Damager>();
			if (damager == null)
			{
				return;
			}
			LazyLoad();
			
			float scaledDamage = ScaleDamage(damager.damage);
            //Vector3 collisionPosition = triggeredCollider.ClosestPoint(damager.transform.position);
            Vector3 collisionPosition = triggeredCollider.bounds.ClosestPoint(damager.transform.position);
             
            damageableBehaviour.TakeDamage(scaledDamage, collisionPosition, damager.alignmentProvider);
			
			damager.HasDamaged(collisionPosition, damageableBehaviour.configuration.alignmentProvider);
		}
	}
}