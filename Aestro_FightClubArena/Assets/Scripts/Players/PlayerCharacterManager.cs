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
    public void CastAbility(GameObject player_gameObject, Vector3 castLocation, int abilityID)
    {
        Transform spawnPoint = player_gameObject.GetComponent<PlayerInputHandler>().abilitySpawnPoint;
        //Animator animator = player_gameObject.GetComponent<Animator>();
        Animator animator = player_gameObject.GetComponent<PlayerInputHandler>().modelAnimator;
        if (animator == null) { Debug.LogError("Animator not found on PlayerCharacterManager"); }
        // Choose "if" logic by matching the given ability name
        // TODO: Is there a better way of doing this???
        if (abilityID == 0)
        {
            AbilitiesHelper.SpawnAbility(player_gameObject, spawnPoint.position,castLocation,
                abilityManager.FireboltProjectileList,  
                abilityManager.ProjectilesHolder, this,abilityManager.abilitiesList,abilityID);
            animator.SetTrigger("isBasicAttacks");
                
        }
        else if (abilityID == 1)
        {
            AbilitiesHelper.SpawnAbility(player_gameObject, castLocation,castLocation,
                abilityManager.FirePillarProjectileList,
                abilityManager.ProjectilesHolder,this,abilityManager.abilitiesList,abilityID);
            animator.SetTrigger("isFirePillar");
        }
        else if (abilityID == 2)
        {
            AbilitiesHelper.SpawnAbility(player_gameObject, spawnPoint.position,castLocation,
                abilityManager.TwinFireboltProjectileList,
                abilityManager.ProjectilesHolder, this,abilityManager.abilitiesList,abilityID);
            animator.SetTrigger("isTwinsFlames");
        }
    }

    // Takes the gameobject of the player that received damage and the damage dealt by that projectile
    // Updates the health of the player that received damage
    public void ReceivedDamage(GameObject player_gameObject, int damage)
    {
        // TODO: Updates the health of the player that received damage
    }
}
