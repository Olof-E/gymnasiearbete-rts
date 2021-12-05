using System;
using UnityEngine;

public class RareMetalsExtractor : PlanetaryStructure
{
    public RareMetalsExtractor(Planet _parentBody)
    {
        maxLevel = 8;
        parentBody = _parentBody;
    }
    public override void Execute()
    {
        Player.instance.totalRareMetals += 10f * Time.fixedDeltaTime * level * parentBody.planetProperties.rareMetalsMultiplier;
    }
}