using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        _controller.Move(move * speed * Time.deltaTime);

        //if we want to jump
        //velocity.y += gravity * Time.deltaTime;
        //_controller.Move(velocity * Time.deltaTime);

        //will be moved to the Player character manager for networking
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AbilitiesHelper.Ability1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AbilitiesHelper.Ability2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AbilitiesHelper.Ability3();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AbilitiesHelper.Ability4();
        }

    }
}
