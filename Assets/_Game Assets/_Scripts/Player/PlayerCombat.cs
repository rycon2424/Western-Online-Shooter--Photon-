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
        pb.anim.SetBool("1Handed", true);
        if (Input.GetMouseButton(0) && canShoot == true && pb.onlineReady)
        {
            pb.pv.RPC("AimRaycast", RpcTarget.All);
        }
        else if (Input.GetMouseButton(0) && canShoot == true)
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
        if (pb.anim.GetBool("Aim") == false || pb.anim.GetBool("Falling") == true)
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
                hit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI();
            }
            canShoot = false;
            Invoke("FireRate", fireRate);
            Debug.Log(pb.pv.Owner + " Shot " + hit.collider.name);
        }
    }
    
    void FireRate()
    {
        canShoot = true;
    }

}
