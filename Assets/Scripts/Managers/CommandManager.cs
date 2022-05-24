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
    public GameObject newFleetKey { get; set; }
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
    public GameObject cmdDirectorHead;
    private void Awake()
    {
        if (instance == null)
        {
            mainCamera = Camera.main;
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

    private void Update()
    {
        if (givingOrders)
        {
            Vector3 mousePosWS = InputManager.GetMousePositionOnXZPlane();
            // if (SelectionManager.instance.selected[0].GetType().IsSubclassOf(typeof(Unit)))
            // {
            //     cmdDirectorLineRend.SetPosition(0, ((Unit)SelectionManager.instance.selected[0]).transform.position);
            // }
            // else
            // {
            //     cmdDirectorLineRend.SetPosition(0, ((SpaceStructure)SelectionManager.instance.selected[0]).transform.position);
            // }
            // cmdDirectorLineRend.SetPosition(1, mousePosWS);
            cmdDirectorHead.transform.position = mousePosWS;
        }
    }

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
                List<Vector3> unitPos = Formation.CalculateFormationPos(FormationType.SQUARE, selectedUnits.Count, (Vector3)orderData);
                Planet targetPlanet = MapManager.instance.activePlanet ?? null;
                if (targetPlanet == null)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(CameraController.instance.mainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity))
                    {
                        if (hitInfo.transform.TryGetComponent<Planet>(out targetPlanet))
                        {
                            Vector3 centerPos = targetPlanet.transform.position + Vector3.left * 75f;
                            unitPos = Formation.CalculateFormationPos(FormationType.SQUARE, selectedUnits.Count, centerPos);
                        }
                    }
                }
                if (targetPlanet != null)
                {
                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        selectedUnits[i].RecieveOrder(
                            new Order()
                            {
                                orderType = OrderType.MOVE_ORDER,
                                movePos = unitPos[i],
                                targetBody = targetPlanet
                            });
                    }
                }

                moveOrder = false;
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
                ShieldManager targetShields;
                RaycastHit hitInfo;
                if (Physics.Raycast(mainCamera.transform.position, ((Vector3)orderData) - mainCamera.transform.position, out hitInfo, 100f))
                {
                    if (hitInfo.collider.gameObject.TryGetComponent<Targetable>(out target))
                    {
                        for (int i = 0; i < selectedUnits.Count; i++)
                        {
                            selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                        }
                    }
                    else if (hitInfo.collider.gameObject.TryGetComponent<ShieldManager>(out targetShields))
                    {
                        for (int i = 0; i < selectedUnits.Count; i++)
                        {
                            selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = targetShields.parent });
                        }
                    }
                }
                attackOrder = false;
            }
            else if (stopOrder)
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.STOP_ORDER });
                }
                stopOrder = false;
            }
            else if (assignOrder)
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    selectedUnits[i].RecieveOrder(new Order() { orderType = OrderType.ASSIGN_ORDER, newFleetKey = (GameObject)orderData });
                }

                assignOrder = false;
            }
        }
        //Handle orders for structures
        else if (SelectionManager.instance.selected.TrueForAll(
            (ISelectable selected) => { return selected.GetType().IsSubclassOf(typeof(SpaceStructure)); }
        ))
        {
            List<SpaceStructure> selectedStrucs = SelectionManager.instance.selected.Cast<SpaceStructure>().ToList();
            if (attackOrder)
            {
                Targetable target;
                RaycastHit[] hitsInfo = Physics.SphereCastAll(mainCamera.transform.position, 1f, Vector3.Normalize((Vector3)orderData - mainCamera.transform.position));
                if (hitsInfo.Length > 0)
                {
                    foreach (RaycastHit hit in hitsInfo)
                    {
                        if (hit.collider.gameObject.TryGetComponent<Targetable>(out target))
                        {

                            for (int i = 0; i < selectedStrucs.Count; i++)
                            {
                                selectedStrucs[i].RecieveOrder(new Order() { orderType = OrderType.ATTACK_ORDER, target = target });
                            }
                            attackOrder = false;
                            return;
                        }
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

        attackOrder = false;
        moveOrder = false;
        stopOrder = false;
        givingOrders = false;
        //cmdDirectorLineRend.enabled = false;
        cmdDirectorHead.SetActive(false);
    }

    public void HandleKeyInput(KeyCode e)
    {
        bool selectionIsOrderable = SelectionManager.instance.selected.TrueForAll((ISelectable obj) => { return obj.isOrderable; });

        if (e == KeyCode.None || e == KeyCode.LeftControl || SelectionManager.instance.selected.Count <= 0 || !selectionIsOrderable)
        {
            return;
        }
        else
        {
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

            if (givingOrders)
            {
                //cmdDirectorLineRend.enabled = true;
                cmdDirectorHead.SetActive(true);
            }
        }
    }
}