using System;
using UnityEngine;

public class tritaniumExtractor : PlanetaryStructure
{

    public tritaniumExtractor(Planet _parentBody)
    {
        maxLevel = 8;
        parentBody = _parentBody;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 4000,
            usedRareMetals = 100,
            usedNanoCarbon = 1000,
            usedCrystals = 2500,
            usedPower = 15,
        };
    }
    public override void Execute()
    {
        Player.instance.totalTritanium += 25f * Time.fixedDeltaTime * (1f + 1f / (level + 1f)) * parentBody.planetProperties.tritaniumMultiplier;
    }

    public override void LevelUp()
    {
        if (level < maxLevel)
        {
            if (level > 0)
            {
                constructionCost = new ResourceConsumtion()
                {
                    usedTritanium = constructionCost.usedTritanium * 0.8f,
                    usedRareMetals = constructionCost.usedRareMetals,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.8f,
                    usedCrystals = constructionCost.usedPower * 0.65f,
                    usedPower = constructionCost.usedPower * 0.8f,
                };
            }
            Player.instance.UseResources(constructionCost);
            level++;
        }
    }
}