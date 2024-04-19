using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private int startingRoom;
    [SerializeField] private List<Room> rooms= new List<Room>();
    
    private CameraMovement currentCamera;

    private static RoomManager _instance = null;

    public static RoomManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        currentCamera = CameraMovement.Instance;

        if(currentCamera != null)
        {
            if (startingRoom < rooms.Count) SetNewRoom(startingRoom, false);
            else SetNewRoom(0, false);
            
        }
    }



    public void SetNewRoom(int id, bool hasTransition)
    {
        currentCamera.ChangeRoom(rooms[id], hasTransition);
    }
}
