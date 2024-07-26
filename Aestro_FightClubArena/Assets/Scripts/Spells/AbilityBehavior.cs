using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBehavior : MonoBehaviour
{
    public AbilityManager abilityManager;

    public Vector3 spawnLocation;
    public Vector3 endLocation;
    public float projectileSpeed = 50f;
    public int damage;
    public float range;
    public float distanceTraveled = 0f;
    
    public int damageOvertime;
    public float duration;
    public float DPS_rate;

    // // Start is called before the first frame update
    void Awake()
    {
        abilityManager = FindAnyObjectByType<AbilityManager>();
    }

    // // Update is called once per frame
    void FixedUpdate()
    {
        if (duration == 0)
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
