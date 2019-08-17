using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerCombat : MonoBehaviourPun
{
    [HideInInspector]
    public PlayerBehaviour pb;

    private Transform chest;
    public Vector3 offset;

    public void StartPlayerCombat()
    {
        pb = GetComponent<PlayerBehaviour>();
        chest = pb.anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    void Update()
    {
        //Shotgun();
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
                weaponRange = 43;
                weaponDamage = 15;
                fireRate = 1;
                weaponZoom = 3.5f;
                break;
            case GunType.rifle:
                weaponRange = 200;
                weaponDamage = 38;
                fireRate = 2;
                weaponZoom = 2;
                break;
            case GunType.tommygun:
                weaponRange = 30;
                weaponDamage = 4;
                fireRate = 0.15f;
                weaponZoom = 3.5f;
                break;
            case GunType.noWeapon:
                weaponRange = 1.8f;
                weaponDamage = 80;
                fireRate = 0;
                weaponZoom = 4;
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
        if (Input.GetKeyDown(KeyCode.Space) && canDodge)
        {
            ZoomIn(false, 0);
            if (pb.onlineReady)
            {
                pb.pv.RPC("Dodge", RpcTarget.All);
            }
            else
            {
                Dodge();
            }
        }
        if (canDodge == false)
        {
            return;
        }
        if (typeGun == GunType.noWeapon)
        {
            pb.anim.SetBool("Aim", false);
            ZoomIn(false, 0);
            pb.blocked.SetActive(false);
            if (Input.GetMouseButton(0) && canShoot)
            {
                if (pb.onlineReady == true)
                {
                    Debug.Log("melee");
                    pb.pv.RPC("SyncKnifeAnim", RpcTarget.All);
                }
                else
                {
                    Debug.Log("melee");
                    pb.anim.Play("Knife");
                    canSwitchWeapons = false;
                }
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
        HeadGlitchCheck();
        if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady && CheckAmmunition() && HeadGlitchCheck() == false)
        {
            pb.pv.RPC("AimRaycast", RpcTarget.All);
        }
        else if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady == false && CheckAmmunition() && HeadGlitchCheck() == false)
        {
            AimRaycast();
        }
        CheckWeaponZoom();
    }

    public void ExitCombat()
    {
        pb.anim.SetBool("Aim", false);
        ZoomIn(false, 0);
        pb.blocked.SetActive(false);
    }

    void LateUpdate()
    {
        if (pb.anim.GetBool("Aim") == false || pb.anim.GetBool("Falling") == true || pb.dead == true || canDodge == false)
        {
            return;
        }
        chest.LookAt(pb.lookObj.position);
        chest.rotation = chest.rotation * Quaternion.Euler(offset);
    }

    #region zooming

    void CheckWeaponZoom()
    {
        switch (typeGun)
        {
            case GunType.revolver:
                ZoomIn(true, weaponZoom);
                break;
            case GunType.rifle:
                ZoomIn(true, weaponZoom);
                break;
            case GunType.tommygun:
                ZoomIn(true, weaponZoom);
                break;
            case GunType.noWeapon:
                ZoomIn(true, weaponZoom);
                break;
            default:
                break;
        }
    }

    void ZoomIn(bool zooming, float zoomDistance)
    {
        if (zooming)
        {
            pb.oc.minDistance = zoomDistance;
            pb.oc.maxDistance = zoomDistance;
        }
        else
        {
            pb.oc.minDistance = 4;
            pb.oc.maxDistance = 4;
        }
    }

    #endregion

    public LayerMask headGlitchHit;
    RaycastHit headHit;
    bool HeadGlitchCheck()
    {
        Vector3 rayOrigin = transform.position + transform.forward * 0.1f + transform.up * 1.35f;
        Debug.DrawRay(rayOrigin, transform.forward * 1.35f, Color.blue, 2);
        if (Physics.Raycast(rayOrigin, transform.forward, out headHit, 1.35f, headGlitchHit))
        {
            if (headHit.collider.tag != "")
            {
                pb.blocked.SetActive(true);
                return true;
            }
            else
            {
                pb.blocked.SetActive(false);
                return false;
            }
        }
        else
        {
            pb.blocked.SetActive(false);
            return false;
        }
    }

    #region dodge
    public bool canDodge;

    [PunRPC]
    void Dodge()
    {
        pb.anim.Play("Dodge");
        HideKnife();
        canDodge = false;
        canSwitchWeapons = false;
    }

    public void CanDodgeAgain()
    {
        Invoke("CooldownDodge", 0.6f);
        pb.cc.height = pb.defaultPlayerHeight;
        pb.cc.center = pb.defaultHitBoxOffset;
        canSwitchWeapons = true;
    }

    void CooldownDodge()
    {
        canDodge = true;
    }
    #endregion

    [Header("WeaponStats")]
    public GunType typeGun;
    public enum GunType { revolver, rifle, tommygun, noWeapon }
    public float weaponRange;
    public int weaponDamage;
    public float fireRate;
    public float weaponZoom;

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
                int damage = weaponDamage;
                int health = hit.collider.GetComponentInParent<PlayerBehaviour>().health;
                health = health - damage;
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateHealthUI(health, "shot", pb.pv.Owner.NickName);
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateLog();
                CheckIfKilled(health);
                HitMarker();
            }
            #region commented hit tags
            /*if (hit.collider.CompareTag("Head"))
            {
                int damage = (weaponDamage * 2);
                int health = hit.collider.GetComponentInParent<PlayerBehaviour>().health;
                health = health - damage;
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateHealthUI(health, "HEADSHOT", pb.pv.Owner.NickName);
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateLog();
                Debug.Log(hit.collider.name);
                HitMarker();
            }
            if (hit.collider.CompareTag("Torso"))
            {
                int damage = weaponDamage;
                int health = hit.collider.GetComponentInParent<PlayerBehaviour>().health;
                health = health - damage;
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateHealthUI(health, "Torso", pb.pv.Owner.NickName);
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateLog();
                Debug.Log(hit.collider.name);
                HitMarker();
            }
            if (hit.collider.CompareTag("Limbs"))
            {
                int damage = (weaponDamage / 2);
                int health = hit.collider.GetComponentInParent<PlayerBehaviour>().health;
                health = health - damage;
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateHealthUI(health, "Limbs", pb.pv.Owner.NickName);
                hit.collider.GetComponentInParent<PlayerBehaviour>().pi.UpdateLog();
                Debug.Log(hit.collider.name);
                HitMarker();
            }*/
            #endregion
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

    void Shotgun()
    {
        Vector3 Width1 = cameraTransform.position + cameraTransform.forward + cameraTransform.right * 0.35f;
        Vector3 Width2 = cameraTransform.position + cameraTransform.forward + cameraTransform.right * -0.35f;
        Vector3 Width3 = cameraTransform.position + cameraTransform.forward + cameraTransform.up * 0.35f;
        Vector3 Width4 = cameraTransform.position + cameraTransform.forward + cameraTransform.up * -0.35f;
        Vector3 Width5 = cameraTransform.position + cameraTransform.forward;
        Debug.DrawRay(Width1, cameraTransform.forward * 30, Color.red, 0.2f);
        Debug.DrawRay(Width2, cameraTransform.forward * 30, Color.red, 0.2f);
        Debug.DrawRay(Width3, cameraTransform.forward * 30, Color.red, 0.2f);
        Debug.DrawRay(Width4, cameraTransform.forward * 30, Color.red, 0.2f);
        Debug.DrawRay(Width5, cameraTransform.forward * 30, Color.red, 0.2f);
    }

    public LayerMask knifeCanHit;
    [PunRPC]
    void KnifeCast()
    {
        Vector3 rayOrigin = transform.position + transform.forward * -0.2f + transform.up * 1.1f;
        Debug.DrawRay(rayOrigin, transform.forward * weaponRange, Color.yellow, 1);

        if (Physics.SphereCast(rayOrigin, 0.3f, transform.forward, out knifeHit, weaponRange, knifeCanHit))
        {
            if (knifeHit.collider.CompareTag("Player"))
            {
                int firsthealth = knifeHit.collider.GetComponent<PlayerBehaviour>().health;
                knifeHit.collider.GetComponent<PlayerBehaviour>().health -= weaponDamage;
                int health = knifeHit.collider.GetComponent<PlayerBehaviour>().health;
                knifeHit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health, "knived", pb.pv.Owner.NickName);
                knifeHit.collider.GetComponent<PlayerBehaviour>().pi.UpdateLog();
                CheckIfKilled(health);
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
        hasKnifeOut = false;
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
                UpdateAmmo(rifleAmmo, GunType.rifle);
                break;
            case GunType.tommygun:
                tommygunAmmo -= 1;
                UpdateAmmo(tommygunAmmo, GunType.tommygun);
                pb.pi.UpdateAmmoUI(tommygunAmmo);
                break;
            case GunType.noWeapon:
                pb.pi.UpdateAmmoUI(-1);
                break;
            default:
                break;
        }
    }
    
    public void UpdateAmmo(int ammoToSync, GunType gunType)
    {
        if (pb.onlineReady)
        {
            pb.pv.RPC("SyncAmmo", RpcTarget.All, ammoToSync, gunType);
        }
        else
        {
            SyncAmmo(ammoToSync, gunType);
        }
    }
    
    [PunRPC]
    void SyncAmmo(int ammoToSync, GunType gun)
    {
        switch (gun)
        {
            case GunType.revolver:
                break;
            case GunType.rifle:
                rifleAmmo = ammoToSync;
                break;
            case GunType.tommygun:
                tommygunAmmo = ammoToSync;
                break;
            case GunType.noWeapon:
                break;
            default:
                break;
        }
    }
    #endregion

    void CheckIfKilled(int health)
    {
        if (pb.pv.IsMine == false)
        {
            return;
        }
        if (health < 1)
        {
            PhotonNetwork.LocalPlayer.AddScore(1);
        }
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