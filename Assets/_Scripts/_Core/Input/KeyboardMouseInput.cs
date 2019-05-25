using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Core.Input
{
	/// <summary>
	/// Base control scheme for desktop devices, which performs CameraRig motion
	/// </summary>
	public class KeyboardMouseInput : CameraInputScheme
	{

		/// <summary>
		/// Gets whether the scheme should be activated or not
		/// </summary>
		public override bool shouldActivate
		{
			get
			{
				if (UnityInput.touchCount > 0)
				{
					return false;
				}
				bool anyKey = UnityInput.anyKey;
				bool buttonPressedThisFrame = InputController.instance.mouseButtonPressedThisFrame;
				bool movedMouseThisFrame = InputController.instance.mouseMovedOnThisFrame;

				return (anyKey || buttonPressedThisFrame || movedMouseThisFrame);
			}
		}

		/// <summary>
		/// This is the default scheme on desktop devices
		/// </summary>
		public override bool isDefault
		{
			get
			{
#if UNITY_STANDALONE || UNITY_EDITOR
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

			InputController controller = InputController.instance;

			controller.spunWheel += OnWheel;
			controller.dragged += OnDrag;
			controller.pressed += OnPress;
            controller.released += OnRelease;
            controller.startedDrag += OnStartedDrag;
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

			InputController controller = InputController.instance;

			controller.pressed -= OnPress;
			controller.dragged -= OnDrag;
			controller.spunWheel -= OnWheel;
            controller.released -= OnRelease;
            controller.startedDrag -= OnStartedDrag;
        }
		
		/// <summary>
		/// Handle camera panning behaviour
		/// </summary>
		protected virtual void Update()
		{
			DoScreenEdgePan();
			DoKeyboardPan();
		}

		/// <summary>
		/// Called when we drag
		/// </summary>
		protected virtual void OnDrag(PointerActionInfo pointer)
		{
		}

		/// <summary>
		/// Called on mouse wheel input
		/// </summary>
		protected virtual void OnWheel(WheelInfo wheel)
		{
			
			DoWheelZoom(wheel);
		}

		/// <summary>
		/// Called on input press, for MMB panning
		/// </summary>
		protected virtual void OnPress(PointerActionInfo pointer)
		{
			DoMiddleMousePan(pointer);
		}

        protected virtual void OnRelease(PointerActionInfo pointer)
        {
        }

        protected virtual void OnStartedDrag(PointerActionInfo pointer)
        {
        }

		/// <summary>
		/// Perform mouse screen-edge panning
		/// </summary>
		protected void DoScreenEdgePan()
		{
			Vector2 mousePos = UnityInput.mousePosition;

			bool mouseInside = (mousePos.x >= 0) &&
			                   (mousePos.x < Screen.width) &&
			                   (mousePos.y >= 0) &&
			                   (mousePos.y < Screen.height);

			// Mouse can be outside of our window
			if (mouseInside)
			{
			}
		}

		/// <summary>
		/// Perform keyboard panning
		/// </summary>
		protected void DoKeyboardPan()
		{
			// Left
			if (UnityInput.GetKey(KeyCode.LeftArrow) || UnityInput.GetKey(KeyCode.A))
			{
				
			}

			// Right
			if (UnityInput.GetKey(KeyCode.RightArrow) || UnityInput.GetKey(KeyCode.D))
			{
			}

			// Down
			if (UnityInput.GetKey(KeyCode.DownArrow) || UnityInput.GetKey(KeyCode.S))
			{
			}

			// Up
			if (UnityInput.GetKey(KeyCode.UpArrow) || UnityInput.GetKey(KeyCode.W))
			{
			}
		}

		/// <summary>
		/// Perform mouse wheel zooming
		/// </summary>
		protected void DoWheelZoom(WheelInfo wheel)
		{
		}

		/// <summary>
		/// Pan with middle mouse
		/// </summary>
		/// <param name="pointer">Pointer with press event</param>
		protected void DoMiddleMousePan(PointerActionInfo pointer)
		{
			var mouseInfo = pointer as MouseButtonInfo;

			// Pan to mouse position on MMB
			if ((mouseInfo != null) &&
			    (mouseInfo.mouseButtonId == 2))
			{
			}
		}
	}
}