
using UnityEngine;
using UnityEngine.UI;

public class Targetable : MonoBehaviour
{
    public float armor;
    public float shields;
    public float maxArmor;
    public float maxShields;
    public Slider shieldBar;
    public Slider armorBar;

    public bool destroyed = false;
    public GameObject gameObj;
    public ShieldManager shieldManager;
    public void TakeDamage(float dmg, Vector3 hitPos)
    {
        if (shields <= 0f)
        {
            //armor -= dmg;
            if (armor <= 0f)
            {
                destroyed = true;
            }
        }
        else
        {
            shieldManager.AddNewHit(hitPos, 1);
            //shields -= dmg;
        }
    }
}
