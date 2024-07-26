using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy_Abilities : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;

    public int health = 5;

    public bool isInFirePillar = false;

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
            AbilityBehavior projectile = other.gameObject.GetComponent<AbilityBehavior>();
            health -= projectile.abilityParameters.damage;
            Debug.Log("Damage Dealt to TempEnemy in OnTriggerEnter! Remaining health: " + health);
            playerCharacterManager.ReceivedDamage(gameObject, projectile.abilityParameters.damage);
            Destroy(other.gameObject);
            if (health == 0) Destroy(this.gameObject);
        }
        else if (other.tag == "Fire Pillar")
        {
            FirePillar firePillar = other.gameObject.GetComponent<FirePillar>();
            isInFirePillar = true;
            InvokeRepeating("FirePillarDamageOverTime", 0.75f, 0.75f);
            playerCharacterManager.ReceivedDamage(gameObject, firePillar.damage_burst);
        }
        else
        {
            Debug.Log("Hit by an unregistered object/tag!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Fire Pillar")
        {
            FirePillar firePillar = other.gameObject.GetComponent<FirePillar>();
            Debug.Log("fire pillar is sticking around...");
        }
    }

    private void FirePillarDamageOverTime()
    {
        if (isInFirePillar)
        {
            Debug.Log("Still in fire pillar, taking damage...");
        }
    }
}
