using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInterface : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        oc = GetComponentInChildren<OrbitCamera>();
        UpdateHealthUI(pb.health);
        UpdateWeaponUI();
        CloseMenu();
    }

    private int currentHealth;

    // Update is called once per frame
    void Update()
    {
        if (pb.pv.IsMine == false && pb.onlineReady == true)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenCloseMenu();
        }
    }

    #region optionsMenu
    [Header("Menu")]
    public OrbitCamera oc;
    public Slider volume;
    public Slider sensitivity;
    //public Slider aimSensitivity; NOT IN USE YET
    public GameObject menu;
    public bool openMenu;

    void OpenCloseMenu()
    {
        if (openMenu == true)
        {
            CloseMenu();
        }
        else if (openMenu == false)
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        menu.SetActive(true);
        openMenu = true;
    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menu.SetActive(false);
        openMenu = false;
    }

    public void UpdateSensVol()
    {
        AudioListener.volume = volume.value;
        oc.sensitivity = sensitivity.value;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Health Sync Stuff
    [Header("Health UI")]
    public Slider healthBar;
    public PlayerBehaviour pb;

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
            pb.Death();
            pb.dead = true;
        }
    }
    #endregion

    #region Weapon Sync Stuff
    [Header("Weapon UI")]
    public Image weaponSort;
    public Sprite noWeapon;
    public Sprite revolver;
    public Sprite rifle;

    public void UpdateWeaponUI()
    {
        if (pb == null)
        {
            pb = GetComponent<PlayerBehaviour>();
        }
        switch (pb.pc.typeGun)
        {
            case PlayerCombat.GunType.revolver:
                weaponSort.sprite = revolver;
                pb.anim.SetInteger("WeaponType", 1);
                if (pb.onlineReady)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                //pb.pv.RPC("SyncWeaponUI", RpcTarget.All);
                break;
            case PlayerCombat.GunType.rifle:
                weaponSort.sprite = rifle;
                pb.anim.SetInteger("WeaponType", 2);
                if (pb.onlineReady)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                break;
            case PlayerCombat.GunType.noWeapon:
                weaponSort.sprite = noWeapon;
                pb.anim.SetInteger("WeaponType", 0);
                if (pb.onlineReady)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                break;
            default:
                break;
        }
    }

    public GameObject revolverModel;
    public GameObject rifleModel;

    [PunRPC]
    public void SyncWeaponUI(int weaponType)
    {
        switch (weaponType)
        {
            case 0:
                revolverModel.SetActive(false);
                rifleModel.SetActive(false);
                break;
            case 1:
                revolverModel.SetActive(true);
                rifleModel.SetActive(false);
                break;
            case 2:
                revolverModel.SetActive(false);
                rifleModel.SetActive(true);
                break;
            default:
                break;
        }
    }
    #endregion
}