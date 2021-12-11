using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class PhasedEnergyBeamArray : Weapon
{
    private LineRenderer beamGameObj;
    private bool firing = false;
    private float timeSinceFired = 0f;
    private void Start()
    {
        beamGameObj = GetComponent<LineRenderer>();
        targetingRadius = 2f;
        reloadTime = 5f;
    }

    private void Update()
    {
        if (target != null)
        {
            Debug.Log("We have target " + target);
            if ((target.gameObj.transform.position - transform.position).magnitude > targetingRadius)
            {
                target = null;
            }
            else if (loaded)
            {
                beamGameObj.SetPosition(1, target.gameObj.transform.position - transform.position);
                firing = true;
            }
            if (firing)
            {
                timeSinceFired += Time.deltaTime;
            }
            if (timeSinceFired > 2.5f)
            {
                loaded = false;
                timeSinceFired = 0f;
                firing = false;
            }
            if (!loaded && !loading)
            {
                StartCoroutine(Reload());
                beamGameObj.SetPosition(1, Vector3.zero);
                loading = true;
            }
        }
        else
        {
            beamGameObj.SetPosition(1, Vector3.zero);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
