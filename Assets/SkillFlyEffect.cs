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
    public Vector3 vEnd;
    private float distanceToTarget;
    public override int AttachActor(AIBehaviors attacker, List<AIBehaviors> targets, List<DamageInfo> skillDamages)
    {
        base.AttachActor(attacker, targets, skillDamages);
        vEnd = targets[0].transform.position;
        distanceToTarget = Vector3.Distance(attacker.transform.position, vEnd);
        if (skill_Effect != null) {
            UpdateEffect();
        }
           
        return 1;
    }
    GameObject flyEffect;
    private void UpdateEffect()
    {
        flyEffect = Instantiate(skill_Effect);
        flyEffect.transform.position = skillActor.transform.position;

        originPoint = myVirtualPosition = myPreviousPosition = flyEffect.transform.position;
        StartCoroutine(ShootLine());
    }
    private bool move = true;
    public float speed ;
    private Vector3 originPoint, myVirtualPosition, myPreviousPosition;
    public float ballisticOffset = 0.5f;
    public float speedUpOverTime = 0.5f;

    IEnumerator ShootLine()
    {
        while (move)
        {
            float originDistance = Vector3.Distance(flyEffect.transform.position, vEnd);
            float distanceToAim = Vector3.Distance(myVirtualPosition, vEnd);

            //总路长/速度=时间
            myVirtualPosition = Vector3.Lerp(flyEffect.transform.position, vEnd, GameManager.RealDeltaTime * speed);

            flyEffect.transform.position = AddBallisticOffset(originDistance, distanceToAim);

            //LookAtDirection2D((Vector3)flyEffect.transform.position - myPreviousPosition);

            myPreviousPosition = flyEffect.transform.position;

            //if (distanceToAim.magnitude < 0.1f)
            if (distanceToAim < 0.1f)
            {
                move = false;
                GameObject.DestroyImmediate(flyEffect);
            }
            yield return null;
        }
    }

    private void LookAtDirection2D(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
