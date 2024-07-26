using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ability_FireBolt : Ability
{
    private List<GameObject> projectileList = new List<GameObject>();

    void Start()
    {
        Debug.Log("Gameobject name: " + this);
        
    }

    public Ability_FireBolt()
    {
        name = "Fire Bolt";
        range = 7;
    }

    public override void Activate(GameObject parent, Vector3 castLocation)
    {
        // Spawn location x value has +1 for now so it spawns generally in front of the player
        // TODO: change the spawn location so it is "+1" in the direction of the end location
        Vector3 spawnLocation = new Vector3(parent.transform.position.x + 1, parent.transform.position.y, parent.transform.position.z);

        //GameObject fireBolt = null;

        //if (projectileList.Count > 0)
        //{
        //    foreach (GameObject obj in projectileList)
        //    {
        //        if (!obj.activeSelf)
        //        {
        //            fireBolt = obj;
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
        //    projectileList.Add(fireBolt);
        //}

        //fireBolt.transform.position = spawnLocation;
        //fireBolt.SetActive(true);

        GameObject fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
        Debug.Log("simple null test: " + fireBolt);
        projectileList.Add(fireBolt);
        Debug.Log("1st in list: " + projectileList[0]);
        foreach (GameObject obj in projectileList)
        {
            Debug.Log("Foreach");
        }
        AbilityBehavior projectile = fireBolt.GetComponent<AbilityBehavior>();
        projectile.spawnLocation = spawnLocation;
        projectile.endLocation = castLocation;
        projectile.abilityParameters.range = range;
        projectile.abilityParameters.damage = damage;
        //Debug.Log("Damage value set from Ability_FireBolt: " + damage);
    }
}
