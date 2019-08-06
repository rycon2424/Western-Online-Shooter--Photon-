using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{

    [Header("Info")]
    public int health;
    public bool dead;

    [Header("Stats")]
    public bool playerRotateWithCam;
    public bool onlineReady;

    [Header("AnimationStuff")]
    public Animator anim;
    public Transform lookObj = null;

    [Header("CameraStuff")]
    public GameObject UI;
    public GameObject hitmarker;
    public Camera cam;
    public AudioListener al;
    public OrbitCamera oc;

    [Header("CodeReferences")]
    public PlayerCombat pc;
    public PlayerInterface pi;
    public PlayerSound ps;

    [HideInInspector]
    public PhotonView pv;

    void Start()
    {
        cam.enabled = false;
        al.enabled = false;
        oc.enabled = false;
        UI.SetActive(false);
        pv = GetComponent<PhotonView>();
        pi = GetComponent<PlayerInterface>();
        pc = GetComponent<PlayerCombat>();
        ps = GetComponent<PlayerSound>();
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
        Cursor.visible = false;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (dead == false)
        {
            OnGround();
            if (onGround == true && anim.GetBool("Falling") == false)
            {
                Movement();
                pc.Combat();
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("Running", true);
            }
            else
            {
                anim.SetBool("Running", false);
            }
            if (playerRotateWithCam)
            {
                RotateToLook();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                pv.RPC("Respawn", RpcTarget.All);
            }
        }
    }

    public void Death()
    {
        anim.SetBool("Alive", false);
        anim.SetBool("Aim", false);
        pv.RPC("DisableCollider", RpcTarget.All);
    }

    [PunRPC]
    void DisableCollider()
    {
        GetComponent<CharacterController>().enabled = false;
    }

    [PunRPC]
    void Respawn()
    {
        GameSetupController gsc;
        gsc = FindObjectOfType<GameSetupController>();
        transform.position = gsc.spawns[Random.Range(0, gsc.spawns.Length)].position;
        GetComponent<CharacterController>().enabled = true;
        health = 100;
        dead = false;
        anim.SetBool("Alive", true);
        pc.typeGun = PlayerCombat.GunType.noWeapon;
        pi.UpdateWeaponUI();
        pi.UpdateHealthUI(health);
    }

    void Movement()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        anim.SetFloat("Horizontal", x);
        anim.SetFloat("Vertical", z);
    }

    void RotateToLook()
    {
        var CharacterRotation = cam.transform.rotation;
        CharacterRotation.x = 0;
        CharacterRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, CharacterRotation, Time.deltaTime * 8);
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
        if (anim.GetBool("Aim") == true || anim.GetBool("Running") == true || dead == true)
        {
            return;
        }
        anim.SetLookAtWeight(1);
        anim.SetLookAtPosition(lookObj.position);
    }
}