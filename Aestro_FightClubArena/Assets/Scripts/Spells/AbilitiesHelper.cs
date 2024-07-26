using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHelper
{
    public static void SpawnAbility(GameObject parent, Vector3 spawnLocation, Vector3 castLocation, 
        List<GameObject> projectileList, GameObject projectilePrefab, Transform projectilesHolder, float range, int damage, MonoBehaviour coroutineStarter)
    {
        // Spawn location x value has +1 for now so it spawns generally in front of the player
        // TODO: change the spawn location so it is "+1" in the direction of the end location
        //Vector3 spawnLocation = new Vector3(parent.transform.position.x+1, parent.transform.position.y, parent.transform.position.z);
        GameObject projectile = null;
        if (projectileList.Count > 0) projectile = CheckForInactiveAbilityPrefab(projectileList);
        if (projectile == null)
        {
            projectile = Object.Instantiate(projectilePrefab, spawnLocation, Quaternion.identity, projectilesHolder);
            projectileList.Add(projectile);
        }
        projectile.transform.position = spawnLocation;
        AbilityBehavior projectileInfo = projectile.GetComponent<AbilityBehavior>();
        SetProjectileInformation(projectileInfo,castLocation, spawnLocation, damage,range);
        projectile.SetActive(true);

        //DisableAbility(projectileInfo.duration, projectile);
        if (projectileInfo.duration != 0)
        {
            coroutineStarter.StartCoroutine(DisableAbility(projectileInfo.duration, projectile));
        }
    }
    private static void SetProjectileInformation(AbilityBehavior projectileInfo, Vector3 castLocation, Vector3 spawnLocation, 
        int damage, float range)
    {
        projectileInfo.distanceTraveled = 0f;
        projectileInfo.spawnLocation = spawnLocation;
        projectileInfo.endLocation = castLocation;
        projectileInfo.range = range;
        projectileInfo.damage = damage;
        //projectileInfo.projectileSpeed = projectileSpeed;
        // projectileInfo.damageOvertime;
        // projectileInfo.duration;
        // projectileInfo.DPS_rate;
    }
    // // CheckForInactiveProjectile() looks through pooled projectiles and returns an inactive
    // //      gameObject projectile that can be reused, returning null if not.
    private static GameObject CheckForInactiveAbilityPrefab(List<GameObject> projectileList)
    {
        foreach (GameObject obj in projectileList)
        {
            if (!obj.activeSelf) return obj;
        }
        return null;
    }
    static IEnumerator DisableAbility(float duration, GameObject gameObject)
    {
        yield return new WaitForSeconds(duration);
        //AbilitiesHelper.ExitFirePillar(playerCharacterManager.tempEnemy);
        Debug.Log("Fire Pillar is being destroyed, stopping TempEnemy invoke and damage.");
        gameObject.SetActive(false);
    }
    
    public static void ExitFirePillar(TempEnemy_Abilities tempEnemy)
    {
        tempEnemy.isInFirePillar = false;
        tempEnemy.CancelInvoke();
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
