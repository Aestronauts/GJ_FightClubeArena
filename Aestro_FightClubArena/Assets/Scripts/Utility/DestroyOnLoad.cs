using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{
    public bool dontDestroyThis;

    void Awake()
    {      
        if(dontDestroyThis)
            DontDestroyOnLoad(this.gameObject);
    }
}
