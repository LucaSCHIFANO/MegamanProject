using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RoomManager : MonoBehaviour
{
    private CameraMovement currentCamera;
    [SerializeField] private int startingRoom;
    [SerializeField] private List<Room> rooms= new List<Room>();

    private static RoomManager _instance = null;

    public static RoomManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        currentCamera = CameraMovement.Instance;

        if(currentCamera != null)
        {
            if (startingRoom < rooms.Count) SetNewRoom(startingRoom, false);
            else
            {
                Debug.LogError($"Room {startingRoom} not found.");
                SetNewRoom(0, false);
            }
        }
    }



    public void SetNewRoom(int id, bool hasTransition)
    {
        currentCamera.ChangeRoom(rooms[id], hasTransition);
    }
}
