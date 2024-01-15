using System;
using System.Collections.Generic;
using UnityEngine;
using static Room;

public class Room : MonoBehaviour
{
    public const float gridX = 2.4f;
    public const float gridY = 1.8f;

    [Header("Room Postition")]
    [SerializeField] private Vector2Int position;

    [Header("Room Limits")]
    [SerializeField] private Vector2Int bottomLeftLimit;
    [SerializeField] private Vector2Int topRightLimit;
    [SerializeField, HideInInspector] private Vector3 worldBottomLeftLimit;
    [SerializeField, HideInInspector] private Vector3 worldTopRightLimit;
    private bool isRoomActive;

    [Header("Transitions")]
    [SerializeField] private RoomTransition roomTransitionPrefab;
    [SerializeField] private List<Transition> transitions = new List<Transition>();
    private List<RoomTransition>roomTransitions = new List<RoomTransition>();

    private float roomColliderThickness = 0.1f;
    private BoxCollider2D transitionCollider;

    public enum TransitionSide
    {
        None,
        Left,
        Right,
        Bottom,
        Top
    }
    
    [Header("Debug")]
    [SerializeField] private Color handlesColor;
    [SerializeField] private float lineThickness = 3;

    #region GetSet

    //Position (Room Ref)
    public Vector3 GameObjectPosition { get => transform.position; set => transform.position = value; }
    public Vector2Int RoomPosition { get => position; set => position = value; }
    public Vector2Int RoomBottomLeftLimit { get => bottomLeftLimit; set => bottomLeftLimit = value; }
    public Vector2Int RoomTopRightLimit { get => topRightLimit; set => topRightLimit = value; }

    //Position (Local Ref)
    public Vector2 LocalBottomLeftLimit { get => worldBottomLeftLimit; set => worldBottomLeftLimit = value; }
    public Vector2 LocalTopRightLimit { get => worldTopRightLimit; set => worldTopRightLimit = value; }

    //Position (World Ref)
    public Vector2 WorldBottomLeftLimit { get => worldBottomLeftLimit + transform.position;}
    public Vector2 WorldTopRightLimit { get => worldTopRightLimit + transform.position;}

    //Debug
    public Color HandlesColor { get => handlesColor; }
    public float LineThickness { get => lineThickness; }

    #endregion

    private void Awake()
    {
        if (roomTransitionPrefab == null) return;

        roomColliderThickness = GameData.roomColliderThickness;
        
        for (int i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].transitionSide != TransitionSide.None) SetTransitionHitbox(transitions[i]);  
        }
    }

    void SetTransitionHitbox(Transition transition)
    {
        Vector3 centralTransitionPoint = Vector3.zero;
        float height = 0.1f;
        float witdh = 0.1f;

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
                height = (worldTopRightLimit.y - worldBottomLeftLimit.y);
                centralTransitionPoint += new Vector3(worldBottomLeftLimit.x, (worldTopRightLimit.y + worldBottomLeftLimit.y) / 2);
                break;

            case TransitionSide.Right:
                height = (worldTopRightLimit.y - worldBottomLeftLimit.y);
                centralTransitionPoint += new Vector3(worldTopRightLimit.x, (worldTopRightLimit.y + worldBottomLeftLimit.y) / 2);
                break;

            case TransitionSide.Bottom:
                witdh = worldTopRightLimit.x - worldBottomLeftLimit.x;
                centralTransitionPoint += new Vector3((worldTopRightLimit.x + worldBottomLeftLimit.x) / 2, worldBottomLeftLimit.y);
                break;

            case TransitionSide.Top:
                witdh = worldTopRightLimit.x - worldBottomLeftLimit.x;
                centralTransitionPoint += new Vector3((worldTopRightLimit.x + worldBottomLeftLimit.x) / 2, worldTopRightLimit.y);
                break;

        }

        var transitionGO = Instantiate(roomTransitionPrefab, transform);
        transitionGO.transform.localPosition = centralTransitionPoint;
        BoxCollider2D transitionCollider = transitionGO.GetComponent<BoxCollider2D>();

        transitionGO.SetData(transition.transitionSide, transition.newRoomID);

        transitionCollider.isTrigger = true;
        transitionCollider.size = new Vector2(witdh, height);
        transitionCollider.enabled = false;

        roomTransitions.Add(transitionGO);
    }
    public void SetRoomActive(bool active)
    {
        isRoomActive = active;
        foreach (var item in roomTransitions)
        {
          item.SetRoomActive(active);
        }
        
    }

    public Vector3 roomPositionToWorldPosition(Vector2Int position)
    {
        return new Vector3(gridX * position.x, gridY * position.y, 0);
    }

    public Vector3 roomBoundPositionToWorldPosition(Vector2Int position, bool isBottomLeft)
    {
        if (isBottomLeft) return new Vector3(-gridX/2 + (gridX*position.x), -gridY / 2 + (gridY * position.y), 0);
        else return new Vector3(gridX / 2 + (gridX * position.x), gridY / 2 + (gridY * position.y), 0);
    }

    public Vector3 roomBoundPositionToWorldPosition(Vector3 position, bool isBottomLeft)
    {
        if (isBottomLeft) return new Vector3(-gridX / 2 + (gridX * position.x), -gridY / 2 + (gridY * position.y), 0);
        else return new Vector3(gridX / 2 + (gridX * position.x), gridY / 2 + (gridY * position.y), 0);
    }
}


[Serializable]
public class Transition
{
    public TransitionSide transitionSide;
    public int newRoomID;

}
