using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class TorpedoLauncher : Weapon
{
    public Torpedo launchedTorpedo;
    public GameObject torpedoPrefab;
    private void Start()
    {
        targetingRadius = 50f;
        reloadTime = 7.5f;
    }

    private void Update()
    {
        if (target != null)
        {
            if ((target.gameObj.transform.position - transform.position).magnitude > targetingRadius)
            {
                target = null;
            }
            else if (loaded)
            {
                //Instaniate torpedo and fire at target 
                launchedTorpedo = Instantiate(torpedoPrefab, transform.position + new Vector3(3f, 0f, 3f), Quaternion.identity).GetComponent<Torpedo>();
                launchedTorpedo.target = target;
                loaded = false;
            }
            if (!loaded && !loading)
            {
                StartCoroutine(Reload());
                Debug.Log("Starting loading sequece of torpedos");
                loading = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
