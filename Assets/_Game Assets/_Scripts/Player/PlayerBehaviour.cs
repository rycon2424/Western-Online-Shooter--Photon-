using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{

    [Header("Info")]
    public int health;

    [Header("Stats")]
    public bool isSprinting;
    public bool playerRotateWithCam;
    public bool onlineReady;

    [Header("AnimationStuff")]
    public Animator anim;
    public Transform lookObj = null;

    [Header("CameraStuff")]
    public GameObject UI;
    public Camera cam;
    public AudioListener al;
    public OrbitCamera oc;

    [Header("CodeReferences")]
    public PlayerCombat pc;
    public PlayerInterface pi;

    [HideInInspector]
    public PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        pi = GetComponent<PlayerInterface>();
        pc = GetComponent<PlayerCombat>();
        anim = GetComponent<Animator>();
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        UI.SetActive(true);
        anim = GetComponent<Animator>();
        cam.enabled = true;
        al.enabled = true;
        oc.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float z;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        if (health > 0)
        {
            OnGround();
            if (onGround == true)
            {
                Movement();
                pc.Combat();
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
    public bool coRoRunning;

    void OnGround()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange, canHit))
        {
            Debug.DrawRay(transform.position, -transform.up * raycastRange, Color.yellow);
            StopCoroutine("OffGround");
            coRoRunning = false;
            onGround = true;
        }
        else
        {
            if (coRoRunning == false)
            {
                StartCoroutine("OffGround");
            }
            onGround = false;
        }
        anim.SetBool("Ground", onGround);
    }

    public void HasLanded()
    {
        anim.SetBool("Falling", false);
    }

    IEnumerator OffGround()
    {
        coRoRunning = true;

        yield return new WaitForSeconds(0.5f);

        anim.SetBool("Falling", true);
        
        coRoRunning = false;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (anim.GetBool("Aim") == true || anim.GetBool("Running") == true || anim.GetInteger("Health") < 1)
        {
            return;
        }
        anim.SetLookAtWeight(1);
        anim.SetLookAtPosition(lookObj.position);
    }

}