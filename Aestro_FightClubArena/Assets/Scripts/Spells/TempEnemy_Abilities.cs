using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy_Abilities : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;

    public int health = 5;

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
        if (other.tag == "Fire Bolt")
        {
            Temp_Projectile projectile = other.gameObject.GetComponent<Temp_Projectile>();
            health -= projectile.damage;
            Debug.Log("Damage Dealt to TempEnemy in OnTriggerEnter! Remaining health: " + health);
            playerCharacterManager.ReceivedDamage(gameObject, projectile.damage);
            Destroy(other.gameObject);
            if (health == 0) Destroy(this.gameObject);
        }
    }
}
