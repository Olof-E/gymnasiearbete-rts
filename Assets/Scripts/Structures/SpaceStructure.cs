using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
Planetary structures:
- Fusion reactor
- Tritanium extractors
- Crystal synthesizers
- rare metals extractors
- Carbon extractors
- Research facility
- Agricultural hub

Space structures:
- Large shipyard
- Small Shipyard
- Spaceport
- Railgun cannon
- Torpedo/Missile laucnher
- Phased energy beam
- Deep space antenna array 
- 

*/

public class SpaceStructure : Targetable, ISelectable
{
    public int playerId { get; set; }
    public int level { get; set; } = 0;
    public int maxLevel { get; set; } = 1;
    public Planet parentBody { get; set; }
    public ResourceConsumtion constructionCost { get; set; }
    public bool selected { get; set; }
    public BoxCollider selectionCollider { get; set; }
    public SpriteRenderer selectedSprite { get; set; }
    public Queue<Order> orderQueue { get; set; }
    public bool isOrderable { get; set; } = false;
    public Targetable target { get; set; }
    [field: SerializeField]
    public Renderer boundsRenderer { get; set; }
    public Vector3 selectablePosition { get; set; }
    public List<Weapon> weapons { get; private set; }
    private Order currOrder { get; set; }
    private bool executingOrder { get; set; }

    public void Initialize()
    {
        weapons = new List<Weapon>();
        weapons.AddRange(GetComponentsInChildren<Weapon>());
    }
    public virtual void LevelUp()
    {
        if (level < maxLevel)
        {
            level++;
        }
    }

    public virtual void ExecuteOrder()
    {
        if (destroyed)
        {
            Destroy(this.gameObject);
        }
        shieldBar.value = shields / maxShields;
        armorBar.value = armor / maxArmor;
        if (selected && !shieldBar.gameObject.activeInHierarchy)
        {
            shieldBar.transform.parent.gameObject.SetActive(true);
        }
        else if (!selected && shieldBar.gameObject.activeInHierarchy)
        {
            shieldBar.transform.parent.gameObject.SetActive(false);
        }

        if (orderQueue.Count > 0)
        {
            if (!executingOrder)
            {
                executingOrder = true;
                currOrder = orderQueue.Dequeue();
            }
        }
        if (currOrder != null)
        {
            if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {
                target = currOrder.target;
                executingOrder = false;
                currOrder = null;
            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        Debug.Log(recievedOrder);
        if (recievedOrder.orderType == OrderType.STOP_ORDER)
        {
            target = null;
            orderQueue.Clear();
            currOrder = null;
            executingOrder = false;
        }
        else
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                target = null;
                orderQueue.Clear();
                currOrder = null;
                executingOrder = false;
            }
            orderQueue.Enqueue(recievedOrder);
        }
    }

    public void Hide(bool hide)
    {
        MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();
        for (int j = 0; j < renderers.Length; j++)
        {
            renderers[j].enabled = !hide;
        }

        for (int j = 0; j < weapons.Count; j++)
        {
            weapons[j].Hide(hide);
        }

        selectionCollider.enabled = !hide;
        selectedSprite.enabled = !hide;
    }
}