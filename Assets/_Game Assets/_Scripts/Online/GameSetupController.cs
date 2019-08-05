using Photon.Pun;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    [Header("UseFFAsettings")]
    public bool FFA;
    public Transform[] spawns;

    void Start()
    {
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player");
        if (FFA)
        {
            PhotonNetwork.Instantiate(Path.Combine("Player", "Cowboy"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(Path.Combine("Player", "Cowboy"), Vector3.zero, Quaternion.identity);
        }
    }
}
