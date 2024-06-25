using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    public List<GameObject> projectileList = new List<GameObject>();
    AbilityManager abilityManager;
    public Ability FireBolt;
    public GameObject model;
    int damage = 1;
    int range = 7;
    
    // Start is called before the first frame update
    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Takes the game object of the player that casted the ability, the ability's name, and the cast location
    // Uses the provided information to then Activate the correct ability
    public void CastAbility(GameObject player_gameObject, string abilityName, Vector3 castLocation)
    {
        // Choose "if" logic by matching the given ability name
        // TODO: Is there a better way of doing this???
        if (abilityName == "Fire Bolt")
        {
            //FireBolt.Activate(player_gameObject, castLocation);
            FireFireBolt(player_gameObject, castLocation);
        }
    }

    // Takes the gameobject of the player that received damage and the damage dealt by that projectile
    // Updates the health of the player that received damage
    public void ReceivedDamage(GameObject player_gameObject, int damage)
    {
        // TODO: Updates the health of the player that received damage
    }

    public void FireFireBolt(GameObject parent, Vector3 castLocation)
    {
        // Spawn location x value has +1 for now so it spawns generally in front of the player
        // TODO: change the spawn location so it is "+1" in the direction of the end location
        Vector3 spawnLocation = new Vector3(parent.transform.position.x + 1, parent.transform.position.y, parent.transform.position.z);

        GameObject fireBolt = null;

        bool foundInactiveObj = false;
        if (projectileList.Count > 0)
        {
            foreach (GameObject obj in projectileList)
            {
                if (!obj.activeSelf)
                {
                    Debug.Log("Found an inactive object.");
                    fireBolt = obj;
                    foundInactiveObj = true;
                    break;
                }
            }
            if (!foundInactiveObj)
            {
                // if no inactive game objects are found, then the current projectiles are all active,
                // so we need to instantiate a new projectile
                Debug.Log("Instantiating a new object.");
                fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
                projectileList.Add(fireBolt);
            }
        }
        else
        {
            Debug.Log("Instantiating a new object.");
            fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
            projectileList.Add(fireBolt);
        }
        //projectileList.Add(fireBolt);

        fireBolt.transform.position = spawnLocation;
        

        //GameObject fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
        //Debug.Log("simple null test: " + fireBolt);
        //projectileList.Add(fireBolt);
        //Debug.Log("1st in list: " + projectileList[0]);
        //foreach (GameObject obj in projectileList)
        //{
        //    Debug.Log("Foreach");
        //}
        Temp_Projectile projectile = fireBolt.GetComponent<Temp_Projectile>();
        projectile.distanceTraveled = 0f;
        projectile.spawnLocation = spawnLocation;
        projectile.endLocation = castLocation;
        projectile.range = range;
        projectile.damage = damage;
        fireBolt.SetActive(true);
        //Debug.Log("Damage value set from Ability_FireBolt: " + damage);
    }
}
