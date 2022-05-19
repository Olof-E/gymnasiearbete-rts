using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class PhasedEnergyBeamArray : Weapon
{
    public float dmg = 7.5f;
    private LineRenderer beamGameObj;
    private bool firing = false;
    private float timeSinceFired = 0f;
    public ShieldManager ignoreShield;
    public BoxCollider ignoreSelectionColl;
    private bool hidden = false;
    private void Start()
    {
        beamGameObj = GetComponent<LineRenderer>();
        ignoreShield = parent.shieldManager;
        ignoreSelectionColl = ((ISelectable)parent).selectionCollider;
        targetingRadius = 400f;
        reloadTime = 2f;
    }

    private void Update()
    {
        if (target != null)
        {
            beamGameObj.enabled = !hidden;
            if ((target.gameObj.transform.position - transform.position).magnitude > targetingRadius)
            {
                target = null;
            }
            else if (loaded && !firing)
            {
                RaycastHit[] hitInfo = Physics.RaycastAll(transform.position, Vector3.Normalize(target.gameObj.transform.position - transform.position));
                RaycastHit closestHit = new RaycastHit();
                float minDist = float.MaxValue;
                for (int i = 0; i < hitInfo.Length; i++)
                {
                    if (hitInfo[i].collider.gameObject == ignoreShield.gameObject || hitInfo[i].collider.gameObject == ignoreSelectionColl.gameObject)
                    {
                        continue;
                    }
                    if (hitInfo[i].distance < minDist)
                    {
                        minDist = hitInfo[i].distance;
                        closestHit = hitInfo[i];
                    }
                }

                beamGameObj.SetPosition(0, transform.position);
                beamGameObj.SetPosition(1, closestHit.point);
                firing = true;

                Targetable hit;
                ShieldManager shieldHit;
                if (closestHit.transform.TryGetComponent<Targetable>(out hit))
                {
                    hit.TakeDamage(dmg, closestHit.point);
                }
                else if (closestHit.transform.gameObject.TryGetComponent<ShieldManager>(out shieldHit))
                {
                    shieldHit.parent.TakeDamage(dmg, closestHit.point);
                }
            }
            if (firing)
            {
                beamGameObj.SetPosition(0, transform.position);
                timeSinceFired += Time.deltaTime;
            }
            if (timeSinceFired > 0.8f)
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
            beamGameObj.SetPosition(0, transform.position);
            beamGameObj.SetPosition(1, transform.position);
        }
    }

    public override void Hide(bool hide)
    {
        hidden = hide;
    }

    private void OnDrawGizmos()
    {
        //Handles.DrawWireDisc(transform.position, Vector3.up, targetingRadius);
    }
}
