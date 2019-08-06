using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInterface : MonoBehaviourPun
{

    public Slider healthBar;
    public PlayerBehaviour pb;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        UpdateHealthUI(pb.health);
    }

    private int currentHealth;

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthUI(int health)
    {
        pb.pv.RPC("SyncHealth", RpcTarget.All, health);
    }

    [PunRPC]
    public void SyncHealth(int hp)
    {
        if (pb == null)
        {
            pb = GetComponent<PlayerBehaviour>();
        }
        pb.health = hp;
        healthBar.value = pb.health;
        pb.anim.SetInteger("Health", pb.health);
        if (pb.health <= 0)
        {
            Debug.Log("Die");
            pb.dead = true;
        }
    }

}