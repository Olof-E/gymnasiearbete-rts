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
    private void Awake()
    {
        if (instance == null)
        {
            mainCamera = Camera.main;
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

    public void GiveOrders(object orderData)
    {
        givingOrders = false;
        //Handle orders for units
        if (SelectionManager.instance.selected.TrueForAll(
            (ISelectable selected) => { return selected.GetType().IsSubclassOf(typeof(Unit)); }
        ))
        {
            if (moveOrder)
            {
                if (SelectionManager.instance.selected.Count > 1)
                {
                    Formation tempFormation = new Formation(SelectionManager.instance.selected.Cast<Unit>().ToList(), FormationType.SQUARE, true);
                    tempFormation.RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = (Vector3)orderData });
                    moveOrder = false;
                }
                else
                {
                    ((Unit)SelectionManager.instance.selected[0]).RecieveOrder(new Order() { orderType = OrderType.MOVE_ORDER, movePos = (Vector3)orderData });
                    moveOrder = false;
                }
            }
            else if (patrolOrder)
            {
                Vector3 patrolPos = new Vector3(((Vector4)orderData).x, ((Vector4)orderData).y, ((Vector4)orderData).z);
                SelectionManager.instance.selected.ForEach((ISelectable unit) =>
                {
                    ((Unit)unit).RecieveOrder(new Order()
                    {
                        orderType = OrderType.PATROL_ORDER,
                        movePos = patrolPos,
                        patrolRadius = ((Vector4)orderData).w
                    });
                });
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
                        Debug.Log("Target is " + target);
                        if (SelectionManager.instance.selected.Count > 1)
                        {
                            Formation tempFormation = new Formation(SelectionManager.instance.selected.Cast<Unit>().ToList(), FormationType.SQUARE, true);
                            tempFormation.RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        }
                        else
                        {
                            ((Unit)SelectionManager.instance.selected[0]).RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        }
                    }
                }
                attackOrder = false;
            }
        }
        //Handle orders for structures
        else if (SelectionManager.instance.selected.TrueForAll(
            (ISelectable selected) => { return selected.GetType() == typeof(SpaceStructure); }
        ))
        {
            if (attackOrder)
            {
                Targetable target;
                RaycastHit hitInfo;
                if (Physics.Raycast(mainCamera.transform.position, ((Vector3)orderData) - mainCamera.transform.position, out hitInfo, 100f))
                {
                    if (hitInfo.collider.gameObject.TryGetComponent<Targetable>(out target))
                    {
                        SelectionManager.instance.selected.ForEach((ISelectable structure) =>
                        {
                            ((SpaceStructure)structure).RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        });
                        attackOrder = false;
                    }
                }
            }
        }
    }

    public void HandleMouseInput(MouseEventArgs e)
    {
        if (CommandManager.instance.moveOrder || CommandManager.instance.attackOrder)
        {
            CommandManager.instance.GiveOrders(e.startDrag);
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

        if (e == KeyCode.A)
        {
            attackOrder = true;
            givingOrders = true;
        }
        else if (e == KeyCode.Q)
        {
            moveOrder = true;
            givingOrders = true;
        }
        else if (e == KeyCode.S)
        {
            stopOrder = true;
            givingOrders = true;
        }

        if (e == KeyCode.Escape)
        {
            givingOrders = false;
        }
    }
}