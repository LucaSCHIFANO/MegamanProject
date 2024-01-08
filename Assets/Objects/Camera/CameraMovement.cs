using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform target;
    private Vector3 hiddenTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed;
    private float currentSpeed;


    [Header("Transition")]
    
    private Room currentRoom;
    private Room previousRoom; //use during transition
    private Vector2 cameraSize;
    private bool inTransition;


    private static CameraMovement _instance = null;

    public static CameraMovement Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;

        var cam = GetComponent<Camera>();
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        cameraSize = new Vector2(width, height);

        currentSpeed = speed;
    }


    private void Update()
    {
        if(target == null) return;

        if (currentRoom != null) {
            var htXpos = Mathf.Clamp(target.position.x, currentRoom.BottomLeftLimit.x + cameraSize.x / 2, currentRoom.TopRightLimit.x - cameraSize.x / 2);
            var htYpos = Mathf.Clamp(target.position.y, currentRoom.BottomLeftLimit.y + cameraSize.y / 2, currentRoom.TopRightLimit.y - cameraSize.y / 2);
            hiddenTarget = new Vector3(htXpos, htYpos, 0);
        }
        else hiddenTarget = target.position;

        transform.position = Vector3.MoveTowards(transform.position, hiddenTarget + offset, currentSpeed * Time.deltaTime);
    }

    public void ChangeRoom(Room newRoom, bool hasTransition)
    {
        if (currentRoom != null)
        {
            currentRoom.SetRoomActive(false);
            previousRoom = currentRoom;
        }

        currentRoom = newRoom;
        var htXpos = Mathf.Clamp(target.position.x, currentRoom.BottomLeftLimit.x + cameraSize.x / 2, currentRoom.TopRightLimit.x - cameraSize.x / 2);
        var htYpos = Mathf.Clamp(target.position.y, currentRoom.BottomLeftLimit.y + cameraSize.y / 2, currentRoom.TopRightLimit.y - cameraSize.y / 2);
        hiddenTarget = new Vector3(htXpos, htYpos, 0);

        if (hasTransition) StartCoroutine(RoomTransition());
        else currentRoom.SetRoomActive(true);
        
    }

    IEnumerator RoomTransition()
    {
        inTransition = true;
        currentSpeed = Vector2.Distance(transform.position, hiddenTarget) / GameData.roomTransitionTime;

        yield return new WaitForSeconds(GameData.roomTransitionTime);

        inTransition = false;
        currentSpeed = speed;
        currentRoom.SetRoomActive(true);

        previousRoom = currentRoom;
    }

/*    public Room.TransitionSide GetCurrentRoomTransition()
    {
        Debug.Log(previousRoom.name);
        Debug.Log(previousRoom.GetTransitionSide);

         return previousRoom.GetTransitionSide;
    }*/

}
