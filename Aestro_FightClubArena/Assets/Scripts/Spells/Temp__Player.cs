using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp__Player : MonoBehaviour
{
    public PlayerCharacterManager playerCharacterManager;
    Camera mainCamera;

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
        // Activates Fire Bolt Ability (left-click)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3? castLocation = GetCastLocation();
            if (castLocation != null)
            {
                Vector3 CastLocation = castLocation.GetValueOrDefault();
                CastLocation = new Vector3(CastLocation.x, 0.5f, CastLocation.z);
                //playerCharacterManager.CastAbility(gameObject, CastLocation,0);
            }

            ////OLD CODE:
            //Debug.Log("Mouse Clicked!");
            //Vector3 mousePosition = Input.mousePosition;
            //Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            //if (Physics.Raycast(ray, out RaycastHit hit))
            //{
            //    Debug.Log("Impact point in world space where the ray hit the collider: " + hit.point);
            //    Vector3 castLocation = hit.point;
            //    //Add a y-axis value to the Cast Location so that the projectile doesn't aim towards the ground
            //    castLocation = new Vector3(castLocation.x, 0.5f, castLocation.z);
            //    playerCharacterManager.CastAbility(gameObject, "Fire Bolt", castLocation);
            //}
        }

        // Activates Fire Pillar Ability (right-click)
        if (Input.GetMouseButtonDown(1))
        {
            Vector3? castLocation = GetCastLocation();
            if (castLocation != null)
            {
                Vector3 CastLocation = castLocation.GetValueOrDefault();
                CastLocation = new Vector3(CastLocation.x, 0.01f, CastLocation.z);
              //  playerCharacterManager.CastAbility(gameObject, CastLocation,1);
            }
        }
    }

    private Vector3? GetCastLocation()
    {
        Vector3? castLocation = null;
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) castLocation = hit.point;
        return castLocation;
    }
}
