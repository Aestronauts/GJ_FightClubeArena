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
    public GameObject FirePillarPrefab;
    [HideInInspector]public List<GameObject> FirePillarProjectileList = new List<GameObject>();
    public int FirePillarDamage = 3;
    public int FirePillarRange = 12;
}
