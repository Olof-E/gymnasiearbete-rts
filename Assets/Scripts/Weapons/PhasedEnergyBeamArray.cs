using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class PhasedEnergyBeamArray : Weapon
{
    public float dmg = 7.5f;
    private LineRenderer beamGameObj;
    private bool firing = false;
    private float timeSinceFired = 0f;
    private void Start()
    {
        beamGameObj = GetComponent<LineRenderer>();
        targetingRadius = 40f;
        reloadTime = 2.5f;
    }

    private void Update()
    {
        if (target != null)
        {
            beamGameObj.enabled = true;
            if ((target.gameObj.transform.position - transform.position).magnitude > targetingRadius)
            {
                target = null;
            }
            else if (loaded)
            {
                beamGameObj.SetPosition(0, transform.position);
                beamGameObj.SetPosition(1, target.gameObj.transform.position);
                firing = true;
            }
            if (firing)
            {
                timeSinceFired += Time.deltaTime;
            }
            if (timeSinceFired > 0.9f)
            {
                loaded = false;
                timeSinceFired = 0f;
                firing = false;
                target.TakeDamage(dmg);
            }
            if (!loaded && !loading)
            {
                StartCoroutine(Reload());
                beamGameObj.SetPosition(0, transform.position);
                beamGameObj.SetPosition(1, transform.position);
                loading = true;
            }
        }
        else
        {
            beamGameObj.SetPosition(0, transform.position);
            beamGameObj.SetPosition(1, transform.position);
            beamGameObj.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
