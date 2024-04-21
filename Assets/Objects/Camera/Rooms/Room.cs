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
    [SerializeField] private bool drawDebug;
    [SerializeField] private Color handlesColor = Color.red;
    [SerializeField] private Color handlesColliderColor = Color.green;
    [SerializeField] private float lineThickness = 3;

    [SerializeField, HideInInspector] public List<Transition> Transitions { get => transitions;}

    #region GetSet

    //Position (Room Ref)
    public Vector2Int RoomPosition { get => position; set => position = value; }
    public Vector2Int RoomBottomLeftLimit { get => bottomLeftLimit; set => bottomLeftLimit = value; }
    public Vector2Int RoomTopRightLimit { get => topRightLimit; set => topRightLimit = value; }

    //Position (Local Ref)
    public Vector2 LocalBottomLeftLimit { get => worldBottomLeftLimit; set => worldBottomLeftLimit = value; }
    public Vector2 LocalTopRightLimit { get => worldTopRightLimit; set => worldTopRightLimit = value; }

    //Position (World Ref)
    public Vector2 WorldBottomLeftLimit { get => worldBottomLeftLimit + transform.position;}
    public Vector2 WorldTopRightLimit { get => worldTopRightLimit + transform.position;}


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

    private void Start()
    {
        foreach (var transition in roomTransitions)
        {
            var nextRoomPosition = transition.transform.position;
            switch (transition.TransitionSide)
            {
                case TransitionSide.Left:
                    nextRoomPosition += new Vector3(0.1f, 0);
                    break;
                case TransitionSide.Right:
                    nextRoomPosition += new Vector3(-0.1f, 0);
                    break;
                case TransitionSide.Bottom:
                    nextRoomPosition += new Vector3(0, 0.1f);
                    break;
                case TransitionSide.Top:
                    nextRoomPosition += new Vector3(0, -0.1f);
                    break;
            }

            var manager = RoomManager.Instance;
            transition.NewRoomID = manager.GetAdjacentId(manager.worldPositionToRoomPosition(nextRoomPosition),transition.TransitionSide);
        }
    }

    void SetTransitionHitbox(Transition transition)
    {
        var transitionGO = Instantiate(roomTransitionPrefab, transform);
        transitionGO.transform.position = GetColliderCentralPoint(transition);
        BoxCollider2D transitionCollider = transitionGO.GetComponent<BoxCollider2D>();

        transitionGO.SetData(transition.transitionSide, transition.onlyOnLadder);

        transitionCollider.isTrigger = true;
        transitionCollider.size = GetColliderHeightWidth(transition);
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


    public Vector3 GetColliderCentralPoint(Transition transition)
    {
        Vector3 centralTransitionPoint = Vector3.zero;
        float offset = 0f;
        
        if(transition.isColliderReduced) offset = GetOffset(transition);

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
                centralTransitionPoint += new Vector3(worldBottomLeftLimit.x, (worldTopRightLimit.y + worldBottomLeftLimit.y) / 2 + offset);
                break;

            case TransitionSide.Right:
                centralTransitionPoint += new Vector3(worldTopRightLimit.x, (worldTopRightLimit.y + worldBottomLeftLimit.y) / 2 + offset);
                break;

            case TransitionSide.Bottom:
                centralTransitionPoint += new Vector3((worldTopRightLimit.x + worldBottomLeftLimit.x) / 2 + offset, worldBottomLeftLimit.y);
                break;

            case TransitionSide.Top:
                centralTransitionPoint += new Vector3((worldTopRightLimit.x + worldBottomLeftLimit.x) / 2 + offset, worldTopRightLimit.y);
                break;

        }

        return centralTransitionPoint + transform.position;
    }

    private float GetOffset(Transition transition)
    {
        float offset = 0f;
        float trueTransitionOffset = (transition.offset + 1) / 2; // From -1,1 to 0, 1

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
            case TransitionSide.Right:
                var height = worldTopRightLimit.y - worldBottomLeftLimit.y;
                offset = (height - (height * transition.size)) * 0.5f;
                break;

            case TransitionSide.Bottom:
            case TransitionSide.Top:
                var width = worldTopRightLimit.x - worldBottomLeftLimit.x;
                offset = (width - (width * transition.size)) * 0.5f;
                break;

        }

        return Mathf.Lerp(-offset , offset , trueTransitionOffset);

    }

    public Vector2 GetColliderHeightWidth(Transition transition)
    {

        float witdh = 0.1f;
        float height = 0.1f;
        float sizeMultiplicator = 1f;

        if (transition.isColliderReduced) sizeMultiplicator = transition.size;

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
                height = (worldTopRightLimit.y - worldBottomLeftLimit.y) * sizeMultiplicator; 
                break;

            case TransitionSide.Right:
                height = (worldTopRightLimit.y - worldBottomLeftLimit.y) * sizeMultiplicator;
                break;

            case TransitionSide.Bottom:
                witdh = (worldTopRightLimit.x - worldBottomLeftLimit.x) * sizeMultiplicator;
                break;

            case TransitionSide.Top:
                witdh = (worldTopRightLimit.x - worldBottomLeftLimit.x) * sizeMultiplicator;
                break;

        }

        return new Vector2(witdh, height);
    }
}


[Serializable]
public class Transition
{
    public TransitionSide transitionSide;
    public int newRoomID;

    public bool onlyOnLadder = false;

    public bool isColliderReduced = false;

    [Range(-1f, 1f)] public float offset = 0f;
    [Range(0.05f, 1)] public float size = 1f;

}
