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
        targetingRadius = 200000000000000000000000f;
        reloadTime = 5f;
    }

    private void Update()
    {
        if (target != null)
        {
            Debug.Log("We have target " + target);
            Debug.Log(transform.parent.name);
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
            if (timeSinceFired > 2.5f)
            {
                loaded = false;
                timeSinceFired = 0f;
                firing = false;
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
            beamGameObj.SetPosition(1, Vector3.zero);
        }
    }

    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
