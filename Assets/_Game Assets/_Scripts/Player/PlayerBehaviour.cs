using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBehaviour : MonoBehaviourPun
{

    [Header("Info")]
    public int maxHealth;
    public int health;
    public bool dead;
    public bool canRespawn;
    public CharacterController cc;
    public float defaultPlayerHeight;
    public Vector3 defaultHitBoxOffset;
    
    [Header("Stats")]
    public bool playerRotateWithCam;
    public bool onlineReady;

    [Header("HitBox")]
    public BoxCollider[] hitboxes;

    [Header("AnimationStuff")]
    public Animator anim;
    public Transform lookObj = null;

    [Header("CameraStuff")]
    public GameObject UI;
    public GameObject hitmarker;
    public GameObject blocked;
    public Camera cam;
    public AudioListener al;
    public OrbitCamera oc;

    [Header("CodeReferences")]
    public PlayerCombat pc;
    public PlayerInterface pi;
    public PlayerSound ps;
    public GameSetupController gsc;

    [HideInInspector]
    public PhotonView pv;

    void Awake()
    {
        cam.enabled = false;
        al.enabled = false;
        oc.enabled = false;
        UI.SetActive(false);
        cc = GetComponent<CharacterController>();
        defaultPlayerHeight = cc.height;
        defaultHitBoxOffset = cc.center;
        pv = GetComponent<PhotonView>();
        pi = GetComponent<PlayerInterface>();
        pc = GetComponent<PlayerCombat>();
        ps = GetComponent<PlayerSound>();
        gsc = FindObjectOfType<GameSetupController>();
        anim = GetComponent<Animator>();
        pc.StartPlayerCombat();
        ps.StartPlayerAudio();
        pi.StartPlayerUI();
        health = maxHealth;
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
        }
        UI.SetActive(true);
        anim = GetComponent<Animator>();
        cam.enabled = true;
        al.enabled = true;
        oc.enabled = true;
        if (onlineReady && canRespawn)
        {
            Invoke("JoinServerMessage", 0.5f);
            pv.RPC("Respawn", RpcTarget.All);
        }
        else
        {
            Respawn();
        }
    }

    void JoinServerMessage()
    {
        pi.bui.JoinLeaveGame(pv.Owner.NickName, true);
    }

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float z;
    public void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (pv.IsMine == false && onlineReady == true)
        {
            return;
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
            if (Input.GetKeyDown(KeyCode.P) && canRespawn == true)
            {
                pv.RPC("Respawn", RpcTarget.All);
            }
        }
    }

    public void Death()
    {
        anim.SetBool("Alive", false);
        anim.SetBool("Aim", false);
        pc.sniperScopeUI.SetActive(false);
        pc.ZoomIn(false, pc.weaponZoom, 60);
        pv.RPC("DisableCollider", RpcTarget.All);
    }

    [PunRPC]
    void DisableCollider()
    {
        foreach (BoxCollider bx in hitboxes)
        {
            bx.enabled = false;
        }
        GetComponent<CharacterController>().enabled = false;
    }

    public void EnableRespawn()
    {
        canRespawn = true;
    }

    [PunRPC]
    void Respawn()
    {
        if (gsc != null)
        {
            transform.position = gsc.spawns[Random.Range(0, gsc.spawns.Length)].position;
        }
        GetComponent<CharacterController>().enabled = true;
        health = maxHealth;
        dead = false;
        anim.SetBool("Alive", true);
        pc.typeGun = PlayerCombat.GunType.revolver;
        pc.AssignDamage();
        pc.HideKnife();
        pi.UpdateWeaponUI();
        pi.UpdateHealthUI(health, "", "");
        pc.CanDodgeAgain();
        foreach (BoxCollider bx in hitboxes)
        {
            bx.enabled = true;
        }
        canRespawn = false;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, CharacterRotation, Time.deltaTime * 12);
    }

    [Header("Raycast")]
    public float raycastRange;
    public LayerMask groundLayers;
    public bool onGround;
    public bool coRoRunning;

    void OnGround()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange, groundLayers))
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
        pc.HideKnife();
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