using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum OrderType
{
    MOVE_ORDER,
    PATROL_ORDER,
    REGROUP_ORDER,
    ASSIGN_ORDER,
    STOP_ORDER,
    ATTACK_ORDER
}

public class Order
{
    public OrderType orderType { get; set; }
    public Vector3 movePos { get; set; }
    public Planet targetBody { get; set; }
    public float patrolRadius { get; set; }
    public int newFleetId { get; set; }
    public Targetable target { get; set; }

}

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;
    public bool moveOrder = false;
    public bool patrolOrder = false;
    public bool regroupOrder = false;
    public bool assignOrder = false;
    public bool stopOrder = false;
    public bool attackOrder = false;
    public bool givingOrders = false;
    private Camera mainCamera;
    private LineRenderer commandLineRend;
    private void Awake()
    {
        if (instance == null)
        {
            mainCamera = Camera.main;
            commandLineRend = GetComponent<LineRenderer>();
            Debug.Log("This actually happend");
            instance = this;
        }
        else
        {
            Debug.Log("Command manager instance already exists...");
            Destroy(this);
        }
    }

    //Selected
    //Press order button
    //Depending on button (Pos, (Pos, Radius), None, fleetId, None, Target)

    public void GiveOrders(object orderData = null)
    {
        givingOrders = false;
        //Handle orders for units
        if (SelectionManager.instance.selected.TrueForAll(
            (ISelectable selected) => { return selected.GetType().IsSubclassOf(typeof(Unit)); }
        ))
        {
            List<Unit> selectedUnits = SelectionManager.instance.selected.Cast<Unit>().ToList();
            if (moveOrder)
            {
                if (selectedUnits.Count > 1)
                {
                    List<Vector3> unitPos = Formation.CalculateFormationPos(FormationType.SQUARE, selectedUnits.Count, (Vector3)orderData);
                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        selectedUnits[i].RecieveOrder(
                            new Order()
                            {
                                orderType = OrderType.MOVE_ORDER,
                                movePos = unitPos[i],
                                targetBody = MapManager.instance.activePlanet
                            });
                    }
                    // Formation tempFormation = new Formation(SelectionManager.instance.selected.Cast<Unit>().ToList(), FormationType.SQUARE, true);
                    // tempFormation.RecieveOrder(
                    //     new Order()
                    //     {
                    //         orderType = OrderType.MOVE_ORDER,
                    //         movePos = (Vector3)orderData,
                    //         targetBody = MapManager.instance.activePlanet
                    //     });
                    moveOrder = false;
                }
                else
                {
                    selectedUnits[0].RecieveOrder(
                        new Order()
                        {
                            orderType = OrderType.MOVE_ORDER,
                            movePos = (Vector3)orderData,
                            targetBody = MapManager.instance.activePlanet
                        });
                    moveOrder = false;
                }
            }
            else if (patrolOrder)
            {
                Vector3 patrolPos = new Vector3(((Vector4)orderData).x, ((Vector4)orderData).y, ((Vector4)orderData).z);
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].RecieveOrder(new Order()
                    {
                        orderType = OrderType.PATROL_ORDER,
                        movePos = patrolPos,
                        patrolRadius = ((Vector4)orderData).w
                    });
                }
                patrolOrder = false;
            }
            else if (attackOrder)
            {
                Targetable target;
                RaycastHit hitInfo;
                if (Physics.Raycast(mainCamera.transform.position, ((Vector3)orderData) - mainCamera.transform.position, out hitInfo, 100f))
                {
                    if (hitInfo.collider.gameObject.TryGetComponent<Targetable>(out target))
                    {
                        if (selectedUnits.Count > 1)
                        {
                            for (int i = 0; i < selectedUnits.Count; i++)
                            {
                                selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                            }
                        }
                        else
                        {
                            selectedUnits[0].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        }
                    }
                }
                attackOrder = false;
            }
            else if (stopOrder)
            {
                if (SelectionManager.instance.selected.Count > 1)
                {
                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.STOP_ORDER });
                    }
                }
                else
                {
                    selectedUnits[0].RecieveOrder(new Order() { orderType = OrderType.STOP_ORDER });
                }
                stopOrder = false;
            }
            else if (assignOrder)
            {
                if (SelectionManager.instance.selected.Count > 1)
                {
                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.ASSIGN_ORDER, newFleetId = (int)orderData });
                    }
                }
                else
                {
                    selectedUnits[0].RecieveOrder(new Order() { orderType = OrderType.ASSIGN_ORDER, newFleetId = (int)orderData });
                }
                assignOrder = false;
            }
        }
        //Handle orders for structures
        else if (SelectionManager.instance.selected.TrueForAll(
            (ISelectable selected) => { return selected.GetType() == typeof(SpaceStructure); }
        ))
        {
            List<SpaceStructure> selectedStrucs = SelectionManager.instance.selected.Cast<SpaceStructure>().ToList();
            if (attackOrder)
            {
                Targetable target;
                RaycastHit hitInfo;
                if (Physics.Raycast(mainCamera.transform.position, ((Vector3)orderData) - mainCamera.transform.position, out hitInfo, 100f))
                {
                    if (hitInfo.collider.gameObject.TryGetComponent<Targetable>(out target))
                    {
                        for (int i = 0; i < selectedStrucs.Count; i++)
                        {
                            selectedStrucs[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        }
                        attackOrder = false;
                    }
                }
            }
            else if (stopOrder)
            {
                for (int i = 0; i < selectedStrucs.Count; i++)
                {
                    selectedStrucs[i].RecieveOrder(new Order() { orderType = OrderType.STOP_ORDER });
                };
                stopOrder = false;
            }
        }
    }

    public void HandleMouseInput(MouseEventArgs e)
    {
        if (moveOrder || attackOrder)
        {
            GiveOrders(e.startDrag);
        }
        else if (givingOrders)
        {
            attackOrder = false;
            moveOrder = false;
            stopOrder = false;
            givingOrders = false;
        }
    }

    public void HandleKeyInput(KeyCode e)
    {
        if (e == KeyCode.None || SelectionManager.instance.selected.Count <= 0)
        {
            return;
        }

        attackOrder = false;
        moveOrder = false;
        stopOrder = false;

        if (e == KeyCode.G)
        {
            attackOrder = true;
            givingOrders = true;
        }
        else if (e == KeyCode.Q)
        {
            moveOrder = true;
            givingOrders = true;
        }
        else if (e == KeyCode.Y)
        {
            stopOrder = true;
            givingOrders = true;
            GiveOrders();
        }
        else if (e == KeyCode.J)
        {
            assignOrder = true;
            givingOrders = true;
        }
    }
}