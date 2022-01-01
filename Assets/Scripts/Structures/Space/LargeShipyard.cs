using System;
using System.Collections.Generic;
using UnityEngine;

public class LargeShipyard : SpaceStructure
{

    private void Start()
    {
        orderQueue = new Queue<Order>();
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        gameObj = gameObject;
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 3000,
            usedRareMetals = 650,
            usedNanoCarbon = 2000,
            usedCrystals = 850,
            usedPower = 55,
        };
        LevelUp();
    }

    public void Update()
    {
        if (selected && !UiManager.instance.actions[0].activeInHierarchy)
        {
            UiManager.instance.ActivateActions(1);
        }
        else if (!selected && UiManager.instance.actions[0].activeInHierarchy)
        {
            UiManager.instance.ActivateActions(-1);
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