using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUlt : MonoBehaviourPun
{

    public enum Character {Cowboy, Shinobi, Sherrif, Doctor, Drunken }
    public Character currentCharacter;

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
        ultCharge += ultChargeGainedSecond;
        UpdateUI();
        StartUltimateScript();
    }

    public void UltGainShot()
    {
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
                CowboyUlt();
                break;
            case Character.Shinobi:
                ShinobiUlt();
                break;
            case Character.Sherrif:
                SherrifUlt();
                break;
            case Character.Doctor:
                DoctorUlt();
                break;
            case Character.Drunken:
                DrunkenUlt();
                break;
            default:
                break;
        }
    }

    void CowboyUlt()
    {
        //halo.SetActive(true);
    }

    void ShinobiUlt()
    {

    }

    void SherrifUlt()
    {

    }

    void DoctorUlt()
    {

    }

    void DrunkenUlt()
    {

    }

}