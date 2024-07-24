using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public Transform ProjectilesHolder; 
    
    [Header("Fire Bolt Information")]
    public GameObject FireBoltPrefab;
    [HideInInspector]public List<GameObject> FireboltProjectileList = new List<GameObject>();
    public int FireBoltDamage = 5;
    public float FireBoltRange = 15;
    
    [Header("Twin Fire Bolt Information")]
    public GameObject TwinFireBoltPrefab;
    [HideInInspector]public List<GameObject> TwinFireboltProjectileList = new List<GameObject>();
    public int TwinFireBoltDamage = 5;
    public float TwinFireBoltRange = 15;
    
    [Header("Fire Pillar Information")]
    public GameObject FirePillar_gameObject;
    [HideInInspector]public List<GameObject> FirePillarProjectileList = new List<GameObject>();
    public int FP_damage_burst = 3;
    public int FP_damage_overtime = 1;
    public int FP_range = 12;
    public float FP_duration = 3f;
    public float FP_DPS_rate = 0.75f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
