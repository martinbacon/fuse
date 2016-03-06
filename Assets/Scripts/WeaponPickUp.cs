/*
Fuse - Fuse is an open source action game created in Unity3D

Copyright (C) 2016  Martin Slanina

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WeaponPickUp : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider col)
    {
        if (col.tag=="Player")
        {
            CmdTransmitPickUp(col.transform.name); 
        }
    }

    [Command]
    void CmdTransmitPickUp(string name)
    {
        if (GameObject.Find(name).transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).childCount==1)
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
        Destroy(gameObject);
    }
}
