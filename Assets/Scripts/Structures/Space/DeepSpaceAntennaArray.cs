using System;
using System.Collections.Generic;
using UnityEngine;

public class DeepSpaceAntennaArray : SpaceStructure
{

    private void Start()
    {
        orderQueue = new Queue<Order>();
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        gameObj = gameObject;
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        maxShields = shields;
        maxArmor = armor;
        shieldManager.Initialize();
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2500,
            usedRareMetals = 850,
            usedNanoCarbon = 750,
            usedCrystals = 850,
            usedPower = 30,
        };
        selectionCollider = GetComponent<BoxCollider>();
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