using UnityEngine;

public class Enemy : Entity
{
    [Header("Damage Received")]
    [SerializeField] protected SOWeaponChart weaponChart;

    protected MegamanController megaman;

    public virtual void Init(MegamanController megaman)
    {
        this.megaman = megaman;
    }
    
    protected void LookAtMegaman()
    {
        if (transform.position.x < megaman.gameObject.transform.position.x)
        {
            //look right
        }
    }
}