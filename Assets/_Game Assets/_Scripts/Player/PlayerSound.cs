using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSound : MonoBehaviourPun
{

    public AudioClip[] footSteps;
    public AudioClip[] revolverShots;
    public AudioSource audioOutput;

    public void Audio_FootStep()
    {
        audioOutput.clip = footSteps[Random.Range(0, footSteps.Length)];
        audioOutput.Play();
    }

    public void Audio_RevolverShot()
    {
        audioOutput.clip = revolverShots[Random.Range(0, revolverShots.Length)];
        audioOutput.Play();
    }

}
