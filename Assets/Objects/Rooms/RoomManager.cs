using System.Collections.Generic;
using UnityEngine;
using static Room;

public class RoomManager : MonoBehaviour
{
    private float gridX = GameData.gridX;
    private float gridY = GameData.gridY;

    [Header("Rooms")]
    [SerializeField] private List<Room> rooms= new List<Room>();
    private Dictionary<Vector2Int, Room> roomGrid = new Dictionary<Vector2Int, Room>();
    
    private CameraMovement currentCamera;

    private static RoomManager _instance = null;

    public static RoomManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;

        foreach (Room room in rooms)
        {
            for (int i = room.RoomBottomLeftLimit.x; i <= room.RoomTopRightLimit.x; i++)
            {
                for (int j = room.RoomBottomLeftLimit.y; j <= room.RoomTopRightLimit.y; j++)
                {
                    roomGrid.Add(new Vector2Int(i + room.RoomPosition.x, j + room.RoomPosition.y), room);
                }
            }
        }
    }

    private void Start()
    {
        SetCameraRooWithMegamanPosition();
    }

    public void SetCameraRooWithMegamanPosition()
    {
        currentCamera = CameraMovement.Instance;

        if (currentCamera != null)
        {
            int roomId = GetAdjacentId(worldPositionToRoomPosition(LevelManager.Instance.Megaman.transform.position), TransitionSide.None);
            if (roomId >= 0 && roomId < rooms.Count) SetNewRoom(roomId, false);
            else SetNewRoom(0, false);

        }
    }

    public Vector2Int worldPositionToRoomPosition(Vector3 position) // same function as room :/
    {
        int posX = Mathf.FloorToInt((position.x + gridX / 2) / gridX);
        int posY = Mathf.FloorToInt((position.y + gridY / 2) / gridY);

        return new Vector2Int(posX, posY);
    }


    public void SetNewRoom(int id, bool hasTransition)
    {
        if (id == -1) return;
        currentCamera.ChangeRoom(rooms[id], hasTransition);
    }



    public int GetAdjacentId(Vector2Int roomPosition, TransitionSide transitionSide)
    {
        Vector2Int nextRoomPosition = roomPosition;

        switch (transitionSide)
        {
            case TransitionSide.Left:
                nextRoomPosition += new Vector2Int(-1, 0);
                break;
            case TransitionSide.Right:
                nextRoomPosition += new Vector2Int(1, 0);
                break;
            case TransitionSide.Bottom:
                nextRoomPosition += new Vector2Int(0, -1);
                break;
            case TransitionSide.Top:
                nextRoomPosition += new Vector2Int(0, 1);
                break;
        }

        foreach (var room in roomGrid)
        {
            if (room.Key == nextRoomPosition)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (room.Value == rooms[i]) return i;
                }
            }
        }
        return -1;
    }
}
