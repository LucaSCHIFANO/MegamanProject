using UnityEngine;

public class Spawner : MonoBehaviour, ILinkedToRoom
{
    [SerializeField] private SpriteRenderer sr;

    [Header("Enemy")]
    [SerializeField] private Enemy enemy;
    private Enemy spawnedEnemy = null;
    private bool canSpawn;
    private bool waitingToSpawn;
    private bool isDisable;

    [Header("Visible On screen")]
    [SerializeField] LayerMask cameraLayer;




    public Enemy Enemy { get => enemy; }

    private void Awake()
    {
        sr.color = Color.clear;
        canSpawn = true;
        spawnedEnemy = Instantiate(enemy, transform.position, transform.rotation);
    }

    private void CreateEnemy()
    {
        if (isDisable || !canSpawn)
        {
            waitingToSpawn = true;
            return;
        }

        spawnedEnemy.gameObject.SetActive(true);
        spawnedEnemy.Init(this, cameraLayer, transform.position);
    }

    public void EnemyDestroyed()
    {
        spawnedEnemy.gameObject.SetActive(false);
        CreateEnemy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((cameraLayer.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            canSpawn = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((cameraLayer.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            canSpawn = true;
            if (waitingToSpawn)
            {
                waitingToSpawn = false;
                CreateEnemy();
            }
        }
    }

    public void Enable()
    {
        isDisable = false;
        canSpawn = true;
        gameObject.SetActive(true);
        CreateEnemy();
    }

    public void Disable()
    {
        isDisable = true;
        if (spawnedEnemy != null)
        {
            spawnedEnemy.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Disable();
    }
}
