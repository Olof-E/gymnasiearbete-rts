using System;
using System.Collections.Generic;
using UnityEngine;

public class RailgunStructure : SpaceStructure
{
    public RailgunCannon railgunCannon;
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
        railgunCannon.parent = this;
        isOrderable = true;
        shieldManager.Initialize();
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2000,
            usedRareMetals = 1000,
            usedNanoCarbon = 1000,
            usedCrystals = 500,
            usedPower = 65,
        };
        selectionCollider = GetComponent<BoxCollider>();
        LevelUp();
    }

    public void Update()
    {
        if (parentBody == null)
        {
            return;
        }
        if (railgunCannon.target == null && target != null)
        {
            railgunCannon.target = target;
        }
        else if (railgunCannon.target != null && target == null)
        {
            railgunCannon.target = null;
        }
        if (selected)
        {
            if (!selectedSprite.gameObject.activeInHierarchy)
            {
                selectedSprite.gameObject.SetActive(true);
            }
        }
        else
        {
            if (selectedSprite.gameObject.activeInHierarchy)
            {
                selectedSprite.gameObject.SetActive(false);
            }
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