using System;
using UnityEngine;

public class TorpedoLauncherStructure : SpaceStructure
{
    public TorpedoLauncher torpedoLauncher;
    private void Start()
    {
        gameObj = gameObject;
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        torpedoLauncher.parent = this;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2000,
            usedRareMetals = 600,
            usedNanoCarbon = 1000,
            usedCrystals = 1500,
            usedPower = 50,
        };
        LevelUp();
    }

    public void Update()
    {
        if (parentBody == null)
        {
            return;
        }
        if (torpedoLauncher.target == null)
        {
            torpedoLauncher.FindTarget(parentBody.targetables);
        }
        ExecuteOrder();
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
    }
}