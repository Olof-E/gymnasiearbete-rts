using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public float maxSpeed = 6.5f;
    public float rotSpeed = 15f;
    public float offsetDist = 15f;
    public float edgeMoveThickness = 100f;
    public bool focusing = false;
    public float speed = 100f;
    public Camera mainCamera { get; private set; }
    Vector3 lastMoveDir;
    Vector3 targetMoveDir;
    float lastZoomPos;
    float targetZoomPos = 15f;
    Vector2 orbitAngles;
    Vector3 focusPos;

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


        Vector3 moveDir = Vector3.Lerp(lastMoveDir, targetMoveDir, 0.25f);
        lastMoveDir = moveDir;
        speed = maxSpeed * (Mathf.Exp(offsetDist / 30));

        transform.Translate(transform.TransformDirection(moveDir) * Time.deltaTime * speed, Space.World);
        if (focusing)
        {
            transform.position = Vector3.MoveTowards(transform.position, focusPos, speed * Vector3.Distance(transform.position, focusPos) * Time.deltaTime);
            if (Vector3.Distance(transform.position, focusPos) < 0.5f)
            {
                focusing = false;
            }
        }
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
        Quaternion targetLookRot = Quaternion.Euler(0f, orbitAngles.y, 0f);
        Quaternion cameraLookRot = Quaternion.Euler(orbitAngles.x, 0f, 0f);

        transform.rotation = Quaternion.Slerp(transform.localRotation, targetLookRot, 0.1f);
        mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, cameraLookRot, 0.1f);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            targetZoomPos = targetZoomPos - Input.mouseScrollDelta.y * Mathf.Clamp(Mathf.Exp(offsetDist / 65f), 1f, 15f);
        }
        offsetDist = Mathf.Clamp(Mathf.Lerp(lastZoomPos, targetZoomPos, 0.075f), 2f, 85f);
        lastZoomPos = offsetDist;

        mainCamera.transform.position = transform.position - mainCamera.transform.forward * offsetDist;
    }

    public void FocusPosition(Vector3 focusPoint)
    {
        focusPos = focusPoint;
        focusing = true;
        //StartCoroutine(Focusing());
        targetZoomPos = 35f;
    }

    // public IEnumerator Focusing()
    // {
    //     yield return new WaitForSeconds(0.15f);
    //     focusing = false;
    // }
}