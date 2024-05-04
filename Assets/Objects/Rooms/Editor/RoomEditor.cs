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
    SerializedProperty m_Position;

    SerializedProperty m_RoomBottomLeftLimit;
    SerializedProperty m_RoomTopRightLimit;

    SerializedProperty m_LocalBottomLeftLimit;
    SerializedProperty m_LocalTopRightLimit;

    SerializedProperty m_HandlesColor;
    SerializedProperty m_HandlesColliderColor;
    SerializedProperty m_HandlesCheckPointColor;
    SerializedProperty m_LineThickness;
    SerializedProperty m_CheckPointLineThickness;
    SerializedProperty m_DrawDebug;

    private void OnEnable()
    {
        m_Position = serializedObject.FindProperty("position");

        m_RoomBottomLeftLimit = serializedObject.FindProperty("bottomLeftLimit");
        m_RoomTopRightLimit = serializedObject.FindProperty("topRightLimit");

        m_LocalBottomLeftLimit = serializedObject.FindProperty("worldBottomLeftLimit");
        m_LocalTopRightLimit = serializedObject.FindProperty("worldTopRightLimit");
        
        m_HandlesColor = serializedObject.FindProperty("handlesColor");
        m_HandlesColliderColor = serializedObject.FindProperty("handlesColliderColor");
        m_HandlesCheckPointColor = serializedObject.FindProperty("handlesCheckPointColor");
        m_LineThickness = serializedObject.FindProperty("lineThickness");
        m_CheckPointLineThickness = serializedObject.FindProperty("checkPointlineThickness");
        m_DrawDebug = serializedObject.FindProperty("drawDebug");
    }

    private void OnSceneGUI()
    {
        var room = target as Room;
        if (room == null) return;
        EditorUtility.SetDirty(room);

        room.gameObject.transform.position = room.roomPositionToWorldPosition(m_Position.vector2IntValue);


        m_LocalBottomLeftLimit.vector3Value = room.roomBoundPositionToWorldPosition(m_RoomBottomLeftLimit.vector2IntValue, true);
        m_LocalTopRightLimit.vector3Value = room.roomBoundPositionToWorldPosition(m_RoomTopRightLimit.vector2IntValue, false);

        var bottomLeftLimit = m_LocalBottomLeftLimit.vector3Value + new Vector3(room.transform.position.x, room.transform.position.y);
        var topRightLimit = m_LocalTopRightLimit.vector3Value + new Vector3(room.transform.position.x, room.transform.position.y);

        var bottomRightLimit = new Vector3(topRightLimit.x, bottomLeftLimit.y, 0);
        var topLeftLimit = new Vector3(bottomLeftLimit.x, topRightLimit.y, 0);



        if (m_DrawDebug.boolValue)
        {
            Handles.color = m_HandlesColor.colorValue;

            Handles.DrawLine(bottomLeftLimit, topLeftLimit, m_LineThickness.floatValue);
            Handles.DrawLine(topLeftLimit, topRightLimit, m_LineThickness.floatValue);
            Handles.DrawLine(topRightLimit, bottomRightLimit, m_LineThickness.floatValue);
            Handles.DrawLine(bottomRightLimit, bottomLeftLimit, m_LineThickness.floatValue);


            if (room.Transitions.Count > 0)
            {
                Handles.color = m_HandlesColliderColor.colorValue;


                for (int i = 0; i < room.Transitions.Count; i++)
                {
                    var size = room.GetColliderHeightWidth(room.Transitions[i]) / 2;
                    var centralPosition = room.GetColliderCentralPoint(room.Transitions[i]);

                    var bottomLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y - size.y);
                    var topRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y + size.y);

                    var bottomRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y - size.y);
                    var topLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y + size.y);

                    Handles.DrawLine(bottomLeftCollider, topLeftCollider, m_LineThickness.floatValue);
                    Handles.DrawLine(topLeftCollider, topRightCollider, m_LineThickness.floatValue);
                    Handles.DrawLine(topRightCollider, bottomRightCollider, m_LineThickness.floatValue);
                    Handles.DrawLine(bottomRightCollider, bottomLeftCollider, m_LineThickness.floatValue);

                }

            }

            if (room.CheckPointRoom.Count > 0)
            {
                Handles.color = m_HandlesCheckPointColor.colorValue;


                for (int i = 0; i < room.CheckPointRoom.Count; i++)
                {
                    var roomX = Mathf.Clamp(
                        room.CheckPointRoom[i].checkPointPosition.x,
                        m_RoomBottomLeftLimit.vector2IntValue.x + m_Position.vector2IntValue.x,
                        m_RoomTopRightLimit.vector2IntValue.x + m_Position.vector2IntValue.x);

                    var roomY = Mathf.Clamp(
                        room.CheckPointRoom[i].checkPointPosition.y,
                        m_RoomBottomLeftLimit.vector2IntValue.y + m_Position.vector2IntValue.y,
                        m_RoomTopRightLimit.vector2IntValue.y + m_Position.vector2IntValue.y);
                    room.CheckPointRoom[i].checkPointPosition = new Vector2Int(roomX, roomY);


                    var size = new Vector2(GameData.gridX, GameData.gridY) / 2;
                    var centralPosition = room.roomPositionToWorldPosition(room.CheckPointRoom[i].checkPointPosition);

                    var bottomLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y - size.y);
                    var topRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y + size.y);

                    var bottomRightCollider = new Vector2(centralPosition.x + size.x, centralPosition.y - size.y);
                    var topLeftCollider = new Vector2(centralPosition.x - size.x, centralPosition.y + size.y);

                    Handles.DrawLine(bottomLeftCollider, topLeftCollider, m_CheckPointLineThickness.floatValue);
                    Handles.DrawLine(topLeftCollider, topRightCollider, m_CheckPointLineThickness.floatValue);
                    Handles.DrawLine(topRightCollider, bottomRightCollider, m_CheckPointLineThickness.floatValue);
                    Handles.DrawLine(bottomRightCollider, bottomLeftCollider, m_CheckPointLineThickness.floatValue);

                }

            }
        }

        PrefabUtility.RecordPrefabInstancePropertyModifications(room);
    }

    public override void OnInspectorGUI()
    {
        Vector2 top = m_RoomTopRightLimit.vector2IntValue;
        Vector2 bottom = m_RoomBottomLeftLimit.vector2IntValue;

        DrawPropertiesExcluding(serializedObject, "handlesColor", "handlesColliderColor", "lineThickness", "handlesCheckPointColor", "checkPointlineThickness");

        if (top != m_RoomTopRightLimit.vector2IntValue)
        {
            var TopX = Mathf.Clamp(
                m_RoomTopRightLimit.vector2IntValue.x,
                m_RoomBottomLeftLimit.vector2IntValue.x,
                int.MaxValue
                );
            var TopY = Mathf.Clamp(
                m_RoomTopRightLimit.vector2IntValue.y,
                m_RoomBottomLeftLimit.vector2IntValue.y,
                int.MaxValue
                );
            m_RoomTopRightLimit.vector2IntValue = new Vector2Int(TopX, TopY);
            OnSceneGUI();
        }
        else if (bottom != m_RoomBottomLeftLimit.vector2IntValue) 
        { 
            var BotX = Mathf.Clamp(
                m_RoomBottomLeftLimit.vector2IntValue.x,
                int.MinValue,
                m_RoomTopRightLimit.vector2IntValue.x
                );
            var BotY = Mathf.Clamp(
                m_RoomBottomLeftLimit.vector2IntValue.y,
                int.MinValue,
                m_RoomTopRightLimit.vector2IntValue.y
                );
            m_RoomBottomLeftLimit.vector2IntValue = new Vector2Int(BotX, BotY);
            OnSceneGUI();
        }

        if (m_DrawDebug.boolValue) 
        {
            EditorGUILayout.PropertyField(m_HandlesColor, new GUIContent("Handles Color"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_HandlesColliderColor, new GUIContent("Handles Collider Color"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_HandlesCheckPointColor, new GUIContent("Handles Checkpoint Color"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_LineThickness, new GUIContent("Line Thickness"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(m_CheckPointLineThickness, new GUIContent("Checkpoint Line Thickness"), GUILayout.Height(20));
        }

            serializedObject.ApplyModifiedProperties();
    }
}



[CustomPropertyDrawer(typeof(Transition)), CanEditMultipleObjects]
public class TransitionEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var transitionSideRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var onlyOnLadderRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);


        var isColliderReducedRect = new Rect(position.x, position.y + 40f, position.width, EditorGUIUtility.singleLineHeight);
        var offsetRect = new Rect(position.x, position.y + 60f, position.width, EditorGUIUtility.singleLineHeight);
        var sizeRect = new Rect(position.x, position.y + 80f, position.width, EditorGUIUtility.singleLineHeight);

        var transitionSide = property.FindPropertyRelative("transitionSide");
        var onlyOnLadder = property.FindPropertyRelative("onlyOnLadder");
        var isColliderReduced = property.FindPropertyRelative("isColliderReduced");
        var offset = property.FindPropertyRelative("offset");
        var size = property.FindPropertyRelative("size");


        transitionSide.intValue = EditorGUI.Popup(transitionSideRect, "Transition side", transitionSide.intValue, transitionSide.enumNames);
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
            return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 5.75f);
        else return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 3.75f);
    }
}

[CustomPropertyDrawer(typeof(CheckPointRoom)), CanEditMultipleObjects]
public class CheckPointRoomEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var checkPointPositionRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var checkPointPosition = property.FindPropertyRelative("checkPointPosition");

        EditorGUI.PropertyField(checkPointPositionRect, checkPointPosition);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 1f);
    }
}