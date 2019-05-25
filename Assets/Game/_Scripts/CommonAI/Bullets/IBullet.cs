using ActionGameFramework.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all bullets.
/// </summary>
public interface IBullet
{
	int GetDamage();
    void Initialize(Transform target);
}
