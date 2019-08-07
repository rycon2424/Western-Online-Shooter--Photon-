using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    [Header("UseFFAsettings")]
    public bool FFA;
    public Transform[] spawns;
    public Transform[] ammoCratesSpawn;
    public GameObject ammoCrate;

    void Start()
    {
        CreatePlayer(); //Create a networked player object for each player that loads into the multiplayer scenes.
        if (FFA)
        {
            StartCoroutine(SpawnAmmoCrate());
        }
    }

    IEnumerator SpawnAmmoCrate()
    {
        yield return new WaitForSeconds(15f);
        PhotonNetwork.InstantiateSceneObject(Path.Combine("Objects", "AmmoCrate"), ammoCratesSpawn[Random.Range(0, ammoCratesSpawn.Length)].position, Quaternion.identity);
        StartCoroutine(SpawnAmmoCrate());
    }

    [PunRPC]
    void SpawnWeaponCrate()
    {
        Instantiate(ammoCrate, ammoCratesSpawn[Random.Range(0, ammoCratesSpawn.Length)].position, Quaternion.identity);
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
