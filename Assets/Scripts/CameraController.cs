using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // //Pan Settings
    // [Header("Pan Settings")]
    // public float panSpeed = 15.0f;
    // public float mousePanMultiplier = 1.0f;
    // public float mouseDeadZone = 100.0f;


    // //Zoom Settings
    // [Header("Zoom Settings")]
    // public float zoomSpeed = 1.0f;
    // public float mouseZoomMultiplier = 5.0f;
    // public float zoomMin = 2.0f;
    // public float zoomMax = 50.0f;


    // //Rotation Settings
    // [Header("Rotation Settings")]
    // public float rotationSpeed = 50.0f;
    // public float mouseRotationMultiplier = 0.2f;

    // //General Speed Settings
    // [Header("General Speed Settings")]
    // public bool smoothing = true;
    // public float smoothingFactor = 0.1f;


    // //Generic Settings
    // [Header("Generic Settings")]
    // public float mouseEdgeBoundary = 40.0f;
    // public static Vector3 cameraTarget;
    // public Vector3 lastMousePos;
    // public Vector3 mouseClicked;

    // //Private Settings
    // private Vector3 lastPanSpeed = Vector3.zero;
    // private Vector3 cameraPosition;
    // private GameObject _camera;

    // private void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    //     else
    //     {
    //         Destroy(this);
    //         Debug.Log("Camera controller already exists...");
    //     }
    // }
    // // Use this for initialization
    // void Start()
    // {
    //     cameraTarget = transform.position;
    //     lastMousePos = Vector3.zero;
    //     mouseClicked = Vector3.zero;
    //     _camera = transform.Find("Main Camera").gameObject;
    // }

    // void Update()
    // {
    //     Rotate();
    //     Pan();
    //     Zoom();
    //     UpdatePosition();

    //     lastMousePos = Input.mousePosition;


    // }

    // private void Pan()
    // {
    //     Vector3 whereTo = Vector3.zero;

    //     //Move the camera with the keyboard
    //     if (KeyboardInput())
    //     {
    //         if (Input.GetKey(KeyCode.W))
    //             whereTo += Vector3.forward;

    //         if (Input.GetKey(KeyCode.A))
    //             whereTo += Vector3.left;

    //         if (Input.GetKey(KeyCode.S))
    //             whereTo += Vector3.back;

    //         if (Input.GetKey(KeyCode.D))
    //             whereTo += Vector3.right;
    //     }

    //     //Move the camera with the right mouse button (keep pressed and move around)
    //     if (MouseInput())
    //     {
    //         if (Input.GetMouseButton(1) && mouseClicked == Vector3.zero)
    //             mouseClicked = Input.mousePosition;

    //         if (Input.GetMouseButton(1))
    //         {
    //             if (mouseClicked != Vector3.zero)
    //             {
    //                 if ((mouseClicked.y < Input.mousePosition.y) && ((Input.mousePosition.y - mouseClicked.y) > mouseDeadZone))
    //                     whereTo += Vector3.forward * (Input.mousePosition.y - mouseClicked.y) / 150 * mousePanMultiplier;

    //                 if ((mouseClicked.x < Input.mousePosition.x) && ((Input.mousePosition.x - mouseClicked.x) > mouseDeadZone))
    //                     whereTo += Vector3.right * (Input.mousePosition.x - mouseClicked.x) / 150 * mousePanMultiplier;

    //                 if ((mouseClicked.y > Input.mousePosition.y) && ((mouseClicked.y - Input.mousePosition.y) > mouseDeadZone))
    //                     whereTo += Vector3.back * (mouseClicked.y - Input.mousePosition.y) / 150 * mousePanMultiplier;

    //                 if ((mouseClicked.x > Input.mousePosition.x) && ((mouseClicked.x - Input.mousePosition.x) > mouseDeadZone))
    //                     whereTo += Vector3.left * (mouseClicked.x - Input.mousePosition.x) / 150 * mousePanMultiplier;
    //             }
    //         }
    //     }

    //     if (Input.GetMouseButtonUp(1))
    //         mouseClicked = Vector3.zero;

    //     //Move the camera by placing the mouse cursor on the edges of the screen
    //     if (MouseOverScreenEdge())
    //     {
    //         if (Input.mousePosition.y > (Screen.height - mouseEdgeBoundary))
    //             whereTo += Vector3.forward * mousePanMultiplier;

    //         if (Input.mousePosition.x > (Screen.width - mouseEdgeBoundary))
    //             whereTo += Vector3.right * mousePanMultiplier;

    //         if (Input.mousePosition.y < mouseEdgeBoundary)
    //             whereTo += Vector3.back * mousePanMultiplier;

    //         if (Input.mousePosition.x < mouseEdgeBoundary)
    //             whereTo += Vector3.left * mousePanMultiplier;
    //     }

    //     //Now that the destination is set we move the camera
    //     SmoothIt(whereTo);
    // }

    // private void Zoom()
    // {
    //     Vector3 whereTo = Vector3.zero;
    //     if (KeyboardInput())
    //     {
    //         if (Input.GetKey(KeyCode.R))
    //             whereTo += Vector3.down * zoomSpeed;

    //         if (Input.GetKey(KeyCode.F))
    //             whereTo += Vector3.up * zoomSpeed;
    //     }

    //     if (MouseInput())
    //     {
    //         if (Input.GetAxis("Mouse ScrollWheel") > 0)
    //             whereTo += Vector3.down * mouseZoomMultiplier;

    //         if (Input.GetAxis("Mouse ScrollWheel") < 0)
    //             whereTo += Vector3.up * mouseZoomMultiplier;
    //     }

    //     SmoothIt(whereTo);
    // }

    // private void Rotate()
    // {
    //     float yRotation = 0.0f;
    //     float xRotation = 0.0f;

    //     if (KeyboardInput())
    //     {
    //         if (Input.GetKey(KeyCode.Q))
    //             yRotation = -1.0f;

    //         if (Input.GetKey(KeyCode.E))
    //             yRotation = 1.0f;
    //     }
    //     transform.Rotate(0, yRotation, 0);

    //     if (MouseInput())
    //     {
    //         if (Input.GetMouseButton(2))
    //         {
    //             Vector3 mousePosition = (Input.mousePosition - lastMousePos);
    //             if (Input.mousePosition.x != lastMousePos.x)
    //             {
    //                 yRotation += mousePosition.x * mouseRotationMultiplier;
    //                 //Rotate the camera horizontally on the Y axis
    //                 transform.Rotate(0, yRotation, 0);
    //             }

    //             if (Input.mousePosition.y != lastMousePos.y)
    //             {
    //                 xRotation -= mousePosition.y * mouseRotationMultiplier;

    //                 _camera.transform.Rotate(xRotation, 0, 0);
    //             }
    //         }
    //     }
    // }

    // private void SmoothIt(Vector3 whereTo)
    // {
    //     Vector3 effectivePanSpeed = whereTo;
    //     if (smoothing)
    //     {
    //         effectivePanSpeed = Vector3.Lerp(lastPanSpeed, whereTo, smoothingFactor);
    //         lastPanSpeed = effectivePanSpeed;
    //     }

    //     cameraTarget = cameraTarget + transform.TransformDirection(effectivePanSpeed) * panSpeed * Time.deltaTime;
    //     cameraPosition = transform.position;
    // }

    // private void UpdatePosition()
    // {
    //     transform.position = cameraTarget;
    // }

    // #region Helpers

    // public static bool KeyboardInput()
    // {
    //     if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
    //         Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.F))
    //         return true;
    //     else return false;
    // }

    // private bool MouseInput()
    // {
    //     if (Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetAxis("Mouse ScrollWheel") != 0)
    //         return true;
    //     else return false;
    // }

    // private bool MouseOverScreenEdge()
    // {
    //     if (Input.mousePosition.x < mouseEdgeBoundary ||
    //         Input.mousePosition.x > Screen.width - mouseEdgeBoundary ||
    //         Input.mousePosition.y < mouseEdgeBoundary ||
    //         Input.mousePosition.y > Screen.height - mouseEdgeBoundary)
    //         return true;
    //     else return false;
    // }

    // #endregion














    public static CameraController instance;
    public float maxSpeed = 100f;
    public float rotSpeed = 15f;
    public float offsetDist = 15f;
    public float edgeMoveThickness = 100f;
    public bool focusing = false;
    private float speed = 0f;
    private Camera mainCamera;
    Vector3 lastMoveDir;
    Vector3 targetMoveDir;
    float lastZoomPos;
    float targetZoomPos;
    Vector2 orbitAngles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.Log("Camera controller already exists...");
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = mainCamera.transform.position - mainCamera.transform.forward * offsetDist;
        orbitAngles = new Vector2(45f, 0f);
        mainCamera.transform.localRotation = Quaternion.Euler(orbitAngles);
    }

    private void FixedUpdate()
    {
        targetMoveDir = Vector3.zero;
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            targetMoveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        }

        // if (Input.mousePosition.x < edgeMoveThickness)
        // {
        //     moveDir.x = -1;//(Input.mousePosition.x - edgeMoveThickness) / (6f * edgeMoveThickness);
        // }
        // else if (Input.mousePosition.x > Screen.width - edgeMoveThickness)
        // {
        //     moveDir.x = 1;//(Input.mousePosition.x - (Screen.width - edgeMoveThickness)) / (6f * edgeMoveThickness);
        // }

        // if (Input.mousePosition.y < edgeMoveThickness)
        // {
        //     moveDir.z = -1;//(Input.mousePosition.y - edgeMoveThickness) / (6f * edgeMoveThickness);
        // }
        // else if (Input.mousePosition.y > Screen.height - edgeMoveThickness)
        // {
        //     moveDir.z = 1;//(Input.mousePosition.y - (Screen.height - edgeMoveThickness)) / (6f * edgeMoveThickness);
        // }

        // if (moveDir != Vector3.zero)
        // {
        //     speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime * 2f);
        // }
        // else
        // {
        //     speed = Mathf.Lerp(speed, 0f, Time.fixedDeltaTime * 2f);
        // }
        Vector3 moveDir = Vector3.Lerp(lastMoveDir, targetMoveDir, 0.25f);
        lastMoveDir = moveDir;
        speed = maxSpeed;

        transform.Translate(moveDir * Time.deltaTime * speed, Space.World);
        //transform.position = new Vector3(target.transform.position.x, 0f, target.transform.position.z);
        //transform.LookAt(target.transform.position, Vector3.up);


        if (Input.GetMouseButton(2))
        {
            Vector2 input = new Vector2(
                -Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X")
            );
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                orbitAngles += rotSpeed * Time.deltaTime * input;
            }

            orbitAngles = new Vector2(Mathf.Clamp(orbitAngles.x, -85f, 85f), orbitAngles.y);
        }
        Quaternion lookRot = Quaternion.Euler(orbitAngles);
        mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, lookRot, 0.1f);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            targetZoomPos = targetZoomPos - Input.mouseScrollDelta.y;
        }
        offsetDist = Mathf.Lerp(lastZoomPos, targetZoomPos, 0.075f);
        lastZoomPos = offsetDist;

        mainCamera.transform.position = transform.position - mainCamera.transform.forward * offsetDist;
    }

    public void FocusPosition(Vector3 focusPoint)
    {
        transform.position = focusPoint;
        focusing = true;
        StartCoroutine(Focusing());
        //offsetDist = 65f;
    }

    public IEnumerator Focusing()
    {
        yield return new WaitForSeconds(0.15f);
        focusing = false;
    }
}