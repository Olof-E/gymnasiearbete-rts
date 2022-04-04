using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    private int maxHitCount = 10;
    private int currHitCount = 0;
    private Vector4[] hitPosArr;
    private float[] hitIntensityArr;
    private float[] hitTimer;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private int hitPosArrId;
    private int hitIntensityId;
    public void Initialize()
    {
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
        int i;
        currHitCount++;
        if (currHitCount < maxHitCount)
        {
            i = currHitCount - 1;
        }
        else
        {
            float minTimer = float.MaxValue;
            int minId = 0;
            for (int j = 0; j < maxHitCount; j++)
            {
                if (minTimer > hitTimer[j])
                {
                    minTimer = hitTimer[j];
                    minId = j;
                }
            }
            i = minId;
        }

        hitPosArr[i] = hitPos;
        hitIntensityArr[i] = 1f;
        hitTimer[i] = 0f;
    }


    private void Update()
    {
        for (int i = 0; i < currHitCount; i++)
        {
            hitTimer[i] += Time.deltaTime;
            if (hitTimer[i] > 3f)
            {
                int idLast = currHitCount - 1;
                hitPosArr[i] = hitPosArr[idLast];
                hitTimer[i] = hitTimer[idLast];
                hitIntensityArr[i] = hitIntensityArr[idLast];
                currHitCount--;
            }
        }
        for (int i = 0; i < currHitCount; i++)
        {
            hitIntensityArr[i] = 1 - Mathf.Clamp01(hitTimer[i] / 3f);
        }
        rend.GetPropertyBlock(mpb);

        mpb.SetVectorArray(hitPosArrId, hitPosArr.ToList());
        mpb.SetFloatArray(hitIntensityId, hitIntensityArr.ToList());

        rend.SetPropertyBlock(mpb);
    }
}
