using System;
using UnityEngine;

public class DeepSpaceAntennaArray : SpaceStructure
{

    private void Start()
    {
        gameObj = gameObject;
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2500,
            usedRareMetals = 850,
            usedNanoCarbon = 750,
            usedCrystals = 850,
            usedPower = 30,
        };
        LevelUp();
    }

    public void Update()
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
                    usedTritanium = constructionCost.usedTritanium * 0.85f,
                    usedRareMetals = constructionCost.usedRareMetals * 0.35f,
                    usedNanoCarbon = constructionCost.usedNanoCarbon * 0.8f,
                    usedCrystals = constructionCost.usedCrystals * 0.5f,
                    usedPower = constructionCost.usedPower * 0.75f,
                };
            }
            Player.instance.UseResources(constructionCost);
            level++;
        }
        ExecuteOrder();
    }
}