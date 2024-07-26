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
      //  Vector3 spawnLocation = new Vector3(player_gameObject.transform.position.x+1, player_gameObject.transform.position.y, player_gameObject.transform.position.z);

        // Choose "if" logic by matching the given ability name
        // TODO: Is there a better way of doing this???
        if (abilityName == "Firebolt")
        {
            Vector3 spawnLocation = new Vector3(player_gameObject.transform.position.x+1, player_gameObject.transform.position.y, player_gameObject.transform.position.z);

            AbilitiesHelper.SpawnAbility(player_gameObject, spawnLocation,castLocation,
                abilityManager.FireboltProjectileList, abilityManager.FireBoltPrefab, 
                abilityManager.ProjectilesHolder, abilityManager.FireBoltRange, abilityManager.FireBoltDamage, this);
        }
        else if (abilityName == "Fire Pillar")
        {
            AbilitiesHelper.SpawnAbility(player_gameObject, castLocation,castLocation,
                abilityManager.FirePillarProjectileList, abilityManager.FirePillarPrefab, 
                abilityManager.ProjectilesHolder, abilityManager.FirePillarRange, abilityManager.FirePillarDamage,this);
        }
        else if (abilityName == "Twin Firebolt")
        {
            Vector3 spawnLocation = new Vector3(player_gameObject.transform.position.x+1, player_gameObject.transform.position.y+1, player_gameObject.transform.position.z);

            AbilitiesHelper.SpawnAbility(player_gameObject, spawnLocation,castLocation,
                abilityManager.TwinFireboltProjectileList, abilityManager.TwinFireBoltPrefab, 
                abilityManager.ProjectilesHolder, abilityManager.TwinFireBoltRange, abilityManager.TwinFireBoltDamage,this);
        }
    }

    // Takes the gameobject of the player that received damage and the damage dealt by that projectile
    // Updates the health of the player that received damage
    public void ReceivedDamage(GameObject player_gameObject, int damage)
    {
        // TODO: Updates the health of the player that received damage
    }
}
