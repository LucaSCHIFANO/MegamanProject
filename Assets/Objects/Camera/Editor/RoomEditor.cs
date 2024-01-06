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

        var bottomLeftLimit = room.BottomLeftLimit;
        var topRightLimit = room.TopRightLimit; 
        var bottomRightLimit = new Vector3(topRightLimit.x, bottomLeftLimit.y, 0);
        var topLeftLimit = new Vector3(bottomLeftLimit.x, topRightLimit.y, 0);

        var lineThickness = room.LineThickness;

        Handles.color = room.HandlesColor;


        Handles.DrawLine(bottomLeftLimit, topLeftLimit, lineThickness);
        Handles.DrawLine(topLeftLimit, topRightLimit, lineThickness);
        Handles.DrawLine(topRightLimit, bottomRightLimit, lineThickness);
        Handles.DrawLine(bottomRightLimit, bottomLeftLimit, lineThickness);
    }
}
