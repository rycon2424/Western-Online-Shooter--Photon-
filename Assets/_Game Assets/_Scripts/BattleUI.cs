using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class BattleUI : MonoBehaviourPun
{
    public Text uitext;
    public Text timeText;
    private PhotonView pv;
    public string savedString;
    public GameObject changemap;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void UpdateBattleLog(string weapon, string killer , string player)
    {
        savedString += killer + " " + weapon + " " + player + " to death \n";
        pv.RPC("SyncChatToMaster", RpcTarget.MasterClient, savedString, false);
    }
    
    [PunRPC]
    void SyncChatToMaster(string stringtoSync, bool add)
    {
        pv.RPC("SyncChatToClients", RpcTarget.AllViaServer, stringtoSync, add);
    }

    [PunRPC]
    void SyncChatToClients(string stringtoSync, bool add)
    {
        if (add == true)
        {
            uitext.text += stringtoSync;
        }
        else
        {
            uitext.text = stringtoSync;
        }
        savedString = uitext.text;
    }
    
    public void JoinLeaveGame(string player, bool joining)
    {
        string stringToAdd;
        if (joining == true)
        {
            stringToAdd = player + " has joined the game \n";
        }
        else
        {
            stringToAdd = player + " has left the game \n";
            Invoke("Leave", 0.3f);
        }
        pv.RPC("SyncChatToMaster", RpcTarget.MasterClient, stringToAdd, true);
    }

    void Leave()
    {
        Application.Quit();
    }

    public void UpdateTimer(string timer)
    {
        timeText.text = timer;
        Debug.Log("Sended " + timer);
        pv.RPC("UpdateForEveryone", RpcTarget.Others, timer);
    }

    [PunRPC]
    void UpdateForEveryone(string timerT)
    {
        timeText.text = timerT;
        Debug.Log("received " + timerT);
    }

    public void EndGame()
    {
        GameSetupController.gameEnded = true;
        changemap.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pv.RPC("EndGameForAll", RpcTarget.Others);
    }

    [PunRPC]
    void EndGameForAll()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameSetupController.gameEnded = true;
    }

    //Button
    public void ChangeMap(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

}
