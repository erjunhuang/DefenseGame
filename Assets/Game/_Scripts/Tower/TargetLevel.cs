using System.Collections.Generic;
using Core.Health;
using TargetDefense.Targets.Data;
using UnityEngine;
using TargetDefense.Targets;
using TargetDefense.Affectors;
[DisallowMultipleComponent]
public class TargetLevel : MonoBehaviour , ISerializationCallbackReceiver
{
    public GameObject buildEffectPrefab;
    public Tower m_ParentTower { get; protected set; }
    public LayerMask mask { get; protected set; }

    Affector[] m_Affectors;
    protected Affector[] Affectors
    {
        get
        {
            if (m_Affectors == null)
            {
                m_Affectors = GetComponents<Affector>();
            }
            return m_Affectors;
        }
    }

    public void Initialize(Tower target, LayerMask enemyMask, IAlignmentProvider alignment)
    {
        mask = enemyMask;
        m_ParentTower = target;
        foreach (Affector effect in Affectors)
        {
            effect.Initialize(alignment, enemyMask);
        }

    }

    public void SetAffectorState(bool state)
    {
        foreach (Affector affector in Affectors)
        {
            if (affector != null)
            {
                affector.enabled = state;
            }
        }
    }

    public List<ITowerRadiusProvider> GetRadiusVisualizers()
    {
        List<ITowerRadiusProvider> visualizers = new List<ITowerRadiusProvider>();
        foreach (Affector affector in Affectors)
        {
            var visualizer = affector as ITowerRadiusProvider;
            if (visualizer != null)
            {
                visualizers.Add(visualizer);
            }
        }
        return visualizers;
    }

    public float GetTowerDps()
    {
        float dps = 0;
        //foreach (Affector affector in Affectors)
        //{
        //    var attack = affector as RemoteAttackAffector;
        //    if (attack != null && attack.damagerProjectile != null)
        //    {
        //        dps += attack.GetProjectileDamage() * attack.fireRate;
        //    }
        //}
        return dps;
    }

    public void Kill()
    {
        m_ParentTower.KillTower();
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        // Setting this member to null is required because we are setting this value on a prefab which will 
        // persists post run in editor, so we null this member to ensure it is repopulated every run
        m_Affectors = null;
    }

    void Start()
    {
        if (buildEffectPrefab != null)
        {
            Instantiate(buildEffectPrefab, transform);
        }
    }

}
