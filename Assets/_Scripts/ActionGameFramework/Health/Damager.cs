using System;
using Core.Health;
using Core.Utilities;
using QGame.Core.FightEnegin.Damage;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// A component that does damage to damageables
    /// </summary>
    /// 
    [SerializeField]
    public class Damager : MonoBehaviour
    {
        /// <summary>
        /// The amount of damage this damager does
        /// </summary>
        public int damage;

        public int heal;

        public BuffInfo buffInfo;

        /// <summary>
        /// The event that fires off when the damager has been damaged
        /// </summary>
        public Action<Vector3> hasDamaged;


        public Action<Vector3> damaged;

        /// <summary>
        /// The particle system to fire off when the damager attacks
        /// </summary>
        public ParticleSystem collisionParticles;

        public IAlignmentProvider alignmentProvider;

        /// <summary>
        /// Damagable will tell damager that it has successful hurt the damagable
        /// </summary>
        public void HasDamaged(Vector3 point, IAlignmentProvider otherAlignment)
        {
            if (hasDamaged != null)
            {
                hasDamaged(point);
            }
        }
	}
}