using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    private bool isInitialized;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Speed")]
    [SerializeField] private float speed;
    private Vector2 movementDirection;

    [Header("Base Damage")]
    [SerializeField] private int baseDamage;

    [Header("Collisions")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameData.Side side;
    [SerializeField] private GameData.WeaponType weaponType;

    [Header("Destroy")]
    private Action<Bullet> killAction;
    IBulletEmiter bulletEmitter;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    public void Init(Vector2 _position, Vector2 _direction, Action<Bullet> _action, IBulletEmiter _bulletEmitter)
    {
        transform.position = _position;
        movementDirection = _direction;
        killAction = _action;
        bulletEmitter = _bulletEmitter;

        isInitialized = true;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized) return;
        if ((collisionMask.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            Entity colliderEntity = collision.gameObject.GetComponent<Entity>();

            if (colliderEntity != null)
            {
                if (colliderEntity.GetSide != side)
                {
                    colliderEntity.TakeDamage(weaponType, baseDamage);
                    DestroyBullet();
                }
            }
            else DestroyBullet();
        }
        
    }


    public void DestroyBullet()
    {
        bulletEmitter.BulletDestroyed(this);
        killAction(this);
    }
}
