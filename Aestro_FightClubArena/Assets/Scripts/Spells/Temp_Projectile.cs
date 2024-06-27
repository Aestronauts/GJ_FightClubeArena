using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Projectile : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;

    public Vector3 spawnLocation;
    public Vector3 endLocation;
    public float projectileSpeed = 50f;
    public int damage;
    public float range;
    public float distanceTraveled = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerCharacterManager = FindAnyObjectByType<PlayerCharacterManager>();
        
        Debug.Log("Player character manager is: " + playerCharacterManager.name);
        //Debug.Log("Distance for the projectile to travel: " + Vector3.Distance(spawnLocation, endLocation));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (distanceTraveled < range && transform.position != endLocation)
        {
            //transform.Translate(endLocation * projectileSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, endLocation, projectileSpeed * Time.deltaTime);
            distanceTraveled = Vector3.Distance(spawnLocation, transform.position);
            //if (distanceTraveled > 10) Debug.Log("distance traveled has reached more than 10!");
        }

        if (distanceTraveled >= range) Debug.Log("Maximum range reached!");
        if (transform.position == endLocation) Debug.Log("Projectile reached Cast location!");

        if (distanceTraveled >= range || transform.position == endLocation)
        {
            Debug.Log("Maximum range reached OR projectile reached cast location!");
            //DelayedDestroy();
            gameObject.SetActive(false);
        }
    }

    //IEnumerator DelayedDestroy()
    //{
    //    yield return new WaitForSeconds(1f);
    //    gameObject.SetActive(false);
    //}
}
