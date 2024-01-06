using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 hiddenTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed;
    private float currentSpeed;

    private Room currentRoom;
    private Vector2 cameraSize;

    [SerializeField] float roomTransitionTime;



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
        currentRoom = newRoom;
        var htXpos = Mathf.Clamp(target.position.x, currentRoom.BottomLeftLimit.x + cameraSize.x / 2, currentRoom.TopRightLimit.x - cameraSize.x / 2);
        var htYpos = Mathf.Clamp(target.position.y, currentRoom.BottomLeftLimit.y + cameraSize.y / 2, currentRoom.TopRightLimit.y - cameraSize.y / 2);
        hiddenTarget = new Vector3(htXpos, htYpos, 0);

        if(hasTransition) StartCoroutine(RoomTransition());
    }

    IEnumerator RoomTransition()
    {
        currentSpeed = Vector2.Distance(transform.position, hiddenTarget) / roomTransitionTime;
        yield return new WaitForSeconds(roomTransitionTime);
        currentSpeed = speed;
    }

}
