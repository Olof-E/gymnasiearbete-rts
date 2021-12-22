using System;
using UnityEngine;

public class SmallShipyard : SpaceStructure
{
    public GameObject[] shipPrefabs;
    private void Start()
    {
        gameObj = gameObject;
        maxLevel = 8;
        armor = 1000;
        shields = 1500;
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2000,
            usedRareMetals = 350,
            usedNanoCarbon = 1250,
            usedCrystals = 600,
            usedPower = 45,
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

    public void BuildShip(int shipIndex)
    {
        GameObject shipGo = GameObject.Instantiate(shipPrefabs[shipIndex], transform.position + Vector3.forward * 2f, Quaternion.identity);
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