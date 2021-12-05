using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 500f;
    public bool fired = false;
    private float timeSinceFired = 0f;
    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (fired)
        {
            transform.Translate(transform.forward * speed * Time.fixedDeltaTime, Space.World);
            timeSinceFired += Time.fixedDeltaTime;
        }
        if (timeSinceFired >= 6f)
        {
            Destroy(this.gameObject);
        }
    }

    public void Fire(Vector3 targetPos)
    {
        transform.LookAt(targetPos);
        fired = true;
    }
}