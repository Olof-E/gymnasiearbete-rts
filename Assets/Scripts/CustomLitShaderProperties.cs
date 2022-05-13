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

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log($"from: {this.gameObject.name}");
        mpb = new MaterialPropertyBlock();
        mainRenderer.GetPropertyBlock(mpb);


        mpb.SetTexture("_BaseMap", diffuseMap);
        mpb.SetTexture("_BumpMap", normalMap);
        mpb.SetTexture("_RoughnessMap", roughnessMap);
        //mpb.SetTexture("_SpecularMap", specularMap);
        mpb.SetTexture("_EmissionMap", emissionMap);
        mpb.SetFloat("_ColorIntensity", colorIntensity);
        mpb.SetFloat("_EmissionIntensity", emissionIntenisty);

        mainRenderer.SetPropertyBlock(mpb);

        mpb.Clear();
        mpb = null;
    }
}
