using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : MonoBehaviour
{
    public static PlayerCharacterManager instance { get; private set; }

    public AbilityManager abilityManager;
    //AbilityManager abilityManager;
    public TempEnemy_Abilities tempEnemy;

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
        //abilityManager = GetComponent<AbilityManager>();
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
                abilityManager.FireboltProjectileList, abilityManager.FireBoltPrefab, abilityManager.ProjectilesHolder);
        }
        else if (abilityName == "Fire Pillar")
        {
            AbilitiesHelper.FireFirePillar(player_gameObject, castLocation, abilityManager,abilityManager.ProjectilesHolder, 
                abilityManager.FirePillarProjectileList, this);
        }
        else if (abilityName == "Twin Firebolt")
        {
            AbilitiesHelper.TwinFireProjectile(player_gameObject, castLocation, 
                abilityManager.TwinFireboltProjectileList, abilityManager.TwinFireBoltPrefab, abilityManager.ProjectilesHolder);
        }
    }

    // Takes the gameobject of the player that received damage and the damage dealt by that projectile
    // Updates the health of the player that received damage
    public void ReceivedDamage(GameObject player_gameObject, int damage)
    {
        // TODO: Updates the health of the player that received damage
    }
}
