using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [SerializeField] private Side side;

    public Side GetSide { get => side; }

    public enum Side
    {
        Player,
        Enemy
    }


    public void TakeDamage()
    {
        Debug.Log($"{gameObject.name} take damage !");
    }
}
