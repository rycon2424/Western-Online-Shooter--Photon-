using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponCrate : MonoBehaviourPun
{
    public int rifleAmmoGiven;
    public int tommyAmmoGiven;
    public int shotgunAmmoGiven;
    public int sniperAmmoGiven;

    private PlayerCombat pc;

    void Start()
    {
        rifleAmmoGiven = Random.Range(10, 26);
        tommyAmmoGiven = Random.Range(30, 61);
        shotgunAmmoGiven = Random.Range(5, 16);
        sniperAmmoGiven = Random.Range(2, 7);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pc = col.GetComponent<PlayerCombat>();
            pc.rifleAmmo += rifleAmmoGiven;
            pc.tommygunAmmo += tommyAmmoGiven;
            pc.shotgunAmmo += shotgunAmmoGiven;
            pc.sniperAmmo += sniperAmmoGiven;
            pc.UpdateAmmo(pc.rifleAmmo, PlayerCombat.GunType.rifle);
            pc.UpdateAmmo(pc.tommygunAmmo, PlayerCombat.GunType.tommygun);
            pc.UpdateAmmo(pc.shotgunAmmo, PlayerCombat.GunType.shotgun);
            pc.UpdateAmmo(pc.sniperAmmo, PlayerCombat.GunType.sniper);
            GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
        }
    }

    [PunRPC]
    void DestroySelf()
    {
        Destroy(gameObject);
    }

}
