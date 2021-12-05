using System;
using UnityEngine;

public class ResearchFacility : PlanetaryStructure
{
    public ResearchFacility()
    {
        maxLevel = 3;
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
        Player.instance.totalScience += 1f * Time.fixedDeltaTime * level;
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
                    usedRareMetals = constructionCost.usedRareMetals * 0.5f,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.8f,
                    usedCrystals = constructionCost.usedPower * 0.9f,
                    usedPower = constructionCost.usedPower * 0.35f,
                };
            }
            Player.instance.UseResources(constructionCost);
            level++;
        }
    }
}