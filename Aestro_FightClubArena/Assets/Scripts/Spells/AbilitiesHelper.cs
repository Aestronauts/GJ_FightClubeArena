using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHelper
{
    public static void FireProjectile(GameObject parent, Vector3 castLocation, List<GameObject> projectileList, GameObject projectile, Transform projectilesHolder)
    {
        // Spawn location x value has +1 for now so it spawns generally in front of the player
        // TODO: change the spawn location so it is "+1" in the direction of the end location
        Vector3 spawnLocation = new Vector3(parent.transform.position.x+1, parent.transform.position.y, parent.transform.position.z);
        GameObject fireBolt = null;
        if (projectileList.Count > 0) fireBolt = CheckForInactiveProjectile(projectileList);
        if (fireBolt == null)
        {
            fireBolt = Object.Instantiate(projectile, spawnLocation, Quaternion.identity, projectilesHolder);
            projectileList.Add(fireBolt);
        }
        fireBolt.transform.position = spawnLocation;
        SetFireboltProjectileInformation(castLocation, spawnLocation, fireBolt);
        fireBolt.SetActive(true);
    }
    // SetProjectileInformation() takes a Vector3 cast location and spawn location, as well as
    //      the GameObject of the fire bolt
    // Sets relevant information for the projectile, including start & end locations, damage, and range
    private static void SetFireboltProjectileInformation(Vector3 castLocation, Vector3 spawnLocation, GameObject projectile)
    {
        Temp_Projectile projectileInfo = projectile.GetComponent<Temp_Projectile>();
        projectileInfo.distanceTraveled = 0f;
        projectileInfo.spawnLocation = spawnLocation;
        projectileInfo.endLocation = castLocation;
        projectileInfo.range = projectileInfo.abilityManager.FireBoltRange;
        projectileInfo.damage = projectileInfo.abilityManager.FireBoltDamage;
    }
    private static void SetTwinFireboltProjectileInformation(Vector3 castLocation, Vector3 spawnLocation, GameObject projectile)
    {
        Temp_Projectile projectileInfo = projectile.GetComponent<Temp_Projectile>();
        projectileInfo.distanceTraveled = 0f;
        projectileInfo.spawnLocation = spawnLocation;
        projectileInfo.endLocation = castLocation;
        projectileInfo.range = projectileInfo.abilityManager.FireBoltRange;
        projectileInfo.damage = projectileInfo.abilityManager.FireBoltDamage;
    }
    // // CheckForInactiveProjectile() looks through pooled projectiles and returns an inactive
    // //      gameObject projectile that can be reused, returning null if not.
    private static GameObject CheckForInactiveProjectile(List<GameObject> projectileList)
    {
        foreach (GameObject obj in projectileList)
        {
            if (!obj.activeSelf) return obj;
        }
        return null;
    }
    
    public static void FireFirePillar(GameObject parent, Vector3 castLocation, AbilityManager abilityManager,Transform projectilesHolder)
    {
        GameObject firePillar = null;
        Vector3 finalLocation;
        // TODO: Pooling for the fire pillar's game object
        Vector3 parentLocation = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
        float distanceToCastLocation = Vector3.Distance(parentLocation, castLocation);
        if (distanceToCastLocation > abilityManager.FP_range)
        {
            float distPercentage = abilityManager.FP_range / distanceToCastLocation;
            finalLocation = Vector3.Lerp(parentLocation, castLocation, distPercentage);
        }
        else finalLocation = castLocation;
        firePillar = Object.Instantiate(abilityManager.FirePillar_gameObject, finalLocation, Quaternion.identity,projectilesHolder);
        firePillar.transform.position = finalLocation; //redundant until pooling
        FirePillar FP = firePillar.GetComponent<FirePillar>();
        FP.damage_burst = abilityManager.FP_damage_burst;
        FP.damage_overtime = abilityManager.FP_damage_overtime;
        FP.duration = abilityManager.FP_duration;
        FP.DPS_rate = abilityManager.FP_DPS_rate;
    }
    
    public static void TwinFireProjectile(GameObject parent, Vector3 castLocation, List<GameObject> projectileList, GameObject projectile, Transform projectilesHolder)
    {
        // Spawn location x value has +1 for now so it spawns generally in front of the player
        // TODO: change the spawn location so it is "+1" in the direction of the end location
        Vector3 spawnLocation = new Vector3(parent.transform.position.x+1, parent.transform.position.y, parent.transform.position.z);
        GameObject fireBolt = null;
        if (projectileList.Count > 0) fireBolt = CheckForInactiveProjectile(projectileList);
        if (fireBolt == null)
        {
            fireBolt = Object.Instantiate(projectile, spawnLocation, Quaternion.identity, projectilesHolder);
            projectileList.Add(fireBolt);
        }
        fireBolt.transform.position = spawnLocation;
        SetTwinFireboltProjectileInformation(castLocation, spawnLocation, fireBolt);
        fireBolt.SetActive(true);
    }

    public static void Ability1()
    {
        //logic for ability2
        Debug.Log("Play ability 1!");
    }

    public static void Ability2()
    {
        //logic for ability2
        Debug.Log("Play ability 2!");
    }
    
    public static void Ability3()
    {
        //logic for ability3
        Debug.Log("Play ability 3!");
    }
    
    public static void Ability4()
    {
        //logic for ability4
        Debug.Log("Play ability 4!");
    }

}
