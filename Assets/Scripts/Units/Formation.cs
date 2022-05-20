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
    public int id;
    private Order currOrder;
    private bool executingOrder = false;

    public Formation(List<Unit> _units, FormationType _type)
    {
        units = _units;
        type = _type;
        orderQueue = new Queue<Order>();

        //UnitManager.instance.fleets.Add(this);
    }

    public void Update()
    {
        //ExecuteOrder();
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
                List<Vector3> unitPositions = CalculateFormationPos(type, units.Count, currOrder.movePos);
                for (int i = 0; i < units.Count; i++)
                {
                    units[i].RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = unitPositions[i], targetBody = currOrder.targetBody });
                }

                executingOrder = false;
                currOrder = null;
            }
            else if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {
                for (int i = 0; i < units.Count; i++)
                {
                    units[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = currOrder.target });
                }
                executingOrder = false;
                currOrder = null;
            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        Debug.Log("Recieved order");

        if (recievedOrder.orderType == OrderType.STOP_ORDER)
        {
            executingOrder = false;
            currOrder = null;
            orderQueue.Clear();
        }
        else
        {
            orderQueue.Enqueue(recievedOrder);
        }
    }

    public static List<Vector3> CalculateFormationPos(FormationType _type, int unitCount, Vector3 centerPos)
    {
        List<Vector3> positions = new List<Vector3>();

        if (_type == FormationType.SQUARE)
        {
            int rows = 4;
            int cols = (int)Math.Ceiling((float)unitCount / (float)rows / 2f);
            int index = 0;
            for (int i = -rows / 2; i < rows / 2; i++)
            {
                for (int j = -cols; j < cols; j++)
                {
                    if (index >= unitCount)
                    {
                        continue;
                    }
                    Vector3 calculatedPos = centerPos + new Vector3(j * 7.5f, 0f, i * 7.5f);
                    positions.Add(calculatedPos);
                    //units[index++].RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = calculatedPos, targetBody = currOrder.targetBody });
                }

            }
        }

        return positions;
    }
}
