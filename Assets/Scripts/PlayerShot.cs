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
using CnControls;

public class PlayerShot : NetworkBehaviour {

    float nextFire;
    int damage = 20;
    float range = 300;
    public float fireRate = 2;
    public float shotSpeed = 100;
    public Transform bulletPos;
    private RaycastHit hit;
    Animator anim;

    AudioSource[] sources;
    AudioSource gunSound;

    Color32 myTrail;

    private PlayerController playerScript;

    public string weaponName = "Glock_v3";

    public int ammunition = 20;

    bool empty;

    void Start()
    {
        sources = GetComponents<AudioSource>();
        gunSound = sources[1];
        anim = GetComponent<Animator>();
        playerScript = GetComponent<PlayerController>();
        if (isLocalPlayer)
        {
            myTrail = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isLocalPlayer)
            return;
        //if (Time.time > nextFire + 0.5F && !playerScript.isTyping)
        //{
        //    anim.SetBool("Shoot", false);
        //}
        if (weaponName!="empty")
        {
            //if (CnInputManager.GetButtonDown("Fire1") && Time.time > nextFire && !playerScript.isTyping && !anim.GetCurrentAnimatorStateInfo(1).IsName("IdleGrab_Neutral") && !anim.GetCurrentAnimatorStateInfo(1).IsName("IdleGrab_Neutral 0"))
            //{
            //    //může střelit za (tento čas + rychlost zbraně)
            //    nextFire = Time.time + fireRate;
            //    anim.SetBool("Shoot", true);
            //}
            //if (CnInputManager.GetButtonDown("Fire1") && Time.time > nextFire && !playerScript.isTyping && (anim.GetCurrentAnimatorStateInfo(1).IsName("IdleGrab_Neutral") || anim.GetCurrentAnimatorStateInfo(1).IsName("IdleGrab_Neutral 0")))
            //{
            //    //může střelit za (tento čas + rychlost zbraně)
            //    nextFire = Time.time + fireRate;
            //    TransmitAudioSource(transform.name);
            //    Shoot();
            //    anim.SetBool("Shoot", true);
            //    if (weaponName=="Glock_v3")
            //    {
            //        if (ammunition==0)
            //        {
            //            weaponName = "empty";
            //            TransmitDrop(transform.name);
            //        }
            //        ammunition--;
            //    }
            //}
            if (CnInputManager.GetButtonDown("Fire1") && Time.time > nextFire && !playerScript.isTyping)
            {
                //může střelit za (tento čas + rychlost zbraně)
                nextFire = Time.time + fireRate;
                TransmitAudioSource(transform.name);
                Shoot();
                anim.SetBool("Shoot", true);
                if (weaponName == "Glock_v3")
                {
                    if (ammunition == 0)
                    {
                        weaponName = "empty";
                        TransmitDrop(transform.name);
                    }
                    ammunition--;
                }
            }
            Debug.DrawRay(bulletPos.position, -bulletPos.forward);
            if (GameObject.FindGameObjectsWithTag("Bullet Trail").Length > 10)
            {
                Destroy(GameObject.FindGameObjectWithTag("Bullet Trail"));
            } 
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (weaponName!="empty")
            {
                weaponName = "empty";
                TransmitDrop(transform.name);
            }
        }
        if (empty&& transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).name!= "BulletPos")
        {
            Debug.Log(transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).name);
            if (transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).name== "Glock_v3(Clone)")
            {
                weaponName = "Glock_v3";
                ammunition = 20;
            }
            empty = false;
        }
    }

    void Shoot()
    {
        if (Physics.Raycast(bulletPos.transform.position,-bulletPos.forward,out hit,range))
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.tag=="Player")
            {
                string uID = hit.transform.name;
                damage = Random.Range(17, 28);
                CmdTellServerWhoWasShot(uID, damage);
            }
            if (hit.transform.tag=="Object")
            {
                TransmitForce(hit.transform.gameObject,-bulletPos.forward,hit.point);
            }
            TransmitTrail(hit.point,myTrail);
        }
    }

    public void Footstep(string name)
    {
        TransmitFootstep(name);
    }
    public void Slide(string name)
    {
        TransmitSlide(name);
    }

    [ClientCallback]
    void TransmitDrop(string name)
    {
        CmdTransmitDrop(name);
    }
    [Command]
    void CmdTransmitDrop(string name)
    {
        RpcRecieveDrop(name);
    }
    [ClientRpc]
    void RpcRecieveDrop(string name)
    {
        GameObject gun = GameObject.Find(name).transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;
        gun.transform.parent = null;
        gun.AddComponent<Rigidbody>();
        gun.AddComponent<MeshCollider>();
        gun.GetComponent<MeshCollider>().convex = true;
        empty = true;
    }

    [ClientCallback]
    void TransmitTrail(Vector3 point, Color32 trailColor)
    {
        CmdTransmitTrail(point,trailColor);
    }
    [Command]
    void CmdTransmitTrail(Vector3 point, Color32 trailColor)
    {
        RpcRecieveTrail(point,trailColor);
    }
    [ClientRpc]
    void RpcRecieveTrail(Vector3 point, Color32 trailColor)
    {
        GameObject trail = new GameObject();
        trail.name = "Bullet Trail";
        trail.tag = "Bullet Trail";
        LineRenderer line = trail.AddComponent<LineRenderer>();
        line.SetPosition(0, bulletPos.transform.position);
        line.SetPosition(1, point);
        line.SetWidth(0.03F, 0.03F);
        line.material = Resources.Load("Materials/Bullet Trail") as Material;
        line.material.color = trailColor;
        Instantiate(trail);
    }

    [ClientCallback]
    void TransmitForce(GameObject rHit, Vector3 direction, Vector3 point)
    {
        if (isLocalPlayer)
        {
            CmdTransmitForce(rHit,direction,point);
        }
    }

    [Command]
    void CmdTransmitForce(GameObject rHit, Vector3 direction, Vector3 point)
    {
        RpcRecieveForce(rHit,direction,point); ;
    }

    [ClientRpc]
    void RpcRecieveForce(GameObject rHit, Vector3 direction, Vector3 point)
    {
        rHit.GetComponent<Rigidbody>().AddForceAtPosition(direction * 40, point);
    }

    [ClientCallback]
    void TransmitAudioSource(string name)
    {
        CmdProvideAudioSource(name);
    }

    [Command]
    void CmdProvideAudioSource(string name)
    {
        RpcPlayShootSound(name);
    }
    [ClientRpc]
    void RpcPlayShootSound(string name)
    {
        AudioSource[] sourcesForeign = GameObject.Find(name).GetComponents<AudioSource>();
        AudioSource sound = sourcesForeign[1];
        sound.pitch = Random.Range(0.9F, 1.1F);
        sound.Play();
    }
    //Footsteps
    [ClientCallback]
    void TransmitFootstep(string name)
    {
        CmdProvideFootstep(name);
    }

    [Command]
    void CmdProvideFootstep(string name)
    {
        RpcPlayFootstep(name);
    }

    [ClientRpc]
    void RpcPlayFootstep(string name)
    {
        AudioSource[] sourcesForeign = GameObject.Find(name).GetComponents<AudioSource>();
        AudioSource sound = sourcesForeign[2];
        sound.pitch = Random.Range(0.9F, 1.1F);
        sound.Play();
    }
    //Slide
    [ClientCallback]
    void TransmitSlide(string name)
    {
        CmdProvideSlide(name);
    }

    [Command]
    void CmdProvideSlide(string name)
    {
        RpcPlaySlide(name);
    }

    [ClientRpc]
    void RpcPlaySlide(string name)
    {
        AudioSource[] sourcesForeign = GameObject.Find(name).GetComponents<AudioSource>();
        AudioSource sound = sourcesForeign[3];
        sound.pitch = Random.Range(0.9F, 1.1F);
        sound.Play();
    }

    [Command]
    void CmdTellServerWhoWasShot(string uID, int dmg)
    {
        GameObject player = GameObject.Find(uID);
        player.GetComponent<PlayerHealth>().DeductHealth(dmg);
    }
}
