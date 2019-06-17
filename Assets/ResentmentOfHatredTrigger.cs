using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResentmentOfHatredTrigger : BaseTrigger
{
    protected override bool Evaluate(AIBehaviors fsm)
    {
        Transform[] tfms = objectFinder.GetTransforms();
        bool result = true;
        if (tfms.Length > 0)
        {
            for (int i = 0; i < tfms.Length; i++)
            {
                AIBehaviors ai = tfms[i].GetComponent<AIBehaviors>();
                AttackState attackState = ai.GetState<AttackState>();

                Transform target = attackState.GetTarget();
                if (target != null)
                {
                    if (target.GetComponent<LevelAgent>().InstanceId == fsm.levelAgent.InstanceId)
                    {
                        return false;
                    }
                    else {
                        result = true;
                    }
                }
            }
        }
        else
        {
            result = true;
        }
        return result;
    }
}
