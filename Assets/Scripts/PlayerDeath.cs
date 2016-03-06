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

public class PlayerDeath : NetworkBehaviour {

    private PlayerHealth healthScript;

	// Use this for initialization
	void Start ()
    {
        healthScript = GetComponent<PlayerHealth>();
        healthScript.EventDie += DisablePlayer;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnDisable()
    {
        healthScript.EventDie -= DisablePlayer;
    }

    void DisablePlayer()
    {
        Instantiate(Resources.Load("Prefabs/Ragdoll"), transform.position + new Vector3(0, 1.5F, 0), transform.rotation);
        GetComponent<NetworkTransform>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerShot>().enabled = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer c in renderers)
        {
            c.enabled = false;
        }

        if (isLocalPlayer)
        {
            GetComponent<PlayerController>().enabled = false;
            GameObject.Find("PlayerCamera(Clone)").GetComponent<CameraMovement>().enabled = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().respawnButton.SetActive(true);
        }
        healthScript.isDead = true;
    }
}
