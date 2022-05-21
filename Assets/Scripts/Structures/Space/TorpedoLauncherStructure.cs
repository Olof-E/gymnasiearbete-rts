using System;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoLauncherStructure : SpaceStructure
{
    public TorpedoLauncher torpedoLauncher;
    private MaterialPropertyBlock mpb;
    private void Start()
    {
        orderQueue = new Queue<Order>();
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        gameObj = gameObject;
        objectName = "Torpedo Launcher";
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        maxShields = shields;
        maxArmor = armor;
        torpedoLauncher.parent = this;
        isOrderable = true;
        shieldManager.Initialize(this);
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2000,
            usedRareMetals = 600,
            usedNanoCarbon = 1000,
            usedCrystals = 1500,
            usedPower = 50,
        };
        selectionCollider = GetComponent<BoxCollider>();
        mpb = new MaterialPropertyBlock();
        Initialize();
        LevelUp();
    }

    public void Update()
    {
        if (parentBody == null)
        {
            return;
        }
        if (torpedoLauncher.target == null && target != null)
        {
            torpedoLauncher.target = target;
        }
        else if (torpedoLauncher.target != null && target == null)
        {
            torpedoLauncher.target = null;
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