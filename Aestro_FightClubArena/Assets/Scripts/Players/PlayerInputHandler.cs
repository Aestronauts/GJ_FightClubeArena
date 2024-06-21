using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : NetworkBehaviour
{
    public float speed = 5.0f;
    public float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 velocity;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (!IsOwner) // requires NetworkBehavior to check if this is you (the player)
            //return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        _controller.Move(move * speed * Time.deltaTime);

        //if we want to jump
        //velocity.y += gravity * Time.deltaTime;
        //_controller.Move(velocity * Time.deltaTime);

        //will be moved to the Player character manager for networking

        AbilityOne();
        AbilityTwo();
        AbilityThree();
        AbilityFour();
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerCameraManager.instance.ShakeCamera(.5f, 2, 2);
            Debug.Log("Camera Shake attempt");
        }
    }

    private void AbilityOne()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerCameraManager.instance.ShakeCamera(.4f, .8f, .5f);
            AbilitiesHelper.Ability1();
        }
    }
    private void AbilityTwo()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AbilitiesHelper.Ability2();
        }
    }

    private void AbilityThree()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AbilitiesHelper.Ability3();
        }
    }

    private void AbilityFour()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AbilitiesHelper.Ability4();
        }
    }





}
