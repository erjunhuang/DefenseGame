using UnityEngine;


namespace AIBehavior
{
	public class CharacterAnimator : MonoBehaviour
	{
		public Animator anim = null;
		//bool hasAnimationComponent = false;

		public void OnAnimationState(AIAnimationState animState)
		{
            //if ( hasAnimationComponent && animState != null )
            if (anim && animState != null)
            {
				string stateName = animState.name;
                anim.SetTrigger(stateName);
                anim.speed = animState.speed;
            }
		}
	}
}