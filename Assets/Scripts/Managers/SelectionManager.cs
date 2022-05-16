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
        if (e.mouseBtn == 0 && !CommandManager.instance.givingOrders && MapManager.instance.mapState == MapState.PLANETARY_VIEW && !Input.GetKey(KeyCode.LeftControl))
        {
            if (selected.Count > 0) { selected.ForEach((ISelectable item) => { item.selected = false; }); }
            UnitManager.instance.selectedFleetKey = null;
            selected.Clear();
        }
        //Check for single left click
        if (e.mouseBtn == 0 && !e.doubleClick && !e.dragging && e.endDrag == Vector3.zero && !CommandManager.instance.givingOrders)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, selectionLayer))
            {
                if (MapManager.instance.mapState == MapState.GALAXY_VIEW)
                {

                }
                else if (MapManager.instance.mapState == MapState.SYSTEM_VIEW)
                {
                }
                else if (MapManager.instance.mapState == MapState.PLANETARY_VIEW)
                {
                    ISelectable currSelection;
                    if (hit.collider.gameObject.TryGetComponent<ISelectable>(out currSelection))
                    {
                        if (currSelection.selected)
                        {
                            currSelection.selected = false;
                            selected.Remove(currSelection);
                        }
                        else
                        {
                            currSelection.selected = true;
                            selected.Add(currSelection);
                        }
                    }
                }
            }
            else
            {
                if (UiManager.instance.actionsActive)
                {
                    UiManager.instance.ActivateActions(-1);
                }
            }
        }

        //Check for double left click
        if (e.mouseBtn == 0 && e.doubleClick)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, selectionLayer))
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
                else if (MapManager.instance.mapState == MapState.PLANETARY_VIEW)
                {
                    CameraController.instance.FocusPosition(hit.transform.position);
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
                selectionRect = Rect.zero;
                return;
            }
            List<ISelectable> selectables = MapManager.instance.activePlanet.selectables;
            for (int i = 0; i < selectables.Count; i++)
            {
                if (selectionRect.Overlaps(GUI3dRectWithObject(selectables[i].boundsRenderer)))
                {
                    if (selectables[i].selected)
                    {
                        selectables[i].selected = false;
                        selected.Remove(selectables[i]);
                    }
                    else
                    {
                        selectables[i].selected = true;
                        selected.Add(selectables[i]);
                    }
                }
            }
            selectionRect = Rect.zero;
        }
    }

    public static Rect GUI3dRectWithObject(Renderer selectableRenderer)
    {

        Vector3 cen = selectableRenderer.bounds.center;
        Vector3 ext = selectableRenderer.bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
        {
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
            WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
            WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }

    public static Vector2 WorldToGUIPoint(Vector3 world)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
        screenPoint.y = (float)Screen.height - screenPoint.y;
        return screenPoint;
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