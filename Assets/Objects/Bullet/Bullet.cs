using System;
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

    private Action<Bullet> killAction;

    public void Init(Vector2 _position, Vector2 _direction, Action<Bullet> _action)
    {
        transform.position = _position;
        movementDirection = _direction;
        killAction = _action;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInitialized) return;
        Debug.Log("collision");
        if (collision.gameObject.tag != "Player") killAction(this);
    }
}
