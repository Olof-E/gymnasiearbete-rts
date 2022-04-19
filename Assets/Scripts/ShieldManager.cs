using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public Targetable parent;
    private int maxHitCount = 10;
    private int currHitCount = 0;
    private Vector4[] hitPosArr;
    private float[] hitIntensityArr;
    private float[] hitTimer;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private int hitPosArrId;
    private int hitIntensityId;
    public void Initialize(Targetable _parent)
    {
        parent = _parent;
        hitPosArr = new Vector4[maxHitCount];
        hitIntensityArr = new float[maxHitCount];
        hitTimer = new float[maxHitCount];
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        hitPosArrId = Shader.PropertyToID("_HitsPosition");
        hitIntensityId = Shader.PropertyToID("_HitsIntensity");
    }

    public void AddNewHit(Vector3 hitPos, float strength)
    {

        if (currHitCount < maxHitCount)
        {
            int i;
            currHitCount++;
            i = currHitCount - 1;
            hitPosArr[i] = transform.InverseTransformPoint(hitPos);
            hitIntensityArr[i] = 1f;
            hitTimer[i] = 0f;
            Debug.Log($"new hit at: {hitPos}");
        }
        else
        {
            // float minTimer = float.MaxValue;
            // int minId = 0;
            // for (int j = 0; j < maxHitCount; j++)
            // {
            //     if (minTimer > hitTimer[j])
            //     {
            //         minTimer = hitTimer[j];
            //         minId = j;
            //     }
            // }
            // i = minId;
        }


    }


    private void Update()
    {
        for (int i = 0; i < currHitCount;)
        {
            hitTimer[i] += Time.deltaTime;
            if (hitTimer[i] > 2.5f)
            {
                int idLast = currHitCount - 1;
                hitPosArr[i] = hitPosArr[idLast];
                hitTimer[i] = hitTimer[idLast];
                hitIntensityArr[i] = hitIntensityArr[idLast];
                currHitCount--;
            }
            else
            {
                i++;
            }
        }
        for (int i = 0; i < currHitCount; i++)
        {
            hitIntensityArr[i] = 1 - Mathf.Clamp01(hitTimer[i] / 2.5f);
        }
        rend.GetPropertyBlock(mpb);

        mpb.SetInt("_HitsCount", currHitCount);
        mpb.SetVectorArray(hitPosArrId, hitPosArr.ToList());
        mpb.SetFloatArray(hitIntensityId, hitIntensityArr.ToList());

        rend.SetPropertyBlock(mpb);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(hitPosArr[0], 0.3f);
    }
}
