using Codice.Client.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
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

        var lineThickness = room.LineThickness;

        Handles.color = room.HandlesColor;


        Handles.DrawLine(bottomLeftLimit, topLeftLimit, lineThickness);
        Handles.DrawLine(topLeftLimit, topRightLimit, lineThickness);
        Handles.DrawLine(topRightLimit, bottomRightLimit, lineThickness);
        Handles.DrawLine(bottomRightLimit, bottomLeftLimit, lineThickness);

        PrefabUtility.RecordPrefabInstancePropertyModifications(room);
    }
}