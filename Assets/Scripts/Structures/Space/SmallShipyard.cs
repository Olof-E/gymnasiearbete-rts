using System;
using System.Collections.Generic;
using UnityEngine;

public class SmallShipyard : SpaceStructure
{
    public GameObject[] shipPrefabs;
    private MaterialPropertyBlock mpb;
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
        shieldManager.Initialize(this);
        constructionCost = new ResourceConsumtion()
        {
            usedTritanium = 2000,
            usedRareMetals = 350,
            usedNanoCarbon = 1250,
            usedCrystals = 600,
            usedPower = 45,
        };
        selectionCollider = GetComponent<BoxCollider>();
        mpb = new MaterialPropertyBlock();
        Initialize();
        LevelUp();
    }

    public void Update()
    {
        if (selected && !UiManager.instance.actionsActive && !UiManager.instance.actions[2].activeInHierarchy)
        {
            UiManager.instance.ActivateActions(2);
        }
        else if (!selected && UiManager.instance.actions[2].activeInHierarchy)
        {
            UiManager.instance.ActivateActions(-1);
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
        if (parentBody == MapManager.instance.activePlanet)
        {
            this.transform.Find("SmallShipyard").GetComponent<MeshRenderer>().GetPropertyBlock(mpb);

            mpb.SetVector("_StructurePosWS", transform.position);

            this.transform.Find("SmallShipyard").GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
        }
        ExecuteOrder();
    }

    public void BuildShip(int shipIndex)
    {
        GameObject shipGo = GameObject.Instantiate(shipPrefabs[shipIndex], transform.position + Vector3.right * 5f, Quaternion.identity);
        Unit newUnit = shipGo.GetComponent<Unit>();
        newUnit.transform.SetParent(MapManager.instance.activePlanet.transform.parent);
        newUnit.parentBody = MapManager.instance.activePlanet;
        newUnit.parentBody.targetables.Add(newUnit);
        newUnit.parentBody.selectables.Add(newUnit);
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