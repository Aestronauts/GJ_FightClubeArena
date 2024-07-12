using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{
    // local variables
    public float speed = 5.0f;
    public float gravity = -9.81f;
    public GameObject mage;

    private CharacterController _controller;
    private Vector3 velocity;

    
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {       

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        Quaternion targetRotation = Quaternion.LookRotation(move);
        mage.transform.rotation = targetRotation;

        _controller.Move(move * speed * Time.deltaTime);

        //if we want to jump
        //velocity.y += gravity * Time.deltaTime;
        //_controller.Move(velocity * Time.deltaTime);

        //will be moved to the Player character manager for networking

        AbilityOne();
        AbilityTwo();
        AbilityThree();
        AbilityFour();
        //drawing

        if (Input.GetMouseButtonDown(0))
        {
            DrawOnScreen.instance.StartDrawing();
        }

        // Check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            KeyValuePair<string, Vector3> DrawResult = DrawOnScreen.instance.StopDrawing();
            if (DrawResult.Key == null)
            {
                Debug.Log("Invalid!");
            }
            else
            {
                //cast ability
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = DrawResult.Value;
            }
        }
    }

    private void AbilityOne()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerCameraManager.instance.ShakeCamera(.2f, .5f, 1);
            AbilitiesHelper.Ability1();
        }
    }
    private void AbilityTwo()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AbilitiesHelper.Ability2();
            StartCoroutine(TwinFlamesFX());
        }
    }

    private void AbilityThree()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            AbilitiesHelper.Ability3();
            PlayerCameraManager.instance.ShakeCamera(1f, .5f, .6f);

        }
    }

    private void AbilityFour()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AbilitiesHelper.Ability4();
        }
    }

    IEnumerator TwinFlamesFX()
    {
        //Shakes camera, pauses, shakes again 
        PlayerCameraManager.instance.ShakeCamera(.2f, .5f, 1);
        yield return new WaitForSeconds(.5f);
        PlayerCameraManager.instance.ShakeCamera(.2f, .5f, 1);
    }





}
