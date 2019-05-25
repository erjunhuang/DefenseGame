using QGame.Core.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Operation of UI icon with spells (user actions).
/// </summary>
public class UserActionIcon : MonoBehaviour
{
	// Spell cooldown
	public float cooldown = 10f;
	// Spell prefab
	public GameObject userActionPrefab;
	// Icon for highlighted state
	public GameObject highlightIcon;
	// Icon for cooldown state
	public GameObject cooldownIcon;
	// Cooldown counter (UI Text)
	public Text cooldownText;

	// Machine state
	private enum MyState
	{
		Active,
		Highligted,
		Cooldown
	}
	// Current state for this instance
	private MyState myState = MyState.Active;
	// Active user action, instantiated when highlited
	private GameObject activeUserAction;
	// Counter for cooldown
	private float cooldownCounter;

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
        XEventBus.Instance.Register(EventId.UserUiClick, UserUiClick);
        XEventBus.Instance.Register(EventId.ActionStart, ActionStart);
        XEventBus.Instance.Register(EventId.ActionCancel, ActionCancel);
    }


	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable()
	{

        XEventBus.Instance.UnRegister(EventId.UserUiClick, UserUiClick);
        XEventBus.Instance.UnRegister(EventId.ActionStart, ActionStart);
        XEventBus.Instance.UnRegister(EventId.ActionCancel, ActionCancel);
    }

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		Debug.Assert(userActionPrefab && highlightIcon && cooldownIcon && cooldownText, "Wrong initial settings");
		StopCooldown();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (myState == MyState.Cooldown)
		{
			if (cooldownCounter > 0f)
			{
				cooldownCounter -= Time.deltaTime;
				UpdateCooldownText();
			}
			else if (cooldownCounter <= 0f)
			{
				StopCooldown();
			}
		}
	}

	/// <summary>
	/// User UI click handler.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="param">Parameter.</param>
	private void UserUiClick(XEventArgs args)
	{
        GameObject obj = args.GetData<GameObject>();
        if (obj == gameObject)	// Clicked on this icon
		{
			if (myState == MyState.Active)
			{
				highlightIcon.SetActive(true);
				activeUserAction = Instantiate(userActionPrefab);
				myState = MyState.Highligted;
			}
		}
		else if (myState == MyState.Highligted) // Clicked on other UI
		{
			highlightIcon.SetActive(false);
			myState = MyState.Active;
		}
	}

	/// <summary>
	/// Action start handler.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="param">Parameter.</param>
	private void ActionStart(XEventArgs args)
	{
        GameObject obj = args.GetData<GameObject>();
        if (obj == activeUserAction)
		{
			activeUserAction = null;
			highlightIcon.SetActive(false);
			StartCooldown();
		}
	}

	/// <summary>
	/// Actions cancel handler.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="param">Parameter.</param>
	private void ActionCancel(XEventArgs args)
	{
        GameObject obj = args.GetData<GameObject>();
        if (obj == activeUserAction)
		{
			activeUserAction = null;
			highlightIcon.SetActive(false);
			myState = MyState.Active;
		}
	}

	/// <summary>
	/// Starts the cooldown.
	/// </summary>
	private void StartCooldown()
	{
		myState = MyState.Cooldown;
		cooldownCounter = cooldown;
		cooldownIcon.gameObject.SetActive(true);
		cooldownText.gameObject.SetActive(true);
	}

	/// <summary>
	/// Stops the cooldown.
	/// </summary>
	private void StopCooldown()
	{
		myState = MyState.Active;
		cooldownCounter = 0f;
		cooldownIcon.gameObject.SetActive(false);
		cooldownText.gameObject.SetActive(false);
	}

	/// <summary>
	/// Updates the cooldown counter text.
	/// </summary>
	private void UpdateCooldownText()
	{
		cooldownText.text = ((int)Mathf.Ceil(cooldownCounter)).ToString();
	}
}
