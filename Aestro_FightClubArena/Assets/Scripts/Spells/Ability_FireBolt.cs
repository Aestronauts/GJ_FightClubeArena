using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ability_FireBolt : Ability
{
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
        GameObject fireBolt = Instantiate(model, spawnLocation, Quaternion.identity);
        Temp_Projectile projectile = fireBolt.GetComponent<Temp_Projectile>();
        projectile.spawnLocation = spawnLocation;
        projectile.endLocation = castLocation;
        projectile.range = range;
        projectile.damage = damage;
        Debug.Log("Damage value set from Ability_FireBolt: " + damage);
    }
}
