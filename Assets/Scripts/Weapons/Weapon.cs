
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public Targetable parent;
    public Targetable target;
    [HideInInspector] public float targetingRadius;
    [HideInInspector] public float reloadTime;
    [HideInInspector] public bool loaded = true;
    [HideInInspector] public bool loading = false;

    public void FindTarget(List<Targetable> possibleTargets)
    {
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            //Debug.Log((possibleTargets[i].gameObj.transform.position - transform.position).magnitude);
            if (
            (possibleTargets[i].gameObj.transform.position - transform.position).magnitude < targetingRadius
            && possibleTargets[i] != parent)
            {
                target = possibleTargets[i];
                break;
            }
        }
    }

    public IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        loaded = true;
        loading = false;
    }
}
