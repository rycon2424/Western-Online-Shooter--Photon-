using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCombat : MonoBehaviourPun
{

    private PlayerBehaviour pb;

    private Transform chest;
    public Vector3 offset;

    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        chest = pb.anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    void Update()
    {
        if (pb.pv.IsMine == false && pb.onlineReady == true)
        {
            return;
        }
        SelectWeapon();
    }
    
    void SelectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (pb.onlineReady)
            {
                pb.pv.RPC("SyncWeaponCycle", RpcTarget.All, true);
            }
            else
            {
                SyncWeaponCycle(true);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (pb.onlineReady)
            {
                pb.pv.RPC("SyncWeaponCycle", RpcTarget.All, false);
            }
            else
            {
                SyncWeaponCycle(false);
            }
        }
        pb.pi.UpdateWeaponUI();
    }

    [PunRPC]
    void SyncWeaponCycle(bool forward)
    {
        if (forward == true)
        {
            if (typeGun == GunType.noWeapon)
            {
                typeGun = GunType.revolver;
            }
            else if (typeGun == GunType.revolver)
            {
                typeGun = GunType.rifle;
            }
            else if (typeGun == GunType.rifle)
            {
                typeGun = GunType.noWeapon;
            }
        }
        else
        {
            if (typeGun == GunType.revolver)
            {
                typeGun = GunType.noWeapon;
            }
            else if (typeGun == GunType.rifle)
            {
                typeGun = GunType.revolver;
            }
            else if (typeGun == GunType.noWeapon)
            {
                typeGun = GunType.rifle;
            }
        }
        switch (typeGun)
        {
            case GunType.revolver:
                weaponRange = 50;
                weaponDamage = 15;
                fireRate = 1;
                break;
            case GunType.rifle:
                weaponRange = 200;
                weaponDamage = 30;
                fireRate = 2;
                break;
            case GunType.noWeapon:
                weaponRange = 0;
                weaponDamage = 0;
                fireRate = 0;
                break;
            default:
                break;
        }
    }

    public void Combat()
    {
        if (typeGun == GunType.noWeapon)
        {
            return;
        }
        if (Input.GetMouseButton(1))
        {
            EnterCombat();
        }
        else
        {
            ExitCombat();
        }
    }
    
    public void EnterCombat()
    {
        pb.anim.SetBool("Aim", true);
        pb.anim.SetBool("1Handed", true);

        if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady)
        {
            pb.pv.RPC("AimRaycast", RpcTarget.All);
        }
        else if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady == false)
        {
            AimRaycast();
        }
    }

    public void ExitCombat()
    {
        pb.anim.SetBool("Aim", false);
        pb.anim.SetBool("1Handed", false);
    }

    void LateUpdate()
    {
        if (pb.anim.GetBool("Aim") == false || pb.anim.GetBool("Falling") == true || pb.dead == true)
        {
            return;
        }
        chest.LookAt(pb.lookObj.position);
        chest.rotation = chest.rotation * Quaternion.Euler(offset);
    }
    
    [Header("WeaponStats")]
    public GunType typeGun;
    public enum GunType { revolver, rifle, noWeapon }
    public float weaponRange;
    public int weaponDamage;
    public float fireRate;
    
    [Header("Other Stats")]
    public bool canShoot;
    public LayerMask canHit;
    public Transform cameraTransform;
    RaycastHit hit;
    
    [PunRPC]
    void AimRaycast()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * weaponRange, Color.red);
        
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, weaponRange, canHit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerBehaviour>().health -= weaponDamage;
                int health = hit.collider.GetComponent<PlayerBehaviour>().health;
                Debug.Log(health);
                hit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health);
                HitMarker();
            }
            Debug.Log(pb.pv.Owner + " Shot " + hit.collider.name);
        }
        else
        {
            Debug.Log(pb.pv.Owner + " missed ");
        }
        GunShotSound();
        Invoke("FireRate", fireRate);
        canShoot = false;
    }

    void GunShotSound()
    {
        switch (typeGun)
        {
            case GunType.revolver:
                pb.ps.Audio_RevolverShot();
                break;
            case GunType.rifle:
                pb.ps.Audio_RifleShot();
                break;
            case GunType.noWeapon:
                break;
            default:
                break;
        }
    }
    
    void FireRate()
    {
        canShoot = true;
    }
    
    void HitMarker()
    {
        pb.hitmarker.SetActive(true);
        pb.ps.Audio_HitMarker();
        Invoke("HideHitMarker", 0.5f);
    }

    void HideHitMarker()
    {
        pb.hitmarker.SetActive(false);
    }

}