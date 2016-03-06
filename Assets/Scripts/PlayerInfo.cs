using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerInfo : NetworkBehaviour {

    [SyncVar]
    public int health = 100;

	public void TakeDamage(int damage)
    {
        if (!isServer)
            return;
        health -= damage;
        if (health<=0)
        {
            //Destroy(gameObject);
        }
        Debug.Log(health);
    }
}
