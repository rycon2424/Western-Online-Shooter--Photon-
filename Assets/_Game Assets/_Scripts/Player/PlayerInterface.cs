using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInterface : MonoBehaviourPun
{
    [Header("Health UI")]
    public Slider healthBar;
    public PlayerBehaviour pb;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        UpdateHealthUI(pb.health);
        UpdateWeaponUI();
    }

    private int currentHealth;

    // Update is called once per frame
    void Update()
    {
        if (pb.pv.IsMine == false)
        {
            return;
        }
    }

    public void UpdateHealthUI(int health)
    {
        pb.pv.RPC("SyncHealth", RpcTarget.All, health);
    }

    [PunRPC]
    public void SyncHealth(int hp)
    {
        if (pb == null)
        {
            pb = GetComponent<PlayerBehaviour>();
        }
        pb.health = hp;
        healthBar.value = pb.health;
        pb.anim.SetInteger("Health", pb.health);
        if (pb.health <= 0)
        {
            Debug.Log("Die");
            pb.dead = true;
        }
    }

    [Header("Weapon UI")]
    public Image weaponSort;
    public Sprite noWeapon;
    public Sprite revolver;
    public Sprite rifle;

    public void UpdateWeaponUI()
    {
        switch (pb.pc.typeGun)
        {
            case PlayerCombat.GunType.revolver:
                weaponSort.sprite = revolver;
                pb.anim.SetInteger("WeaponType", 1);
                break;
            case PlayerCombat.GunType.rifle:
                weaponSort.sprite = rifle;
                pb.anim.SetInteger("WeaponType", 2);
                break;
            case PlayerCombat.GunType.noWeapon:
                weaponSort.sprite = noWeapon;
                pb.anim.SetInteger("WeaponType", 0);
                break;
            default:
                break;
        }
    }

}