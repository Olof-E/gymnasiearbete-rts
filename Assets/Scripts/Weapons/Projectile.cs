using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float dmg = 25f;
    public float speed = 500f;
    public bool fired = false;
    private float timeSinceFired = 0f;
    public Targetable target;
    public ShieldManager ignoreShield;
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
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 0.5f))
        {
            if (hitInfo.collider.gameObject == ignoreShield.gameObject)
            {
                return;
            }
            Debug.Log("Hit target obj: " + hitInfo.collider.name);
            target.TakeDamage(dmg, hitInfo.point);
            Destroy(this.gameObject);
        }
    }

    public void Fire(Vector3 targetPos)
    {
        transform.LookAt(targetPos);
        fired = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }
}