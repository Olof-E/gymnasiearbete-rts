using System;
using UnityEngine;

public class FusionReactor : PlanetaryStructure
{
    public FusionReactor()
    {
        maxLevel = 6;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 4000,
            usedRareMetals = 1500,
            usedNanoCarbon = 2250,
            usedCrystals = 1500,
            usedPower = 0,
        };
        LevelUp();
    }

    public override void Execute()
    {

    }

    public override void LevelUp()
    {
        if (level < maxLevel)
        {
            if (level > 0)
            {
                constructionCost = new ResourceConsumtion()
                {
                    usedTritanium = constructionCost.usedTritanium,
                    usedRareMetals = constructionCost.usedRareMetals * 0.9f,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.6f,
                    usedCrystals = constructionCost.usedPower * 1.2f,
                    usedPower = 0,
                };
            }
            Player.instance.UseResources(constructionCost);
            Player.instance.totalPower += 100f * Mathf.Exp(-0.2f * level);
            Player.instance.availablePower = Player.instance.totalPower - Player.instance.usedPower;
            level++;
        }
    }
}