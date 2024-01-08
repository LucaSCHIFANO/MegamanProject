using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapons/New Weapon Chart")]
public class SOWeaponChart : ScriptableObject
{
    public List<WeaponDamage> weaponDamages = new List<WeaponDamage>();
}

[Serializable]
public class WeaponDamage
{
    public GameData.WeaponType WeaponType;
    public int Damage;
}
