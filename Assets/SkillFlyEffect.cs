using ActionGameFramework.Health;
using AIBehavior;
using QGame.Core.FightEnegin.Damage;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.Game;
using UnityEngine;

public class SkillFlyEffect : SkillBase
{
    [HideInInspector]
    public Vector3 targetable;

    public SkillTargetEffect skillTargetEffect;
    public override int AttachActor(LevelAgent attacker, List<LevelAgent> targets, List<DamageInfo> skillDamages)
    {
        base.AttachActor(attacker, targets, skillDamages);
        targetable = targets[0].transform.position;

        if (skill_Effect != null) {
            UpdateEffect();
        }
           
        return 1;
    }
    GameObject flyEffect;
    private void UpdateEffect()
    {
        flyEffect = Instantiate(skill_Effect);
        flyEffect.transform.position = attacker.transform.position;
        childIcon = flyEffect.transform.Find("Icon");

        counter = 0;
        originPoint = myVirtualPosition = myPreviousPosition = flyEffect.transform.position;
        StartCoroutine(ShootLine());
    }
    private bool move = true;
    public float speed ;
    private Vector3 originPoint, myVirtualPosition, myPreviousPosition;
    public float ballisticOffset = 0.5f;
    public float speedUpOverTime = 0.5f;
    public float hitDistance = 0.2f;
    private float counter;
    public bool freezeRotation = false;


    private Transform childIcon;
    IEnumerator ShootLine()
    {
        while (move)
        {
            counter += Time.fixedDeltaTime;
            // Add acceleration
            speed += Time.fixedDeltaTime * speedUpOverTime;

            // Calculate distance from firepoint to aim
            Vector3 originDistance = targetable - originPoint;
            // Calculate remaining distance
            Vector3 distanceToAim = targetable - (Vector3)myVirtualPosition;
            // Move towards aim
            myVirtualPosition = Vector3.Lerp(originPoint, targetable, counter * speed / originDistance.magnitude);
            // Add ballistic offset to trajectory
            flyEffect.transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);

            // Rotate bullet towards trajectory
            LookAtDirection2D((Vector3)flyEffect.transform.position - myPreviousPosition);

            myPreviousPosition = flyEffect.transform.position;

            // Close enough to hit
            if (distanceToAim.magnitude <= hitDistance)
            {
                move = false;
                GameObject.DestroyImmediate(flyEffect);
                if (isOver!=null) {
                    isOver(this);
                }
                skillTargetEffect.LoadSkill();
                skillTargetEffect.DamageEnemy();
            }
            yield return null;
        }
    }

    private void LookAtDirection2D(Vector3 direction)
    {
        if (freezeRotation == false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            childIcon.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private Vector3 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            // Calculate sinus offset
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            // Add offset to trajectory
            return (Vector3)myVirtualPosition + (ballisticOffset * offset * Vector3.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }
}
