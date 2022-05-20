using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Escort : Unit
{
    MaterialPropertyBlock mpb;
    private void Start()
    {
        mpb = new MaterialPropertyBlock();
        Initialize(13f, 65f, 350, 750);
        //RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = new Vector3(0.7f, 0f, 1.5f) * 10f });
    }


    private void FixedUpdate()
    {
        ExecuteOrder();
    }
}