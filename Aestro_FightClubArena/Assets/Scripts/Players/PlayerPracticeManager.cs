using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// <para>
/// Manages the different elements for entering practice mode as a local player. It populates ui characters and maps, manages selections from those UI, and spawns those models in-game
/// </para>
/// </summary>
public class PlayerPracticeManager : MonoBehaviour
{
    public enum GAMEMODE {Practice, MultiplayerOnline, };
    public GAMEMODE selectedGamemode;
    public int numberOfPlayers = 2;
    private int localPlayerID = 0;

    #region Variables: Character Select
    [Tooltip("The Different UI Objects under our canvas that hold relevant data to selecting characters")]
    public Transform obj_UI_Holder_CharSelect, obj_UI_Holder_CharOptions, obj_UI_Holder_CharVisEven, obj_UI_Holder_CharVisOdds;
    [Tooltip("The images of the different character artworks that will show when the linked Icon is selected in Character Options")]
    public List<Sprite> characterVisIcons = new List<Sprite>();
    [Tooltip("The character icons that we can select from during character select")]
    public List<Outline> characterOptionOutline = new List<Outline>();
    // the character option we have selected
    private int charSelectId = 0;
    // a reference to the player card template we will duplicate in the ui (must be in-scene to be reasonably easy)
    private CharacterSelectCard playerCardTemplate;
    // the character cards
    private List<CharacterSelectCard> charCardsList = new List<CharacterSelectCard>();
    // the outline amount we want for selected icons and ui
    private Vector2 outlineSelection = new Vector2(15, -15);
    #endregion var: character select

    #region Variables: Map Select
    #endregion var: map select

    #region Variables: Model References
    #endregion var: model references

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeCharacterSelectId(charSelectId + 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeCharacterSelectId(charSelectId - 1);
    }

    public void SelectedPracticeMode()
    {
        selectedGamemode = GAMEMODE.Practice;
        obj_UI_Holder_CharSelect.gameObject.SetActive(true);
        ChangeCharacterSelectId(0);
    }

    #region Functions: Character Select

    public void ChangeCharacterSelectByButton(Button pressedButton)
    {
        for(int i = 0; i < characterOptionOutline.Count; i++)
        {
            if (pressedButton.transform == characterOptionOutline[i].transform)
            {
                ChangeCharacterSelectId(i);
                break;
            }
        }
    }

    public void ChangeCharacterSelectId(int _newSelectionId)
    {
        if (characterOptionOutline.Count == 0 || characterVisIcons.Count == 0)
            return;

        characterOptionOutline[charSelectId].GetComponent<Outline>().effectDistance = Vector2.zero;            

        charSelectId = _newSelectionId;

        if (charSelectId > characterOptionOutline.Count-1 || charSelectId > characterVisIcons.Count-1)
            charSelectId = 0;
        if (charSelectId < 0)
            charSelectId = characterOptionOutline.Count-1;
        // because ZERO = training dummy, we dont want to let the player select that
        if (charSelectId == 0)
            charSelectId++;

        characterOptionOutline[charSelectId].GetComponent<Outline>().effectDistance = outlineSelection;
        UpdateAllCardData();
    }

    public void UpdateAllCardData()
    {
        if (!playerCardTemplate)
        {
            obj_UI_Holder_CharVisEven.transform.GetChild(0).TryGetComponent<CharacterSelectCard>(out playerCardTemplate);
            if (playerCardTemplate == null)
                return;
        }


        for(int i = 0; i < numberOfPlayers; i++)
        {
            Transform uiParent = null;

            if (i % 2 == 0)// even
                uiParent = obj_UI_Holder_CharVisEven;
            else // odd
                uiParent = obj_UI_Holder_CharVisOdds;

            // if we can pool the data, we do, otherwise spawn new cards
            Transform charCardClone = null;
            if (i <= charCardsList.Count - 1)
                charCardClone = charCardsList[i].transform;
            else
                charCardClone = Instantiate(playerCardTemplate.transform, uiParent);
            // ref the data script
            CharacterSelectCard cardData = null;
            charCardClone.TryGetComponent<CharacterSelectCard>(out cardData);
            if (!cardData)
                continue;
            // add the card to our list
            charCardsList.Add(cardData);
            // make it visible
            charCardClone.gameObject.SetActive(true);
            // if it is our card we populate it with our data
            if (i == localPlayerID)
                cardData.UpdateCardData(characterVisIcons[charSelectId], "Human Player", outlineSelection);
            else // if it's not us
            {
                if(selectedGamemode == GAMEMODE.Practice)// if it is practice mode -> all other players are bots
                {
                    cardData.UpdateCardData(characterVisIcons[0], "Bot Dummy", Vector2.zero);
                }
            }
            
        }
    }


    #endregion func: character select

}// end of PlayerPracticeManager class
