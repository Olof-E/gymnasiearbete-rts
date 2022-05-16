using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float dmg = 25f;
    public float speed = 500f;
    public bool fired = false;
    private float timeSinceFired = 0f;
    public Targetable target;
    public ShieldManager ignoreShield;
    public BoxCollider ignoreSelectionColl;
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
        if (Physics.Raycast(transform.position, -transform.forward, out hitInfo, 2f))
        {
            if (hitInfo.collider.gameObject == ignoreShield.gameObject || hitInfo.collider.gameObject == ignoreSelectionColl.gameObject)
            {
                return;
            }
            Targetable hit;
            ShieldManager shieldHit;
            if (hitInfo.transform.TryGetComponent<Targetable>(out hit))
            {
                hit.TakeDamage(dmg, hitInfo.point);
            }
            else if (hitInfo.transform.gameObject.TryGetComponent<ShieldManager>(out shieldHit))
            {
                shieldHit.parent.TakeDamage(dmg, hitInfo.point);
            }

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
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * 2f);
    }
}