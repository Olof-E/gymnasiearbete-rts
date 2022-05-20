using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class RailgunCannon : Weapon
{
    public Projectile firedProjectile;
    public GameObject projectilePrefab;
    private bool hidden = false;

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
            else
            {
                if (loaded && Vector3.Angle(transform.forward, Vector3.Normalize(target.gameObj.transform.position - transform.position)) < 0.5f)
                {
                    //Instaniate railgun projectile and fire at targe
                    firedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
                    firedProjectile.GetComponent<LineRenderer>().enabled = !hidden;
                    firedProjectile.ignoreShield = parent.shieldManager;
                    firedProjectile.ignoreSelectionColl = ((ISelectable)parent).selectionCollider;
                    firedProjectile.target = target;
                    firedProjectile.Fire(target.gameObj.transform.position);
                    loaded = false;
                }
                if (!loaded && !loading)
                {
                    StartCoroutine(Reload());
                    loading = true;
                }
                Quaternion targetRot = Quaternion.LookRotation(-Vector3.Normalize(target.gameObj.transform.position - transform.position), Vector3.up);
                transform.parent.rotation = Quaternion.RotateTowards(transform.parent.rotation, targetRot, Time.deltaTime * 4.5f);
            }
        }
    }

    public override void Hide(bool hide)
    {
        hidden = hide;
    }

    private void OnDrawGizmosSelected()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
