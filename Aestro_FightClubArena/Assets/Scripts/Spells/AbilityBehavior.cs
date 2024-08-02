using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBehavior : MonoBehaviour
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
                instantiatedObjectDamageCanvas = Instantiate(DamageUIPrefab, transform.position, Quaternion.identity, AbilityManager.instance.ProjectilesHolder);
                instantiatedObjectDamageCanvas.GetComponent<DamageUICanvas>().abilityDamage = AbilityManager.instance.abilitiesList[abilityID].damage;
                gameObject.SetActive(false);
            }
        }

        if (instantiatedObjectDamageOvertimeCanvas == null)
        {
            //spawn damage number for non distance traveling attacks
            if (AbilityManager.instance.abilitiesList[abilityID].damageOvertime != 0)
            {
                instantiatedObjectDamageOvertimeCanvas = Instantiate(DamageUIPrefab, transform.position, Quaternion.identity, AbilityManager.instance.ProjectilesHolder);
                instantiatedObjectDamageOvertimeCanvas.GetComponent<DamageUICanvas>().abilityDamage =
                    AbilityManager.instance.abilitiesList[abilityID].damageOvertime;
            }
        }

        if (instantiatedObjectDamageCanvas == null)
        {
            if (AbilityManager.instance.abilitiesList[abilityID].duration != 0)
            {
                instantiatedObjectDamageCanvas = Instantiate(DamageUIPrefab, transform.position, Quaternion.identity,
                    AbilityManager.instance.ProjectilesHolder);
                instantiatedObjectDamageCanvas.GetComponent<DamageUICanvas>().abilityDamage =
                    AbilityManager.instance.abilitiesList[abilityID].damage;
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("InvisWall"))
        {
            instantiatedObjectDamageCanvas = Instantiate(DamageUIPrefab, transform.position, Quaternion.identity, AbilityManager.instance.ProjectilesHolder);
            instantiatedObjectDamageCanvas.GetComponent<DamageUICanvas>().abilityDamage = AbilityManager.instance.abilitiesList[abilityID].damage;
            // Disable the bullet when it hits another collider
            gameObject.SetActive(false);
        }
    }
}
