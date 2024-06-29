using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePillar : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;

    public Vector3 spawnLocation;
    public int damage_burst;
    public int damage_overtime;
    public float range;
    public float duration;
    public float DPS_rate;

    private void Awake()
    {
        playerCharacterManager = FindAnyObjectByType<PlayerCharacterManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyFirePillar());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    IEnumerator DestroyFirePillar()
    {
        yield return new WaitForSeconds(duration);
        playerCharacterManager.ExitFirePillar();
        Debug.Log("Fire Pillar is being destroyed, stopping TempEnemy invoke and damage.");
        Destroy(this.gameObject);
    }
}
