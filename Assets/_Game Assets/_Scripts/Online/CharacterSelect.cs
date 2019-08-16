using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [Header("Stats")]
    public int character;
    public int characterHP;
    public string characterName;
    public string characterSpeed;

    [Header("References")]
    GameSetupController gsc;
    public Button playButton;
    public Text currentselectedChar;
    public Text healthText;
    public Text speedText;

    void Start()
    {
        gsc = FindObjectOfType<GameSetupController>();
        currentselectedChar.text = "None";
    }

    public void ChangeCharacter()
    {
        playButton.interactable = true;
        gsc.currentSelectedPlayer = character;
        currentselectedChar.text = characterName;
        healthText.text = characterHP.ToString();
        speedText.text = characterSpeed;
    }
}
