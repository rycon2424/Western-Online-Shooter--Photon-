using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInstance : MonoBehaviour
{
    public static MenuInstance instance;

    [SerializeField]
    private GameObject lobbyPanel; //panel for displaying lobby.
    [SerializeField]
    private Animator anim;

    void Awake()
    {
        instance = this;
    }

    public void GoToMenu()
    {
        anim.SetTrigger("Menu");
    }

    public void EnableLobbyPannel()
    {
        lobbyPanel.SetActive(true);
    }
}
