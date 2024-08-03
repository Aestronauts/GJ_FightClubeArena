using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{

    public int startingHealth = 100;
    public Image healthImage;
    public int currentPlayerHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        currentPlayerHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float fillAmount = (float)currentPlayerHealth / startingHealth;
        healthImage.fillAmount = fillAmount;

        //yay reset for free, we never die!
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = startingHealth;
        }
    }
}
