using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private MegamanController megaman;

    public MegamanController Megaman { get => megaman; }

    private static LevelManager _instance = null;

    public static LevelManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

}
