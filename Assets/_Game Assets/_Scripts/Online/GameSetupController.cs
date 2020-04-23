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
    [Space]
    public GameObject characterSelect;
    public int currentSelectedPlayer;
    [Header("Time Settings")]
    public float maxTime;
    public float timer;
    public float timeGoneBy;

    private string niceTime;
    private BattleUI bu;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        bu = FindObjectOfType<BattleUI>();
        if (FFA)
        {
            StartCoroutine(SpawnAmmoCrate());
        }
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Timer());
            StartCoroutine(UpdateTimer());
        }
    }

    IEnumerator UpdateTimer()
    {
        while (timeGoneBy >= 0)
        {
            yield return new WaitForSeconds(1f);
            bu.UpdateTimer(niceTime);
        }
    }
    
    IEnumerator Timer()
    {
        timeGoneBy = maxTime - timer;
        while (timeGoneBy >= 0)
        {
            timer += Time.deltaTime;
            timeGoneBy = maxTime - timer;
            int minutes = Mathf.FloorToInt(timeGoneBy / 60F);
            int seconds = Mathf.FloorToInt(timeGoneBy - minutes * 60);
            niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForEndOfFrame();
        }
        niceTime = string.Format("{0:0}:{1:00}", 0, 0);
        bu.UpdateTimer(niceTime);
    }
    
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
            case 4:
                PhotonNetwork.Instantiate(Path.Combine("Player", "Drunken"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            case 5:
                PhotonNetwork.Instantiate(Path.Combine("Player", "FemPirate"), spawns[Random.Range(0, spawns.Length)].position, Quaternion.identity);
                break;
            default:
                break;
        }
        characterSelect.SetActive(false);
    }

    #region ammoCrates
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
    #endregion
}
