using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public int character;
    GameSetupController gsc;
    public Text currentselectedChar;
    public string characterName;

    void Start()
    {
        gsc = FindObjectOfType<GameSetupController>();
    }

    public void ChangeCharacter()
    {
        gsc.currentSelectedPlayer = character;
        currentselectedChar.text = characterName;
    }
}
