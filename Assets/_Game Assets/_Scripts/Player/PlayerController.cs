using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public float speed;

    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine == false)
        {
            return;
        }
    }

    void Update()
    {
        if (pv.IsMine == false)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.forward * -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(transform.right * -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * speed * Time.deltaTime);
        }
    }

    [Header("Raycast")]
    public float raycastRange;
    public LayerMask canHit;

    void OnGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange, canHit ))
        {
            Debug.DrawRay(transform.position, -transform.up * raycastRange, Color.yellow);
            Debug.Log("InGround");
        }
        else
        {
            Debug.Log("InAir");
        }
    }

}
