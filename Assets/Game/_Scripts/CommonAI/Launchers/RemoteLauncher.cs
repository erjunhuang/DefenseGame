using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using ActionGameFramework.Projectiles;
using UnityEngine;

namespace TargetDefense.Towers.TowerLaunchers
{
    /// <summary>
    /// An implementation of ILauncher that firest homing missiles
    /// </summary>
    public class RemoteLauncher : Launcher
    {
        public ParticleSystem fireParticleSystem;

        /// <summary>
        /// Launches homing missile at a target from a starting position
        /// </summary>
        /// <param name="enemy">
        /// The enemy to attack
        /// </param>
        /// <param name="attack">
        /// The projectile used to attack
        /// </param>
        /// <param name="firingPoint">
        /// The point the projectile is being fired from
        /// </param>
        public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint)
        {
            if (enemy != null)
            {
                // Create arrow
                attack.transform.position = firingPoint.position;
                attack.transform.rotation = firingPoint.rotation;
                IBullet bullet = attack.GetComponent<IBullet>();
                bullet.Initialize(enemy.transform);
            }
        }
    }
}