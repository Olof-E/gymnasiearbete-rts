using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Escort : Unit
{
    private void Start()
    {
        orderQueue = new Queue<Order>();
        speed = 3f;
        maneuverability = 35f;
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        weapons = new List<Weapon>();
        weapons.AddRange(GetComponentsInChildren<Weapon>());
        //RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = new Vector3(0.7f, 0f, 1.5f) * 10f });
    }

    private void Update()
    {
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
    }

    private void FixedUpdate()
    {
        ExecuteOrder();
    }
}