using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Projectile : MonoBehaviour
{
    public float projectileSpeed = 50f;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.right * projectileSpeed * Time.deltaTime);
    }
}
