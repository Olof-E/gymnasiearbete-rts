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
    private bool pathFound = false;
    private List<Vertex> path;
    private List<Vector3> currPathLine = new List<Vector3>();
    private LineRenderer pathLineRend;

    public void Initialize(float _speed, float _maneuverability)
    {
        orderQueue = new Queue<Order>();
        speed = _speed;
        maneuverability = _maneuverability;
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        weapons = new List<Weapon>();
        weapons.AddRange(GetComponentsInChildren<Weapon>());
        pathLineRend = transform.Find("PathLine").GetComponent<LineRenderer>();

        selectionCollider = GetComponent<BoxCollider>();
    }

    public void ExecuteOrder()
    {
        if (selected)
        {
            if (!selectedSprite.gameObject.activeInHierarchy)
            {
                selectedSprite.gameObject.SetActive(true);
            }
            if (!pathLineRend.gameObject.activeInHierarchy)
            {
                pathLineRend.gameObject.SetActive(true);
            }
        }
        else
        {
            if (selectedSprite.gameObject.activeInHierarchy)
            {
                selectedSprite.gameObject.SetActive(false);
            }
            if (pathLineRend.gameObject.activeInHierarchy)
            {
                pathLineRend.gameObject.SetActive(false);
            }
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
            if (currOrder.orderType == OrderType.MOVE_ORDER)
            {
                //Debug.Log($"target body is: {currOrder.targetBody}");
                if (parentBody == currOrder.targetBody)
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

                    pathLineRend.SetPosition(0, transform.position);

                    if (Vector3.Distance(transform.position, currOrder.movePos) < 1e-2)
                    {
                        executingOrder = false;
                        currOrder = null;
                        currPathLine.RemoveAt(1);

                        pathLineRend.positionCount = currPathLine.Count;
                        pathLineRend.SetPositions(currPathLine.ToArray());
                    }

                    // if (currOrder.movePos != null)
                    // {
                    //     pathLineRend.positionCount = 2;
                    //     pathLineRend.SetPosition(0, transform.position);
                    //     pathLineRend.SetPosition(1, currOrder.movePos);
                    // }
                }
                else if (parentBody.parentSystem == currOrder.targetBody.parentSystem)
                {
                    parentBody.targetables.Remove(this);
                    parentBody = currOrder.targetBody;
                    transform.SetParent(parentBody.transform.parent);
                    parentBody.targetables.Add(this);
                    Debug.Log($"Moving to new target body: {parentBody}");
                    transform.position = parentBody.transform.position + (currOrder.movePos - parentBody.transform.position).normalized * 45f;

                    MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();
                    for (int j = 0; j < renderers.Length; j++)
                    {
                        renderers[j].enabled = true;
                    }

                    selectionCollider.enabled = true;
                    //selectedSprite.enabled = true;

                    currPathLine.Add(transform.position);
                    currPathLine.Add(currOrder.movePos);

                    pathLineRend.positionCount = currPathLine.Count;
                    pathLineRend.SetPositions(currPathLine.ToArray());
                }
                else
                {
                    if (!pathFound)
                    {
                        pathFound = true;
                        Vertex currSystem = MapManager.instance.mapGraph.vertices[parentBody.parentSystem.id];
                        Vertex targetSystem = MapManager.instance.mapGraph.vertices[currOrder.targetBody.parentSystem.id];

                        Debug.Log(currOrder.targetBody.parentSystem.id);
                        Debug.Log(parentBody.parentSystem.id);

                        path = Pathfinding.CalculatePath(ref MapManager.instance.mapGraph, currSystem, targetSystem);
                        pathLineRend.positionCount = path.Count;

                        for (int i = 0; i < path.Count; i++)
                        {
                            pathLineRend.SetPosition(i, path[i].position);
                        }

                        transform.position = path[0].position;

                        Debug.Log(path.Count);
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, transform.position + (path[1].position - transform.position).normalized, speed * 0.05f * Time.fixedDeltaTime);
                        pathLineRend.SetPosition(0, transform.position);

                        if (Vector3.Distance(transform.position, path[1].position) < 1e-2)
                        {
                            if (path.Count == 2)
                            {
                                parentBody.targetables.Remove(this);
                                parentBody = currOrder.targetBody;
                                transform.SetParent(parentBody.transform.parent);
                                parentBody.targetables.Add(this);
                                Debug.Log($"Moving to new target body: {parentBody}");
                                transform.position = parentBody.transform.position + (currOrder.movePos - parentBody.transform.position).normalized * 45f;

                                MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();
                                for (int j = 0; j < renderers.Length; j++)
                                {
                                    renderers[j].enabled = true;
                                }

                                selectionCollider.enabled = true;
                                selectedSprite.enabled = true;

                                path.Clear();
                                currPathLine.Clear();
                                pathLineRend.positionCount = 0;

                                currPathLine.Add(transform.position);
                                currPathLine.Add(currOrder.movePos);

                                pathLineRend.positionCount = currPathLine.Count;
                                pathLineRend.SetPositions(currPathLine.ToArray());

                                pathFound = false;
                            }
                            else
                            {
                                //path.RemoveAt(0);
                                path.RemoveAt(1);
                                pathLineRend.positionCount = path.Count;
                                for (int i = 0; i < path.Count; i++)
                                {
                                    pathLineRend.SetPosition(i, path[i].position);
                                }
                            }
                        }
                    }
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

        UpdatePath();
    }

    private void UpdatePath()
    {
        Order[] orders = orderQueue.ToArray();
        if (orderQueue.Count <= 0 || orders[orders.Length - 1].targetBody != parentBody)
        {
            pathLineRend.positionCount = 0;
            return;
        }


        if (currPathLine.Count <= 0)
            currPathLine.Add(transform.position);


        currPathLine.Add(orders[orders.Length - 1].movePos);
        pathLineRend.positionCount = currPathLine.Count;
        pathLineRend.SetPositions(currPathLine.ToArray());
    }
}
