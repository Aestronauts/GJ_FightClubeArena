using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Transform ProjectilesHolder; 
    
    [Header("Fire Bolt Information")]
    public GameObject FireBoltPrefab;
    [HideInInspector]public List<GameObject> ProjectileList = new List<GameObject>();
    public int FireBoltDamage = 5;
    public float FireBoltRange = 15;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
