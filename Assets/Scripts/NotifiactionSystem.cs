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
using System.Collections.Generic;
using UnityEngine.UI;

public class NotifiactionSystem : MonoBehaviour {

    public Text notificationText;
    public Image notificationIcon;
    private Animator anim;
    private AudioSource sound;
    private bool show;
    private float nextTime;
    private int queue;
    private List<string> listText = new List<string>();
    private List<string> listIcon = new List<string>();
    public AudioClip notificationAudioClip;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
        NotificationShow("1", "process");
        NotificationShow("2", "info");
        NotificationShow("3", "process");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (show)
        {
            if (Time.time>nextTime)
            {
                anim.ResetTrigger("Show");
                nextTime = Time.time + 1F;
                show = false;
            }
        }
        if (queue>0&&!show&&Time.time>nextTime)
        {
            notificationText.text = listText[0];
            notificationIcon.sprite = Resources.Load<Sprite>("UI/Icons/" + listIcon[0]);
            anim.SetBool("Show", true);
            show = true;
            sound.PlayOneShot(notificationAudioClip);
            nextTime = Time.time + 6F;
            listText.RemoveAt(0);
            listIcon.RemoveAt(0);
            queue--;
        }
	
	}

    public void NotificationShow(string text,string icon)
    {
        if (!show)
        {
            notificationText.text = text;
            notificationIcon.sprite = Resources.Load<Sprite>("UI/Icons/" + icon);
            anim.SetBool("Show", true);
            show = true;
            sound.PlayOneShot(notificationAudioClip);
            nextTime = Time.time + 6F; 
        }
        else
        {
            listText.Add(text);
            listIcon.Add(icon);
            queue++;
        }

    }
}
