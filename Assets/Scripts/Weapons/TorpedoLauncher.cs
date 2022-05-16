using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class TorpedoLauncher : Weapon
{
    public Torpedo launchedTorpedo;
    public GameObject torpedoPrefab;
    private bool hidden = false;
    private void Start()
    {
        targetingRadius = 500f;
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
                launchedTorpedo = Instantiate(torpedoPrefab, transform.position, Quaternion.identity).GetComponent<Torpedo>();
                if (hidden)
                {
                    for (int i = 0; i < launchedTorpedo.transform.childCount; i++)
                    {
                        launchedTorpedo.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    for (int i = 0; i < launchedTorpedo.transform.childCount; i++)
                    {
                        launchedTorpedo.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                Debug.Log("Launched");
                Debug.Log(parent.shieldManager);
                launchedTorpedo.ignoreShield = parent.shieldManager;
                launchedTorpedo.ignoreSelectionColl = ((ISelectable)parent).selectionCollider;
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

    public override void Hide(bool hide)
    {
        hidden = hide;
        if (launchedTorpedo != null)
        {
            if (hidden)
            {
                for (int i = 0; i < launchedTorpedo.transform.childCount; i++)
                {
                    launchedTorpedo.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < launchedTorpedo.transform.childCount; i++)
                {
                    launchedTorpedo.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
