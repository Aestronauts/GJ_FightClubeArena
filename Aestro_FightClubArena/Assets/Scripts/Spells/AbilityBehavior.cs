using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityBehavior : MonoBehaviour //NetworkBehavior
{
   // public AbilityManager abilityManager;

    public AbilityData abilityParameters;

    [HideInInspector]public Vector3 spawnLocation;
    [HideInInspector]public Vector3 endLocation;
    [HideInInspector]public float distanceTraveled = 0f;

    [HideInInspector]public int abilityID;

    public GameObject DamageUIPrefab;

    private GameObject instantiatedObjectDamageCanvas = null;
    private GameObject instantiatedObjectDamageOvertimeCanvas = null;
    
    private bool dpsTickWaiting;
    
    private Coroutine triggerStayCoroutine;

    private NetworkObject casterNetworkObj;
    private int casterObjID;
    private GameObject instantiator;

    // // Start is called before the first frame update
    void Awake()
    {
    }

    // // Update is called once per frame
    void FixedUpdate()
    {
        if (AbilityManager.instance.abilitiesList[abilityID].duration == 0)
        {
            if (distanceTraveled < abilityParameters.range && transform.position != endLocation)
            {
                transform.position = Vector3.MoveTowards(transform.position, endLocation, abilityParameters.projectileSpeed * Time.deltaTime);
                distanceTraveled = Vector3.Distance(spawnLocation, transform.position);
            }
    
            if (distanceTraveled >= abilityParameters.range || transform.position == endLocation)
            {
                gameObject.SetActive(false);
            }
        }

        if (!casterNetworkObj)
        {
            instantiator.TryGetComponent<NetworkObject>(out casterNetworkObj);
            if(casterNetworkObj)
                casterObjID = (int)casterNetworkObj.NetworkObjectId;
        }
    }
    
    //this collision check is for environment objects mostly, if it hits, disable the projectile
    //if its not a projectile i.e non-0 duration, it stays active until the duration is up
    //(handled in the AbilitiesHelper script)
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("InvisWall"))
        {
            // Disable the bullet when it hits another collider
            if (AbilityManager.instance.abilitiesList[abilityID].duration == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    
    //check if the ability cast has collided with a player, if it has
    //spawn the damage ui canvas relevant to the ability that this is
    //we have a check distinction between duration lengths because if its a projectile,
    //we disable it on collision, if its a long range cast, it stays active until the duration is up
    void OnTriggerEnter(Collider other)
    {
        // This method is called when another collider enters the trigger collider
        Debug.Log("Entered trigger with: " + other.gameObject.name);

        // You can check for specific collision types, tags, etc.
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>() != null)
        {
            if ((int)other.gameObject.GetComponent<NetworkObject>().NetworkObjectId != casterObjID)
            {
                if (AbilityManager.instance.abilitiesList[abilityID].duration != 0)
                {
                    instantiatedObjectDamageCanvas = Instantiate(DamageUIPrefab, transform.position,
                        Quaternion.identity,
                        AbilityManager.instance.ProjectilesHolder);
                    instantiatedObjectDamageCanvas.GetComponent<DamageUICanvas>().abilityDamage =
                        AbilityManager.instance.abilitiesList[abilityID].damage;
                    other.gameObject.GetComponent<PlayerHealthManager>().currentPlayerHealth -=
                        AbilityManager.instance.abilitiesList[abilityID].damage;
                }

                if (AbilityManager.instance.abilitiesList[abilityID].duration == 0)
                {
                    instantiatedObjectDamageCanvas = Instantiate(DamageUIPrefab, transform.position,
                        Quaternion.identity, AbilityManager.instance.ProjectilesHolder);
                    instantiatedObjectDamageCanvas.GetComponent<DamageUICanvas>().abilityDamage =
                        AbilityManager.instance.abilitiesList[abilityID].damage;
                    other.gameObject.GetComponent<PlayerHealthManager>().currentPlayerHealth -=
                        AbilityManager.instance.abilitiesList[abilityID].damage;
                    gameObject.SetActive(false);
                }
            }
        }
    }
    
    //check if the player is standing in a damage collision area, if it is
    //and the ability cast has a damage over time rate, do it
    void OnTriggerStay(Collider other)
    {
        // This method is called as long as another collider stays within the trigger collider
        Debug.Log("Staying in trigger with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>() != null)
        {
            if ((int)other.gameObject.GetComponent<NetworkObject>().NetworkObjectId != casterObjID)
            {
                if (!dpsTickWaiting)
                {
                    if (AbilityManager.instance.abilitiesList[abilityID].DPS_rate != 0)
                    {
                        triggerStayCoroutine = StartCoroutine(TriggerStayDPS(other.gameObject));
                    }
                }
            }
        }
    }

    //if we leave the damage overtime collision area, set dps tick waiting to false so
    //if we re-enter it, we can take damage overtime again
    void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
        dpsTickWaiting = false;
    }
    
    //make sure we set the dps tick waiting to false when we enable the
    //object so it can spawn damage over time when its turned on
    void OnEnable()
    {
        dpsTickWaiting = false;
    }
    
    //make sure we set the dps tick waiting to false when we disable the
    //object so it can spawn damage over time when its turned on again
    void OnDisable()
    {        
        if (triggerStayCoroutine != null)
        {
            StopCoroutine(triggerStayCoroutine);
        }
        casterNetworkObj = null;
        dpsTickWaiting = false;
    }

    //call this coroutine to repeatedly spawn damage ui canvases for damage overtime. only do it
    //if there is a damage to be applied i.e not 0
    private IEnumerator TriggerStayDPS(GameObject collidingObject)
    {
        dpsTickWaiting = true;
        while (true)
        {
            //spawn damage number for non distance traveling attacks
            if (AbilityManager.instance.abilitiesList[abilityID].damageOvertime != 0)
            {
                instantiatedObjectDamageOvertimeCanvas = Instantiate(DamageUIPrefab, transform.position, Quaternion.identity, AbilityManager.instance.ProjectilesHolder);
                instantiatedObjectDamageOvertimeCanvas.GetComponent<DamageUICanvas>().abilityDamage =
                    AbilityManager.instance.abilitiesList[abilityID].damageOvertime;
                collidingObject.GetComponent<PlayerHealthManager>().currentPlayerHealth -=
                    AbilityManager.instance.abilitiesList[abilityID].damageOvertime;
            }

            yield return new WaitForSeconds(AbilityManager.instance.abilitiesList[abilityID].DPS_rate);
        }
    }
    
    public void Initialize(GameObject instantiator)
    {
        this.instantiator = instantiator;
        Debug.Log("Instantiated by: " + instantiator.name);
    }
}
