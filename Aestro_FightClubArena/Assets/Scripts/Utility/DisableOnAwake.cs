using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    public bool disableThisObj = false;

    void Awake()
    {
        gameObject.SetActive(!disableThisObj);
    }

   
}
