using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Escort : Unit
{
    private void Start()
    {
        Initialize(7.5f, 65f);
        //RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = new Vector3(0.7f, 0f, 1.5f) * 10f });
    }

    private void FixedUpdate()
    {
        ExecuteOrder();
    }
}