using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float dmg = 25f;
    public float speed = 500f;
    public bool fired = false;
    private float timeSinceFired = 0f;
    public Targetable target;
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
        if (timeSinceFired >= 4f)
        {
            Destroy(this.gameObject);
        }
    }

    public void Fire(Vector3 targetPos)
    {
        transform.LookAt(targetPos);
        fired = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit target obj");
        Destroy(this.gameObject);
        target.TakeDamage(dmg);
    }
}