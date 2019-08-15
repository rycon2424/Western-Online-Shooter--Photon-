using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponCrate : MonoBehaviourPun
{
    public int rifleAmmoGiven;
    public int tommyAmmoGiven;

    private PlayerCombat pc;

    void Start()
    {
        rifleAmmoGiven = Random.Range(1, 10);
        tommyAmmoGiven = Random.Range(20, 40);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pc = col.GetComponent<PlayerCombat>();
            pc.rifleAmmo += rifleAmmoGiven;
            pc.tommygunAmmo += tommyAmmoGiven;
            pc.UpdateAmmo(pc.rifleAmmo, PlayerCombat.GunType.rifle);
            pc.UpdateAmmo(pc.tommygunAmmo, PlayerCombat.GunType.tommygun);
            //Debug.Log("has now " + pc.rifleAmmo + " Rifle ammo" + " and " + pc.tommygunAmmo + " tommygun ammo ");
            GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroySelf()
    {
        Destroy(gameObject);
    }

}
