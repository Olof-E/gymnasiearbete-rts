using UnityEngine;

[CreateAssetMenu(fileName = "PlanetProperties", menuName = "GymnasieArbeteClient/PlanetProperties", order = 0)]
public class PlanetProperties : ScriptableObject
{
    public float tritaniumMultiplier;
    public float crystalMultiplier;
    public float carbonMultiplier;
    public float rareMetalsMultiplier;
    public float gasMultiplier;
    public Material material;
}