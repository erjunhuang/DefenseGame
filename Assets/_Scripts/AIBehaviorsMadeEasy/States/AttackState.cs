using UnityEngine;
using System.Collections;
using ActionGameFramework.Health;
using Core.Health;
using QGame.Core.Event;
using QGame.Core.FightEnegin;
using GameModel;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
#endif


namespace AIBehavior
{
	public class AttackState : CooldownableState
	{
        public long rangedAttackSkillId;
		public bool findVisibleTargetsOnly = true;
	    public Transform target = null;
		Transform lastKnownTarget = null;

		public string attackAnimName = "";
		public float attackPoint = 0.5f;

		public bool inheritPreviousStateMovement = false;

		public Component scriptWithAttackMethod;
		public string methodName = "";

		public enum AttackMode
		{
			Animation,
			Interval
		}
         
        public AttackMode attackBasedOn = AttackMode.Animation;

		public float initialAttackTime = 0.5f;
		public float attackInterval = 0.5f;
        public float intervalAttackTime;

        public Animation attackAnimation;
        public AnimationCullingType initialCullingType;
        public float animationLength = 0.0f;
        public float curAnimPosition = 0.0f;
        public float previousSamplePosition = 0.0f;

		public int attacksBeforeReload = 10;
		public int attackCount = 0;
		public BaseState reloadState = null;
		public BaseState noTargetState = null;

        public SkinnedMeshRenderer skinnedMeshRenderer = null;

		bool goingToReload = false;

        public PlayerSystem playerSystem;

        public eAttackState currentAttackState;

        private AIBehaviors fsm;

        public float attackRange;


        public Transform GetTarget() {
            return target;
        }
        public enum AttackType
        {
            Melee,
            Range,
        }
        public AttackType attackType = AttackType.Range;

        public enum CounterattackMode
        {
            Active,
            Passive,
        }
        public CounterattackMode counterattackMode = CounterattackMode.Active;

        protected override void Awake()
		{
			skinnedMeshRenderer = transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
			base.Awake();
		}
        //public void ChangeState(eAttackState state) {
        //    playerSystem.ChangeState(state);
        //}

        public bool IsCanAttack() {

            if (attackBasedOn == AttackMode.Interval)
            {
                return HandleIntervalAttackMode(fsm, target);
            }
            else
            {
                return HandleAnimationAttackMode(fsm, target);
            }
        }

        public void ResetCoolDownTime()
        {
            if (attackBasedOn == AttackMode.Interval)
            {
                intervalAttackTime = Time.time + attackInterval;
            }
            else
            {
                previousSamplePosition = curAnimPosition;
            }
        }
        protected override void Init(AIBehaviors fsm)
		{
            //playerSystem = new PlayerSystem(this,fsm);

            this.fsm = fsm;
            fsm.SetNavMeshAgentStoppingDistance(attackRange);
            if (fsm.target != null)
            {
                target = fsm.target;
            }
           
            goingToReload = false;
            curAnimPosition = 0.0f;
            previousSamplePosition = 0.0f;

            if (inheritPreviousStateMovement && fsm.previousState != null)
            {
                fsm.MoveAgent(fsm.previousState.GetNextMovement(fsm), movementSpeed, rotationSpeed);
            }
            else
            {
                fsm.MoveAgent(fsm.transform, 0.0f, rotationSpeed);
            }

            if (attackBasedOn == AttackMode.Animation)
            {
                attackAnimation = fsm.gameObject.GetComponentInChildren<Animation>();

                if (attackAnimation != null && attackAnimation[attackAnimName] != null)
                {
                    initialCullingType = attackAnimation.cullingType;
                    attackAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
                    animationLength = attackAnimation[attackAnimName].length;
                }
                else
                {
                    animationLength = 1.0f;
                }
            }
            else
            {
                intervalAttackTime = Time.time + initialAttackTime;
            }
        }

		protected override void StateEnded(AIBehaviors fsm)
		{
			base.StateEnded (fsm);

			if (attackAnimation != null)
			{
				attackAnimation.cullingType = initialCullingType;
			}

            lastKnownTarget = target = null;
            fsm.SetNavMeshAgentStoppingDistance(0);
        }


		protected override bool Reason(AIBehaviors fsm)
		{
			return true;
		}

        protected override void Action(AIBehaviors fsm)
		{
            //playerSystem.PerformAction();

            lastKnownTarget = GetTarget(fsm);
            if (target != null)
            {
                if (counterattackMode == CounterattackMode.Active) {
                    HandlePreviousStateMovement(fsm, target);
                }
                fsm.RotateAgent(target, rotationSpeed);

                if (!goingToReload) {
                    float sqrDistanceThreshold = attackRange * attackRange;
                    Vector3 targetDir = target.transform.position - fsm.transform.position;
                    if (targetDir.sqrMagnitude < sqrDistanceThreshold)
                    {
                        HandleAttack(fsm, target);
                    }
                }
            }
            else
            {
                target = lastKnownTarget;
                if (target == null) {
                    fsm.ChangeActiveState(noTargetState);
                }
            }

            //Vector3 targetDir = target.transform.position - fsm.transform.position;
            //Debug.Log("targetDir:" + targetDir.sqrMagnitude);

            //currentAttackState = playerSystem.attack_state.CurrentState;
        }
        
        protected Transform GetTarget(AIBehaviors fsm)
		{
			if ( findVisibleTargetsOnly )
			{
				return fsm.GetClosestPlayerWithinSight(objectFinder.GetTransforms());
			}
			else
			{
				return fsm.GetClosestPlayer(objectFinder.GetTransforms());
			}
		}


		protected void HandleAttack(AIBehaviors fsm, Transform target)
		{
   //         if ( scriptWithAttackMethod != null && !string.IsNullOrEmpty(methodName) )
			//{
				if ( attackBasedOn.Equals(AttackMode.Animation) )
				{
					HandleAnimationAttackMode(fsm, target);
				}
				else
				{
                    HandleIntervalAttackMode(fsm, target);
                }
			//}
		}

        protected virtual bool HandleIntervalAttackMode(AIBehaviors fsm, Transform target) {
            if (Time.time > intervalAttackTime)
            {
                Attack(fsm, target);
                intervalAttackTime = Time.time + attackInterval;
                return true;
            }
            return false;
        }


        protected virtual bool HandleAnimationAttackMode(AIBehaviors fsm, Transform target)
		{
			bool useAnimationTime = skinnedMeshRenderer == null || skinnedMeshRenderer.isVisible;
			string animationName = animationStates[0].name;
			float adjCurAnimPosition = 0.0f;

			if ( !useAnimationTime && attackAnimation != null )
			{
				useAnimationTime = attackAnimation.cullingType == AnimationCullingType.AlwaysAnimate;
			}

			if ( useAnimationTime && attackAnimation != null && attackAnimation[animationName] != null )
			{
				curAnimPosition = attackAnimation[animationName].normalizedTime % 1.0f;
				adjCurAnimPosition = curAnimPosition;
			}
			else
			{
				curAnimPosition %= 1.0f;
				curAnimPosition += Time.deltaTime / animationLength;
			}

			if ( previousSamplePosition > curAnimPosition )
			{
				adjCurAnimPosition++;
			}

			if ( previousSamplePosition > attackPoint || adjCurAnimPosition < attackPoint )
			{
				previousSamplePosition = curAnimPosition;
				return false;
			}
            Attack(fsm, target);
            previousSamplePosition = curAnimPosition;
            return true;
		}


		protected void HandlePreviousStateMovement (AIBehaviors fsm, Transform target)
		{
			if ( inheritPreviousStateMovement )
			{
				BaseState previousState = fsm.previousState;
				Vector3 destination;
				float moveSpeed;
				float rotateSpeed;

				if ( previousState == null )
				{
					destination = GetNextMovement(fsm);
					moveSpeed = movementSpeed;
					rotateSpeed = rotationSpeed;
				}
				else
				{
					destination = previousState.GetNextMovement(fsm);
					moveSpeed = previousState.movementSpeed;
					rotateSpeed = previousState.rotationSpeed;
				}

				fsm.MoveAgent(destination, moveSpeed, rotateSpeed, target);
			}
			else
			{
				fsm.MoveAgent(target, movementSpeed, rotationSpeed);
			}
		}


		protected virtual float GetThresholdMultiplier()
		{
			return 1.0f;
		}

		public virtual void Attack(AIBehaviors fsm, Transform target)
		{
            //scriptWithAttackMethod.SendMessage(methodName, new AttackData(fsm, target, rangedAttackSkillId));


            AttackData attackData = new AttackData(fsm, target, rangedAttackSkillId);

            SkillCfg skillCfg = attackData.fsm.levelAgent.GetSkill();
            if (skillCfg != null)
            {
                //技能 没有就是普通攻击
                attackData.skillId = skillCfg.Id;
                attackData.fsm.levelAgent.ResetSkill(skillCfg);

                XEventBus.Instance.Post(EventId.BattleSkillHurt, new XEventArgs(attackData));
            }
            else
            {
                if (attackType == AttackState.AttackType.Melee)
                {
                    XEventBus.Instance.Post(EventId.BattleHurt, new XEventArgs(attackData));
                }
                else
                {
                    XEventBus.Instance.Post(EventId.BattleSkillHurt, new XEventArgs(attackData));
                }
            }

            fsm.PlayAudio();

			attackCount++;

			if ( attackCount > attacksBeforeReload )
			{
				attackCount = 0;
				goingToReload = true;
				StartCoroutine (ChangeStateWhenAnimationFinished (fsm));
			}
		}


		protected virtual IEnumerator ChangeStateWhenAnimationFinished(AIBehaviors fsm)
		{
			BaseState currentAttackState = fsm.currentState;
			string animationName = animationStates[0].name;

			if(attackAnimation != null && attackAnimation[animationName] != null )
			{
				do
				{
					yield return null;
					curAnimPosition = attackAnimation [animationName].normalizedTime % 1.0f;
				} 
				while (curAnimPosition < 0.95f || curAnimPosition < attackPoint);
			}
				
			if (fsm.currentState == currentAttackState) // Check if the state hasn't changed for some reason
				fsm.ChangeActiveState(reloadState);
		}
		
		public override string DefaultDisplayName()
		{
			return "Attack";
		}

#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_State, AIBehaviors fsm)
		{
			SerializedProperty m_property;

			string[] animNames = AIBehaviorsAnimationEditorGUI.GetAnimationStateNames(m_State);
			int curAttackAnimIndex = 0;

			for ( int i = 0; i < animNames.Length; i++ )
			{
				if ( animNames[i] == animationStates[0].name )
				{
					curAttackAnimIndex = i;
				}
			}

			GUILayout.Label ("Attack Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);

            m_property = m_State.FindProperty("rangedAttackSkillId");
            EditorGUILayout.PropertyField(m_property);
            EditorGUILayout.Separator();

            m_property = m_State.FindProperty("attackType");
            EditorGUILayout.PropertyField(m_property);
            EditorGUILayout.Separator();


            m_property = m_State.FindProperty("counterattackMode");
            EditorGUILayout.PropertyField(m_property);
            EditorGUILayout.Separator();
             

            m_property = m_State.FindProperty("attackRange");
            EditorGUILayout.PropertyField(m_property);
            EditorGUILayout.Separator();

            m_property = m_State.FindProperty("findVisibleTargetsOnly");
			EditorGUILayout.PropertyField(m_property);

			EditorGUILayout.Separator();

			// Movement Settings

			GUILayout.Label("Movement Settings:", EditorStyles.boldLabel);

			m_property = m_State.FindProperty("inheritPreviousStateMovement");
			EditorGUILayout.PropertyField(m_property);

			EditorGUILayout.Separator();

			// Attack Based on Animation / Interval

			m_property = m_State.FindProperty("attackBasedOn");
			EditorGUILayout.PropertyField(m_property);

			if ( m_property.intValue == (int)AttackMode.Animation )
			{
				// Animation Settings

				GUILayout.Label("Animation Settings:", EditorStyles.boldLabel);

				m_property = m_State.FindProperty("attackPoint");
				EditorGUILayout.Slider(m_property, 0.0f, 1.0f);

				if ( !Application.isPlaying )
				{
					if ( curAttackAnimIndex != -1 && curAttackAnimIndex < animNames.Length )
					{
						float calcAttackTime = SampleAttackAnimation(fsm, animNames[curAttackAnimIndex], m_property.floatValue);
						if (calcAttackTime > 0) 
						{
							EditorGUILayout.LabelField ("Time: ", (calcAttackTime + " seconds"));
						}
					}
				}
			}
			else
			{
				m_property = m_State.FindProperty("initialAttackTime");
				EditorGUILayout.PropertyField(m_property);
				m_property = m_State.FindProperty("attackInterval");
				EditorGUILayout.PropertyField(m_property);
			}

			EditorGUILayout.Separator();

			// === Reload Properties === //

			GUILayout.Label("Reload Settings:", EditorStyles.boldLabel);

			m_property = m_State.FindProperty("attacksBeforeReload");
			EditorGUILayout.PropertyField(m_property, new GUIContent("Attacks Before Reload"));

			m_property = m_State.FindProperty("attackCount");
			EditorGUILayout.PropertyField(m_property, new GUIContent("Attack count (of " + attacksBeforeReload + ")"));

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Reload State:");
				m_property = m_State.FindProperty("reloadState");
				m_property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, m_property.objectReferenceValue as BaseState);

				if ( reloadState == null )
				{
					m_property.objectReferenceValue = this;
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("No Target State:");
				m_property = m_State.FindProperty("noTargetState");
				m_property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, m_property.objectReferenceValue as BaseState);

				if ( reloadState == null )
				{
					m_property.objectReferenceValue = this;
				}
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			// === Attack Method === //

			GUILayout.Label("Attack Method:", EditorStyles.boldLabel);

			Component[] components = GetAttackMethodComponents(fsm.gameObject);
			int selectedComponent = -1, newSelectedComponent = 0;

			if ( components.Length > 0 )
			{
				string[] componentNames = GetAttackMethodComponentNames(components);

				for ( int i = 0; i < components.Length; i++ )
				{
					if ( components[i] == scriptWithAttackMethod )
					{
						selectedComponent = i;
						break;
					}
				}

				newSelectedComponent = EditorGUILayout.Popup(selectedComponent, componentNames);

				if ( selectedComponent != newSelectedComponent )
				{
					m_property = m_State.FindProperty("scriptWithAttackMethod");
					m_property.objectReferenceValue = components[newSelectedComponent];
				}
			}
			else
			{
				AIBehaviorsCodeSampleGUI.Draw(typeof(AttackData), "attackData", "OnAttack");
			}

			if ( components.Length > 0 )
			{
				string[] methodNames = GetAttackMethodNamesForComponent(components[selectedComponent < 0 ? 0 : selectedComponent]);
				int curSelectedMethod = -1, newSelectedMethod = 0;

				for ( int i = 0; i < methodNames.Length; i++ )
				{
					if ( methodNames[i] == methodName )
					{
						curSelectedMethod = i;
						break;
					}
				}

				newSelectedMethod = EditorGUILayout.Popup(curSelectedMethod, methodNames);
		
				if ( curSelectedMethod != newSelectedMethod )
				{
					m_property = m_State.FindProperty("methodName");
					m_property.stringValue = methodNames[newSelectedMethod];
				}
			}

			GUILayout.EndVertical();

			m_State.ApplyModifiedProperties();

			if ( Application.isPlaying )
			{
				GUILayout.BeginVertical(GUI.skin.box);
				{
					GUILayout.Label("Last known target: " + (target == null ? "Null" : target.name));
				}
				GUILayout.EndVertical();
			}
		}


		Component[] GetAttackMethodComponents(GameObject fsmGO)
		{
			Component[] components = AIBehaviorsComponentInfoHelper.GetNonFSMComponents(fsmGO);
			List<Component> componentList = new List<Component>();

			foreach ( Component component in components )
			{
				if ( GetAttackMethodNamesForComponent(component).Length > 0 )
					componentList.Add(component);
			}

			return componentList.ToArray();
		}


		string[] GetAttackMethodComponentNames(Component[] components)
		{
			string[] componentNames = new string[components.Length];

			for ( int i = 0; i < components.Length; i++ )
			{
				componentNames[i] = components[i].GetType().ToString();
			}

			return componentNames;
		}


		string[] GetAttackMethodNamesForComponent(Component component)
		{
			if ( component != null )
			{
				List<string> methodNames = new List<string>();
				Type type = component.GetType();
				MethodInfo[] methods = type.GetMethods();

				foreach ( MethodInfo mi in methods )
				{
					ParameterInfo[] parameters = mi.GetParameters();

					if ( parameters.Length == 1 )
					{
						if ( parameters[0].ParameterType == typeof(AttackData) )
						{
							methodNames.Add(mi.Name);
						}
					}
				}

				return methodNames.ToArray();
			}

			return new string[0];
		}


		protected virtual float SampleAttackAnimation(AIBehaviors stateMachine, string clipName, float position)
		{
			Animation anim = stateMachine.gameObject.GetComponentInChildren<Animation>();

			if ( anim != null )
			{
				AnimationClip clip = anim.GetClip(clipName);

				if ( clip != null )
				{
					anim.Play(clip.name);
					anim[clip.name].normalizedTime = position;
					anim.Sample();
					anim[clip.name].normalizedTime = 0.0f;

					return anim[clip.name].length * position;
				}
			}

			return 0.0f;
		}


		protected override bool UsesMultipleAnimations()
		{
			return false;
		}
#endif
	}
}