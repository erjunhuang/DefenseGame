using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTrigger : BaseTrigger
{   
    protected override void Awake()
    {
    }


    protected override void Init(AIBehaviors fsm)
    {
    }


    protected override bool Evaluate(AIBehaviors fsm)
    {
        AISkillState state = fsm.skillStates.GetStateNoCoolingTime();
        if (state != null) {
            return true;
        }
        return false;
    }


    protected override void OnTriggered()
    {
       
    }
}
