﻿using System.Collections;
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
        Debug.Log("EnterCombat");
        pb.anim.SetBool("Aim", true);
        pb.anim.SetBool("1Handed", true);
    }

    public void ExitCombat()
    {
        Debug.Log("ExitCombat");
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

}
