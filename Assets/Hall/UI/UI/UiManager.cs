using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TargetDefense.Economy;
using QGame.Core.Event;
using TargetDefense.Level;
using Core.Utilities;

/// <summary>
/// User interface and events manager.
/// </summary>
public class UiManager : Singleton<UiManager>
{
    // This scene will loaded after whis level exit
    public string exitSceneName;
	// Start screen canvas
	public GameObject startScreen;
    // Pause menu canvas
    public GameObject pauseMenu;
    // Defeat menu canvas
    public GameObject defeatMenu;
    // Victory menu canvas
    public GameObject victoryMenu;
    // Level interface
    public GameObject levelUI;
    // Avaliable gold amount
    public Text goldAmount;
	// Capture attempts before defeat
	public Text defeatAttempts;
	// Victory and defeat menu display delay
	public float menuDisplayDelay = 1f;

    // Is game paused?
    private bool paused;
    // Camera is dragging now
    private bool cameraIsDragged;
    // Origin point of camera dragging start
    private Vector3 dragOrigin = Vector3.zero;
    // Camera control component
    private CameraControl cameraControl;

    private LevelManager levelManager;

    /// <summary>
    /// Awake this instance.
    /// </summary>
   protected override void Awake()
	{
        base.Awake();
		cameraControl = FindObjectOfType<CameraControl>();
    }
 
    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        if (TargetDefense.Level.LevelManager.instanceExists)
        {
            levelManager = TargetDefense.Level.LevelManager.instance;
            levelManager.currency.currencyChanged += OnCurrencyChanged;
        }

        PauseGame(true);
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (paused == false)
        {
            if (cameraIsDragged == true)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
				// Camera dragging (inverted)
                cameraControl.MoveX(-pos.x);
                cameraControl.MoveY(-pos.y);
            }
        }
    }

    /// <summary>
    /// Stop current scene and load new scene
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
    private void LoadScene(string sceneName)
    {
        XEventBus.Instance.Post(EventId.SceneQuit);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
	private void ResumeGame()
    {
        GoToLevel();
        PauseGame(false);
    }

    /// <summary>
    /// Gos to main menu.
    /// </summary>
	private void ExitFromLevel()
    {
        LoadScene(exitSceneName);
    }

    /// <summary>
    /// Closes all UI canvases.
    /// </summary>
    private void CloseAllUI()
    {
		startScreen.SetActive (false);
        pauseMenu.SetActive(false);
        defeatMenu.SetActive(false);
        victoryMenu.SetActive(false);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    /// <param name="pause">If set to <c>true</c> pause.</param>
    private void PauseGame(bool pause)
    {   
        paused = pause;
        // Stop the time on pause
        Time.timeScale = pause ? 0f : 1f;
        XEventBus.Instance.Post(EventId.GamePaused, new XEventArgs(null,pause.ToString()));
    }

    /// <summary>
    /// Gos to pause menu.
    /// </summary>
	private void GoToPauseMenu()
    {
        PauseGame(true);
        CloseAllUI();
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Gos to level.
    /// </summary>
    public void GoToLevel()
    {
        CloseAllUI();
        levelUI.SetActive(true);
        PauseGame(false);
        XEventBus.Instance.Post(EventId.WaveStart);
    }

    /// <summary>
    /// Gos to defeat menu.
    /// </summary>
    public void GoToDefeatMenu()
    {
		StartCoroutine("DefeatCoroutine");
    }

	/// <summary>
	/// Display defeat menu after delay.
	/// </summary>
	/// <returns>The coroutine.</returns>
	private IEnumerator DefeatCoroutine()
	{
		yield return new WaitForSeconds(menuDisplayDelay);
		PauseGame(true);
		CloseAllUI();
		defeatMenu.SetActive(true);
	}

    /// <summary>
    /// Gos to victory menu.
    /// </summary>
    public void GoToVictoryMenu()
    {
		StartCoroutine("VictoryCoroutine");
    }

	/// <summary>
	/// Display victory menu after delay.
	/// </summary>
	/// <returns>The coroutine.</returns>
	private IEnumerator VictoryCoroutine()
	{
		yield return new WaitForSeconds(menuDisplayDelay);
		PauseGame(true);
		CloseAllUI();

		victoryMenu.SetActive(true);
	}

    /// <summary>
    /// Restarts current level.
    /// </summary>
	private void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Gets current gold amount.
    /// </summary>
    /// <returns>The gold.</returns>
    private int GetGold()
    {
        return TargetDefense.Level.LevelManager.instance.currency.currentCurrency;
    }

    public void OnCurrencyChanged()
    {
        goldAmount.text = GetGold().ToString();
    }


	/// <summary>
	/// Sets the defeat attempts.
	/// </summary>
	/// <param name="attempts">Attempts.</param>
	public void SetDefeatAttempts(int attempts)
	{
		defeatAttempts.text = attempts.ToString();
	}

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    protected override void OnDestroy()
	{
        base.OnDestroy();
        if (levelManager != null)
        {
            levelManager.currency.currencyChanged -= OnCurrencyChanged;
        }
        StopAllCoroutines();
    }
}
