using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolBulletManager : MonoBehaviour
{
    public ObjectPool<Bullet> Pool { get => pool; }
    private ObjectPool<Bullet> pool;

    [SerializeField] private Bullet bullet;
    
    
    
    private static PoolBulletManager _instance = null;

    public static PoolBulletManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
        pool = new ObjectPool<Bullet>(CreateFunction,  OnGetFunction, OnReleaseFunction, OnDestroyFunction, false, 1, 3);
    }

    #region Pool Functions

    private Bullet CreateFunction()
    {
        return Instantiate(bullet);
    }

    private void OnGetFunction(Bullet _bulletGet)
    {
        _bulletGet.gameObject.SetActive(true);
    }

    private void OnReleaseFunction(Bullet _bulletToRelease)
    {
        _bulletToRelease.gameObject.SetActive(false);
    }

    private void OnDestroyFunction(Bullet _bulletToDestroy)
    {
        Destroy(_bulletToDestroy.gameObject);
    }
    #endregion 
}
