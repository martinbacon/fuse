﻿/*
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
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {


	public void StartupHost()
    {
        PlayerInformation ply = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        //ply.messageOfTheDayTitle = GameObject.Find("MotdTitle").GetComponent<Text>().text;
        //ply.messageOfTheDay = GameObject.Find("MotdText").GetComponent<Text>().text;
        //ply.messageOfTheDayEnabled = GameObject.Find("MotdToggle").GetComponent<Toggle>().isOn;
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetIPAdress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    public void JoinLocalhost()
    {
        NetworkManager.singleton.networkAddress = "localhost";
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    public void JoinDevServer()
    {
        NetworkManager.singleton.networkAddress = "127.0.0.1";
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    void SetIPAdress()
    {
        string ipAddress = GameObject.Find("IP Input Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 25565;
    }

    //override public void OnServerDisconnect(NetworkConnection conn)
    //{
    //    NetworkServer.DestroyPlayersForConnection(conn);
    //}
}
