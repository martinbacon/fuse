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
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public AudioMixer mixer;

    [SyncVar(hook = "ChangeTime")]
    float time = 180F;

    private double nextTime = 0;

    Text timeText;

    bool showed;

    public GameObject respawnButton;

    [Client]
    public void ChangeTime(float time)
    {
        if (time>=0)
        {
            timeText.text = time.ToString(); 
        }
        if (time == 140) { StartCoroutine(FadeTo("clapVol", 2F)); }
        if (time == 100) { StartCoroutine(FadeTo("hatsVol", 2F)); }
        if (time == 60) { StartCoroutine(FadeTo("kickVol", 2F)); }
        if (time == 20) { StartCoroutine(FadeTo("bassVol", 2F)); }
        if (time < 140 && time != 0) { mixer.SetFloat("clapVol", 0F); }
        if (time < 100 && time != 0) { mixer.SetFloat("hatsVol", 0F); }
        if (time < 60 && time != 0) { mixer.SetFloat("kickVol", 0F); }
        if (time < 20 && time != 0) { mixer.SetFloat("bassVol", 0F); }
        if (time == 10) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/10") as AudioClip); }
        if (time == 9) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/9") as AudioClip); }
        if (time == 8) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/8") as AudioClip); }
        if (time == 7) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/7") as AudioClip); }
        if (time == 6) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/6") as AudioClip); }
        if (time == 5) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/5") as AudioClip); }
        if (time == 4) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/4") as AudioClip); }
        if (time == 3) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/3") as AudioClip); }
        if (time == 2) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/2") as AudioClip); }
        if (time == 1) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/1") as AudioClip); }
        if (time == 0) { GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/voice/male/time_over") as AudioClip); }
    }

    void Update()
    {
        if (isServer)
        {
            if (Time.time > nextTime)
            {
                nextTime = Time.time + 1F;
                time--;
            }
        }
    }

    void Start()
    {

#if UNITY_STANDALONE
        Destroy(GameObject.Find("Joystick Movement"));
        Destroy(GameObject.Find("Joystick Rotation"));
        Destroy(GameObject.Find("Jump"));
        Destroy(GameObject.Find("Fire"));
        Destroy(GameObject.Find("Crouch"));
        Destroy(GameObject.Find("Sprint"));
        Destroy(GameObject.Find("Drop"));
#endif

        timeText = GameObject.Find("UITime").GetComponent<Text>();
        if (isServer)
        {
            time = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>().time;
        }
        mixer.SetFloat("melodyVol", 0F);
        mixer.SetFloat("clapVol", -50F);
        mixer.SetFloat("hatsVol", -50F);
        mixer.SetFloat("kickVol", -50F);
        mixer.SetFloat("bassVol", -50F);
    }

	public void Kill()
    {

    }

    public void Connected()
    {
        PlayerInformation ply = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        if (isServer&&ply.messageOfTheDayEnabled)
        {
            CmdMotd(ply.messageOfTheDayTitle, ply.messageOfTheDay);
        }
    }

    [Command]
    void CmdMotd(string motdTitle, string motdText)
    {
        RpcMotd(motdTitle, motdText);
    }

    [ClientRpc]
    void RpcMotd(string motdTitle, string motdText)
    {
        if (!showed)
        {
            CanvasGroup cg = GameObject.Find("MotdPanel").GetComponent<CanvasGroup>();
            cg.alpha = 1;
            cg.interactable = true;
            GameObject.Find("MotdTitle").GetComponent<Text>().text = motdTitle;
            GameObject.Find("MotdText").GetComponent<Text>().text = motdText;
            showed = true;
        }
    }

    public void HideMotd()
    {
        DestroyObject(GameObject.Find("MotdPanel"));
    }

    IEnumerator FadeTo(string name, float aTime)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            float volume = Mathf.Lerp(-50, 0, t);
            mixer.SetFloat(name, volume);
            yield return null;
        }
    }
}
