using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera")]
    private Transform target;
    private Vector3 hiddenTarget;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed;
    private float currentSpeed;


    [Header("Transition")]
    
    private Room currentRoom;
    private Vector2 cameraSize;


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
    private void Start()
    {
        target = LevelManager.Instance.Megaman.transform;
    }


    private void Update()
    {
        if(target == null) return;

        if (currentRoom != null) {
            var htXpos = Mathf.Clamp(target.position.x, currentRoom.WorldBottomLeftLimit.x + cameraSize.x / 2, currentRoom.WorldTopRightLimit.x - cameraSize.x / 2);
            var htYpos = Mathf.Clamp(target.position.y, currentRoom.WorldBottomLeftLimit.y + cameraSize.y / 2, currentRoom.WorldTopRightLimit.y - cameraSize.y / 2);
            hiddenTarget = new Vector3(htXpos, htYpos, 0);
        }
        else hiddenTarget = target.position;

        transform.position = Vector3.MoveTowards(transform.position, hiddenTarget + offset, currentSpeed * Time.deltaTime);
    }

    public void ChangeRoom(Room newRoom, RoomTransition transition)
    {
        currentRoom?.SetRoomActive(false);
        currentRoom = newRoom;
        
        var htXpos = Mathf.Clamp(target.position.x, currentRoom.WorldBottomLeftLimit.x + cameraSize.x / 2, currentRoom.WorldTopRightLimit.x - cameraSize.x / 2);
        var htYpos = Mathf.Clamp(target.position.y, currentRoom.WorldBottomLeftLimit.y + cameraSize.y / 2, currentRoom.WorldTopRightLimit.y - cameraSize.y / 2);
        hiddenTarget = new Vector3(htXpos, htYpos, 0);

        if (transition != null) StartCoroutine(RoomTransition(transition));
        else currentRoom.SetRoomActive(true);
        
    }

    public IEnumerator WaitToActivateRoom(float timeToWait)
    {
        currentRoom.SetRoomActive(false);
        yield return new WaitForSeconds(timeToWait);
        currentRoom.SetRoomActive(true);
    }

    IEnumerator RoomTransition(RoomTransition transition)
    {
        currentSpeed = Vector2.Distance(transform.position, hiddenTarget) / GameData.roomTransitionTime;

        if (transition.CurrentBossDoor)
            yield return new WaitForSeconds(GameData.roomTransitionTime + transition.CurrentBossDoor.DoorAnimationLenght);
        else
            yield return new WaitForSeconds(GameData.roomTransitionTime);

        currentSpeed = speed;
        currentRoom.SetRoomActive(true);

    }
}
