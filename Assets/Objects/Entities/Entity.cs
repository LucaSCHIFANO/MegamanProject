using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    protected int currentHealth;

    [SerializeField] private GameData.Side side;

    public GameData.Side GetSide { get => side; }
    private void Awake()
    {
        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(GameData.WeaponType weaponType, int baseDamage)
    {
        ReduceLife(baseDamage);
    }

    public virtual void ReduceLife(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} take {damage} damage!");
        if (currentHealth <= 0) Debug.Log($"{gameObject.name} is dead");
    }
}
