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
            else if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {
                weapons.ForEach((Weapon weapon) =>
                {
                    weapon.target = target;
                });
            }

            else if (currOrder.orderType == OrderType.ASSIGN_ORDER)
            {
                UnitManager.instance.AssignFleet(this, currOrder.newFleetId);
            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        Debug.Log("Recieved order");
        orderQueue.Enqueue(recievedOrder);
    }
}
