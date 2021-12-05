using System;
using UnityEngine;

public class CrystalSynthesizer : PlanetaryStructure
{
    public CrystalSynthesizer(Planet _parentBody)
    {
        maxLevel = 8;
        parentBody = _parentBody;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 1250,
            usedRareMetals = 1000,
            usedNanoCarbon = 2000,
            usedCrystals = 500,
            usedPower = 50,
        };
    }
    public override void Execute()
    {
        Player.instance.totalCrystals += 25f * Time.fixedDeltaTime * level * parentBody.planetProperties.crystalMultiplier;
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