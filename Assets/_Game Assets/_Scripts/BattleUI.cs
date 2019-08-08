using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BattleUI : MonoBehaviourPun
{
    public Text uitext;
    public string oldText;
    private PhotonView pv;
    string savedString;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void UpdateBattleLog(string weapon, string killer , string player)
    {
        savedString += oldText + killer + " " + weapon + " " + player + " to death \n";
        pv.RPC("SyncKillFeed", RpcTarget.All, savedString);
    }
    
    [PunRPC]
    void SyncKillFeed(string stringtoSync)
    {
        uitext.text = stringtoSync;
        savedString = uitext.text;
    }

}
