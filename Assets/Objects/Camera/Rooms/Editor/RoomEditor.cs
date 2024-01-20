using Codice.Client.Commands;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    SerializedProperty m_HandlesColor;
    SerializedProperty m_HandlesColliderColor;
    SerializedProperty m_LineThickness;

    private void OnEnable()
    {
        m_HandlesColor = serializedObject.FindProperty("handlesColor");
        m_HandlesColliderColor = serializedObject.FindProperty("handlesColliderColor");
        m_LineThickness = serializedObject.FindProperty("lineThickness");
    }

    private void OnSceneGUI()
    {
        var room = target as Room;
        if (room == null) return;
        EditorUtility.SetDirty(room);

        room.gameObject.transform.position = room.roomPositionToWorldPosition(room.RoomPosition);

        room.LocalBottomLeftLimit = room.roomBoundPositionToWorldPosition(room.RoomBottomLeftLimit, true);
        room.LocalTopRightLimit = room.roomBoundPositionToWorldPosition(room.RoomTopRightLimit, false);

        var bottomLeftLimit = room.LocalBottomLeftLimit + new Vector2(room.transform.position.x, room.transform.position.y);
        var topRightLimit = room.LocalTopRightLimit + new Vector2(room.transform.position.x, room.transform.position.y);

        var bottomRightLimit = new Vector3(topRightLimit.x, bottomLeftLimit.y, 0);
        var topLeftLimit = new Vector3(bottomLeftLimit.x, topRightLimit.y, 0);



        if (room.DrawDebug)
        {

            var lineThickness = room.LineThickness;

            Handles.color = room.HandlesColor;

            Handles.DrawLine(bottomLeftLimit, topLeftLimit, lineThickness);
            Handles.DrawLine(topLeftLimit, topRightLimit, lineThickness);
            Handles.DrawLine(topRightLimit, bottomRightLimit, lineThickness);
            Handles.DrawLine(bottomRightLimit, bottomLeftLimit, lineThickness);


            if (room.Transitions.Count > 0)
            {
                Handles.color = room.HandlesColliderColor;


                for (int i = 0; i < room.Transitions.Count; i++)
                {
                    var size = room.GetColliderHeightWidth(room.Transitions[i]) / 2;
                    var centralPosition = room.GetColliderCentralPoint(room.Transitions[i]);

                    var bottomLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y - size.y);
                    var topRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y + size.y);

                    var bottomRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y - size.y);
                    var topLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y + size.y);

                    Handles.DrawLine(bottomLeftCollider, topLeftCollider, lineThickness);
                    Handles.DrawLine(topLeftCollider, topRightCollider, lineThickness);
                    Handles.DrawLine(topRightCollider, bottomRightCollider, lineThickness);
                    Handles.DrawLine(bottomRightCollider, bottomLeftCollider, lineThickness);

                }

            }
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(room);
    }

    public override void OnInspectorGUI()
    {
        var room = target as Room;

        DrawPropertiesExcluding(serializedObject, "handlesColor", "handlesColliderColor", "lineThickness");

        if (room.DrawDebug) 
        {
            EditorGUILayout.PropertyField(m_HandlesColor, new GUIContent("Handles Color"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_HandlesColliderColor, new GUIContent("Handles Collider Color"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_LineThickness, new GUIContent("Line Thickness"), GUILayout.Height(20));
        }

            serializedObject.ApplyModifiedProperties();
    }
}



[CustomPropertyDrawer(typeof(Transition))]
public class TransitionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position,label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var transitionSideRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var newRoomIdRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);
        var onlyOnLadderRect = new Rect(position.x, position.y + 40f, position.width, EditorGUIUtility.singleLineHeight);


        var isColliderReducedRect = new Rect(position.x, position.y + 70f, position.width, EditorGUIUtility.singleLineHeight);
        var offsetRect = new Rect(position.x, position.y + 90f, position.width, EditorGUIUtility.singleLineHeight);
        var sizeRect = new Rect(position.x, position.y + 110f, position.width, EditorGUIUtility.singleLineHeight);

        var transitionSide = property.FindPropertyRelative("transitionSide");
        var newRoomId = property.FindPropertyRelative("newRoomID");
        var onlyOnLadder = property.FindPropertyRelative("onlyOnLadder");
        var isColliderReduced = property.FindPropertyRelative("isColliderReduced");
        var offset = property.FindPropertyRelative("offset");
        var size = property.FindPropertyRelative("size");


        transitionSide.intValue = EditorGUI.Popup(transitionSideRect, "Transition side", transitionSide.intValue, transitionSide.enumNames);
        EditorGUI.PropertyField(newRoomIdRect, newRoomId);
        EditorGUI.PropertyField(onlyOnLadderRect, onlyOnLadder);
        EditorGUI.PropertyField(isColliderReducedRect, isColliderReduced);

        if (isColliderReduced.boolValue)
        {
            EditorGUI.PropertyField(offsetRect, offset);
            EditorGUI.PropertyField(sizeRect, size);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.FindPropertyRelative("isColliderReduced").boolValue)
            return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 7f);
        else return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 5f);
    }
}