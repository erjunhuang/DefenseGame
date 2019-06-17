using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroNavigationTrigger : BaseTrigger
{
    protected override bool Evaluate(AIBehaviors fsm)
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position,Vector3.forward, out hit))
            {
                AIBehaviors ai = GetComponent<AIBehaviors>();
                SeekState seekState = ai.GetState<SeekState>();
                seekState.seekTarget = hit.collider.transform;
            }
            return true;
        }

        return false;
    }
}
