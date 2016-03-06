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

public class PlayerRotationSync : NetworkBehaviour {

    [SyncVar]
    private Quaternion playerRotationSync;

    [SerializeField]
    Transform myTransofrm;

    [SerializeField]
    float lerpRate = 15;


    // Use this for initialization
    void Update ()
    {
        LerpRotation();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        TransmitPosition();
        LerpRotation();	
	}

    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            myTransofrm.rotation = Quaternion.Lerp(myTransofrm.rotation, playerRotationSync, lerpRate * Time.deltaTime);
        }
    }

    [Command]
    void CmdProvideRotationToServer(Quaternion pos)
    {
        playerRotationSync = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvideRotationToServer(myTransofrm.rotation);
        }
    }
}
