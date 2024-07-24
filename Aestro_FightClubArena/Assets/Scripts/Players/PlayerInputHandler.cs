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
    public Animator _mageAnimator;
    private Vector3 velocity;
    
    private float originalY;
    
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        originalY = transform.position.y;
        mainCamera = Camera.main;

        _mageAnimator = mage.GetComponent<Animator>();
        if (_mageAnimator == null) Debug.LogError("Mage Animator Null");
    }

    // Update is called once per frame
    void Update()
    {       

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            mage.transform.rotation = Quaternion.Slerp(mage.transform.rotation, targetRotation, Time.deltaTime * 10f);
            _mageAnimator.SetTrigger("isWalking");
            Debug.Log("Walking"); 
        }
        else
        {
            _mageAnimator.SetTrigger("isIdle");
           // Debug.Log("Idle");

        }

        _controller.Move(move * speed * Time.deltaTime);
        
        velocity.y = -2f;
        
        if (transform.position.y > originalY)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = originalY;
            transform.position = clampedPosition;
        }

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
            RotateToMouse();
            if(DrawOnScreen.instance != null) DrawOnScreen.instance.StartDrawing();

        }

        // Check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            if (DrawOnScreen.instance == null) return;
            
            KeyValuePair<string, Vector3> DrawResult = DrawOnScreen.instance.StopDrawing();
            if (DrawResult.Key == null)
            {
                Debug.Log("Invalid!");
            }
            else
            {
                //cast ability
                //Debug.Log("Casting Ability from PlayerInputHandler.cs");
                //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //g.transform.position = DrawResult.Value;
                PlayerCharacterManager.instance.CastAbility(GameManager.Instance.player1.gameObject, DrawResult.Key, DrawResult.Value);
            }
        }
    }

    private void AbilityOne()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerCameraManager.instance.ShakeCamera(.2f, .5f, 1);
            _mageAnimator.SetTrigger("isBasicAttacks");
            //AbilitiesHelper.Ability1();
        }
    }
    private void AbilityTwo()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _mageAnimator.SetTrigger("isTwinFlames");
            AbilitiesHelper.Ability2();
            StartCoroutine(TwinFlamesFX());
        }
    }

    private void AbilityThree()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _mageAnimator.SetTrigger("isFirePillar");
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
    
    void RotateToMouse()
    {
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Convert the screen position to a point in the world
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);

            // Calculate the direction vector from the character to the mouse position
            Vector3 direction = mouseWorldPosition - transform.position;
            direction.y = 0; // Keep the direction strictly horizontal

            // Rotate the character to face the calculated direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            mage.transform.rotation = targetRotation;
        }
    }





}
