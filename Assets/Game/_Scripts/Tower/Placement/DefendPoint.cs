using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position for defenders.
/// </summary>
public class DefendPoint : MonoBehaviour
{
	// List with defend places for this defend point
	private List<Transform> defendPlaces = new List<Transform>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		foreach (Transform child in transform)
		{
			defendPlaces.Add(child);
		}
	}

    /// <summary>
    /// Gets the defend points list.
    /// </summary>
    /// <returns>The defend points.</returns>
    public List<Transform> GetDefendPoints()
    {
		return defendPlaces;
    }
}
