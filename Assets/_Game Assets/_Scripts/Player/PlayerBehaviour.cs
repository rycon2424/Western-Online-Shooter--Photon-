using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{
    [Header("Stats")]
    public bool isSprinting;
    public bool playerRotateWithCam;
    public bool onlineReady;

    [HideInInspector]
    public Animator anim;

    public GameObject cam;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        anim = GetComponent<Animator>();
        cam.SetActive(true);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float z;
    public void Update()
    {
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        if (OnGround() == true)
        {
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Sprint();
        }
        if (playerRotateWithCam)
        {
            RotateToLook();
        }
    }

    void Movement()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", z);
    }

    void Sprint()
    {
        isSprinting = !isSprinting;
        anim.SetBool("Running", isSprinting);
    }

    void RotateToLook()
    {
        var CharacterRotation = cam.transform.rotation;
        CharacterRotation.x = 0;
        CharacterRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, CharacterRotation, Time.deltaTime * 4);
    }

    [Header("Raycast")]
    public float raycastRange;
    public LayerMask canHit;

    bool OnGround()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange, canHit))
        {
            Debug.DrawRay(transform.position, -transform.up * raycastRange, Color.yellow);
            if (anim.GetBool("Ground") == false)
            {
                anim.Play("Hard Landing");
            }
            anim.SetBool("Ground", true);
            return true;
        }
        else
        {
            anim.SetBool("Ground", false);
            return false;
        }
    }

}