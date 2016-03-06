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
using UnityEngine.UI;

public class Connect : MonoBehaviour {

    NetworkManager manager;
    public Text ipAddress;
    public string name = "Player 1";
    int numberLine = 0;

    void Start()
    {
        manager = GetComponent<NetworkManager>();
    }

    void Update()
    {

    }

    public void ConnectToServer()
    {
        manager.networkAddress = ipAddress.text;
        manager.StartClient();
    }

    public void ConnectToSpecificServer(string ip)
    {
        manager.networkAddress = ip;
        manager.StartClient();
    }

    public void CreateServer()
    {
        //ply.messageOfTheDayTitle = GameObject.Find("MotdTitle").GetComponent<Text>().text;
        //ply.messageOfTheDay = GameObject.Find("MotdText").GetComponent<Text>().text;
        //ply.messageOfTheDayEnabled = GameObject.Find("MotdToggle").GetComponent<Toggle>().isOn;
        manager.StartHost();
    }

    public void Connected(string name)
    {
        Debug.Log("Připojil se " + name);
    }
}
