using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Exclusive")]
    [Space]

    [Header("Damage Received")]
    [SerializeField] private SOWeaponChart weaponChart;

    [Header("Visible on Screen")]
    private LayerMask cameraLayer;
    private bool isActive = false;
    private bool canDespawn = false;
    private Spawner spawner;

    public void Init(Spawner spawner, LayerMask mask, Vector3 position)
    {
        this.spawner = spawner;
        this.cameraLayer = mask;
        this.transform.position = position;
    }

    private void Update()
    {
        if(isActive)
            transform.position += Vector3.right * 0.001f;
    }

    public override void TakeDamage(GameData.WeaponType weaponType, int baseDamage)
    {
        if (!isActive) return;

        bool typeFound = false;
        if (weaponChart != null)
        {
            foreach (var item in weaponChart.weaponDamages)
            {
                if (item.WeaponType == weaponType)
                {
                    ReduceLife(item.Damage);
                    typeFound = true; break;
                }
            }
        }

        if (!typeFound) ReduceLife(baseDamage);
    }

    public override void ReduceLife(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} take {damage} damage!");
        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} is dead");

            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        if (spawner != null)
            spawner.EnemyDestroyed();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((cameraLayer.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            isActive = true;
            canDespawn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((cameraLayer.value & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            isActive = false;

            if(canDespawn)
                DestroyEnemy();
        }
    }

}