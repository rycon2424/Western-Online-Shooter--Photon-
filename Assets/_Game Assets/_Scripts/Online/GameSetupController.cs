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

    public GameObject characterSelect;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (FFA)
        {
            StartCoroutine(SpawnAmmoCrate());
        }
    }

    public int currentSelectedPlayer;
    public void CreatePlayer()
    {
        switch (currentSelectedPlayer)
        {
            case 0:
                PhotonNetwork.Instantiate(Path.Combine("Player", "Cowboy"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            case 1:
                PhotonNetwork.Instantiate(Path.Combine("Player", "Shinobi"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            case 2:
                PhotonNetwork.Instantiate(Path.Combine("Player", "Sherrif"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            case 3:
                PhotonNetwork.Instantiate(Path.Combine("Player", "Doctor"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            default:
                break;
        }
        characterSelect.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        
        //PhotonNetwork.Instantiate(Path.Combine("Player", "Cowboy"), Vector3.zero, Quaternion.identity);
    }

    #region ammoCrates
    IEnumerator SpawnAmmoCrate()
    {
        yield return new WaitForSeconds(30f);
        PhotonNetwork.InstantiateSceneObject(Path.Combine("Objects", "AmmoCrate"), ammoCratesSpawn[Random.Range(0, ammoCratesSpawn.Length)].position, Quaternion.identity);
        StartCoroutine(SpawnAmmoCrate());
    }

    [PunRPC]
    void SpawnWeaponCrate()
    {
        Instantiate(ammoCrate, ammoCratesSpawn[Random.Range(0, ammoCratesSpawn.Length)].position, Quaternion.identity);
    }
    #endregion
}
