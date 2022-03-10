using System;
using UnityEngine;

public class NanoCarbonExtruder : PlanetaryStructure
{
    public NanoCarbonExtruder(Planet _parentBody)
    {
        maxLevel = 8;
        parentBody = _parentBody;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 4000,
            usedRareMetals = 800,
            usedNanoCarbon = 2000,
            usedCrystals = 500,
            usedPower = 65,
        };
    }
    public override void Execute()
    {
        Player.instance.totalNanoCarbon += 25f * Time.fixedDeltaTime * (1f + 1f / (level + 1f)) * parentBody.planetProperties.carbonMultiplier;
    }
    public override void LevelUp()
    {
        if (level < maxLevel)
        {
            if (level > 0)
            {
                constructionCost = new ResourceConsumtion()
                {
                    usedTritanium = constructionCost.usedTritanium * 0.6f,
                    usedRareMetals = constructionCost.usedRareMetals * 0.5f,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.6f,
                    usedCrystals = constructionCost.usedCrystals * 0.75f,
                    usedPower = constructionCost.usedPower * 0.4f,
                };
            }
            Player.instance.UseResources(constructionCost);
            level++;
        }
    }
}