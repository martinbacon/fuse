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

public class PlayerHealth : NetworkBehaviour {

    [SyncVar (hook ="OnHealthChanged")]
    private int health = 100;
    private Text healthText;
    private bool shouldDie = false;
    public bool isDead = false;

    public delegate void DieDelegate();
    public event DieDelegate EventDie;

    public delegate void RespawnDelegate();
    public event RespawnDelegate EventRespawn;

    private RectTransform bar;

    // Use this for initialization
    void Start ()
    {
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        bar = GameObject.Find("HealthBarEnergy").GetComponent<RectTransform>();
        SetHealthText();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckCondition();
	}

    void CheckCondition()
    {
        if (health<=0&& !shouldDie && !isDead)
        {
            shouldDie = true;
            Debug.Log("ano");
        }

        if (health <=0 && shouldDie)
        {
            if (EventDie!=null)
            {
                EventDie();
            }
            shouldDie = false;
        }

        if (health>0&& isDead)
        {
            if (EventRespawn!=null)
            {
                EventRespawn();
            }
            isDead = false;
        }
    }

    public void ResetHealth()
    {
        health = 100;
    }

    void SetHealthText()
    {
        if (isLocalPlayer)
        {
            float damage = int.Parse(healthText.text) - health;
            if (health<0)
            {
                healthText.text = 0.ToString();
            }
            else
            {
                healthText.text = health.ToString();
            }
            if (health > 0 && isDead)
            {
                bar.anchoredPosition = new Vector2(-7, bar.anchoredPosition.y);
            }
            else
            {
                bar.anchoredPosition = new Vector2(bar.anchoredPosition.x - damage * 9F, bar.anchoredPosition.y);
            }
        }
    }

    public void DeductHealth(int dmg)
    {
        health -= dmg;
    }

    void OnHealthChanged(int hp)
    {
        health = hp;
        SetHealthText();
    }
}
