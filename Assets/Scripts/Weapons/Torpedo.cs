using UnityEditor;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public float dmg = 20f;
    public Targetable target;
    public float maxSpeed = 1000f;
    public float angularSpeed = 75f;
    public float acceleration = 3.5f;
    private Rigidbody rb;
    private float speed = 0f;
    private Vector3 oldPos = Vector3.zero;
    private Vector3 oldTargetPos = Vector3.zero;
    private Vector3 currPos = Vector3.zero;
    private Vector3 currTargetPos = Vector3.zero;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.AddForce(transform.forward * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    Vector3 aC;
    private void FixedUpdate()
    {
        if (target != null)
        {
            oldPos = currPos;
            oldTargetPos = currTargetPos;
            currTargetPos = target.gameObj.transform.position;
            currPos = transform.position;

            float N = 4.25f;
            //Calculate Line Of Sight
            Vector3 oldLOS = oldTargetPos - oldPos;
            Vector3 newLOS = currTargetPos - currPos;

            //Calculate the change in LOS 
            Vector3 LOSDelta = newLOS - oldLOS;
            float LOSRate = LOSDelta.magnitude;

            //PN guidance law
            aC = newLOS * N * -LOSRate;

            //Move missile in accordance with the APN result
            Debug.Log(Vector3.Cross(aC, transform.forward));

            //transform.LookAt(target.transform.position + target.transform.forward * 4f * Time.fixedDeltaTime * Mathf.Clamp(100f / distance, 0, 1), transform.up);
            rb.angularVelocity = Vector3.Cross(aC, transform.forward) * angularSpeed * Time.fixedDeltaTime;
            rb.velocity = transform.forward * speed * Time.fixedDeltaTime;

            speed = Mathf.Lerp(speed, maxSpeed, acceleration * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit target obj");
        Destroy(this.gameObject);
        target.TakeDamage(dmg, other.contacts[0].point);
    }
}