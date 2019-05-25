using GameModel;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Targets;
using UnityEngine;

public class RadiusVisualizerController : MonoBehaviour {
    public GameObject radiusVisualizerPrefab;
    public float radiusVisualizerHeight = 0.02f;
    public Vector3 localEuler;
    readonly List<GameObject> m_RadiusVisualizers = new List<GameObject>();
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupRadiusVisualizers(Monster target, Transform ghost) {
      
        if (m_RadiusVisualizers.Count < 1)
        {
            m_RadiusVisualizers.Add(Instantiate(radiusVisualizerPrefab));
        }
        GameObject radiusVisualizer = m_RadiusVisualizers[0];
        radiusVisualizer.SetActive(true);
        radiusVisualizer.transform.SetParent(ghost);
        radiusVisualizer.transform.localPosition = new Vector3(0, radiusVisualizerHeight, 0);
        radiusVisualizer.transform.localScale = Vector3.one * target.EffectRadius * 2.0f;
        radiusVisualizer.transform.localRotation = new Quaternion { eulerAngles = localEuler };

        var visualizerRenderer = radiusVisualizer.GetComponent<SpriteRenderer>();
        if (visualizerRenderer != null)
        {
            visualizerRenderer.color = new Color(1, 1, 0, 0.5f);
        }
    }

    public void HideRadiusVisualizers()
    {
        foreach (GameObject radiusVisualizer in m_RadiusVisualizers)
        {
            radiusVisualizer.transform.parent = transform;
            radiusVisualizer.SetActive(false);
        }
    }
}
