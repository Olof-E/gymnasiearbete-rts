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

    public Formation(List<Unit> _units, FormationType _type)
    {
        units = _units;
        type = _type;
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
            if (currOrder.orderType == OrderType.MOVE_ORDER)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    if (type == FormationType.SQUARE)
                    {
                        Vector3 calculatedPos = Vector3.zero;
                        units[i].RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = calculatedPos });
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
