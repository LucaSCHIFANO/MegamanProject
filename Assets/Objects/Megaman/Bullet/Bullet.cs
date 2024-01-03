using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isInitialized;

    [SerializeField] private float speed;
    private Vector2 movementDirection;

    public void Init(Vector2 _direction)
    {
        movementDirection = _direction;
        isInitialized = true;
    }

    public void Init(Vector2 _direction, float _speed)
    {
        movementDirection = _direction;
        speed = _speed;
        isInitialized = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }


    void FixedUpdate()
    {
        if (!isInitialized) return;

        Move();
    }

    private void Move()
    {
        rb.velocity = movementDirection * speed;
        if(movementDirection.x < 0) sr.flipX = true;
        else sr.flipX = false;
    }
}
