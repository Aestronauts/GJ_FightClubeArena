using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityData
{
    [HideInInspector]public int abilityID;
    public GameObject abilityPrefab;
    
    public float projectileSpeed = 50f;
    public int damage;
    public float range;

    public int damageOvertime;
    public float duration;
    public float DPS_rate;

    public Texture2D abilityDrawing;
}
