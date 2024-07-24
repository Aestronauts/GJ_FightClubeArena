using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Projectile : MonoBehaviour
{
    //public PlayerCharacterManager playerCharacterManager;
    public AbilityManager abilityManager;

    public Vector3 spawnLocation;
    public Vector3 endLocation;
    public float projectileSpeed = 50f;
    public int damage;
    public float range;
    public float distanceTraveled = 0f;

    // // Start is called before the first frame update
    void Awake()
    {
        //if(!playerCharacterManager)
        //    TryGetComponent<PlayerCharacterManager>(out playerCharacterManager);
        abilityManager = FindAnyObjectByType<AbilityManager>();
    
        //if(playerCharacterManager)
        //    Debug.Log("Player character manager is: " + playerCharacterManager.name);
        //Debug.Log("Distance for the projectile to travel: " + Vector3.Distance(spawnLocation, endLocation));
    }

    // // Update is called once per frame
    void FixedUpdate()
    {
        if (distanceTraveled < range && transform.position != endLocation)
        {
            transform.position = Vector3.MoveTowards(transform.position, endLocation, projectileSpeed * Time.deltaTime);
            distanceTraveled = Vector3.Distance(spawnLocation, transform.position);
        }
    
        if (distanceTraveled >= range || transform.position == endLocation)
        {
            gameObject.SetActive(false);
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
