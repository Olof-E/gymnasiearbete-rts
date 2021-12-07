using System;
using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    SQUARE,
    CIRCLE,
    DIAMOND,
    TRIANGLE
}

public class Formation
{
    public List<Unit> units;
    public FormationType type;
    public Queue<Order> orderQueue;
    private Order currOrder;
    private bool executingOrder = false;

    public Formation(List<Unit> _units, FormationType _type, bool _temp)
    {
        units = _units;
        type = _type;
        orderQueue = new Queue<Order>();
        if (_temp)
        {
            UnitManager.instance.tempFormations.Add(this);
        }
        else
        {
            UnitManager.instance.fleets.Add(this);
        }
    }

    public void Update()
    {
        ExecuteOrder();
    }


    public void ExecuteOrder()
    {
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
            Debug.Log("Executing order");
            if (currOrder.orderType == OrderType.MOVE_ORDER)
            {
                if (type == FormationType.SQUARE)
                {

                    int rows = 4;
                    int cols = (int)Math.Ceiling((float)units.Count / (float)rows / 2f);
                    int index = 0;
                    for (int i = -rows / 2; i < rows / 2; i++)
                    {
                        for (int j = -cols; j < cols; j++)
                        {
                            if (index >= units.Count)
                            {
                                continue;
                            }
                            Vector3 calculatedPos = currOrder.movePos + new Vector3(j * 1.5f, 0f, i * 1.5f);
                            //Debug.Log("we get here");
                            units[index++].RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = calculatedPos });
                        }

                    }
                }
                executingOrder = false;
                currOrder = null;
            }
            else if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {

            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        Debug.Log("Recieved order");
        orderQueue.Enqueue(recievedOrder);
    }
}
