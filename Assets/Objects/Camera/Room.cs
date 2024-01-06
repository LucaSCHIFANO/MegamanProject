using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Limits")]
    [SerializeField] private Vector3 bottomLeftLimit = new Vector3(-1.2f, -0.9f, 0);
    [SerializeField] private Vector3 topRightLimit = new Vector3(1.2f, 0.9f, 0);
    
    [Header("Debug")]
    [SerializeField] private Color handlesColor;
    [SerializeField] private float lineThickness = 3;

    public Vector3 BottomLeftLimit { get => bottomLeftLimit + transform.position;  }
    public Vector3 TopRightLimit { get => topRightLimit + transform.position;  }
    public Color HandlesColor { get => handlesColor; }
    public float LineThickness { get => lineThickness; }

}
