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

    private void Awake()
    {
        playerCharacterManager = FindAnyObjectByType<PlayerCharacterManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
