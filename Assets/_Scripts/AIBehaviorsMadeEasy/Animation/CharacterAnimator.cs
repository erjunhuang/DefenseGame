using UnityEngine;


namespace AIBehavior
{
	public class CharacterAnimator : MonoBehaviour
	{
		public Animator anim = null;
		bool hasAnimationComponent = false;


		void Awake()
		{
			if ( anim == null )
			{
				anim = GetComponentInChildren<Animator>();

				if ( anim == null )
				{
					anim = GetComponentInParent<Animator>();
				}
			}

			hasAnimationComponent = anim != null;

			if ( !hasAnimationComponent )
			{
				Debug.LogWarning("No animation component found for the '" + gameObject.name + "' object or child objects");
			}
		}


		public void OnAnimationState(AIAnimationState animState)
		{
			if ( hasAnimationComponent && animState != null )
			{
				string stateName = animState.name;

				//if ( anim[stateName] != null )
				//{
                    anim.CrossFade(animState.name, 0.2f);
                    //anim[stateName].wrapMode = animState.animationWrapMode;
                    //anim[stateName].speed = animState.speed;
                    //anim.CrossFade(stateName);
    //            }
				//else
				//{
				//	Debug.LogWarning("The animation state \"" + stateName + "\" couldn't be found.");
				//}
			}
		}
	}
}