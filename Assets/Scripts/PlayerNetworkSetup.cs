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

using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerNetworkSetup : NetworkBehaviour {

    [SerializeField]
    GameObject PlayerCamera;

    [SerializeField]
    public GameObject PlayerCanvas;

    Animator chatAnimator;
    InputField input;

    Text chatText;
    Text infoText;


    PlayerInformation plyInfo;

    GameManager gm;

    int numberLine = 0;

    NetworkManager manager;

    AudioSource[] sources;
    AudioSource chatSound;

    float nextTime = 4;

    List<string> players = new List<string>();

    bool voiceMenu;

    void Start ()
    {
        if (isLocalPlayer)
        {
            chatText = GameObject.Find("UIChatText").GetComponent<Text>();
            plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
            input = GameObject.Find("UIChat").GetComponent<InputField>();
            chatAnimator = GameObject.Find("UIChat").GetComponent<Animator>();
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            PlayerCamera = Instantiate(Resources.Load("Prefabs/PlayerCamera"), new Vector3(transform.position.x, transform.position.y + 7.7F, transform.position.z - 3.2F), Quaternion.Euler(60, 0, 0)) as GameObject;
            Camera.main.gameObject.SetActive(false);
            GetComponent<PlayerController>().enabled = true;
            GetComponent<PlayerController>().PlayerCameraComponent = PlayerCamera.GetComponent<Camera>();
            PlayerCamera.GetComponent<CameraMovement>().player = gameObject;
            PlayerCamera.GetComponent<CameraMovement>().enabled = true;
            PlayerCamera.GetComponent<AudioListener>().enabled = true;
            PlayerCamera.GetComponent<Camera>().enabled = true;

        }
        PlayerCanvas = Instantiate(Resources.Load("Prefabs/PlayerCanvas"), transform.position+new Vector3(0,2,0), Quaternion.Euler(45, 0, 0)) as GameObject;
        PlayerCanvas.GetComponent<PlayerCanvas>().player = gameObject;
        PlayerCanvas.transform.name = transform.name + "Canvas";
        //SendMyName();
        nextTime = Time.time + 4;

        sources = GetComponents<AudioSource>();
        chatSound = sources[0];
        TransmitPlayerInfo("Připojil se: ");

    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (chatAnimator.GetBool("show"))
                {
                    if (input.text!="")
                    {
                        SendChatMessage();
                        input.text = "";
                        input.DeactivateInputField();
                        chatAnimator.SetBool("show", false);
                    }
                    else
                    {
                        input.DeactivateInputField();
                        chatAnimator.SetBool("show", false);
                    }
                    GetComponent<PlayerController>().isTyping = false;

                    StartCoroutine(FadeTo(0F, 4F));
                }
                else
                {
                    GetComponent<PlayerController>().isTyping = true;
                    chatAnimator.SetBool("show", true);
                    input.ActivateInputField();
                    input.Select();

                    StopAllCoroutines();
                    Text textInfo = GameObject.Find("UIInfo").GetComponent<Text>();
                    textInfo.color = new Color(1, 1, 1, 1);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) && NetworkClient.active)
            {
                StartCoroutine(WaitAndDisconnect(0.2F));
            }
        }
        if (isLocalPlayer&&Time.time>nextTime)
        {
            nextTime = Time.time + 10;
            SendMyName();
        }
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X)&&!voiceMenu)
        {
            voiceMenu = true;
            GameObject.Find("Voice Menu").GetComponent<Text>().text = "(1) Medic" + Environment.NewLine + "(2) Target Engaged" + Environment.NewLine + "(3) Target Destroyed";
        }
        else if (isLocalPlayer && Input.GetKeyDown(KeyCode.X) && voiceMenu)
        {
            voiceMenu = false;
            GameObject.Find("Voice Menu").GetComponent<Text>().text = "";
        }
        if (voiceMenu)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Talk(transform.name, "Sounds/voice/male/war_medic");
                GameObject.Find("Voice Menu").GetComponent<Text>().text = "";
                voiceMenu = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Talk(transform.name, "Sounds/voice/male/war_target_engaged");
                GameObject.Find("Voice Menu").GetComponent<Text>().text = "";
                voiceMenu = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Talk(transform.name, "Sounds/voice/male/war_target_destroyed");
                GameObject.Find("Voice Menu").GetComponent<Text>().text = "";
                voiceMenu = false;
            }
        }
    }

    void OnDestroy()
    {
        Destroy(GameObject.Find(transform.name + "Canvas"));
    }

    [ClientCallback]
    void Talk(string name, string sound)
    {
        CmdTransmitTalk(name, sound);
    }
    [Command]
    void CmdTransmitTalk(string name, string sound)
    {
        RpcRecieveTalk(name, sound);
    }
    [ClientRpc]
    void RpcRecieveTalk(string name, string sound)
    {
        AudioSource[] sourcesForeign = GameObject.Find(name).GetComponents<AudioSource>();
        AudioSource voiceAudioSource = sourcesForeign[4];
        voiceAudioSource.PlayOneShot(Resources.Load(sound) as AudioClip);
    }

    [ClientCallback]
    void SendMyName()
    {
        PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
        if (plyInfo.nickName == "Player" || plyInfo.nickName == "")
        {
            plyInfo.nickName = Environment.UserName;
        }
        CmdAddNickname(transform.name+"Canvas", plyInfo.nickName,plyInfo.equipment);
    }

    [Command]
    void CmdAddNickname(string name, string nickname,string[] equipment)
    {
        RpcSyncNicknames(name,nickname,equipment);
    }

    [ClientRpc]
    void RpcSyncNicknames(string name, string nickname,string[] equipment)
    {
        //string nameWithout = name.Substring(0, name.Length - 6);
        //Debug.Log(players.Count);
        //if (players.Count>0)
        //{
        //    for (int i = 0; i < players.Count; i++)
        //    {
        //        if (players[i] == nameWithout)
        //        {
        //            Debug.Log("Hráč " + nameWithout + " je již registrován");
        //            break;
        //        }
        //        else
        //        {
        //            Debug.Log("Registruji hráče " + nameWithout + " na index " + (players.Count - 1));
        //            players.Add(nameWithout);
        //        }
        //    } 
        //}
        //else
        //{
        //    Debug.Log("Registruji hráče " + nameWithout + " na index " + 0);
        //    players.Add(nameWithout);
        //}
        //GameObject[] playersObject = GameObject.FindGameObjectsWithTag("Player");
        //for (int i = 0; i < players.Count; i++)
        //{
        //    for (int j = 0; j < playersObject.Length; j++)
        //    {
        //        if (playersObject[j].transform.name == players[i])
        //        {
        //            Debug.Log("Hráč " + players[i] + " je stále ve hře");
        //            break;
        //        }
        //        else
        //        {
        //            Debug.Log("Hráč " + players[i] + " už není ve hře");
        //            Destroy(GameObject.Find(players[i] + "Canvas"));
        //        }
        //    }
        //}

        GameObject.Find(name).GetComponentInChildren<Text>().text = nickname;
        GameObject otherPlayer = GameObject.Find(name.Substring(0, name.Length - 6));
        if (otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).childCount == 0 & equipment[0] != "empty")
        {
            GameObject head = Instantiate(Resources.Load(equipment[0]) as GameObject);
            head.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1);
            head.transform.localPosition = Vector3.zero;
            head.transform.localEulerAngles = Vector3.zero;
            head.transform.localScale = Vector3.one; 
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).Find(equipment[1].Substring(equipment[1].LastIndexOf('/') + 1, equipment[1].Length - equipment[1].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[1] != "empty")
        {
            GameObject body = Instantiate(Resources.Load(equipment[1]) as GameObject);
            body.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1);
            body.transform.localPosition = Vector3.zero;
            body.transform.localEulerAngles = Vector3.zero;
            body.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).Find(equipment[2].Substring(equipment[2].LastIndexOf('/') + 1, equipment[2].Length - equipment[2].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[2] != "empty")
        {
            GameObject bodyLelbow = Instantiate(Resources.Load(equipment[2]) as GameObject);
            bodyLelbow.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
            bodyLelbow.transform.localPosition = Vector3.zero;
            bodyLelbow.transform.localEulerAngles = Vector3.zero;
            bodyLelbow.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).Find(equipment[3].Substring(equipment[3].LastIndexOf('/') + 1, equipment[3].Length - equipment[3].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[3] != "empty")
        {
            GameObject bodyLelbow = Instantiate(Resources.Load(equipment[3]) as GameObject);
            bodyLelbow.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0);
            bodyLelbow.transform.localPosition = Vector3.zero;
            bodyLelbow.transform.localEulerAngles = Vector3.zero;
            bodyLelbow.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).Find(equipment[4].Substring(equipment[4].LastIndexOf('/') + 1, equipment[4].Length - equipment[4].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[4] != "empty")
        {
            GameObject bodyRelbow = Instantiate(Resources.Load(equipment[4]) as GameObject);
            bodyRelbow.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
            bodyRelbow.transform.localPosition = Vector3.zero;
            bodyRelbow.transform.localEulerAngles = Vector3.zero;
            bodyRelbow.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).Find(equipment[5].Substring(equipment[5].LastIndexOf('/') + 1, equipment[5].Length - equipment[5].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[5] != "empty")
        {
            GameObject bodyRelbow = Instantiate(Resources.Load(equipment[5]) as GameObject);
            bodyRelbow.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2);
            bodyRelbow.transform.localPosition = Vector3.zero;
            bodyRelbow.transform.localEulerAngles = Vector3.zero;
            bodyRelbow.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(1).Find(equipment[6].Substring(equipment[6].LastIndexOf('/') + 1, equipment[6].Length - equipment[6].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[6] != "empty")
        {
            GameObject pantsBody = Instantiate(Resources.Load(equipment[6]) as GameObject);
            pantsBody.transform.parent = otherPlayer.transform.GetChild(0).GetChild(1);
            pantsBody.transform.localPosition = Vector3.zero;
            pantsBody.transform.localEulerAngles = Vector3.zero;
            pantsBody.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0).Find(equipment[7].Substring(equipment[7].LastIndexOf('/') + 1, equipment[7].Length - equipment[7].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[7] != "empty")
        {
            GameObject pantsLknee = Instantiate(Resources.Load(equipment[7]) as GameObject);
            pantsLknee.transform.parent = otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0);
            pantsLknee.transform.localPosition = Vector3.zero;
            pantsLknee.transform.localEulerAngles = Vector3.zero;
            pantsLknee.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0).Find(equipment[8].Substring(equipment[8].LastIndexOf('/') + 1, equipment[8].Length - equipment[8].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[8] != "empty")
        {
            GameObject pantsRknee = Instantiate(Resources.Load(equipment[8]) as GameObject);
            pantsRknee.transform.parent = otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0);
            pantsRknee.transform.localPosition = Vector3.zero;
            pantsRknee.transform.localEulerAngles = Vector3.zero;
            pantsRknee.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(0).Find(equipment[9].Substring(equipment[9].LastIndexOf('/') + 1, equipment[9].Length - equipment[9].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[9] != "empty")
        {
            GameObject pantsLthigh = Instantiate(Resources.Load(equipment[9]) as GameObject);
            pantsLthigh.transform.parent = otherPlayer.transform.GetChild(0).GetChild(0);
            pantsLthigh.transform.localPosition = Vector3.zero;
            pantsLthigh.transform.localEulerAngles = Vector3.zero;
            pantsLthigh.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(2).Find(equipment[10].Substring(equipment[10].LastIndexOf('/') + 1, equipment[10].Length - equipment[10].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[10] != "empty")
        {
            GameObject pantsRthigh = Instantiate(Resources.Load(equipment[10]) as GameObject);
            pantsRthigh.transform.parent = otherPlayer.transform.GetChild(0).GetChild(2);
            pantsRthigh.transform.localPosition = Vector3.zero;
            pantsRthigh.transform.localEulerAngles = Vector3.zero;
            pantsRthigh.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find(equipment[11].Substring(equipment[11].LastIndexOf('/') + 1, equipment[11].Length - equipment[11].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[11] != "empty")
        {
            GameObject bootsLankle = Instantiate(Resources.Load(equipment[11]) as GameObject);
            bootsLankle.transform.parent = otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            bootsLankle.transform.localPosition = Vector3.zero;
            bootsLankle.transform.localEulerAngles = Vector3.zero;
            bootsLankle.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).Find(equipment[12].Substring(equipment[12].LastIndexOf('/') + 1, equipment[12].Length - equipment[12].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[12] != "empty")
        {
            GameObject bootsRankle = Instantiate(Resources.Load(equipment[12]) as GameObject);
            bootsRankle.transform.parent = otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0);
            bootsRankle.transform.localPosition = Vector3.zero;
            bootsRankle.transform.localEulerAngles = Vector3.zero;
            bootsRankle.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0).Find(equipment[13].Substring(equipment[13].LastIndexOf('/') + 1, equipment[13].Length - equipment[13].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[13] != "empty")
        {
            GameObject bootsLknee = Instantiate(Resources.Load(equipment[13]) as GameObject);
            bootsLknee.transform.parent = otherPlayer.transform.GetChild(0).GetChild(0).GetChild(0);
            bootsLknee.transform.localPosition = Vector3.zero;
            bootsLknee.transform.localEulerAngles = Vector3.zero;
            bootsLknee.transform.localScale = Vector3.one;
        }
        if (otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0).Find(equipment[14].Substring(equipment[14].LastIndexOf('/') + 1, equipment[14].Length - equipment[14].LastIndexOf('/') - 1) + "(Clone)") == null & equipment[14] != "empty")
        {
            GameObject bootsRknee = Instantiate(Resources.Load(equipment[14]) as GameObject);
            bootsRknee.transform.parent = otherPlayer.transform.GetChild(0).GetChild(2).GetChild(0);
            bootsRknee.transform.localPosition = Vector3.zero;
            bootsRknee.transform.localEulerAngles = Vector3.zero;
            bootsRknee.transform.localScale = Vector3.one;
        }
    }

    [ClientCallback]
    void TransmitPlayerInfo(string text)
    {
        if (isLocalPlayer)
        {
            PlayerInformation plyInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
            if (plyInfo.nickName == "Player" || plyInfo.nickName == "")
            {
                plyInfo.nickName = Environment.UserName;
            }
            CmdProvidePlayerInfo(text,plyInfo.nickName);
        }
    }

    [Command]
    void CmdProvidePlayerInfo(string text, string nick)
    {
        RpcPlayerInfo(text,nick);
    }

    [ClientRpc]
    void RpcPlayerInfo(string text,string nickname)
    {
        Debug.Log(text + nickname);
        Text textInfo = GameObject.Find("UIInfo").GetComponent<Text>();
        if (numberLine > 4)
        {
            int index = textInfo.text.IndexOf(System.Environment.NewLine);
            textInfo.text = textInfo.text.Substring(index + System.Environment.NewLine.Length);
        }
        else
        {
            numberLine++;
            RectTransform textTransform = GameObject.Find("UIInfo").GetComponent<RectTransform>();
            textTransform.sizeDelta = new Vector2(textTransform.rect.width, textTransform.rect.height + 46);
        }
        textInfo.color = new Color(1, 1, 1, 1);
        textInfo.text += text + nickname + Environment.NewLine;
        StartCoroutine(FadeTo(0F, 2F));
        if (text=="Připojil se: ")
        {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.Connected();
        }
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        Text textInfo = GameObject.Find("UIInfo").GetComponent<Text>();
        float alpha = textInfo.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            textInfo.color = newColor;
            yield return null;
        }
    }

    [ClientCallback]
    void SendChatMessage()
    {
        CmdProvideChatMessage(plyInfo.nickName, chatText.text);
    }

    [Command]
    void CmdProvideChatMessage(string nick, string message)
    {
        RpcSendChatMessage(nick, message);
    }

    [ClientRpc]
    void RpcSendChatMessage(string nick, string message)
    {
        chatSound.Play();

        Text textInfo = GameObject.Find("UIInfo").GetComponent<Text>();
        if (numberLine > 4)
        {
            int index = textInfo.text.IndexOf(System.Environment.NewLine);
            textInfo.text = textInfo.text.Substring(index + System.Environment.NewLine.Length);
        }
        else
        {
            numberLine++;
            RectTransform textTransform = GameObject.Find("UIInfo").GetComponent<RectTransform>();
            textTransform.sizeDelta = new Vector2(textTransform.rect.width, textTransform.rect.height + 46);
        }
        textInfo.text += nick + ": " + message + Environment.NewLine;
        textInfo.color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeTo(0F, 4F));
    }

    public void CapsuleChange(string name, int state,float height,Vector3 center)
    {
        TransmitCapsuleChange(name, state,height,center);
    }

    [ClientCallback]
    void TransmitCapsuleChange(string name,int state, float height, Vector3 center)
    {
        CmdTransmitCapsuleChange(name,state,height,center);
    }
    [Command]
    void CmdTransmitCapsuleChange(string name,int state, float height, Vector3 center)
    {
        RpcRecieveCapsuleChanged(name,state,height,center);
    }
    [ClientRpc]
    void RpcRecieveCapsuleChanged(string name,int state, float height, Vector3 center)
    {
        if (state == 0)
        {
            GameObject.Find(name).GetComponent<CharacterController>().height = 0.2F;
            GameObject.Find(name).GetComponent<CharacterController>().center = new Vector3(0, 0.6F, 0);
            Debug.Log("Měním na menší");
        }
        else
        {
            GetComponent<CharacterController>().height = height;
            GetComponent<CharacterController>().center = center;
            Debug.Log("Měním zpátky na: " + height + " " + center);
        }
    }

    //[ClientCallback]
    //void DestroyNickname()
    //{
    //    if (isLocalPlayer)
    //    {
    //        CmdDestroyNickname(transform.name + "Canvas");
    //    }
    //}

    //[Command]
    //void CmdDestroyNickname(string name)
    //{
    //    RpcDestroyNickname(name);
    //}

    //[ClientRpc]
    //void RpcDestroyNickname(string name)
    //{
    //    Destroy(GameObject.Find(name));
    //}

    IEnumerator WaitAndDisconnect(float waitTime)
    {
        TransmitPlayerInfo("Odpojil se: ");
        //DestroyNickname();
        yield return new WaitForSeconds(waitTime);
        Destroy(GameObject.Find("PlayerInformation"));
        manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        manager.StopHost();
        //Destroy(GameObject.Find("NetworkManager"));
    }
}
