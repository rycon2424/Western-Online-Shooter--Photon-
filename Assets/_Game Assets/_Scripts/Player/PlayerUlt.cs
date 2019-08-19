using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUlt : MonoBehaviourPun
{

    public enum Character {Cowboy, Shinobi, Sherrif, Doctor, Drunken }
    public Character currentCharacter;

    private PlayerBehaviour pb;
    private PlayerCombat pc;
    private PlayerInterface pi;

    [Header("Global Stats")]
    public bool isUlting;
    public float ultCharge;
    public float ultDuration;
    public float ultChargeGainedSecond;
    public float ultChargeGainedOnShot;
    public Slider ultBar;

    [Header("CowBoy")]
    public GameObject halo;

    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        pc = GetComponent<PlayerCombat>();
        pi = GetComponent<PlayerInterface>();

        ultBar.value = ultCharge;
        UpdateUI();
    }

    public void StartUltimateScript()
    {
        if (ultCharge >= 100)
        {
            ultCharge = 100;
            return;
        }
        Invoke("UltGain", 1);
    }

    void UltGain()
    {
        if (isUlting){ return; }
        ultCharge += ultChargeGainedSecond;
        UpdateUI();
        StartUltimateScript();
    }

    public void UltGainShot()
    {
        if (isUlting) { return; }
        if (ultCharge >= 100)
        {
            ultCharge = 100;
            return;
        }
        ultCharge += ultChargeGainedOnShot;
        UpdateUI();
    }

    void UpdateUI()
    {
        ultBar.value = ultCharge;
    }

    float curlerp = 0.0f;
    void UltDuration()
    {
        if (ultCharge <= 0)
        {
            isUlting = false;
            ultCharge = 0;
            UpdateUI();
            StartUltimateScript();
            if (pb.onlineReady)
            {
                pb.pv.RPC("EndUlt", RpcTarget.All);
            }
            else
            {
                EndUlt();
            }
        }
        curlerp += Time.deltaTime / ultDuration;
        ultCharge = (int)Mathf.Lerp(100, 0, curlerp);
        UpdateUI();
    }
    
    public void CastUltimate()
    {
        if (Input.GetKeyDown(KeyCode.Q) && ultCharge == 100 && isUlting == false)
        {
            isUlting = true;
            curlerp = 0;
            Ult();
        }
        if (isUlting)
        {
            UltDuration();
        }
    }

    void Ult()
    {
        switch (currentCharacter)
        {
            case Character.Cowboy:
                if (pb.onlineReady)
                {
                    pb.pv.RPC("CowboyUlt", RpcTarget.All);
                }
                else
                {
                    CowboyUlt();
                }
                break;
            case Character.Shinobi:
                if (pb.onlineReady)
                {
                    pb.pv.RPC("ShinobiUlt", RpcTarget.All);
                }
                else
                {
                    ShinobiUlt();
                }
                break;
            case Character.Sherrif:
                if (pb.onlineReady)
                {
                    pb.pv.RPC("SherrifUlt", RpcTarget.All);
                }
                else
                {
                    SherrifUlt();
                }
                break;
            case Character.Doctor:
                if (pb.onlineReady)
                {
                    pb.pv.RPC("CowboyUlt", RpcTarget.All);
                }
                else
                {
                    CowboyUlt();
                }
                break;
            case Character.Drunken:
                if (pb.onlineReady)
                {
                    pb.pv.RPC("DrunkenUlt", RpcTarget.All);
                }
                else
                {
                    DrunkenUlt();
                }
                break;
            default:
                break;
        }
    }

    [PunRPC]
    void CowboyUlt()
    {
        halo.SetActive(true);
        pc.typeGun = PlayerCombat.GunType.revolver;
        pc.AssignDamage();
        pc.canSwitchWeapons = false;
        pi.UpdateWeaponUI();
        pc.weaponDamage = 20;
        pc.weaponRange = 200;
        pc.weaponZoom = 5f;
        pc.fireRate = 0.4f;
    }

    [PunRPC]
    void ShinobiUlt()
    {

    }

    [PunRPC]
    void SherrifUlt()
    {

    }

    [PunRPC]
    void DoctorUlt()
    {

    }

    [PunRPC]
    void DrunkenUlt()
    {

    }

    [PunRPC]
    void EndUlt()
    {
        switch (currentCharacter)
        {
            case Character.Cowboy:
                halo.SetActive(false);
                pc.AssignDamage();
                pc.canSwitchWeapons = true;
                break;
            case Character.Shinobi:
                break;
            case Character.Sherrif:
                break;
            case Character.Doctor:
                break;
            case Character.Drunken:
                break;
            default:
                break;
        }
    }

}