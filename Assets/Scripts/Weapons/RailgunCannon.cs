using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class RailgunCannon : Weapon
{
    public Projectile firedProjectile;
    public GameObject projectilePrefab;
    private void Start()
    {
        targetingRadius = 50f;
        reloadTime = 1f;
    }

    private void Update()
    {
        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.gameObj.transform.position) > targetingRadius)
            {
                target = null;
            }
            else if (loaded)
            {
                //Instaniate railgun projectile and fire at targe
                firedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
                firedProjectile.ignoreShield = parent.shieldManager;
                firedProjectile.target = target;
                firedProjectile.Fire(target.gameObj.transform.position);
                loaded = false;
            }
            if (!loaded && !loading)
            {
                StartCoroutine(Reload());
                loading = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
