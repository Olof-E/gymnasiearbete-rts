
using UnityEngine;

public class Targetable : MonoBehaviour
{
    public float armor;
    public float shields;
    public bool destroyed = false;
    public GameObject gameObj;

    public void TakeDamage(float dmg)
    {
        if (shields <= 0f)
        {
            armor -= dmg;
            if (armor <= 0f)
            {
                destroyed = true;
            }
        }
        else
        {
            shields -= dmg;

        }
    }
}
