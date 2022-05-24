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

    //Calculate positions for units in formation
    public static List<Vector3> CalculateFormationPos(FormationType _type, int unitCount, Vector3 centerPos)
    {
        List<Vector3> positions = new List<Vector3>();

        if (_type == FormationType.SQUARE)
        {
            for (int i = 0; i < unitCount; i++)
            {
                positions.Add(centerPos + SpiralPosition(i + 1) * 5f);
            }
        }

        return positions;
    }

    //Calculate rectangular spiral
    private static Vector3 SpiralPosition(int n)
    {
        float k = Mathf.Ceil((Mathf.Sqrt(n) - 1f) / 2f);
        float t = 2f * k + 1f;
        float m = Mathf.Pow(t, 2f);

        t -= 1f;

        if (n >= m - t)
        {
            return new Vector3(k - (m - n), 0, -k);
        }

        m -= t;

        if (n >= m - t)
        {
            return new Vector3(-k, 0, -k + (m - n));
        }

        m -= t;

        if (n >= m - t)
        {
            return new Vector3(-k + (m - n), 0, k);
        }

        return new Vector3(k, 0, k - (m - n - t));
    }
}
