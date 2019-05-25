using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Core.Health;
using GameModel;
using ActionGameFramework.Health;
using QGame.Core.FightEnegin;
using QGame.Core.FightEnegin.Damage;
using QGame.Core.FightEnegin.Damage.Buff;

#if UNITY_EDITOR
using System.Reflection;
#endif

using Random = UnityEngine.Random;


namespace AIBehavior
{
	/// <summary>
	/// This class is the brain of the AIBehavior system. It controls state changes and various callbacks.
	/// </summary>
	[RequireComponent(typeof(AIAnimationStates))]
    [RequireComponent(typeof(AISkillStates))]
    public class AIBehaviors : Targetable
    {
		/// <summary>
		/// Is this AI active (Read Only)?
		/// </summary>
		public bool isActive { get; private set; }

		/// <summary>
		/// Is this AI in defensive mode?
		/// </summary>
		public bool isDefending = false;

		/// <summary>
		/// This is a multiplier for the amount of damage this AI will receive
		/// </summary>
		[SerializeField] protected float damageMultiplier = 1.0f;

		/// <summary>
		/// The number of possible states this AI has.
		/// </summary>
		public int stateCount { get { return states.Length; } }

		public Transform aiTransform;
		Vector3 thisPos;
		NavMeshAgent navMeshAgent;

		/// <summary>
		/// The use sight transform.
		/// </summary>
		public bool useSightTransform = false;

		/// <summary>
		/// The sight transform.
		/// </summary>
		public Transform sightTransform = null;

		/// <summary>
		/// This is the state the AI is in once the game is playing.
		/// </summary>
		public BaseState initialState;

		/// <summary>
		/// This is the state the AI is currently in (Read Only).
		/// </summary>
		public BaseState currentState { get; private set; }

		/// <summary>
		/// Gets the previous state the AI was in
		/// </summary>
		public BaseState previousState { get; private set; }

		/// <summary>
		/// An array of all the states that belong to this AI.
		/// </summary>
		public BaseState[] states = new BaseState[0];

		/// <summary>
		/// The array of global triggers.
		/// </summary>
		public BaseTrigger[] triggers = new BaseTrigger[0];

		// === General Properties === //

		public TaggedObjectFinder objectFinder;

		/// <summary>
		/// These are the layers that block the AI from seeing what it's looking for
		/// </summary>
		/// IE. If this AI is an enemy looking for a player, you should EXCLUDE the player layer from these layers
		public LayerMask raycastLayers = -1;

		/// <summary>
		/// If this is true the AI is less likely to see toward the edge of it's FOV
		/// </summary>
		public bool useSightFalloff = true;

		/// <summary>
		/// The field of view in which the AI can see
		/// </summary>
		public float sightFOV = 180.0f;

		/// <summary>
		/// The maximum distance the AI can see
		/// </summary>
		public float sightDistance = 50.0f;

		/// <summary>
		/// The position that the AI looks from
		/// </summary>
		public Vector3 eyePosition = new Vector3(0.0f, 1.5f, 0.0f);

		/// <summary>
		/// Should the agent's local y axis always point up?
		/// </summary>
		public bool keepAgentUpright = true;

		/// <summary>
		/// The object where the AI states and triggers are located
		/// </summary>
		public GameObject statesGameObject = null;

		// === Animation Callback Info === //

		/// <summary>
		/// The component that will be called when a new animation should play
		/// </summary>
		public Component animationCallbackComponent = null;

        public Component skillCallbackComponent = null;

        /// <summary>
        /// The name of the method that should be called when a new animation should play.
        /// </summary>
        public string animationCallbackMethodName = "";

        public string skillCallbackMethodName = "";

        /// <summary>
        /// This is the list of available animation states for the AI
        /// </summary>
        public AIAnimationStates animationStates;


        public AISkillStates skillStates;
        // === Delegates === //

        /// <summary>
        /// This delegate is used for the 'onStateChanged' callback
        /// </summary>
        public delegate void StateChangedDelegate(BaseState newState, BaseState previousState);

		/// <summary>
		/// Any time a state changes, this delegate is called.
		/// </summary>
		public StateChangedDelegate onStateChanged = null;

		/// <summary>
		/// This delegate is used for the 'onPlayAnimation' callback
		/// </summary>
		public delegate void AnimationCallbackDelegate(AIAnimationState animationState);

		/// <summary>
		/// This delegate is called anytime an animation should play.
		/// </summary>
		public AnimationCallbackDelegate onPlayAnimation = null;


      
		public delegate void  SkillCallbackDelegate(SkillData animationState);
      
        public SkillCallbackDelegate onPlaySkill = null;

        /// <summary>
        /// This delegate is used for the 'externalMove' callback
        /// </summary>
        public delegate void ExternalMoveDelegate(Vector3 targetPoint, float targetSpeed, float rotationSpeed);

		/// <summary>
		/// This delegate is called whenever a custom or 3rd party navigation system needs new location data.
		/// </summary>
		public ExternalMoveDelegate externalMove = null;

		// === Targetting and Rotation === //

		/// <summary>
		/// Gets the destination the AI is currently trying to reach
		/// </summary>
		public Vector3 currentDestination { get; private set; }
		protected Transform lastKnownRotationTarget;
		protected Vector3 targetRotationPoint;
		protected float rotationSpeed;

		public bool showDebugMessages = true;

		// AI Behaviors custom variables
		public AiBehaviorVariable[] userVariables = new AiBehaviorVariable[0];


        public Monster monsterInfo { get; private set; }

        public Dictionary<eBuffType, BuffBase> list_buff = new Dictionary<eBuffType, BuffBase>();

        // === Methods === //

        public AIBehaviors()
		{
            objectFinder = CreateObjectFinder();
        }


		protected virtual TaggedObjectFinder CreateObjectFinder()
		{
			return new TaggedObjectFinder();
		}

        public float GetHealthValue()
        {
            return this.configuration.currentHealth;
        }

        public override void Remove()
        {
            base.Remove();
            RemoveAllBuff();
            Destroy(this.gameObject);
        }


        protected override void Awake()
        {
            base.Awake();
        }
        public virtual void Initialize(Monster monsterInfo, Object alignment) {

            this.monsterInfo = monsterInfo;
            InitData(monsterInfo.MaxHealth, alignment);
            Init();
            InitialState();
        }

        public void InitData(float maxHealth, Object alignment)
        {
            //health = healthAmount;
            this.configuration.SetMaxHealth(maxHealth);
            this.configuration.SetHealth(maxHealth);

            if (this.configuration.alignment == null)
            {
                this.configuration.alignment = new SerializableIAlignmentProvider();
                this.configuration.alignment.unityObjectReference = alignment;
            }

            objectFinder.Initialize(this.configuration.alignment);
             
            //BuffInfo buffInfo = new BuffInfo();
            //buffInfo.AllValue = 500;
            //buffInfo.buffType = eBuffType.heal;
            //buffInfo.durationTime = 10f;
            //AddBuff(BuffBase.GetBuff(buffInfo,this,this));
        }

        void Init()
		{
			#if USE_ASTAR
			Debug.Log("Use Astar!");
			#endif

			aiTransform = GetTransform();
			animationStates = aiTransform.GetComponent<AIAnimationStates>();
            skillStates = aiTransform.GetComponent<AISkillStates>();
            navMeshAgent = aiTransform.GetComponent<NavMeshAgent>();

            InitGlobalTriggers();

			if ( !useSightTransform || sightTransform == null )
			{
				sightTransform = new GameObject("Sight Transform").transform;
				sightTransform.parent = aiTransform;
				sightTransform.localRotation = Quaternion.identity;
				sightTransform.localPosition = eyePosition;
                //useSightTransform = false;
                useSightTransform = true;
            }

			currentDestination = aiTransform.position;

			SetActive(true);
		}


		void InitialState()
		{
			ChangeActiveState(initialState);
		}

        private eBuffType[] keys;
        bool blDelete = false;
        public void Update()
		{
			if ( isActive && Time.timeScale > 0 )
			{
				objectFinder.CacheTransforms(CachePoint.EveryFrame);

				if ( currentState.RotatesTowardTarget() )
				{
					RotateAgent();
				}

				thisPos = aiTransform.position;

				for ( int i = 0; i < triggers.Length; i++ )
				{
					if ( triggers[i].HandleEvaluate(this) && triggers[i].transitionState != null )
					{
						currentState = triggers[i].transitionState;
					}
				}

				// If the state remained the same, do the action
				if ( currentState.HandleReason(this) )
				{
					currentState.HandleAction(this);
				}


                blDelete = false;
                //´¦Àíbuff
                if (list_buff != null && list_buff.Count > 0)
                {
                    if (keys == null)
                    {
                        keys = new eBuffType[list_buff.Count];
                        list_buff.Keys.CopyTo(keys, 0);
                    }

                    foreach (eBuffType buffType in keys)
                    {
                        BuffBase bb = list_buff[buffType];
                        bb.onUpdate();
                        bb.durationTime -= Time.deltaTime;
                        if (bb.durationTime <= 0)
                        {
                            blDelete = true;
                            list_buff.Remove(bb.buffType);
                            bb.onExit();
                        }
                    }
                    if (blDelete)
                    {
                        if (list_buff.Count > 0)
                        {
                            keys = new eBuffType[list_buff.Count];
                            list_buff.Keys.CopyTo(keys, 0);
                        }
                        else {
                            keys = null;
                        }
                    }
                }
            }
		}

        /// <summary>
        /// ¾»»¯
        /// </summary>
        public void JingHua()
        {
            foreach (eBuffType buffType in keys)
            {
                BuffBase bb = list_buff[buffType];
                if (bb.blDebuff)
                {
                    bb.onExit();
                    list_buff.Remove(bb.buffType);
                }
            }
        }

        private void RemoveAllBuff()
        {
            if (list_buff != null)
            {
                foreach (BuffBase bi in list_buff.Values)
                {
                    bi.onExit();
                }
                list_buff.Clear();
            }
        }

        public void AddBuff(BuffBase buff)
        {
            if (this.configuration.currentHealth <= 0)
                return;
            if (buff == null)
                return;
            if (buff.onEnert())
                list_buff.Add(buff.buffType, buff);
        }

        Transform GetTransform()
		{
			if ( aiTransform == null )
			{
				aiTransform = transform;
			}

			return aiTransform;
		}


		void InitGlobalTriggers ()
		{
			for ( int i = 0; i < triggers.Length; i++ )
			{
				triggers[i].HandleInit(this, objectFinder);
			}
		}


		/// <summary>
		/// Sets whether this AI is actively evaluating its' behaviors
		/// </summary>
		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
		}

		/// <summary>
		/// Tells the AI that it got hit and it will subtract the amount of health passed from its' total health
		/// </summary>
		public void Damage(float damageValue, Vector3 damagePoint, IAlignmentProvider alignment)
		{
			// We don't want to enable this state if the FSM is dead
			//GotHitState gotHitState = GetState<GotHitState>();

			//if ( gotHitState != null && gotHitState.CanGetHit(this) )
			//{
				float totalDamage = damageValue * GetDamageMultiplier ();
                //SubtractHealthValue(totalDamage);
                this.TakeDamage(totalDamage, damagePoint, alignment);
				Debug.Log ("Got " + totalDamage + " damage");

				//if ( gotHitState.CoolDownFinished() )
				//{
				//	ChangeActiveState(gotHitState);
				//}
			//}
		}

        public void Cure(float addCureValue, Vector3 damagePoint, IAlignmentProvider alignment) {
            this.TakeCure(addCureValue, damagePoint, alignment);
        }


        public virtual void SetDamageMultiplier (float newDamageMultiplier)
		{
			damageMultiplier = newDamageMultiplier;
		}


		public virtual float GetDamageMultiplier ()
		{
			return damageMultiplier;
		}


		// === GetStates === //

		/// <summary>
		/// Returns all of the states that belong to this AI
		/// </summary>
		/// <returns>All of this AI's states.</returns>
		public BaseState[] GetAllStates()
		{
			return states;
		}


		/// <summary>
		/// Gets a state at a specific index.
		/// </summary>
		/// <returns>The state by index.</returns>
		/// <param name="stateIndex">State index.</param>
		public BaseState GetStateByIndex(int index)
		{
			if ( index < states.Length )
			{
				return states[index];
			}

			return null;
		}


		/// <summary>
		/// Gets a state according to it's user provided name. IE "Secondary Attack", "Idle" or "Seek Health"
		/// </summary>
		/// <returns>The state by name.</returns>
		/// <param name="stateName">State name.</param>
		public BaseState GetStateByName(string stateName)
		{
			foreach ( BaseState state in states )
			{
				if ( state.name.Equals(stateName) )
				{
					return state;
				}
			}

			return null;
		}


		/// <summary>
		/// Gets the first state of a certain Type. IE IdleState result = ai.GetState<IdleState>(); or BaseState result = ai.GetState<IdleState>();
		/// </summary>
		/// <returns>The state.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetState<T> () where T : BaseState
		{
			foreach ( BaseState state in states )
			{
				if ( state is T )
				{
					return state as T;
				}
			}

			return null;
		}


		/// <summary>
		/// Gets the first state of a certain Type. IE IdleState result = ai.GetState(typeof(IdleState)) as IdleState; or BaseState result = ai.GetState(typeof(IdleState)) as BaseState;
		/// </summary>
		/// <returns>The state.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public BaseState GetState(System.Type type)
		{
			foreach ( BaseState state in states )
			{
				if ( state.GetType() == type )
				{
					return state;
				}
			}

			return null;
		}


		// === GetStates === //

		/// <summary>
		/// Gets all states of a certain Type. IE IdleState[] result = ai.GetStates<IdleState>();
		/// </summary>
		/// <returns>The states.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T[] GetStates<T> () where T : BaseState
		{
			List<T> stateList = new List<T>();

			foreach ( BaseState state in states )
			{
				if ( state is T )
				{
					stateList.Add(state as T);
				}
			}

			return stateList.ToArray();
		}


		/// <summary>
		/// Gets all states of a certain Type. IE BaseState[] result = ai.GetStates(typeof(IdleState));
		/// </summary>
		/// <returns>The states.</returns>
		/// <param name="type">Type.</param>
		public BaseState[] GetStates(System.Type type)
		{
			List<BaseState> stateList = new List<BaseState>();

			foreach ( BaseState state in states )
			{
				if ( state.GetType() == type )
				{
					stateList.Add(state);
				}
			}

			return stateList.ToArray();
		}


		// === Replace States === //

		/// <summary>
		/// Replaces all states for this AI with the specified array.
		/// </summary>
		/// <param name="newStates">New states.</param>
		public void ReplaceAllStates(BaseState[] newStates)
		{
			states = newStates;
		}


		/// <summary>
		/// Replaces the state at the specified index
		/// </summary>
		/// <param name="newState">New state.</param>
		/// <param name="index">Index.</param>
		public void ReplaceStateAtIndex(BaseState newState, int index)
		{
			states[index] = newState;
		}

        /// <summary>
        /// Add state 
        /// </summary>
        // <param name="newState">New state.</param>
        public void AddSubTrigger(BaseState newState)
        {
            BaseState[] newSubStates = new BaseState[states.Length + 1];

            for (int i = 0; i < states.Length; i++)
            {
                newSubStates[i] = states[i];
            }

            newSubStates[states.Length] = newState;

            states = newSubStates;
        }

        // === Change Current Active State === //

        /// <summary>
        /// Force the AI to the specified state. Note that this state does NOT have to exist in the states array
        /// </summary>
        /// <param name="newState">New state.</param>
        /// The 'onStateChanged' delegate is called at the end of this method
        public void ChangeActiveState(BaseState newState)
		{
			if ( newState == null || !newState.CanSwitchToState())
			{
				return;
			}

			objectFinder.CacheTransforms(CachePoint.StateChanged);
			InitGlobalTriggers();

			previousState = currentState;

			if ( previousState != null )
			{
				previousState.EndState(this);
			}

			currentState = newState;
			newState.InitState(this);

			if ( onStateChanged != null )
			{
				onStateChanged(newState, previousState);
			}
		}


		/// <summary>
		/// Force the AI to change to the user named state. IE If an IdleState is named "Second Idle" you can force AIBehaviors to go to that state by calling ChangeActiveStateByName("Second Idle")
		/// </summary>
		/// <param name="stateName">State name.</param>
		public void ChangeActiveStateByName(string stateName)
		{
			foreach ( BaseState state in states )
			{
				if ( state.name.Equals(stateName) )
				{
					ChangeActiveState(state);
					return;
				}
			}
		}


		/// <summary>
		/// Force the AI to go to the state at the specified index
		/// </summary>
		/// <param name="index">Index.</param>
		public void ChangeActiveStateByIndex(int index)
		{
			if ( index < states.Length )
			{
				ChangeActiveState(states[index]);
			}
		}


        private Vector3 prevPosition;
        public void MoveAgentWithVector(Transform target, float targetSpeed, float rotationSpeed)
        {   
            Vector3 targetPoint = target.position;
            currentDestination = targetPoint;
            targetRotationPoint = targetPoint;
            this.rotationSpeed = rotationSpeed;
            transform.position = Vector3.MoveTowards(transform.position, target.position, targetSpeed*0.1f * Time.fixedDeltaTime);

            if (externalMove != null)
            {
                externalMove(targetPoint, targetSpeed, rotationSpeed);
            }
        }

        /// <summary>
        /// Sets the movement and rotation targets and values
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="targetSpeed">Target speed.</param>
        /// <param name="rotationSpeed">Rotation speed.</param>
        public void MoveAgent(Transform target, float targetSpeed, float rotationSpeed)
		{   
			if ( target != null )
			{
				RotateAgent(target, rotationSpeed);
			}

			MoveAgent(target.position, targetSpeed, rotationSpeed);
		}


		/// <summary>
		/// Sets the movement and rotation targets and values
		/// </summary>
		/// <param name="targetPoint">Target point.</param>
		/// <param name="targetSpeed">Target speed.</param>
		/// <param name="rotationSpeed">Rotation speed.</param>
		/// If there is a NavMeshAgent component its' speed, roation and destination will be handled automatically
		/// If the 'externalMove' delegate is assigned it will be called at the end of the method
		/// If both a NavMeshAgent component exists and the 'externalMove' delegate is assigned, both will be called
		public void MoveAgent(Vector3 targetPoint, float targetSpeed, float rotationSpeed, Transform rotationTargetOverride = null)
		{
			bool isNavAgent = navMeshAgent != null;

			currentDestination = targetPoint;
			targetRotationPoint = targetPoint;
			this.rotationSpeed = rotationSpeed;

			if ( isNavAgent && navMeshAgent.enabled )
			{
				Vector3 velocity = navMeshAgent.velocity;
				float velocityMagnitude = velocity.magnitude;
				float rotationMultiplier = Mathf.InverseLerp(0, targetSpeed, velocityMagnitude);

				navMeshAgent.speed = targetSpeed;
				navMeshAgent.angularSpeed = 0;
				navMeshAgent.destination = targetPoint;

				if ( rotationTargetOverride != null )
				{
					velocity = (rotationTargetOverride.position - aiTransform.position).normalized;
				}

				if ( keepAgentUpright )
				{
					velocity.y = 0.0f;
					velocity = velocity.normalized * velocityMagnitude;
				}

				aiTransform.Rotate(Vector3.Cross(aiTransform.forward, velocity) * Time.deltaTime * rotationSpeed * rotationMultiplier);
			}

			if ( externalMove != null )
			{
				externalMove(targetPoint, targetSpeed, rotationSpeed);
			}
		}


		/// <summary>
		/// Sets the rotation properties
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="rotationSpeed">Rotation speed.</param>
		public void RotateAgent(Transform target, float rotationSpeed)
		{
			targetRotationPoint = target.position;
			lastKnownRotationTarget = target;
			this.rotationSpeed = rotationSpeed;
		}


		protected internal virtual void RotateAgent()
		{
			bool isNavAgent = navMeshAgent != null;
			bool updateRot = false;

			Vector3 pos = aiTransform.position;
			Quaternion curRotation = aiTransform.rotation;
			Vector3 sightPosition = GetSightPosition(sightTransform);
			Vector3 targetPos;

			if ( lastKnownRotationTarget != null )
			{
				RaycastHit hit;

				targetPos = lastKnownRotationTarget.position;

				if ( Physics.Linecast(sightPosition, targetPos, out hit, raycastLayers) && hit.transform != lastKnownRotationTarget )
				{
					if ( showDebugMessages )
					{
						Debug.LogWarning("Can't rotate toward target. The object '" + hit.transform.name + "' is in the way of '" + lastKnownRotationTarget.name + "'.");
					}

					lastKnownRotationTarget = null;
				}
				else
				{
					targetRotationPoint = targetPos;
				}
			}

			targetRotationPoint.y = pos.y;
			aiTransform.LookAt(targetRotationPoint);

			if ( isNavAgent )
			{
				updateRot = navMeshAgent.updateRotation;
				navMeshAgent.updateRotation = false;
			}

			if ( rotationSpeed > 0.0f && Time.deltaTime > 0.0f )
			{
				//thisTFM.rotation = Quaternion.RotateTowards(curRotation, thisTFM.rotation, rotationSpeed * Time.deltaTime);
			}

			if ( isNavAgent )
			{
				navMeshAgent.updateRotation = updateRot;
			}
		}
         

        /// <summary>
        /// Tells the system to play an AIAnimationState
        /// </summary>
        /// <param name="animState">Animation state.</param>
        /// If the 'onPlayAnimation' delegate is defined it will call that.\n
        /// If 'onPlayAnimation' is not defined it will attempt to call the method 'animationCallbackMethodName' defined on the 'animationCallbackComponent'
        public void PlayAnimation(AIAnimationState animState)
		{
			if ( onPlayAnimation != null )
			{
				onPlayAnimation(animState);
			}
			else if ( animationCallbackComponent != null && animState != null )
			{
				if ( !string.IsNullOrEmpty(animationCallbackMethodName) )
				{
					animationCallbackComponent.SendMessage(animationCallbackMethodName, animState);
				}
			}
		}

        public void PlaySkill(SkillData skillState)
        {
            if (onPlaySkill != null)
            {
                onPlaySkill(skillState);
            }
            else if (skillCallbackComponent != null && skillState != null)
            {
                if (!string.IsNullOrEmpty(skillCallbackMethodName))
                {
                    skillCallbackComponent.SendMessage(skillCallbackMethodName, skillState);
                }
            }
        }

        /// <summary>
        /// Gets the closest player found that is tagged with a tag in the playerTags array.
        /// </summary>
        /// <returns>The closest player.</returns>
        /// Note that if the tagged GameObject was added after this AI was initialized it may not find it
        /// - One solution is to call the CheckForPlayers method to refresh this list
        /// - Another solution is to set the checkForPlayersInterval to something other than 0
        /// - Idealy you want to call CheckForPlayers as little as possible as it has some performance implications.
        public Transform GetClosestPlayer(Transform[] playerTransforms)
		{
			float sqrDist;
			return GetClosestPlayer(playerTransforms, out sqrDist);
		}


		/// <summary>
		/// This method is the same as the main GetClosestPlayer method, only that it allows you to get the squareDistance to the closest player in the out parameter
		/// </summary>
		/// <returns>The closest player.</returns>
		/// <param name="squareDistance">Square distance.</param>
		public Transform GetClosestPlayer(Transform[] playerTransforms, out float squareDistance)
		{
			int closestPlayerIndex = -1;

			squareDistance = Mathf.Infinity;

			for ( int i = 0; i < playerTransforms.Length; i++ )
			{
				Vector3 playerPosition = playerTransforms[i].position;
				Vector3 targetDifference = playerPosition - thisPos;

				// is the target within distance?
				if ( targetDifference.sqrMagnitude < squareDistance )
				{
					squareDistance = targetDifference.sqrMagnitude;
					closestPlayerIndex = i;
				}
			}

			if ( closestPlayerIndex == -1 )
				return null;
			else
				return playerTransforms[closestPlayerIndex];
		}


		/// <summary>
		/// This returns the closest player within the AI's sight. Sight falloff is NOT used.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms)
		{
			return GetClosestPlayerWithinSight(playerTransforms, false);
		}


		/// <summary>
		/// This returns the closest player within the AI's sight. Sight falloff can be specified.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		/// <param name="includeSightFalloff">If set to <c>true</c> include sight falloff.</param>
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms, bool includeSightFalloff, Transform sightTransformOverride = null)
		{
			float dist = 0.0f;
			return GetClosestPlayerWithinSight(playerTransforms, out dist, includeSightFalloff, sightTransformOverride);
		}


		/// <summary>
		/// Gets the closest player within sight. Sight falloff can be specified and the squareDistance can be retrieved with the out parameter.
		/// </summary>
		/// <returns>The closest player within sight.</returns>
		/// <param name="squareDistance">Square distance.</param>
		/// <param name="includeSightFalloff">If set to <c>true</c> include sight falloff.</param>
		// TODO: Refactor this method to not use random values.
		public Transform GetClosestPlayerWithinSight(Transform[] playerTransforms, out float squareDistance, bool includeSightFalloff, Transform sightTransformOverride = null)
		{
			float sqrSightDistance = sightDistance * sightDistance;
			int closestPlayerIndex = -1;

			squareDistance = Mathf.Infinity;

			if ( sightTransformOverride == null )
			{
				sightTransformOverride = sightTransform;
			}

			for ( int i = 0; i < playerTransforms.Length; i++ )
			{
				Vector3 playerPosition = playerTransforms[i].position;
				Vector3 targetDifference = playerPosition - thisPos;
				float targetSqrMagnitude = targetDifference.sqrMagnitude;
				float angle = Vector3.Angle(targetDifference, sightTransformOverride.forward);
				Vector3 sightPosition = GetSightPosition(sightTransformOverride);

				// If we already have a player in sight
				if ( closestPlayerIndex != -1 )
				{
					// Is the new one closer than the previous closest one?
					if ( targetSqrMagnitude > squareDistance )
					{
						continue;
					}
				}

				// is the target within distance?
				if ( targetSqrMagnitude < sqrSightDistance )
				{
					float halfFOV = sightFOV / 2.0f;

					if ( angle < halfFOV )
					{
						if ( useSightFalloff && includeSightFalloff )
						{
							float anglePercentage = Mathf.InverseLerp(0.0f, halfFOV, angle);

							if ( Random.value < anglePercentage )
							{
								continue;
							}
						}

						RaycastHit hit;

						// Make sure there isn't anything between the player
						if ( !Physics.Linecast(sightPosition, playerPosition, out hit, raycastLayers) || hit.transform == playerTransforms[i] || hit.transform == aiTransform )
						{
							squareDistance = targetSqrMagnitude;
							closestPlayerIndex = i;
						}
					}
				}
			}

			if ( closestPlayerIndex == -1 )
				return null;
			else
				return playerTransforms[closestPlayerIndex];
		}


		public Vector3 GetSightPosition (Transform sightTransformOverride = null)
		{
			if (sightTransformOverride == null)
			{
				if (sightTransform == null)
				{
					return GetTransform().TransformPoint(eyePosition);
				}
				else
				{
					sightTransformOverride = sightTransform;
				}
			}

			return sightTransformOverride.position;
		}


		public Vector3 GetSightDirection (Transform sightTransformOverride = null)
		{
			if (sightTransformOverride == null)
			{
				if (sightTransform == null)
				{
					return GetTransform().forward;
				}
				else
				{
					sightTransformOverride = sightTransform;
				}
			}

			return sightTransformOverride.forward;
		}


		/// <summary>
		/// Tells the AI to play the audio clip contained within the current active state.
		/// </summary>
		public void PlayAudio()
		{
			AudioSource audioSource;

			if ( currentState.sounds.Length == 0 )
			{
				return;
			}

			audioSource = GetComponent<AudioSource>();

			if ( audioSource == null )
			{
				audioSource = gameObject.AddComponent<AudioSource>();
			}

			int index = Random.Range (0, currentState.sounds.Length);
			audioSource.loop = currentState.sounds[index].loopAudio;
			audioSource.volume = currentState.sounds[index].audioVolume;
			audioSource.pitch = currentState.sounds[index].audioPitch + ((Random.value * currentState.sounds[index].audioPitchRandomness) - (currentState.sounds[index].audioPitchRandomness / 2.0f)) * 2.0f;
			audioSource.clip = currentState.sounds[index].audioClip;
			audioSource.Play();
		}

		public string[] GetVariableNames()
		{
			List<string> varNames = new List<string>();
			for(int i = 0; i < userVariables.Length; i++)
			{
				varNames.Add(userVariables[i].name);
			}
			return varNames.ToArray();
		}
	}
}