using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp__Player : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;
    Camera mainCamera;
    public Ability FireBolt;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // To be replaced with a call to the Player Character Manager
            //FireBolt.Activate(gameObject);
        }

        // Only activates Fire Bolt Ability for now...
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked!");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Debug.Log("Ray Casted from Camera. Raycast hit info: " + hit.transform);
                //Debug.Log("Here's that hit info's position: " + hit.transform.position);
                //Debug.Log("Ray's distance from origin to impact point: " + hit.distance);

                //This one is End Position Data!!! Uses ground plane, so y position value will == 0.
                Debug.Log("Impact point in world space where the ray hit the collider: " + hit.point);
                Vector3 castLocation = hit.point;
                //Add a y-axis value to the Cast Location so that the projectile doesn't aim towards the ground
                castLocation = new Vector3(castLocation.x, 0.5f, castLocation.z);
                //FireBolt.Activate(gameObject, castLocation);
                playerCharacterManager.CastAbility(gameObject, "Fire Bolt", castLocation);
            }
        }
    }
}
