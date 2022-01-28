using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public float maxSpeed = 100f;
    public float rotSpeed = 15f;
    public float offsetDist = 15f;
    public float edgeMoveThickness = 100f;
    public bool focusing = false;
    private float speed = 0f;
    private Camera mainCamera;
    Vector3 moveDir;

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
    }

    private void Update()
    {
        moveDir = Vector3.zero;
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
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

        if (moveDir != Vector3.zero)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime * 2f);
        }
        else
        {
            speed = Mathf.Lerp(speed, 0f, Time.fixedDeltaTime * 2f);
        }

        transform.Translate(moveDir.normalized * Time.fixedDeltaTime * speed * Mathf.Exp(0.05f * offsetDist), Space.World);
        //transform.position = new Vector3(target.transform.position.x, 0f, target.transform.position.z);
        //transform.LookAt(target.transform.position, Vector3.up);


        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1))
        {
            Quaternion newRot = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * rotSpeed, mainCamera.transform.rotation.eulerAngles.z);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, newRot, rotSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            offsetDist = Mathf.Clamp(offsetDist - Input.mouseScrollDelta.y * 50f * Time.fixedDeltaTime, 5f, 65f);
            mainCamera.transform.position = transform.position - mainCamera.transform.forward * offsetDist;
        }
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