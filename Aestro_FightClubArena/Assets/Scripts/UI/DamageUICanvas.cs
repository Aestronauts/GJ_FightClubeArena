using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class DamageUICanvas : MonoBehaviour
{
    private Transform mainCameraTransform;
    private Canvas canvas;
    public TMP_Text damageNumberText;
    
    public float destroyAfterSeconds = 2f;
    public float fontSize = 10;
    public float endFontSize = 2;
    public float targetTextDistance = 1f;

    private RectTransform textTransform;

    [HideInInspector]public int abilityDamage;

    public bool isDamageCanvas = true;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
        
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        textTransform = GetComponent<RectTransform>();

        if (isDamageCanvas)
        {
            damageNumberText.text = abilityDamage.ToString();
            damageNumberText.fontSize = fontSize;
        
            StartCoroutine(IncreaseFontSize());
            Destroy(gameObject, destroyAfterSeconds);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }
    
    private IEnumerator IncreaseFontSize()
    {
        float initialYPosition = textTransform.anchoredPosition.y;

        float elapsed = 0f;

        while (elapsed < destroyAfterSeconds)
        {
            elapsed += Time.deltaTime;
            damageNumberText.fontSize = Mathf.Lerp(fontSize, endFontSize, elapsed / destroyAfterSeconds);
            textTransform.anchoredPosition = new Vector2(textTransform.anchoredPosition.x, Mathf.Lerp(initialYPosition, targetTextDistance, elapsed / destroyAfterSeconds));
            yield return null;
        }

        // Ensure the final values are set
        damageNumberText.fontSize = endFontSize;
        textTransform.anchoredPosition = new Vector2(textTransform.anchoredPosition.x, targetTextDistance);

    }
}
