using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLitShaderProperties : MonoBehaviour
{
    public MeshRenderer mainRenderer;
    public Texture diffuseMap;
    public Texture normalMap;
    public Texture roughnessMap;
    public Texture specularMap;
    public Texture emissionMap;
    public float colorIntensity;
    public float emissionIntenisty;
    private MaterialPropertyBlock mpb;
    private int ShaderObjectPosId = Shader.PropertyToID("_ObjectPosWS");

    // Start is called before the first frame update
    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        mainRenderer.GetPropertyBlock(mpb);

        if (diffuseMap != null)
            mpb.SetTexture("_BaseMap", diffuseMap);
        if (normalMap != null)
            mpb.SetTexture("_BumpMap", normalMap);
        if (roughnessMap != null)
            mpb.SetTexture("_RoughnessMap", roughnessMap);
        if (emissionMap != null)
            mpb.SetTexture("_EmissionMap", emissionMap);

        mpb.SetFloat("_ColorIntensity", colorIntensity);
        mpb.SetFloat("_EmissionIntensity", emissionIntenisty);

        mainRenderer.SetPropertyBlock(mpb);

        mpb.Clear();
        //mpb = null;
    }

    private void FixedUpdate()
    {
        if (mainRenderer.enabled)
        {
            mainRenderer.GetPropertyBlock(mpb);

            mpb.SetVector(ShaderObjectPosId, transform.position - MapManager.instance.activeSystem.star.transform.position);

            mainRenderer.SetPropertyBlock(mpb);
        }
    }
}
