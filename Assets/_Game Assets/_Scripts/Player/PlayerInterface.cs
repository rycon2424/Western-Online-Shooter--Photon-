﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Text;

public class PlayerInterface : MonoBehaviourPun
{
    public BattleUI bui;
    public GameObject scoreBoard;
    private bool scoreboardfound = false;

    // Start is called before the first frame update
    public void StartPlayerUI()
    {
        pb = GetComponent<PlayerBehaviour>();
        oc = GetComponentInChildren<OrbitCamera>();
        bui = FindObjectOfType<BattleUI>();
        healthBar.maxValue = pb.maxHealth;
        UpdateWeaponUI();
        if (pb.pv.IsMine)
        {
            CloseMenu();
        }
        else if (pb.onlineReady == false)
        {
            CloseMenu();
        }
        UpdateHealthUI(pb.health, "", "");
        if (bui == null)
        {
            Debug.Log("There is no Battle Ui, probably because you're in offline mode");
        }
        else
        {
            scoreBoard = GameObject.Find("GlobalUI").transform.Find("ScoreBoard").gameObject;
            scoreboardfound = true;
        }
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
        if (Input.GetKey(KeyCode.Tab) && scoreboardfound && GameSetupController.gameEnded == false)
        {
            OpenScoreBoard();
        }
        if (Input.GetKeyUp(KeyCode.Tab) && scoreboardfound && GameSetupController.gameEnded == false)
        {
            scoreBoard.SetActive(false);
        }
        if (pb.dead && Input.GetKeyDown(KeyCode.H))
        {
            SwitchCharacter();
        }
    }

    #region ScoreBoard

    int playerCount;

    public void OpenScoreBoard()
    {
        scoreBoard.SetActive(true);
        UpdateScoreBoard();
    }

    void UpdateScoreBoard()
    {
        //Get playerCount
        playerCount = PhotonNetwork.PlayerList.Length;

        //Get playerNames
        var playerNames = new StringBuilder();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerNames.Append(player.NickName + " Kills: " + player.GetScore() + "\n" + "\n");
        }

        //Update Text of playernames
        scoreBoard.transform.Find("PlayerNames").GetComponent<Text>().text = playerNames.ToString();

        //Update total players playing currently
        string playerCountText = "Total Players: " + playerCount.ToString();
        scoreBoard.transform.Find("PlayerCount").GetComponent<Text>().text = playerCountText;
    }

    #endregion

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
        oc.pause = true;
        Cursor.visible = true;
        menu.SetActive(true);
        openMenu = true;
    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        oc.pause = false;
        menu.SetActive(false);
        openMenu = false;
    }

    public void UpdateSensVol()
    {
        AudioListener.volume = volume.value;
        oc.sensitivity = sensitivity.value;
    }

    void SwitchCharacter()
    {
        pb.gsc.characterSelect.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void ExitGame()
    {
        if (bui != null)
        {
            bui.JoinLeaveGame(pb.pv.Owner.NickName, false);
        }
    }
    #endregion

    #region Health Sync Stuff
    [Header("Health UI")]
    public Slider healthBar;
    public PlayerBehaviour pb;

    public void UpdateHealthUI(int health, string weapon, string killer)
    {
        if (pb.onlineReady == true)
        {
            pb.pv.RPC("SyncHealth", RpcTarget.All, health, weapon, killer);
        }
        if (pb.onlineReady == false)
        {
            SyncHealth(health, weapon, killer);
        }
    }

    [PunRPC]
    public void SyncHealth(int hp, string _swep, string _killer)
    {
        if (pb == null)
        {
            pb = GetComponent<PlayerBehaviour>();
        }
        pb.health = hp;
        healthBar.value = pb.health;
        killer = _killer;
        attackType = _swep;
        pb.anim.SetInteger("Health", pb.health);
        if (pb.health <= 0)
        {
            pb.Death();
            pb.dead = true;
        }
    }

    public void UpdateLog()
    {
        if (pb.health <= 0)
        {
            bui.UpdateBattleLog(attackType, killer, pb.pv.Owner.NickName);
        }
    }

    [Header("Recently Attacked")]
    public string killer;
    public string attackType;
    
    #endregion

    #region Weapon Sync Stuff
    [Header("Weapon UI")]
    public Image weaponSort;
    public Sprite noWeapon;
    public Sprite revolver;
    public Sprite rifle;
    public Sprite tommygun;
    public Sprite shotgun;
    public Sprite sniper;

    [Header("Weapon Ammo")]
    public Text ammoText;

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
                UpdateAmmoUI(-1);
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
                UpdateAmmoUI(pb.pc.rifleAmmo);
                break;
            case PlayerCombat.GunType.tommygun:
                weaponSort.sprite = tommygun;
                pb.anim.SetInteger("WeaponType", 3);
                if (pb.onlineReady)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                UpdateAmmoUI(pb.pc.tommygunAmmo);
                break;

            case PlayerCombat.GunType.shotgun:
                weaponSort.sprite = shotgun;
                pb.anim.SetInteger("WeaponType", 4);
                if (pb.onlineReady && pb.pv.IsMine)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                UpdateAmmoUI(pb.pc.shotgunAmmo);
                break;
            case PlayerCombat.GunType.sniper:
                weaponSort.sprite = sniper;
                pb.anim.SetInteger("WeaponType", 5);
                if (pb.onlineReady && pb.pv.IsMine)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                UpdateAmmoUI(pb.pc.sniperAmmo);
                break;
            case PlayerCombat.GunType.noWeapon:
                weaponSort.sprite = noWeapon;
                pb.anim.SetInteger("WeaponType", 0);
                if (pb.onlineReady && pb.pv.IsMine)
                {
                    pb.pv.RPC("SyncWeaponUI", RpcTarget.All, pb.anim.GetInteger("WeaponType"));
                }
                else
                {
                    SyncWeaponUI(pb.anim.GetInteger("WeaponType"));
                }
                UpdateAmmoUI(-1);
                break;
            default:
                break;
        }
    }

    public void UpdateAmmoUI(int ammo)
    {
        if (ammo == -1)
        {
            ammoText.text = "∞";
        }
        else
        {
            ammoText.text = ammo.ToString();
        }
    }

    [Header("Gun models")]
    public GameObject revolverModel;
    public GameObject rifleModel;
    public GameObject tommygunModel;
    public GameObject shotgunModel;
    public GameObject sniperModel;

    [PunRPC]
    public void SyncWeaponUI(int weaponType)
    {
        switch (weaponType)
        {
            case 0:
                revolverModel.SetActive(false);
                rifleModel.SetActive(false);
                tommygunModel.SetActive(false);
                shotgunModel.SetActive(false);
                sniperModel.SetActive(false);
                break;
            case 1:
                revolverModel.SetActive(true);
                rifleModel.SetActive(false);
                tommygunModel.SetActive(false);
                shotgunModel.SetActive(false);
                sniperModel.SetActive(false);
                break;
            case 2:
                revolverModel.SetActive(false);
                rifleModel.SetActive(true);
                tommygunModel.SetActive(false);
                shotgunModel.SetActive(false);
                sniperModel.SetActive(false);
                break;
            case 3:
                revolverModel.SetActive(false);
                rifleModel.SetActive(false);
                tommygunModel.SetActive(true);
                shotgunModel.SetActive(false);
                sniperModel.SetActive(false);
                break;
            case 4:
                revolverModel.SetActive(false);
                rifleModel.SetActive(false);
                tommygunModel.SetActive(false);
                shotgunModel.SetActive(true);
                sniperModel.SetActive(false);
                break;
            case 5:
                revolverModel.SetActive(false);
                rifleModel.SetActive(false);
                tommygunModel.SetActive(false);
                shotgunModel.SetActive(false);
                sniperModel.SetActive(true);
                break;
            default:
                break;
        }
    }
    #endregion
}