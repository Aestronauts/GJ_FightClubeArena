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

    // // Start is called before the first frame update
    void Awake()
    {
     //   abilityManager = FindAnyObjectByType<AbilityManager>();
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
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("InvisWall"))
        {
            // Disable the bullet when it hits another collider
            gameObject.SetActive(false);
        }
    }
}
