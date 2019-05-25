using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Core.Input
{
	/// <summary>
	/// Base control scheme for touch devices, which performs CameraRig control
	/// </summary>
	public class TouchInput : CameraInputScheme
	{
		/// <summary>
		/// Configuration of the pan speed
		/// </summary>
		public float panSpeed = 5;

		/// <summary>
		/// How quickly flicks decay
		/// </summary>
		public float flickDecayFactor = 0.2f;

		/// <summary>
		/// Flick direction
		/// </summary>
		Vector3 m_FlickDirection;

		/// <summary>
		/// Gets whether the scheme should be activated or not
		/// </summary>
		public override bool shouldActivate
		{
			get { return UnityInput.touchCount > 0; }
		}

		/// <summary>
		/// This default scheme on IOS and Android devices
		/// </summary>
		public override bool isDefault
		{
			get
			{
#if UNITY_IOS || UNITY_ANDROID
				return true;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// Register input events
		/// </summary>
		protected virtual void OnEnable()
		{
			if (!InputController.instanceExists)
			{
				Debug.LogError("[UI] Keyboard and Mouse UI requires InputController");
				return;
			}
			
			// Register drag event
			InputController inputController = InputController.instance;
			inputController.pressed += OnPress;
			inputController.released += OnRelease;
			inputController.dragged += OnDrag;
			inputController.pinched += OnPinch;
		}
		
		/// <summary>
		/// Deregister input events
		/// </summary>
		protected virtual void OnDisable()
		{
			if (!InputController.instanceExists)
			{
				return;
			}
			
			if (InputController.instanceExists)
			{
				InputController inputController = InputController.instance;
				inputController.pressed -= OnPress;
				inputController.released -= OnRelease;
				inputController.dragged -= OnDrag;
				inputController.pinched -= OnPinch;
			}
		}

		/// <summary>
		/// Perform flick and zoom
		/// </summary>
		protected virtual void Update()
		{
			
			UpdateFlick();
		}

		/// <summary>
		/// Called on input press
		/// </summary>
		protected virtual void OnPress(PointerActionInfo pointer)
		{
			
			DoFlickCatch(pointer);
		}
		
		/// <summary>
		/// Called on input release
		/// </summary>
		protected virtual void OnRelease(PointerActionInfo pointer)
		{
			
			DoReleaseFlick(pointer);
		}

		/// <summary>
		/// Called when we drag
		/// </summary>
		protected virtual void OnDrag(PointerActionInfo pointer)
		{
			// Drag panning for touch input
			
			DoDragPan(pointer);
		}

		/// <summary>
		/// Called on pinch gestures
		/// </summary>
		protected virtual void OnPinch(PinchInfo pinch)
		{
			
			DoPinchZoom(pinch);
		}

		/// <summary>
		/// Update current flick velocity
		/// </summary>
		protected void UpdateFlick()
		{
			// Flick?
			if (m_FlickDirection.sqrMagnitude > Mathf.Epsilon)
			{
				m_FlickDirection *= flickDecayFactor;
			}
		}

		/// <summary>
		/// "Catch" flicks on press, to stop the panning momentum
		/// </summary>
		/// <param name="pointer">The press pointer event</param>
		protected void DoFlickCatch(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			// Stop flicks on touch
			if (touchInfo != null)
			{
				m_FlickDirection = Vector2.zero;
			}
		}
		
		/// <summary>
		/// Do flicks, on release only
		/// </summary>
		/// <param name="pointer">The release pointer event</param>
		protected void DoReleaseFlick(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;

			if (touchInfo != null && touchInfo.flickVelocity.sqrMagnitude > Mathf.Epsilon)
			{
			}
		}

		/// <summary>
		/// Controls the pan with a drag
		/// </summary>
		protected void DoDragPan(PointerActionInfo pointer)
		{
			var touchInfo = pointer as TouchInfo;
			if (touchInfo != null)
			{
			}
		}
		
		/// <summary>
		/// Perform a zoom with the given pinch
		/// </summary>
		protected void DoPinchZoom(PinchInfo pinch)
		{
			float currentDistance = (pinch.touch1.currentPosition - pinch.touch2.currentPosition).magnitude;
			float prevDistance = (pinch.touch1.previousPosition - pinch.touch2.previousPosition).magnitude;

			float zoomChange = prevDistance / currentDistance;
		}
	}
}