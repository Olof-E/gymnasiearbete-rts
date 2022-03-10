using System;
using UnityEngine;

public class GasSeparator : PlanetaryStructure
{
    public GasSeparator(Planet _parentBody)
    {
        maxLevel = 8;
        parentBody = _parentBody;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 1500,
            usedRareMetals = 750,
            usedNanoCarbon = 1500,
            usedCrystals = 350,
            usedPower = 20,
        };
    }
    public override void Execute()
    {
        Player.instance.totalGas += 20f * Time.fixedDeltaTime * (1f + 1f / (level + 1f)) * parentBody.planetProperties.gasMultiplier;
    }

    public override void LevelUp()
    {
        if (level < maxLevel)
        {
            if (level > 0)
            {
                constructionCost = new ResourceConsumtion()
                {
                    usedTritanium = constructionCost.usedTritanium * 0.65f,
                    usedRareMetals = constructionCost.usedRareMetals * 0.85f,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.65f,
                    usedCrystals = constructionCost.usedCrystals * 0.3f,
                    usedPower = constructionCost.usedPower * 0.5f,
                };
            }
            Player.instance.UseResources(constructionCost);
            level++;
        }
    }
}