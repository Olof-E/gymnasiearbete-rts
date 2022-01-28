using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Class for storing the mouse event data
public class MouseEventArgs : EventArgs
{
    public int mouseBtn { get; set; }
    public bool doubleClick { get; set; }
    public bool dragging { get; set; }
    public Vector3 startDrag { get; set; }
    public Vector3 endDrag { get; set; }
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public delegate void MouseEventHandler(MouseEventArgs e);
    public event MouseEventHandler MouseEventOccured;
    public delegate void KeyEventHandler(KeyCode e);
    public event KeyEventHandler keyEventOccured;
    private float maxTimeDoubleClick = 0.2f;
    private float timeDoubleClick = 0f;
    private bool checkDoubleClick = false;
    private bool isDragging = false;
    private Vector3 startDrag;
    private static Plane XZPlane = new Plane(Vector3.up, Vector3.zero);

    //Create a singelton instance of the input manager
    private void Start()
    {
        if (instance == null)
        {
            MouseEventOccured += SelectionManager.instance.HandleMouseInput;
            MouseEventOccured += BuildingManager.instance.HandleMouseInput;
            MouseEventOccured += CommandManager.instance.HandleMouseInput;
            //MouseEventOccured += PrintMouseInput;
            keyEventOccured += CommandManager.instance.HandleKeyInput;
            instance = this;
        }
        else
        {
            Debug.Log("Input manager already exists...");
            Destroy(this);
        }
    }

    private void Update()
    {
        //Check for single left click
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            startDrag = GetMousePositionOnXZPlane();
            //Check for another left click and raise double click event
            if (checkDoubleClick)
            {
                timeDoubleClick = 0f;
                MouseEventOccured(new MouseEventArgs() { mouseBtn = 0, doubleClick = true, startDrag = startDrag });
                checkDoubleClick = false;
            }
            else
            {
                checkDoubleClick = true;
                MouseEventOccured(new MouseEventArgs() { mouseBtn = 0, doubleClick = false, startDrag = startDrag });
            }
        }
        //Check if time for double click has elapsed and raise single click event
        else if (timeDoubleClick >= maxTimeDoubleClick)
        {
            checkDoubleClick = false;
            timeDoubleClick = 0f;
            //MouseEventOccured(new MouseEventArgs() { mouseBtn = 0, doubleClick = false });
        }
        else if (checkDoubleClick)
        {
            timeDoubleClick += Time.deltaTime;
        }

        //Check for drag click
        if (Input.GetMouseButton(0) && !IsPointerOverUIObject())
        {
            if (Vector3.Distance(startDrag, GetMousePositionOnXZPlane()) > 0.5f && !CameraController.instance.focusing)
            {
                checkDoubleClick = false;
                timeDoubleClick = 0f;
                isDragging = true;
                MouseEventOccured(
                    new MouseEventArgs()
                    {
                        mouseBtn = 0,
                        doubleClick = false,
                        dragging = true,
                        startDrag = startDrag,
                        endDrag = GetMousePositionOnXZPlane()
                    });
            }
        }

        //Check for mouse end drag
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            MouseEventOccured(
                new MouseEventArgs()
                {
                    mouseBtn = 0,
                    doubleClick = false,
                    dragging = false,
                    startDrag = startDrag,
                    endDrag = GetMousePositionOnXZPlane()
                });
        }

        //Check for single right click and raise that event
        if (Input.GetMouseButtonDown(1) && !IsPointerOverUIObject())
        {
            MouseEventOccured(new MouseEventArgs() { mouseBtn = 1, doubleClick = false });
        }
    }

    //Check if cursor is over Ui object
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Gets the mouse position on the world xz-plane
    public static Vector3 GetMousePositionOnXZPlane()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (XZPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Ensure y position is exactly zero
            hitPoint.y = 0;
            return hitPoint;
        }
        return Vector3.zero;
    }

    public void PrintMouseInput(MouseEventArgs e)
    {
        Debug.Log($"Button: {e.mouseBtn}\nDoubleClk: {e.doubleClick}\ndragging: {e.dragging}\nstartDrag: {e.startDrag}\nendDrag {e.endDrag}");
    }

    private void OnGUI()
    {
        //Code for raising key event
        if (Event.current.isKey && Event.current.type == EventType.KeyDown)
        {
            keyEventOccured(Event.current.keyCode);
        }
    }
}