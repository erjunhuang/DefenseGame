using UnityEngine;
using System.Collections.Generic;


namespace AIBehavior
{
    public class AISkillStates : MonoBehaviour
    {
        public AISkillState[] states = new AISkillState[1];

        public GameObject skillStatesGameObject = null;

        private Dictionary<string, AISkillState> statesDictionary = new Dictionary<string, AISkillState>();


        public AISkillState GetStateWithName(string stateName)
        {
            if (statesDictionary.ContainsKey(stateName))
            {
                return statesDictionary[stateName];
            }

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].name == stateName)
                {
                    statesDictionary[stateName] = states[i];
                    return states[i];
                }
            }

            return null;
        }

        public AISkillState GetStateNoCoolingTime()
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].currentCoolDown <= 0 && states[i].Id > 0)
                {
                    return states[i];
                }
            }
            return null;
        }
    }
}