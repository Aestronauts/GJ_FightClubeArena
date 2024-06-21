using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ability_FireBolt : Ability
{
    public override void Activate(GameObject parent)
    {
        //Debug.Log("Ability_FireBolt.cs has fired a fire bolt!");

        GameObject fireBolt = Instantiate(model, new Vector3(parent.transform.position.x + 1, parent.transform.position.y, parent.transform.position.z), Quaternion.identity);
        Temp_Projectile projectile = fireBolt.GetComponent<Temp_Projectile>();
        projectile.damage = damage;
        Debug.Log("Damage value set from Ability_FireBolt: " + damage);
    }
}
