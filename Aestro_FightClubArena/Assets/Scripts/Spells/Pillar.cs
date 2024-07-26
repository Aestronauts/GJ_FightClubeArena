using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public int health = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fire Bolt")
        {
            AbilityBehavior projectile = other.gameObject.GetComponent<AbilityBehavior>();
            health -= projectile.damage;
            Debug.Log("Damage Dealt to Pillar! Remaining health: " + health);
            Destroy(other.gameObject);
            if (health == 0) Destroy(this.gameObject);
        }
    }
}
