using System;
using System.Collections.Generic;
using ActionGameFramework.Health;
using Core.Health;
using QGame.Core.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TargetDefense.Targetting
{
    /// <summary>
    /// Class used to track targets for an affector
    /// </summary>
    public class Targetter : MonoBehaviour
    {
        /// <summary>
        /// Fires when a targetable enters the target collider
        /// </summary>
        public event Action<Targetable> targetEntersRange;

        /// <summary>
        /// Fires when a targetable exits the target collider
        /// </summary>
        public event Action<Targetable> targetExitsRange;

        /// <summary>
        /// Fires when an appropriate target is found
        /// </summary>
        public event Action<Targetable> acquiredTarget;

        /// <summary>
        /// Fires when the current target was lost
        /// </summary>
        public event Action lostTarget;

        /// <summary>
        /// The search rate in searches per second
        /// </summary>
        public float searchRate;


        /// <summary>
        /// The collider attached to the targetter
        /// </summary>
        public SphereCollider attachedCollider;

        /// <summary>
        /// The current targetables in the collider
        /// </summary>
        protected List<Targetable> m_TargetsInRange = new List<Targetable>();

        /// <summary>
        /// The seconds until a search is allowed
        /// </summary>
        protected float m_SearchTimer = 0.0f;

        /// <summary>
        /// The current targetable
        /// </summary>
        public Targetable m_CurrrentTargetable;


        /// <summary>
        /// If there was a targetable in the last frame
        /// </summary>
        protected bool m_HadTarget;

        /// <summary>
        /// returns the radius of the collider whether
        /// its a sphere or capsule
        /// </summary>
        public float effectRadius
        {
            get
            {
                var sphere = attachedCollider as SphereCollider;
                if (sphere != null)
                {
                    return sphere.radius;
                }
                return 0;
            }
        }

        /// <summary>
        /// The alignment of the affector
        /// </summary>
        public IAlignmentProvider alignment;

        /// <summary>
        /// Returns the current target
        /// </summary>
        public Targetable GetTarget()
        {
            return m_CurrrentTargetable;
        }

        public List<Targetable> GetTargetsInRange()
        {
            return m_TargetsInRange;
        }


        /// <summary>
        /// Clears the list of current targets and clears all events
        /// </summary>
        public virtual void ResetTargetter()
        {
            m_TargetsInRange.Clear();
            m_CurrrentTargetable = null;

            targetEntersRange = null;
            targetExitsRange = null;
            acquiredTarget = null;
            lostTarget = null;
        }

        /// <summary>
        /// Returns all the targets within the collider. This list must not be changed as it is the working
        /// list of the targetter. Changing it could break the targetter
        /// </summary>
        public List<Targetable> GetAllTargets()
        {
            return m_TargetsInRange;
        }

        /// <summary>
        /// Checks if the targetable is a valid target
        /// </summary>
        /// <param name="targetable"></param>
        /// <returns>true if targetable is vaild, false if not</returns>
        protected virtual bool IsTargetableValid(Targetable targetable)
        {
            if (targetable == null)
            {
                return false;
            }

            IAlignmentProvider targetAlignment = targetable.configuration.alignmentProvider;
            bool canDamage = alignment == null || targetAlignment == null ||
                             alignment.CanHarm(targetAlignment);

            return canDamage;
        }

        /// <summary>
        /// On exiting the trigger, a valid targetable is removed from the tracking list.
        /// </summary>
        /// <param name="other">The other collider in the collision</param>
        protected virtual void OnTriggerExit(Collider other)
        {
            var targetable = other.GetComponent<Targetable>();
            if (!IsTargetableValid(targetable))
            {
                return;
            }

            m_TargetsInRange.Remove(targetable);
            if (targetExitsRange != null)
            {
                targetExitsRange(targetable);
            }
            if (targetable == m_CurrrentTargetable)
            {
                OnTargetRemoved(targetable);
            }
            else
            {
                // Only need to remove if we're not our actual target, otherwise OnTargetRemoved will do the work above
                targetable.removed -= OnTargetRemoved;
            }
        }

        /// <summary>
        /// On entering the trigger, a valid targetable is added to the tracking list.
        /// </summary>
        /// <param name="other">The other collider in the collision</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            var targetable = other.GetComponent<Targetable>();
            if (!IsTargetableValid(targetable))
            {
                return;
            }
            targetable.removed += OnTargetRemoved;
            m_TargetsInRange.Add(targetable);
            if (targetEntersRange != null)
            {
                targetEntersRange(targetable);
            }
        }

        /// <summary>
        /// Returns the nearest targetable within the currently tracked targetables 
        /// </summary>
        /// <returns>The nearest targetable if there is one, null otherwise</returns>
        protected virtual Targetable GetNearestTargetable()
        {
            int length = m_TargetsInRange.Count;

            if (length == 0)
            {
                return null;
            }

            Targetable nearest = null;
            float distance = float.MaxValue;
            for (int i = length - 1; i >= 0; i--)
            {
                Targetable targetable = m_TargetsInRange[i];
                if (targetable == null || targetable.isDead)
                {
                    m_TargetsInRange.RemoveAt(i);
                    continue;
                }
                float currentDistance = Vector3.Distance(transform.position, targetable.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    nearest = targetable;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Starts the search timer
        /// </summary>
        protected virtual void Start()
        {
            m_SearchTimer = searchRate;
        }

        /// <summary>
        /// Checks if any targets are destroyed and aquires a new targetable if appropriate
        /// </summary>
        protected virtual void Update()
        {
            m_SearchTimer -= Time.deltaTime;

            if (m_SearchTimer <= 0.0f && m_CurrrentTargetable == null && m_TargetsInRange.Count > 0)
            {
                m_CurrrentTargetable = GetNearestTargetable();
                if (m_CurrrentTargetable != null)
                {
                    AcquiredTarget();
                    m_SearchTimer = searchRate;
                }
            }

            m_HadTarget = m_CurrrentTargetable != null;
        }

        protected virtual void AcquiredTarget(){
            if (acquiredTarget != null)
            {
                acquiredTarget(m_CurrrentTargetable);
            }
        }

        protected virtual void LostTarget()
        {
            if (lostTarget != null)
            {
                lostTarget();
            }
        }

        /// <summary>
        /// Fired by the agents died event or when the current target moves out of range,
        /// Fires the lostTarget event.
        /// </summary>
        void OnTargetRemoved(DamageableBehaviour target)
        {
            target.removed -= OnTargetRemoved;
            if (m_CurrrentTargetable != null && target.configuration == m_CurrrentTargetable.configuration)
            {
                LostTarget();
                m_HadTarget = false;
                m_TargetsInRange.Remove(m_CurrrentTargetable);
                m_CurrrentTargetable = null;
            }
            else //wasnt the current target, find and remove from targets list
            {
                for (int i = 0; i < m_TargetsInRange.Count; i++)
                {
                    if (m_TargetsInRange[i].configuration == target.configuration)
                    {
                        m_TargetsInRange.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}