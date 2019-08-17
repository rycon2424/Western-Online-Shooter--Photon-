using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
    public int damagePerTick;
    public string killer;
    public string killmethod;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerBehaviour>().health -= damagePerTick;
            int health = col.GetComponent<PlayerBehaviour>().health;
            col.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health, killmethod, killer);
            col.GetComponent<PlayerBehaviour>().pi.UpdateLog();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerBehaviour>().health -= damagePerTick;
            int health = col.GetComponent<PlayerBehaviour>().health;
            col.GetComponent<PlayerBehaviour>().pi.UpdateHealthUI(health, killmethod, killer);
            col.GetComponent<PlayerBehaviour>().pi.UpdateLog();
        }
    }
}
