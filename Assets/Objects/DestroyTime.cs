using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    [SerializeField] private float timeBeforeDestruction;

    private void Awake()
    {
        if(timeBeforeDestruction < 0) timeBeforeDestruction = 0;
        Destroy(gameObject, timeBeforeDestruction);
    }
}
