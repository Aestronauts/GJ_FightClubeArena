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
    public static PlayerPracticeManager Instance { get; private set; }

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
    [Tooltip("The Different UI Objects under our canvas that hold relevant data to selecting a map")]
    public Transform obj_UI_Holder_MapSelect, obj_UI_Holder_MapOptions, obj_UI_Holder_MapVisual;
    [Tooltip("The map icons that we can select from during character select")]
    public List<Outline> mapOptionOutline = new List<Outline>();
    // the map option we have selected
    private int mapSelectId = 0;
    #endregion var: map select


    #region Variables: Model References
    [Tooltip("The list of player models / characters that should be organized to match the character selection icons / cards. Each character's selection during character selection will then be used to pass the data to another script as a new list")]
    public List<Transform> playerModelsByOrder = new List<Transform>();
    [Tooltip("The list of map models that should be organized to match the map selection icons / cards. The map selection will then be used to pass the data to another script as a new list")]
    public List<Transform> mapModelsByOrder = new List<Transform>();
    // the list we will populate as we selected characters
    private List<Transform> storeSelectedCharacters = new List<Transform>();
    // the map we will store as we select our map
    private Transform storeSelectedMap;
    #endregion var: model references

    public void Awake()
    {
        if (PlayerPracticeManager.Instance != null && PlayerPracticeManager.Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (obj_UI_Holder_CharSelect.gameObject.activeSelf == true)
                ChangeCharacterSelectId(charSelectId + 1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(obj_UI_Holder_CharSelect.gameObject.activeSelf == true)
                ChangeCharacterSelectId(charSelectId - 1);
        }
    }

    public void SelectedPracticeMode()
    {
        selectedGamemode = GAMEMODE.Practice;
        obj_UI_Holder_CharSelect.gameObject.SetActive(true); // turns on character select
        ChangeCharacterSelectId(0); // sets character select ID to be 0 / first available option
    }
  

    public  void CallPlayerManager(List<Transform> _charactersToSpawn, Transform _mapToSpawn)
    {
        GameManager.Instance.SetMatchStartData(_charactersToSpawn, _mapToSpawn);
    }


    #region Functions: Character Select

    public void ChangeCharacterSelectByButton(Button pressedButton)
    {
        if (obj_UI_Holder_CharSelect.gameObject.activeSelf == false)
            return;

        for (int i = 0; i < characterOptionOutline.Count; i++)
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
    
    public void ReadCharacterSelectionData()
    {
        if (playerModelsByOrder.Count == 0)
            return;

        List<Transform> charactersSelected = new List<Transform>();

        // loop through the different cards we have spawned for character select        
        foreach (CharacterSelectCard _charSelCard in charCardsList)
        {
            int charId = 0;
            foreach (Sprite _iconAvailable in characterVisIcons)
            {
                if (_charSelCard.characterSprite.sprite == _iconAvailable)
                {
                    charactersSelected.Add(playerModelsByOrder[charId]);
                    break;
                }
                else
                    charId++;
            }

        }
        storeSelectedCharacters = charactersSelected;
        obj_UI_Holder_CharSelect.gameObject.SetActive(false); // turns off character select        
        CallPlayerManager(storeSelectedCharacters, mapModelsByOrder[0]); // take this out
    }

    #endregion func: character select

    #region Functions: Map Select

    public void OpenMapSelect()
    {
        // open up the map scene
        obj_UI_Holder_MapOptions.gameObject.SetActive(true);
        // start selection
    }

    public void ChangeMapSelectId(int _newSelectionId)
    {
        if (mapOptionOutline.Count == 0 || characterVisIcons.Count == 0)
            return;

        characterOptionOutline[charSelectId].GetComponent<Outline>().effectDistance = Vector2.zero;

        charSelectId = _newSelectionId;

        if (charSelectId > characterOptionOutline.Count - 1 || charSelectId > characterVisIcons.Count - 1)
            charSelectId = 0;
        if (charSelectId < 0)
            charSelectId = characterOptionOutline.Count - 1;
        // because ZERO = training dummy, we dont want to let the player select that
        if (charSelectId == 0)
            charSelectId++;

        characterOptionOutline[charSelectId].GetComponent<Outline>().effectDistance = outlineSelection;
        UpdateAllCardData();
    }

    #endregion fun: map select

}// end of PlayerPracticeManager class
