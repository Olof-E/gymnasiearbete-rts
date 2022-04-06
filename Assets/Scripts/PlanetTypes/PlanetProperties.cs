using UnityEngine;

[CreateAssetMenu(fileName = "PlanetProperties", menuName = "GymnasieArbeteClient/PlanetProperties", order = 0)]
public class PlanetProperties : ScriptableObject
{
    public float tritaniumMultiplier;
    public float crystalMultiplier;
    public float carbonMultiplier;
    public float rareMetalsMultiplier;
    public float gasMultiplier;
    public Texture diffuseMap;
    public Texture normalMap;
    public Texture roughnessMap;
    public Texture specularMap;
    public Texture emissionMap;
    [ColorUsage(true, true)]
    public Color atmosphereColor;

}