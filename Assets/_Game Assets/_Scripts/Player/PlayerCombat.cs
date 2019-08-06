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
        if (pb.pv.IsMine == false && pb.onlineReady)
        {
            return;
        }
    }
    
    public void Combat()
    {
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
        if (typeGun == GunType.revolver)
        {
            pb.anim.SetBool("1Handed", true);
        }
        else
        {
            pb.anim.SetBool("2Handed", true);
        }
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
        pb.anim.SetBool("2Handed", false);
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
    public enum GunType { revolver, shotgun, rifle}
    public float weaponRange;
    public int weaponDamage;
    public float fireRate;
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
            case GunType.shotgun:
                break;
            case GunType.rifle:
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