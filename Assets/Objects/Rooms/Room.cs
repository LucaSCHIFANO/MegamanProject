using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;


public class Room : MonoBehaviour
{
    private float gridX = GameData.gridX;
    private float gridY = GameData.gridY;

    enum RoomType
    {
        Normal,
        CheckPoint,
        Boss,
    }

    //[Space]
    [SerializeField] private RoomType roomType;

    //[Header("Room Postition")]
    [SerializeField] private Vector2Int position;

    //[Header("Room Limits")]
    [SerializeField] private Vector2Int bottomLeftLimit;
    [SerializeField] private Vector2Int topRightLimit;
    [SerializeField, HideInInspector] private Vector3 worldBottomLeftLimit;
    [SerializeField, HideInInspector] private Vector3 worldTopRightLimit;
    private bool isRoomActive;

    //[Header("Transitions")]
    [SerializeField] private RoomTransition roomTransitionPrefab;
    [SerializeField] private List<Transition> transitions = new List<Transition>();
    private List<RoomTransition>roomTransitions = new List<RoomTransition>();

    //[Space]
    //[Header("Check Points")]
    [SerializeField] private Checkpoint roomCheckPointPrefab;
    [SerializeField] private List<CheckPointRoom> checkPoint = new List<CheckPointRoom>();

    //Boss
    [SerializeField] private Boss bossPrefab;

    public enum TransitionSide
    {
        None,
        Left,
        Right,
        Bottom,
        Top
    }
    
    //Debug
    [SerializeField] private bool drawDebug;
    [SerializeField] private Color handlesColor = Color.red;
    [SerializeField] private Color handlesColliderColor = Color.green;
    [SerializeField] private Color handlesCheckPointColor = Color.blue;
    [SerializeField] private float lineThickness = 3;
    [SerializeField] private float checkPointlineThickness = 1.5f;


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

    //Lists
    [SerializeField, HideInInspector] public List<Transition> Transitions { get => transitions;}
    public List<CheckPointRoom> CheckPointRoom { get => checkPoint; set => checkPoint = value; }
    #endregion


    private void Awake()
    {
        if (roomTransitionPrefab == null) return;
      
        for (int i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].transitionSide != TransitionSide.None) SetTransitionHitbox(transitions[i]);  
        }

        SetRoomActive(false);
    }

    private void Start()
    {
        foreach (var transition in roomTransitions)
        {
            var nextRoomPosition = transition.transform.position;
            switch (transition.TransitionSide)
            {
                case TransitionSide.Left:
                    nextRoomPosition += new Vector3(GameData.roomColliderThickness, 0);
                    break;
                case TransitionSide.Right:
                    nextRoomPosition += new Vector3(-GameData.roomColliderThickness, 0);
                    break;
                case TransitionSide.Bottom:
                    nextRoomPosition += new Vector3(0, GameData.roomColliderThickness);
                    break;
                case TransitionSide.Top:
                    nextRoomPosition += new Vector3(0, -GameData.roomColliderThickness);
                    break;
            }

            var manager = RoomManager.Instance;
            transition.NewRoomID = manager.GetAdjacentId(manager.worldPositionToRoomPosition(nextRoomPosition),transition.TransitionSide);
        }

        if (roomType == RoomType.CheckPoint)
        {
            foreach (var checkPoint in checkPoint)
            {
                var newCheckPoint = Instantiate(roomCheckPointPrefab, roomPositionToWorldPosition(checkPoint.checkPointPosition), transform.rotation, transform);

                newCheckPoint.SetRespawnPoint(checkPoint.spawnPointPosition);

                var cpCollider = newCheckPoint.GetComponent<BoxCollider2D>();
                cpCollider.isTrigger = true;
                cpCollider.size = new Vector2(gridX, gridY);
            }
        }
    }

    void SetTransitionHitbox(Transition transition)
    {
        var transitionGO = Instantiate(roomTransitionPrefab, transform);
        transitionGO.transform.position = GetColliderCentralPoint(transition);
        BoxCollider2D transitionCollider = transitionGO.GetComponent<BoxCollider2D>();

        transitionGO.SetData(transition.transitionSide, transition.onlyOnLadder, transition.isBossTransition);

        transitionCollider.isTrigger = true;
        transitionCollider.size = GetColliderHeightWidth(transition);
        transitionCollider.enabled = false;
        if (transition.isBossTransition)
            transitionCollider.offset -= new Vector2(0, transition.size / 4); // Set the collider at the bottom of the door

        roomTransitions.Add(transitionGO);
    }

    public void SetRoomActive(bool active)
    {
        isRoomActive = active;

        if (roomType != RoomType.Boss)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (active)
                    transform.GetChild(i).GetComponent<ILinkedToRoom>()?.Enable();
                else
                    transform.GetChild(i).GetComponent<ILinkedToRoom>()?.Disable();
            }
        }
        else
        {

        }

        
    }

    #region SetPosition
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
    #endregion

    #region Collider
    public Vector3 GetColliderCentralPoint(Transition transition)
    {
        Vector3 centralTransitionPoint = Vector3.zero;
        float offset = 0f;

        if(transition.isColliderReduced || transition.isBossTransition) offset = GetOffset(transition);

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
        float size = 0f;
        if (transition.isBossTransition)
            size = GameData.bossTransitionHeightEditor;
        else
            size = transition.size;

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
            case TransitionSide.Right:
                var height = worldTopRightLimit.y - worldBottomLeftLimit.y;
                offset = (height - (height * size)) * 0.5f;
                break;

            case TransitionSide.Bottom:
            case TransitionSide.Top:
                var width = worldTopRightLimit.x - worldBottomLeftLimit.x;
                offset = (width - (width * size)) * 0.5f;
                break;

        }

        return Mathf.Lerp(-offset , offset , trueTransitionOffset);

    }

    public Vector2 GetColliderHeightWidth(Transition transition, bool isEditor = false)
    {
        float witdh = GameData.roomColliderThickness;
        float height = GameData.roomColliderThickness;
        float sizeMultiplicator = 1f;

        if (transition.isBossTransition)
        {
            witdh = GameData.bossRoomColliderThickness;
            height = GameData.bossRoomColliderThickness;
            sizeMultiplicator = isEditor ? GameData.bossTransitionHeightEditor : GameData.bossTransitionHeight;
        }
        else if (transition.isColliderReduced) sizeMultiplicator = transition.size;

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
    #endregion

    [ContextMenu("Add room to RoomManager")]
    public void AddRoomToRoomManager()
    {
        RoomManager[] rooms = FindObjectsOfType(typeof(RoomManager)) as RoomManager[];
        switch (rooms.Length)
        {
            case 0:
                EditorUtility.DisplayDialog("No roomManager found!", "There is no roomManager in the scene or they're deactivated. Add one and retry.", "Ok");
                break;
            case 1:
                if(rooms[0].AddRoom(this))
                    EditorUtility.DisplayDialog("Room added!", $"\"{this.name}\" was successfully added to \"{rooms[0].name}\"!", "Ok");
                else
                    EditorUtility.DisplayDialog("Error", $"Only one roomManager was detected (\"{rooms[0].name}\"), but an error occurred. " +
                        $"Maybe \"{this.name}\" is already in \"{rooms[0].name}\"", "Ok");
                break;
            default:
                foreach (RoomManager item in rooms)
                {
                    if (EditorUtility.DisplayDialogComplex("RoomManager found!", $"Do you want to add \"{this.name}\" to \"{item.name}\"?", "Yes", "No", "") == 0)
                        if (item.AddRoom(this))
                            EditorUtility.DisplayDialog("Room added!", $"\"{this.name}\" was successfully added to \"{item.name}\"!", "Ok");
                        else
                            EditorUtility.DisplayDialog("Error", $"Maybe \"{this.name}\" is already in \"{item.name}\"", "Ok");
                }
                break;
        }
    }
}
[Serializable]
public class Transition
{
    public Room.TransitionSide transitionSide;
    public int newRoomID;

    public bool onlyOnLadder = false;
    public bool isBossTransition = false;

    public bool isColliderReduced = false;

    [Range(-1f, 1f)] public float offset = 0f;
    [Range(0.05f, 1)] public float size = 1f;

}

[Serializable]
public class CheckPointRoom
{
    public Vector2Int checkPointPosition;
    [Range(0f, 1f)] public float offset = 0.5f;
    [Range(0f, 1f)] public float minimumHeight = 0f;
    [HideInInspector] public Vector2 spawnPointPosition;
}
