using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSound : MonoBehaviourPun
{

    public AudioClip[] footSteps;
    public AudioClip[] revolverShots;
    public AudioClip[] rifleShots;
    public AudioClip[] tommygunShots;
    public AudioSource playerSource;
    public AudioSource gunSource;
    public AudioSource hitmarkerSource;

    private PlayerBehaviour pb;

    public void StartPlayerAudio()
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

    public void Audio_RifleShot()
    {
        gunSource.clip = rifleShots[Random.Range(0, rifleShots.Length)];
        gunSource.Play();
    }

    public void Audio_TommygunShot()
    {
        gunSource.clip = tommygunShots[Random.Range(0, tommygunShots.Length)];
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
