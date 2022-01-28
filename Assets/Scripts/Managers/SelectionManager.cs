using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;
    public List<ISelectable> selected;
    public LayerMask selectionLayer;
    private Camera mainCamera;
    private Bounds selectionBounds;
    private Texture2D selectRectTex;
    private Rect selectionRect;


    //Create a singelton instance of the selection manager
    private void Awake()
    {
        if (instance == null)
        {
            selected = new List<ISelectable>();
            mainCamera = Camera.main;
            selectRectTex = new Texture2D(1, 1);
            selectRectTex.SetPixel(0, 0, new Color(0, 0.8f, 0f, 0.33f));
            selectRectTex.Apply();
            //InputManager.instance.MouseEventOccured += HandleMouseInput;
            instance = this;
        }
        else
        {
            Debug.Log("Selection manager instance already exists...");
            Destroy(this);
        }
    }

    //When mouse event is raised check if it has anything to do with the selection manager
    public void HandleMouseInput(MouseEventArgs e)
    {
        if (e.mouseBtn == 0 && !CommandManager.instance.givingOrders && MapManager.instance.mapState == MapState.PLANETARY_VIEW)
        {
            if (selected.Count > 0) { selected.ForEach((ISelectable item) => { item.selected = false; }); }
            selected.Clear();
            UnitManager.instance.selectedFleet = -1;
        }
        //Check for single left click
        if (e.mouseBtn == 0 && !e.doubleClick && !e.dragging && e.endDrag == Vector3.zero && !CommandManager.instance.givingOrders)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100f, selectionLayer))
            {
                if (MapManager.instance.mapState == MapState.GALAXY_VIEW)
                {

                }
                else if (MapManager.instance.mapState == MapState.SYSTEM_VIEW)
                {
                    // Planet selectedPlanet = hit.collider.gameObject.GetComponent<Planet>();
                    // selectedPlanet.selected = true;
                    // selected.Add(selectedPlanet);
                    //MapManager.instance.FocusPlanet(MapManager.instance.GetPlanet(hit.collider.gameObject));
                }
                else if (MapManager.instance.mapState == MapState.PLANETARY_VIEW)
                {
                    ISelectable currSelection;
                    if (hit.collider.gameObject.TryGetComponent<ISelectable>(out currSelection))
                    {
                        currSelection.selected = true;
                        selected.Add(currSelection);
                    }
                    // if (hit.collider.gameObject.GetComponent<ISelectable>().GetType() == typeof(Planet))
                    // {
                    //     Planet selectedPlanet = hit.collider.gameObject.GetComponent<Planet>();
                    //     selectedPlanet.selected = true;
                    //     selected.Add(selectedPlanet);
                    // }
                    // else if (hit.collider.gameObject.GetComponent<ISelectable>().GetType().IsSubclassOf(typeof(Unit)))
                    // {
                    //     Unit selectedUnit = hit.collider.gameObject.GetComponent<Unit>();
                    //     selectedUnit.selected = true;
                    //     selected.Add(selectedUnit);
                    // }
                }

            }
        }

        //Check for double left click
        if (e.mouseBtn == 0 && e.doubleClick)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100f, selectionLayer))
            {
                if (MapManager.instance.mapState == MapState.GALAXY_VIEW)
                {
                    MapManager.instance.FocusSystem(MapManager.instance.GetSystem(hit.collider.gameObject));
                }
                else if (MapManager.instance.mapState == MapState.SYSTEM_VIEW)
                {
                    MapManager.instance.activeSystem.FocusPlanet(hit.collider.gameObject);
                    //MapManager.instance.FocusPlanet(MapManager.instance.GetPlanet(hit.collider.gameObject));
                }
            }
        }

        //Check for drag selection
        if (e.mouseBtn == 0 && e.dragging)
        {
            Vector3 dragPos1 = mainCamera.WorldToScreenPoint(e.startDrag);
            Vector3 dragPos2 = mainCamera.WorldToScreenPoint(e.endDrag);

            Vector3 screenPos1 = new Vector3(dragPos1.x, Screen.height - dragPos1.y, 0f);
            Vector3 screenPos2 = new Vector3(dragPos2.x, Screen.height - dragPos2.y, 0f);

            Vector3 minRect = Vector3.Min(screenPos1, screenPos2);
            Vector3 maxRect = Vector3.Max(screenPos1, screenPos2);
            selectionRect = Rect.MinMaxRect(minRect.x, minRect.y, maxRect.x, maxRect.y);
        }

        //Check for end drag selection 
        if (e.mouseBtn == 0 && e.endDrag != Vector3.zero && !e.dragging)
        {
            if (MapManager.instance.mapState != MapState.PLANETARY_VIEW)
            {
                //return;
            }

            selectionRect = Rect.zero;
            Vector3 viewPos1 = e.startDrag;
            Vector3 viewPos2 = e.endDrag;

            Vector3 topLeft = Vector3.Min(viewPos1, viewPos2);
            Vector3 botRight = Vector3.Max(viewPos1, viewPos2);
            selectionBounds.SetMinMax(topLeft, botRight);

            Collider[] selections = Physics.OverlapBox(selectionBounds.center, selectionBounds.extents, Quaternion.identity, selectionLayer);
            for (int i = 0; i < selections.Length; i++)
            {
                ISelectable selectedObj = selections[i].gameObject.GetComponent<ISelectable>();
                if (selectedObj != null)
                {
                    selectedObj.selected = true;
                    selected.Add(selectedObj);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(selectionBounds.center, selectionBounds.size);
    }
    //Draw selection box onto the screen
    private void OnGUI()
    {
        if (selectionRect != Rect.zero)
        {
            GUI.DrawTexture(selectionRect, selectRectTex);
        }
    }
}