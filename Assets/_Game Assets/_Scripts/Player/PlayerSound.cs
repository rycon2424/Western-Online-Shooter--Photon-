using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSound : MonoBehaviourPun
{

    public AudioClip[] footSteps;
    public AudioClip[] revolverShots;
    public AudioSource playerSource;
    public AudioSource gunSource;
    public AudioSource hitmarkerSource;

    private PlayerBehaviour pb;

    void Start()
    {
        pb = GetComponent<PlayerBehaviour>();
    }

    public void Audio_FootStep()
    {
        playerSource.clip = footSteps[Random.Range(0, footSteps.Length)];
        playerSource.Play();
    }

    public void Audio_RevolverShot()
    {
        gunSource.clip = revolverShots[Random.Range(0, revolverShots.Length)];
        gunSource.Play();
    }

    public void Audio_HitMarker()
    {
        if (pb.pv.IsMine == false)
        {
            return;
        }
        hitmarkerSource.Play();
    }

}
