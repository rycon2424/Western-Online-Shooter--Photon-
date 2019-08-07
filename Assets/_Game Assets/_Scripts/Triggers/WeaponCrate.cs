using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponCrate : MonoBehaviourPun
{
    public int rifleAmmoGiven;
    public int tommyAmmoGiven;

    void Start()
    {
        rifleAmmoGiven = Random.Range(1, 10);
        tommyAmmoGiven = Random.Range(20, 40);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerCombat>().rifleAmmo += rifleAmmoGiven;
            col.GetComponent<PlayerCombat>().tommygunAmmo += tommyAmmoGiven;
            GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroySelf()
    {
        Destroy(gameObject);
    }

}
