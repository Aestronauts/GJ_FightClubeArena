using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectCard : MonoBehaviour
{
    public Image characterSprite;
    public TextMeshProUGUI titleText;
    public Outline titleCardOutline;

    public void UpdateCardData(Sprite _newCharImg, string _newTitleText, Vector2 _newOutlineAmount)
    {
        if (_newCharImg != null && characterSprite != null)
            characterSprite.sprite = _newCharImg;

        if (!string.IsNullOrEmpty(_newTitleText) && titleText != null)
            titleText.text = _newTitleText;

        if (_newOutlineAmount != null && titleCardOutline != null)
            titleCardOutline.effectDistance = _newOutlineAmount;
    }
}
