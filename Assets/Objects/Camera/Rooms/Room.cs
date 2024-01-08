using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Room;

public class Room : MonoBehaviour
{
    [Header("Room Limits")]
    [SerializeField] private Vector3 bottomLeftLimit = new Vector3(-1.2f, -0.9f, 0);
    [SerializeField] private Vector3 topRightLimit = new Vector3(1.2f, 0.9f, 0);
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
        Top,
        Bottom
    }
    
    [Header("Debug")]
    [SerializeField] private Color handlesColor;
    [SerializeField] private float lineThickness = 3;

    public Vector3 BottomLeftLimit { get => bottomLeftLimit + transform.position;  }
    public Vector3 TopRightLimit { get => topRightLimit + transform.position;  }
    public Color HandlesColor { get => handlesColor; }
    public float LineThickness { get => lineThickness; }

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
        Vector3 centralTransitionPoint = transform.position;
        float height = 0.1f;
        float witdh = 0.1f;

        switch (transition.transitionSide)
        {
            case TransitionSide.Left:
                height = topRightLimit.y - bottomLeftLimit.y;
                centralTransitionPoint += new Vector3(bottomLeftLimit.x, bottomLeftLimit.y + height / 2);
                break;
            case TransitionSide.Right:
                height = topRightLimit.y - bottomLeftLimit.y;
                centralTransitionPoint += new Vector3(topRightLimit.x, topRightLimit.y - height / 2);
                break;
            case TransitionSide.Top:
                witdh = topRightLimit.x - bottomLeftLimit.x;
                centralTransitionPoint += new Vector3(topRightLimit.x - witdh / 2, topRightLimit.y);
                break;
            case TransitionSide.Bottom:
                witdh = topRightLimit.x - bottomLeftLimit.x;
                centralTransitionPoint += new Vector3(bottomLeftLimit.x + witdh / 2, bottomLeftLimit.y);
                break;
            default:
                break;
        }

        var transitionGO = Instantiate(roomTransitionPrefab, centralTransitionPoint, transform.rotation, transform);
        transitionGO.SetData(transition.transitionSide, transition.newRoomID);
        BoxCollider2D transitionCollider = transitionGO.GetComponent<BoxCollider2D>();

        //transitionCollider.offset = centralTransitionPoint;
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

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRoomActive && collision.gameObject.tag == "Player")
        {
            RoomManager.Instance.SetNewRoom(newRoomID, true);
        }
    }*/



}

[Serializable]
public class Transition
{
    public TransitionSide transitionSide;
    public int newRoomID;

}
