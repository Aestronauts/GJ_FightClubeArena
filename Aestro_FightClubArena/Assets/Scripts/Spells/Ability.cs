using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string name;
    public GameObject model;
    public int damage;

    public enum AbilityType { Projectile, AOE };
    public AbilityType abilityType;

    public virtual void Activate(GameObject parent) { }
}
