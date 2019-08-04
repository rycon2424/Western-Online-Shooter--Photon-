using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCombat : MonoBehaviourPun
{

    public PlayerBehaviour pb;

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

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (pb.anim.GetBool("Aim") == true)
        {
            return;
        }
    }

}
