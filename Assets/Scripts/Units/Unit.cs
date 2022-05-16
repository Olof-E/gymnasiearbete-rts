using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Unit : Targetable, ISelectable
{
    public Targetable target { get; set; }
    public float speed { get; set; }
    public float maneuverability { get; set; }
    public bool selected { get; set; }
    public BoxCollider selectionCollider { get; set; }
    public Planet parentBody { get; set; }
    public Queue<Order> orderQueue;
    public List<Weapon> weapons { get; private set; }
    public bool isOrderable { get; set; } = true;
    public GameObject enginesGameObj;
    private Order currOrder;
    private bool executingOrder = false;
    public SpriteRenderer selectedSprite { get; set; }
    public int fleetId { get; set; } = -1;
    public Vector3 selectablePosition { get; set; }
    [field: SerializeField]
    public Renderer boundsRenderer { get; set; }
    private bool pathFound = false;
    private List<Vertex> path;
    private List<Vector3> currPathLine = new List<Vector3>();
    private LineRenderer pathLineRend;

    public void Initialize(float _speed, float _maneuverability, int _armor, int _shields)
    {
        orderQueue = new Queue<Order>();
        speed = _speed;
        armor = _armor;
        shields = _shields;
        maxShields = shields;
        maxArmor = armor;
        maneuverability = _maneuverability;
        selectedSprite = transform.Find("SelectedSprite").GetComponent<SpriteRenderer>();
        weapons = new List<Weapon>();
        weapons.AddRange(GetComponentsInChildren<Weapon>());
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].parent = this;
        }
        pathLineRend = transform.Find("PathLine").GetComponent<LineRenderer>();
        shieldManager.Initialize(this);
        selectionCollider = GetComponent<BoxCollider>();
    }

    public void ExecuteOrder()
    {
        if (destroyed)
        {
            Destroy(this.gameObject);
        }
        shieldBar.value = shields / maxShields;
        armorBar.value = armor / maxArmor;
        // if (parentBody.focused)
        // {
        //     transform.position = transform.position - parentBody.transform.position;
        // }
        if (selected)
        {
            if (!selectedSprite.gameObject.activeInHierarchy)
            {
                selectedSprite.gameObject.SetActive(true);
                shieldBar.transform.parent.gameObject.SetActive(true);
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
                shieldBar.transform.parent.gameObject.SetActive(false);
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
                Move();
            }
            else if (currOrder.orderType == OrderType.ATTACK_ORDER)
            {
                Attack();
            }
            else if (currOrder.orderType == OrderType.ASSIGN_ORDER)
            {
                AssignFleet();
            }
        }
    }

    public void RecieveOrder(Order recievedOrder)
    {
        //Debug.Log("Recieved order");

        if (recievedOrder.orderType == OrderType.STOP_ORDER)
        {
            pathLineRend.positionCount = 0;
            currPathLine.Clear();
            target = null;
            weapons.ForEach((Weapon weapon) =>
            {
                weapon.target = null;
            });
            executingOrder = false;
            currOrder = null;
            orderQueue.Clear();
        }
        else
        {
            //Clear queue unless we want to additively give commands
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (recievedOrder.orderType == OrderType.ATTACK_ORDER)
                {
                    target = null;
                    weapons.ForEach((Weapon weapon) =>
                    {
                        weapon.target = null;
                    });
                }
                else
                {
                    pathLineRend.positionCount = 0;
                    currPathLine.Clear();
                    executingOrder = false;
                    currOrder = null;
                    orderQueue.Clear();
                }
            }
            orderQueue.Enqueue(recievedOrder);
        }
        if (recievedOrder.orderType == OrderType.MOVE_ORDER)
        {
            UpdatePath();
        }
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

    public void Hide(bool hide)
    {
        MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();
        for (int j = 0; j < renderers.Length; j++)
        {
            renderers[j].enabled = !hide;
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].Hide(hide);
        }
        enginesGameObj.SetActive(!hide);
        selectionCollider.enabled = !hide;
        selectedSprite.enabled = !hide;
    }

    private void Move()
    {
        if (parentBody == currOrder.targetBody)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(currOrder.movePos - transform.position, Vector3.up),
                maneuverability * 4f * Time.deltaTime
            );

            if (
                Quaternion.Angle(
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.z),
                    Quaternion.LookRotation(Vector3.Normalize(currOrder.movePos - transform.position), Vector3.up)) < 45f
                )
            {
                float angle = Quaternion.Angle(
                    Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f),
                    Quaternion.LookRotation(currOrder.movePos - transform.position, Vector3.up));

                transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed * (1f / Mathf.Clamp(angle, 1f, 45f)) * Time.deltaTime);
                selectablePosition = transform.position;
            }
            Debug.Log(pathLineRend.positionCount);
            pathLineRend.SetPosition(0, transform.position);

            if (Vector3.Distance(transform.position, currOrder.movePos) < 1e-2)
            {
                executingOrder = false;
                currOrder = null;
                currPathLine.RemoveAt(1);

                pathLineRend.positionCount = currPathLine.Count;
                pathLineRend.SetPositions(currPathLine.ToArray());
            }
        }
        else if (parentBody.parentSystem == currOrder.targetBody.parentSystem)
        {
            parentBody.targetables.Remove(this);
            parentBody.selectables.Remove(this);
            parentBody = currOrder.targetBody;
            transform.SetParent(parentBody.transform.parent);
            parentBody.targetables.Add(this);
            parentBody.selectables.Add(this);
            Debug.Log($"Moving to new target body: {parentBody}");
            transform.position = parentBody.transform.position + (currOrder.movePos - parentBody.transform.position).normalized * 75f;

            MeshRenderer[] renderers = this.GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                renderers[j].enabled = true;
            }

            selectionCollider.enabled = true;

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
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (path[1].position - transform.position).normalized, speed * 0.05f * Time.deltaTime);
                pathLineRend.SetPosition(0, transform.position);

                if (Vector3.Distance(transform.position, path[1].position) < 1e-2)
                {
                    if (path.Count == 2)
                    {
                        parentBody.targetables.Remove(this);
                        parentBody.selectables.Add(this);
                        parentBody = currOrder.targetBody;
                        transform.SetParent(parentBody.transform.parent);
                        parentBody.targetables.Add(this);
                        parentBody.selectables.Add(this);
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

    private void Attack()
    {
        if (target == null)
        {
            Debug.Log("Target is " + currOrder.target);
            target = currOrder.target;
            weapons.ForEach((Weapon weapon) =>
            {
                weapon.target = currOrder.target;
            });
            executingOrder = false;
            currOrder = null;
        }
    }

    private void AssignFleet()
    {
        UnitManager.instance.AssignFleet(this, currOrder.newFleetKey);
        executingOrder = false;
        currOrder = null;
    }
}
