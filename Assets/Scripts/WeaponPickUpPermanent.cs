using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WeaponPickUpPermanent : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            CmdTransmitPickUp(col.transform.name);
        }
    }

    [Command]
    void CmdTransmitPickUp(string name)
    {
        if (GameObject.Find(name).transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).childCount == 1)
        {
            RpcRecievePickUp(name);
            //Destroy(gameObject, 0.3F);
        }
    }
    [ClientRpc]
    void RpcRecievePickUp(string name)
    {
        GameObject hand = GameObject.Find(name).transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject;
        GameObject weapon = Instantiate(Resources.Load("Prefabs/Glock_v3") as GameObject);
        weapon.transform.parent = hand.transform;
        weapon.transform.SetAsFirstSibling();
        weapon.transform.localPosition = new Vector3(0, -0.1F, -0.1F);
        weapon.transform.localEulerAngles = new Vector3(0, -90, 180);
        weapon.transform.localScale = Vector3.one;
    }
}
