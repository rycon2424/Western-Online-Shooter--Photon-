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

    [Header("AnimationStuff")]
    public Animator anim;
    public Transform lookObj = null;

    [Header("CamereStuff")]
    public Camera cam;
    public AudioListener al;
    public OrbitCamera oc;

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
        cam.enabled = true;
        al.enabled = true;
        oc.enabled = true;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float z;
    public void Update()
    {
        OnGround();
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        if (onGround == true)
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
    public bool onGround;
    public bool hardlanding;

    void OnGround()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange, canHit))
        {
            Debug.DrawRay(transform.position, -transform.up * raycastRange, Color.yellow);
            if (anim.GetBool("Ground") == false && hardlanding == true)
            {
                anim.Play("Hard Landing");
                hardlanding = false;
            }
            StopCoroutine("OffGround");
            anim.SetBool("Ground", true);
            onGround = true;
        }
        else
        {
            anim.SetBool("Ground", false);
            if (hardlanding == false)
            {
                StartCoroutine("OffGround");
            }
            onGround = false;
        }
    }
    
    IEnumerator OffGround()
    {
        yield return new WaitForSeconds(0.5f);
        hardlanding = true;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        anim.SetLookAtWeight(1);
        anim.SetLookAtPosition(lookObj.position);
    }

}