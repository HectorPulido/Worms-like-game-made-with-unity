using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class WormyHealth : NetworkBehaviour
{
    [SyncVar (hook = "UpdateHealth")]
    public int health;

    public int maxHealth;
    public Text txtHealth;

	void Start ()
    {
        if (!isServer)
            return;

        health = maxHealth;
    }

    public void ChangeHealth(int change)
    {
        if (!isServer)
            return;

        health += change;
        if (health > maxHealth)
        {
            health = maxHealth;
        }       
    }

    public void UpdateHealth(int h)
    {
        health = h;
        txtHealth.text = health.ToString();
    }
}
