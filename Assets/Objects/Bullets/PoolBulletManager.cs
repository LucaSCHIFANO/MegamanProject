using UnityEngine;
using UnityEngine.Pool;

public class PoolBulletManager : MonoBehaviour
{
    private ObjectPool<Bullet> pool;
    public ObjectPool<Bullet> Pool { get => pool; }

    [SerializeField] private Bullet bullet;

    [SerializeField] private int initialBullets;
    [SerializeField] private int maxBullets;
    
    
    
    private static PoolBulletManager _instance = null;

    public static PoolBulletManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if(_instance == null) _instance = this;
        pool = new ObjectPool<Bullet>(CreateFunction,  OnGetFunction, OnReleaseFunction, OnDestroyFunction, false, initialBullets, maxBullets);
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
