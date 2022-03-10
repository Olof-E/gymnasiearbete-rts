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
        Player.instance.totalRareMetals += 10f * Time.fixedDeltaTime * (1f + 1f / (level + 1f)) * parentBody.planetProperties.rareMetalsMultiplier;
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