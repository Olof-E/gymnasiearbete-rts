
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public float armor;
    public float shields;
    public GameObject gameObj;

    public void TakeDamage(float dmg)
    {
        if (shields <= 0)
        {
            armor -= dmg;
        }
        else
        {
            shields -= dmg;

        }
    }
}
