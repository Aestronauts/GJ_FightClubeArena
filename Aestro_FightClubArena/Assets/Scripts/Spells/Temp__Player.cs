using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp__Player : MonoBehaviour
{
    public Ability FireBolt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // To be replaced with a call to the Player Character Manager
            FireBolt.Activate(gameObject);
        }
    }
}
