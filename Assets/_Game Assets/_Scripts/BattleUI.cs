using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleUI : MonoBehaviourPun
{
    public Text uitext;
    private PhotonView pv;
    string savedString;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {

    }

    public void UpdateBattleLog(string weapon, string killer , string player)
    {
        savedString += killer + " " + weapon + " " + player + " to death \n";
        pv.RPC("SyncKillFeed", RpcTarget.All, savedString);
    }

    public void JoinLeaveGame(string player, bool joining)
    {
        if (joining == true)
        {
            savedString += player + " has joined the game \n";
        }
        else
        {
            savedString += player + " has left the game \n";
        }
        pv.RPC("SyncKillFeed", RpcTarget.All, savedString);
    }
    
    [PunRPC]
    void SyncKillFeed(string stringtoSync)
    {
        uitext.text += stringtoSync;
    }

}
