using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleUI : MonoBehaviourPun
{
    public Text uitext;
    private PhotonView pv;
    public string savedString;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void UpdateBattleLog(string weapon, string killer , string player)
    {
        savedString += killer + " " + weapon + " " + player + " to death \n";
        pv.RPC("SyncChatToMaster", RpcTarget.MasterClient, savedString);
    }
    
    [PunRPC]
    void SyncChatToMaster(string stringtoSync)
    {
        pv.RPC("SyncChatToClients", RpcTarget.AllViaServer, savedString);
    }

    [PunRPC]
    void SyncChatToClients(string stringtoSync)
    {
        uitext.text = stringtoSync;
        savedString = uitext.text;
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
        pv.RPC("SyncChatToMaster", RpcTarget.MasterClient, savedString);
    }
    
}
