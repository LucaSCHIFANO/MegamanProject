using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Megaman")]
    [SerializeField] private MegamanController megamanPrefab;

    private MegamanController megaman;
    public MegamanController Megaman { get => megaman; }

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    private RoomManager roomManager;


    private static LevelManager _instance = null;

    public static LevelManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance != null) return; 
        
        _instance = this;
        megaman = Instantiate(megamanPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void Start()
    {
        roomManager = RoomManager.Instance;
    }

    public void Restart()
    {
        megaman.transform.position = spawnPoint.position;
        roomManager.SetCameraRooWithMegamanPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Restart();
    }

}
