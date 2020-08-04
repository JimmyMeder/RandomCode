using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus: MonoBehaviour
{

    public int prevHealth;
    public int health;
    public int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        prevHealth = health;
    }

    public bool DamageChar(int amount)
    {
        if (health > 0)
        {
            prevHealth = health;
            health -= amount;
            health = Mathf.Clamp(health, 0, maxHealth);
            if (health == 0)
            {
                //Death();
            }
            return true;
        }
        return false;
    }
    public bool HealChar(int amount)
    {
        if (health < maxHealth)
        {
            prevHealth = health;
            health += amount;
            health = Mathf.Clamp(health, 0, maxHealth);
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
