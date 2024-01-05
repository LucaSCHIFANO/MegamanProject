using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Exclusive")]
    [SerializeField] private SOWeaponChart weaponChart;

    public override void TakeDamage(GameData.WeaponType weaponType, int baseDamage)
    {
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
}
