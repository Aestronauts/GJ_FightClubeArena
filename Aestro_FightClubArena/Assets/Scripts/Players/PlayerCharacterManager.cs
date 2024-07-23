using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityManager))]
public class PlayerCharacterManager : MonoBehaviour
{
    public static PlayerCharacterManager instance { get; private set; }

    AbilityManager abilityManager;
    public TempEnemy_Abilities tempEnemy;

    [Header("Fire Pillar Information")]
    public GameObject FirePillar_gameObject;
    public int FP_damage_burst = 3;
    public int FP_damage_overtime = 1;
    public int FP_range = 12;
    public float FP_duration = 3f;
    public float FP_DPS_rate = 0.75f;

    private void Awake()
    {
        if (PlayerCharacterManager.instance != null && PlayerCharacterManager.instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

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
        if (abilityName == "Firebolt")
        {
            AbilitiesHelper.FireProjectile(player_gameObject, castLocation, 
                abilityManager.ProjectileList, abilityManager.FireBoltPrefab, abilityManager.ProjectilesHolder);
        }
        else if (abilityName == "Fire Pillar")
        {
            FireFirePillar(player_gameObject, castLocation);
        }
    }

    // Takes the gameobject of the player that received damage and the damage dealt by that projectile
    // Updates the health of the player that received damage
    public void ReceivedDamage(GameObject player_gameObject, int damage)
    {
        // TODO: Updates the health of the player that received damage
    }

    private void FireFirePillar(GameObject parent, Vector3 castLocation)
    {
        GameObject firePillar = null;
        Vector3 finalLocation;
        // TODO: Pooling for the fire pillar's game object
        Vector3 parentLocation = new Vector3(parent.transform.position.x, parent.transform.position.y, parent.transform.position.z);
        float distanceToCastLocation = Vector3.Distance(parentLocation, castLocation);
        if (distanceToCastLocation > FP_range)
        {
            float distPercentage = FP_range / distanceToCastLocation;
            finalLocation = Vector3.Lerp(parentLocation, castLocation, distPercentage);
        }
        else finalLocation = castLocation;
        firePillar = Instantiate(FirePillar_gameObject, finalLocation, Quaternion.identity);
        firePillar.transform.position = finalLocation; //redundant until pooling
        FirePillar FP = firePillar.GetComponent<FirePillar>();
        FP.damage_burst = FP_damage_burst;
        FP.damage_overtime = FP_damage_overtime;
        FP.duration = FP_duration;
        FP.DPS_rate = FP_DPS_rate;
    }

    public void ExitFirePillar()
    {
        tempEnemy.isInFirePillar = false;
        tempEnemy.CancelInvoke();
    }
}
