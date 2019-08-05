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
        if (Input.GetMouseButton(0))
        {
            pb.pv.RPC("AimRaycast", RpcTarget.All);
        }
    }

    public void ExitCombat()
    {
        pb.anim.SetBool("Aim", false);
        pb.anim.SetBool("1Handed", false);
    }

    void LateUpdate()
    {
        if (pb.anim.GetBool("Aim") == false)
        {
            return;
        }
        chest.LookAt(pb.lookObj.position);
        chest.rotation = chest.rotation * Quaternion.Euler(offset);
    }

    [Header("AimRange")]
    public float hitRange;
    public LayerMask canHit;
    public Transform cameraTransform;
    RaycastHit hit;
    
    [PunRPC]
    void AimRaycast()
    {
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * hitRange, Color.red);

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, hitRange, canHit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<PlayerBehaviour>().health -= 5;
                hit.collider.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI();

            }
            Debug.Log(pb.pv.Owner + " Shot " + hit.collider.name);
        }
    }

}
