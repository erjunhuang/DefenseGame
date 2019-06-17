using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Nodes;
using UnityEditor;
using UnityEngine;

public class OffensiveState : BaseState
{
    public float sqrDistanceThreshold = 0.1f;
    public Node currentNode;
    protected override void Action(AIBehaviors fsm)
    {
        if (currentNode != null)
        {
            // If destination reached

            Vector3 targetDir = currentNode.transform.position - transform.position;
            if (targetDir.sqrMagnitude<= sqrDistanceThreshold* sqrDistanceThreshold)
            {
                // Get next waypoint from my path
                currentNode = currentNode.GetNextNode();
                if (currentNode != null)
                {
                    // Set destination for navigation agent
                    //fsm.levelAgent.MoveAgentWithVector(currentNode.transform, movementSpeed, rotationSpeed);
                    fsm.MoveAgent(currentNode.transform, movementSpeed, rotationSpeed);
                }
            }
            else {
                //fsm.levelAgent.MoveAgentWithVector(currentNode.transform, movementSpeed, rotationSpeed);
                //fsm.MoveAgent(currentNode.transform, movementSpeed, rotationSpeed);
            }
        }
    }

    protected override void Init(AIBehaviors fsm)
    {
        movementSpeed = fsm.levelAgent.currentTargetLevelData.monster.MoveSpeed;
        rotationSpeed = 0;
        Debug.Log(currentNode.transform.localPosition);
        fsm.MoveAgent(currentNode.transform, movementSpeed, rotationSpeed);
    }

    protected override bool Reason(AIBehaviors fsm)
    {
        return true;
    }

    protected override void StateEnded(AIBehaviors fsm)
    {
    
    }
#if UNITY_EDITOR
    // === Editor Functions === //

    public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
    {
    }


    protected override void DrawStateInspectorEditor(SerializedObject stateObject, AIBehaviors fsm)
    {
        SerializedProperty property;

        GUILayout.Label("巡逻属性:", EditorStyles.boldLabel);

        GUILayout.BeginVertical(GUI.skin.box);

        property = stateObject.FindProperty("currentNode");
        EditorGUILayout.PropertyField(property);

        GUILayout.EndVertical();

        stateObject.ApplyModifiedProperties(); ;
    }
#endif

}
