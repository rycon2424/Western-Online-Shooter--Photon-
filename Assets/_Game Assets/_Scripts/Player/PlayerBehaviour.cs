using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{
    [Header("Stats")]
    public bool isSprinting;
    public bool playerRotateWithCam;

    [HideInInspector]
    public Animator anim;

    public GameObject cam;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        if (pv.IsMine == false)
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
        if (pv.IsMine == false)
        {
            return;
        }
        Movement();
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

}