using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para> Handles Player Movement, Rotation, and Animation, but requires character controller component</para>
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{

    /// <summary>
    ///  Our Public Movement Variables References
    /// </summary>
    [Space]
    [Header("MOVEMENT VARIABLES\n________________")]
    [Tooltip("The Multiplier Speed Our Character Moves By")]
    public float speedMove = 5.0f;
    [Tooltip("The Multiplier Speed Our Character Rotates By")]
    public float speedRotation = 10f;
    [Tooltip("When true, our Object To Rotate will try to look at the mouse || THIS REQUIRES THE GROUND TO BE SET TO A SPECIFIC LAYER ||. When false, they will just look forward in the direction they move")]
    public bool rotateTowardsMouse = true;
    [Tooltip("Set The Layer(s) Our Mouse Can detect for player casting and looking at")]
    public LayerMask layerForMouseDetection;

    /// <summary>
    ///  Our Public Component References
    /// </summary>
    [Space]
    [Header("COMPONENT REFERENCES\n________________")]
    [Tooltip("The Controller component we will move our character with")]
    public CharacterController _controller;
    [Tooltip("The Object we'll try to Rotate")]
    public Transform objectToRotate;
    [Tooltip("The animator we will send signals to when moveing and casting")]
    public Animator modelAnimator;
    [Tooltip("The Spawn Transform / Origin Our Abilities will come from")]
    public Transform abilitySpawnPoint;

    /// <summary>
    ///  Our Private  Movement Variables References
    /// </summary>
    [Tooltip("???")]
    private float originalY;
    [Tooltip("The movement Number recieved from using the input-axis")]
    private float moveX, moveZ;
    [Tooltip("The move direction based on our movement number")]
    private Vector3 moveDir;
    [Tooltip("The input axises we will read to get the move data")]
    private string axisHorizontal = "Horizontal", axisVertical = "Vertical";
    //[Tooltip("")]
    //public float gravity = -9.81f;
    //[Tooltip("")]
    //private Vector3 velocity = new Vector3(0, -2f,0);

    /// <summary>
    ///  Our Private Component References
    /// </summary>
    [Tooltip("The (Main) Camera we will be using to move / adjust")]
    private Camera mainCamera;


    private void Start()
    {
        // store var
        originalY = transform.position.y;

        // try to get refs
        if (!objectToRotate)
            objectToRotate = transform;
        if (Camera.main)
            mainCamera = Camera.main;
        if (!modelAnimator)
            objectToRotate.TryGetComponent<Animator>(out modelAnimator);
        if (!_controller)
            TryGetComponent<CharacterController>(out _controller);


        ErrorCheck();
    }

    private bool ErrorCheck() // debug issues and returns if we found ones
    {
        if (!modelAnimator) { Debug.LogError($"Animator Null: On Obj - {transform.name}"); return true; }
        if (!_controller) { Debug.LogError($"Controller Null: On Obj - {transform.name}"); return true; }
        if (!mainCamera) { Debug.LogError($"MainCamera Null: On Obj - {transform.name}"); return true; }
        return false;
    }


    private void Update()
    {
        if (ErrorCheck())
            return;        

        CheckForMoveInput();
        CheckForDrawInput();

        // Brett's testing camera interactions
        AbilityOne();
        AbilityTwo();
        AbilityThree();
        AbilityFour();
    }

    private void CheckForMoveInput()
    {
        moveX = Input.GetAxis(axisHorizontal);
        moveZ = Input.GetAxis(axisVertical);
        moveDir = Vector3.right * moveX + Vector3.forward * moveZ;

        if (moveDir != Vector3.zero)
        {
            UpdateMovement();
            if (rotateTowardsMouse)
                RotateToMouse();
            else
                UpdateRotationFromMove();
            UpdateAnimationTrigger("isWalking");
        }
        else
            UpdateAnimationTrigger("isIdle");
    }

    private void UpdateMovement()
    {
        if (!_controller)
            return;

        _controller.Move(moveDir * speedMove * Time.deltaTime);

        if (transform.position.y > originalY)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = originalY;
            transform.position = clampedPosition;
        }

        //if we want to jump
        //velocity.y += gravity * Time.deltaTime;
        //_controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateRotationFromMove()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        objectToRotate.rotation = Quaternion.Slerp(objectToRotate.transform.rotation, targetRotation, Time.deltaTime * speedRotation);

        if (transform.position.y > originalY)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.y = originalY;
            transform.position = clampedPosition;
        }
    }


    private void CheckForDrawInput()
    {
        //drawing look at
        if (Input.GetMouseButtonDown(0))
        {
            RotateToMouse();
            if (DrawOnScreen.instance != null) DrawOnScreen.instance.StartDrawing();

        }

        // Check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            if (DrawOnScreen.instance == null) return;

            KeyValuePair<int, Vector3> DrawResult = DrawOnScreen.instance.StopDrawing();
            if (DrawResult.Key == null)
            {
                Debug.Log("Invalid!");
            }
            else
            {
                //cast ability
                //Debug.Log("Casting Ability from PlayerInputHandler.cs");                
                PlayerCharacterManager.instance.CastAbility(GameManager.Instance.player1.gameObject, DrawResult.Value, DrawResult.Key);
            }
        }
    }


    private void RotateToMouse() //layerForMouseDetection
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        if (Physics.Raycast(ray.origin, mainCamera.transform.forward, out hit, Mathf.Infinity, layerForMouseDetection))
        {
            Debug.DrawRay(ray.origin, mainCamera.transform.forward * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

            Vector3 mouseWorldPosition = ray.GetPoint(hit.distance);

            // Calculate the direction vector from the character to the mouse position
            Vector3 direction = mouseWorldPosition - transform.position;
            direction.y = 0; // Keep the direction strictly horizontal

            // Rotate the character to face the calculated direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            objectToRotate.transform.rotation = targetRotation;
        }
    }

    private void UpdateAnimationTrigger(string _triggerString)
    {
        modelAnimator.SetTrigger(_triggerString);
    }

    #region Ability Camera Testing
    private void AbilityOne()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerCameraManager.instance.ShakeCamera(.2f, .5f, 1);
            modelAnimator.SetTrigger("isBasicAttacks");
            //AbilitiesHelper.Ability1();
        }
    }
    private void AbilityTwo()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            modelAnimator.SetTrigger("isTwinFlames");
            AbilitiesHelper.Ability2();
            StartCoroutine(TwinFlamesFX());
        }
    }

    private void AbilityThree()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            modelAnimator.SetTrigger("isFirePillar");
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
    #endregion Ability Camera Testing



}
