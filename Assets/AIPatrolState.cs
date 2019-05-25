using AIBehavior;
using System.Collections;
using System.Collections.Generic;
using TargetDefense.Nodes;
using UnityEditor;
using UnityEngine;

public class AIPatrolState : BaseState
{
    public Node currentNode;
    protected override void Action(AIBehaviors fsm)
    {
        if (currentNode != null)
        {
            // If destination reached
            if ((Vector2)currentNode.transform.position == (Vector2)transform.position)
            {
                // Get next waypoint from my path
                currentNode = currentNode.GetNextNode();
                if (currentNode != null)
                {
                    // Set destination for navigation agent
                    fsm.MoveAgentWithVector(currentNode.transform, movementSpeed, rotationSpeed);
                }
            }
            else {
                fsm.MoveAgentWithVector(currentNode.transform, movementSpeed, rotationSpeed);
            }
        }
    }

    protected override void Init(AIBehaviors fsm)
    {
        movementSpeed = fsm.monsterInfo.MoveSpeed;
        rotationSpeed = 0;
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
