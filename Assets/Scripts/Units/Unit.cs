using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Targetable, ISelectable
{
    public Targetable target { get; set; }
    public float speed { get; set; }
    public float maneuverability { get; set; }
    public bool selected { get; set; }
    public BoxCollider selectionCollider { get; set; }
    public Planet parentBody { get; set; }
    public Queue<Order> orderQueue;
    public List<Weapon> weapons;
    private Order currOrder;
    private bool executingOrder = false;
    public SpriteRenderer selectedSprite { get; set; }
    public int fleetId { get; set; } = -1;
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
                if (MapManager.instance.mapState == MapState.PLANETARY_VIEW)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        Quaternion.LookRotation(currOrder.movePos - transform.position, Vector3.up),
                        maneuverability * Time.fixedDeltaTime
                        );

                    // transform.rotation = Quaternion.RotateTowards(
                    //     transform.rotation,
                    //     Quaternion.Euler(
                    //         transform.rotation.eulerAngles.x,
                    //         transform.rotation.eulerAngles.y,
                    //         maneuverability * 4f * -Quaternion.Angle(
                    //             Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
                    //             Quaternion.LookRotation(currOrder.movePos - transform.position, Vector3.up)
                    //             ) / 180f
                    //     ),
                    //     maneuverability * 4f * Time.fixedDeltaTime
                    //     );

                    if (
                        Quaternion.Angle(
                            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f),
                            Quaternion.LookRotation(currOrder.movePos - transform.position, Vector3.up)) < 1f
                        )
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + currOrder.movePos - transform.position, speed * Time.fixedDeltaTime);
                    }
                    if (transform.position == currOrder.movePos)
                    {
                        executingOrder = false;
                        currOrder = null;
                    }
                }
                else if (MapManager.instance.mapState == MapState.SYSTEM_VIEW)
                {

                }
            }
            else if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {
                if (target == null)
                {
                    Debug.Log("Target is " + currOrder.target);
                    target = currOrder.target;
                    weapons.ForEach((Weapon weapon) =>
                    {
                        weapon.target = currOrder.target;
                    });
                }
            }

            else if (currOrder.orderType == OrderType.ASSIGN_ORDER)
            {
                UnitManager.instance.AssignFleet(this, currOrder.newFleetId);
            }
            else if (currOrder.orderType == OrderType.STOP_ORDER)
            {
                currOrder = null;
                orderQueue.Clear();
            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        Debug.Log("Recieved order");
        orderQueue.Enqueue(recievedOrder);
    }
}
