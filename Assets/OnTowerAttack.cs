using ActionGameFramework.Health;
using AIBehavior;
using Core.Health;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Towers;
using UnityEngine;

public class OnTowerAttack : MonoBehaviour {
    public GameObject projectilePrefab;
    protected Transform launchPointWeapon;
    protected ILauncher m_Launcher;
    public Damager damagerProjectile
    {
        get { return projectilePrefab == null ? null : projectilePrefab.GetComponent<Damager>(); }
    }
    private void Awake()
    {
        if (m_Launcher == null)
        {
            m_Launcher = GetComponent<ILauncher>();
        }
        launchPointWeapon = transform;
    }
    public void MeleeAttack(AttackData attackData) {
        Debug.Log("Melee attack");
        AIBehaviors aIBehaviors = attackData.target.GetComponent<AIBehaviors>();
        aIBehaviors.Damage(attackData.attackState.attackDamage, attackData.target.position, attackData.fsm.configuration.alignmentProvider);
    }

    public void RangedAttack(AttackData attackData)
    {
        if (attackData.target != null)
        {
            damagerProjectile.alignmentProvider = attackData.target.GetComponent<AIBehaviors>().configuration.alignmentProvider;
            damagerProjectile.damage = (int)attackData.attackState.attackDamage;

            GameObject attack = GameObject.Instantiate(projectilePrefab, launchPointWeapon.position, launchPointWeapon.rotation) as GameObject;
            attack.transform.position = launchPointWeapon.position;
            attack.transform.rotation = launchPointWeapon.rotation;
            IBullet bullet = attack.GetComponent<IBullet>();
            bullet.Initialize(attackData.target);

            Debug.Log("Attacked target '" + attackData.target.name + "' with attack state named '" + attackData.attackState.name + "' with damage " + attackData.damage);
        }
        else
        {
            Debug.LogWarning("attackData.target is null, you may want to have a NoPlayerInSight trigger on the AI '" + attackData.attackState.transform.parent.name + "'");
        }
    }

    float CalculateDamage(AttackData attackData)
    {
        float minDamage = attackData.damage - attackData.plusOrMinusDamage;
        float maxDamage = attackData.damage + attackData.plusOrMinusDamage;

        return Random.Range(minDamage, maxDamage);
    }

}
