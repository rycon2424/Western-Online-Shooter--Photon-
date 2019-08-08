using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCombat : MonoBehaviourPun
{
    [HideInInspector]
    public PlayerBehaviour pb;

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
        if (canSwitchWeapons)
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
                typeGun = GunType.tommygun;
            }
            else if (typeGun == GunType.tommygun)
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
            else if (typeGun == GunType.tommygun)
            {
                typeGun = GunType.rifle;
            }
            else if (typeGun == GunType.noWeapon)
            {
                typeGun = GunType.tommygun;
            }
        }
        #region old version
        /*if (forward == true)
        {
            if (typeGun == GunType.noWeapon)
            {
                typeGun = GunType.revolver;
            }
            else if (typeGun == GunType.revolver)
            {
                if (rifleAmmo > 0)
                {
                    typeGun = GunType.rifle;
                }
                else if (tommygunAmmo > 0)
                {
                    typeGun = GunType.tommygun;
                }
                else
                {
                    typeGun = GunType.noWeapon;
                }
            }
            else if (typeGun == GunType.rifle)
            {
                if (tommygunAmmo > 0)
                {
                    typeGun = GunType.tommygun;
                }
                else
                {
                    typeGun = GunType.noWeapon;
                }
            }
            else if (typeGun == GunType.tommygun)
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
            else if (typeGun == GunType.tommygun)
            {
                if (rifleAmmo > 0)
                {
                    typeGun = GunType.rifle;
                }
                else
                {
                    typeGun = GunType.revolver;
                }
            }
            else if (typeGun == GunType.noWeapon)
            {
                if (tommygunAmmo > 0)
                {
                    typeGun = GunType.tommygun;
                }
                else if (rifleAmmo > 0)
                {
                    typeGun = GunType.rifle;
                }
                else
                {
                    typeGun = GunType.revolver;
                }
            }
        }*/
        #endregion
        if (pb.onlineReady)
        {
            pb.pv.RPC("AssignDamage", RpcTarget.All);
        }
        else if (pb.onlineReady == false)
        {
            AssignDamage();
        }
    }

    [PunRPC]
    public void AssignDamage()
    {
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
            case GunType.tommygun:
                weaponRange = 30;
                weaponDamage = 5;
                fireRate = 0.15f;
                break;
            case GunType.noWeapon:
                weaponRange = 1.5f;
                weaponDamage = 80;
                fireRate = 0;
                break;
            default:
                break;
        }
    }

    public void Combat()
    {
        if (pb.pi.openMenu == true)
        {
            return;
        }
        if (typeGun == GunType.noWeapon)
        {
            pb.anim.SetBool("Aim", false);
            if (Input.GetMouseButton(0) && pb.onlineReady == true && canShoot)
            {
                Debug.Log("melee");
                pb.pv.RPC("SyncKnifeAnim", RpcTarget.All);
            }
            else if (Input.GetMouseButton(0) && canShoot)
            {
                Debug.Log("melee");
                pb.anim.Play("Knife");
                canSwitchWeapons = false;
            }
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

        if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady && CheckAmmunition())
        {
            pb.pv.RPC("AimRaycast", RpcTarget.All);
        }
        else if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady == false && CheckAmmunition())
        {
            AimRaycast();
        }
    }

    public void ExitCombat()
    {
        pb.anim.SetBool("Aim", false);
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
    public enum GunType { revolver, rifle, tommygun, noWeapon }
    public float weaponRange;
    public int weaponDamage;
    public float fireRate;

    [Header("Ammunition")]
    public int rifleAmmo;
    public int tommygunAmmo;

    [Header("Other Stats")]
    public bool canSwitchWeapons = true;
    public bool canShoot;
    public LayerMask canHit;
    public Transform cameraTransform;
    RaycastHit hit;
    
    [PunRPC]
    void AimRaycast()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * weaponRange, Color.red, 0.5f);
        
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, weaponRange, canHit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerBehaviour>().health -= weaponDamage;
                int health = hit.collider.GetComponent<PlayerBehaviour>().health;
                hit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health, "shot", pb.pv.Owner.NickName);
                hit.collider.GetComponent<PlayerBehaviour>().pi.UpdateLog();
                HitMarker();
            }
            if (hit.collider.CompareTag("Finish"))
            {
                HitMarker();
            }
        }
        else
        {
            Debug.Log("missed");
        }
        GunShotSound();
        UseAmmo();
        Invoke("FireRate", fireRate);
        canShoot = false;
    }

    #region knife

    [Header("Knife")]
    public GameObject sheatedKnife;
    public GameObject knifeInHand;
    RaycastHit knifeHit;

    [PunRPC]
    void SyncKnifeAnim()
    {
        canSwitchWeapons = false;
        pb.anim.Play("Knife");
    }

    public void CastKnife()
    {
        if (pb.pv.IsMine == false && pb.onlineReady == true)
        {
            return;
        }
        if (pb.onlineReady)
        {
            pb.pv.RPC("KnifeCast", RpcTarget.All);
        }
        else
        {
            KnifeCast();
        }
    }

    [PunRPC]
    void KnifeCast()
    {
        Vector3 rayOrigin = transform.position + transform.forward * 0.3f + transform.up * 1f;
        Debug.DrawRay(rayOrigin, transform.forward * weaponRange, Color.blue, 1);

        if (Physics.Raycast(rayOrigin, transform.forward, out knifeHit, weaponRange, canHit))
        {
            if (knifeHit.collider.CompareTag("Player"))
            {
                int firsthealth = knifeHit.collider.GetComponent<PlayerBehaviour>().health;
                knifeHit.collider.GetComponent<PlayerBehaviour>().health -= weaponDamage;
                int health = knifeHit.collider.GetComponent<PlayerBehaviour>().health;
                knifeHit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health, "knived", pb.pv.Owner.NickName);
                knifeHit.collider.GetComponent<PlayerBehaviour>().pi.UpdateLog();
                HitMarker();
            }
            if (knifeHit.collider.CompareTag("Finish"))
            {
                HitMarker();
            }
        }
        else
        {
            Debug.Log("missed");
        }
        canShoot = false;
    }

    public void CanKnifeAgain()
    {
        canSwitchWeapons = true;
        canShoot = true;
    }

    [HideInInspector]
    public bool hasKnifeOut;

    public void ShowHideKnife()
    {
        SyncKnifeVisiblity();
    }
    
    void SyncKnifeVisiblity()
    {
        if (hasKnifeOut)
        {
            knifeInHand.SetActive(false);
            sheatedKnife.SetActive(true);
            hasKnifeOut = false;
        }
        else
        {
            sheatedKnife.SetActive(false);
            knifeInHand.SetActive(true);
            hasKnifeOut = true;
        }
    }

    //When respawning but knife is still visible
    public void HideKnife()
    {
        knifeInHand.SetActive(false);
        sheatedKnife.SetActive(true);
        canSwitchWeapons = true;
        canShoot = true;
    }

    #endregion

    #region check ammo / use ammo / sync ammo
    bool CheckAmmunition()
    {
        switch (typeGun)
        {
            case GunType.revolver:
                return true;
            case GunType.rifle:
                if (rifleAmmo > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case GunType.tommygun:
                if (tommygunAmmo > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case GunType.noWeapon:
                return false;
            default:
                return false;
        }
    }
    
    void UseAmmo()
    {
        switch (typeGun)
        {
            case GunType.revolver:
                pb.pi.UpdateAmmoUI(-1);
                break;
            case GunType.rifle:
                rifleAmmo -= 1;
                pb.pi.UpdateAmmoUI(rifleAmmo);
                UpdateAmmo(rifleAmmo, true, false);
                break;
            case GunType.tommygun:
                tommygunAmmo -= 1;
                UpdateAmmo(tommygunAmmo, false, true);
                pb.pi.UpdateAmmoUI(tommygunAmmo);
                break;
            case GunType.noWeapon:
                pb.pi.UpdateAmmoUI(-1);
                break;
            default:
                break;
        }
    }
    
    //NEEDS ANOTHER METHOD DEFINITELY trash code
    public void UpdateAmmo(int ammoToSync, bool isRifle, bool isTommygun)
    {
        if (pb.onlineReady)
        {
            pb.pv.RPC("SyncAmmo", RpcTarget.All, ammoToSync, isRifle, isTommygun);
        }
        else
        {
            SyncAmmo(ammoToSync, isRifle, isTommygun);
        }
    }

    //NEEDS ANOTHER METHOD DEFINITELY trash code
    [PunRPC]
    void SyncAmmo(int ammoToSync, bool isRifle, bool isTommygun)
    {
        if (isRifle)
        {
            rifleAmmo = ammoToSync;
        }
        if (isTommygun)
        {
            tommygunAmmo = ammoToSync;
        }
    }
    #endregion

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
            case GunType.tommygun:
                pb.ps.Audio_TommygunShot();
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