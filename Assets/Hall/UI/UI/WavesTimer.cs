using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TargetDefense.Level;
using Core.Utilities;
using QGame.Core.Event;
/// <summary>
/// Timer to display current enemy wave.
/// </summary>
[RequireComponent(typeof(Image))]
public class WavesTimer : MonoBehaviour
{
	// Visualisation of remaining TO
	public Image timeBar;
    // Current wave text field
    public Text currentWaveText;
	// Max wave text field
    public Text maxWaveNumberText;
	// Effect of highlighted timer
	public GameObject highlightedFX;
	// Duration for highlighted effect
	public float highlightedTO = 0.2f;


    // TO before next wave
    private WaveManager waveManager;

    private float currentWaveFinishTime;
    private float time;
    private bool isStartWave;
    /// <summary>
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
	{
		StopAllCoroutines ();
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
    void Awake()
    {
        Debug.Assert(timeBar && highlightedFX && timeBar && currentWaveText && maxWaveNumberText, "Wrong initial settings");
    }

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
    {
        XEventBus.Instance.Register(EventId.WaveChanged, OnWaveChanged);

        highlightedFX.SetActive(false);
        currentWaveText.text = "0";
        timeBar.fillAmount = 0;
        time = 0;
        isStartWave = false;
    }
    void OnWaveChanged(XEventArgs args) {

        int waveNumber = args.GetData<int>(0);
        int timeToNextWave = args.GetData<int>(1);
        int wavesCount = args.GetData<int>(2);
         
        currentWaveText.text = waveNumber.ToString();
        currentWaveFinishTime = timeToNextWave;
        maxWaveNumberText.text = wavesCount.ToString();
        isStartWave = true;

        StartCoroutine("HighlightTimer");
    }
   
    /// <summary>
    /// Update this instance.
    /// </summary>
    void FixedUpdate()
    {
        if (isStartWave) {
            time += Time.fixedDeltaTime;
            if (time >= currentWaveFinishTime)
            {
                time = 0;
                isStartWave = false;
            }
            timeBar.fillAmount = time / currentWaveFinishTime;
        }
    }

	/// <summary>
	/// Highlights the timer coroutine.
	/// </summary>
	/// <returns>The timer.</returns>
	private IEnumerator HighlightTimer()
	{
		highlightedFX.SetActive(true);
		yield return new WaitForSeconds(highlightedTO);
		highlightedFX.SetActive(false);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
        XEventBus.Instance.UnRegister(EventId.WaveChanged, OnWaveChanged);
	}
}
