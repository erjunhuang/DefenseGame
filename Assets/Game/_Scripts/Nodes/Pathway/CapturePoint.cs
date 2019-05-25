using QGame.Core.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If enemy rise this point - player will be defeated.
/// </summary>
public class CapturePoint : MonoBehaviour
{
    /// <summary>
    /// Raises the trigger enter2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        //EventManager.TriggerEvent("Captured", other.gameObject, null);
        XEventBus.Instance.Post(EventId.Captured);
    }
}
