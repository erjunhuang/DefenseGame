using UnityEngine;

namespace AIBehavior
{
	public struct AttackData
	{
		public AIBehaviors fsm;
		public Transform target;
        public long skillId;

		public AttackData(AIBehaviors aiBehaviors, Transform target, long skillId)
		{
			this.fsm = aiBehaviors;
			this.target = target;
            this.skillId = skillId;
        }
	}
}