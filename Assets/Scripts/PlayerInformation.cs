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
using System.IO;
using System;

public class PlayerInformation : MonoBehaviour {

    public string nickName = "Player";
    public string messageOfTheDayTitle = "Title";
    public string messageOfTheDay = "Text";
    public bool messageOfTheDayEnabled = true;
    public string[] equipment = new string[14];
    public float time = 180;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = "empty";
        }
    }
}
