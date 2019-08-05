using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInterface : MonoBehaviourPun
{

    public Slider healthBar;
    private PlayerBehaviour pb;

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthUI()
    {
        healthBar.value = pb.health;
        pb.anim.SetInteger("Health", pb.health);
        if (pb.health <= 0)
        {
            Debug.Log("Die");
            pb.dead = true;
        }
    }

}