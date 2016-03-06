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

public class BlackHole : NetworkBehaviour {

    float nextTime = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other)
    {
        if (Time.time>nextTime)
        {
            nextTime = Time.time + 2F;
            string uID = other.transform.name;
            CmdKillPlayer(uID);
        }
    }

    [Command]
    void CmdKillPlayer(string uID)
    {
        GameObject player = GameObject.Find(uID);
        player.GetComponent<PlayerHealth>().DeductHealth(1000);
    }
}
