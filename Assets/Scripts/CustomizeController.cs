using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class CustomizeController : MonoBehaviour {

    public GameObject heads;

    void Start()
    {
        Load();
    }

    void Load()
    {
        PlayerData data = new PlayerData();
        InventoryList list = new InventoryList();

        Button[] headButtons = heads.GetComponentsInChildren<Button>();
        for (int i = 0; i < 7; i++)
        {
            Text headText = headButtons[i].GetComponentInChildren<Text>();
            //načte text z headName a využije data od hráče (data.head) kde je uloženo ID o itemu co vlastní v každém slotu, proto se použije data.head[i]. př. 0,1,2,3,... -> data.head[0,1,2,3,...] -> ID 0,12,82,30,... -> list.headname[0,12,82,30,...] -> prázdno,super helma,stříbrná helma, čepice,...
            headText.text = list.headName[data.head[i]];
        }
        //najde button který je defaultne nastavený (data.headChecked) a nastaví mu zelenou barvu
        headButtons[data.headChecked].GetComponent<Image>().color = Color.green;
    }

    public void EquipHead(int slot)
    {
        PlayerData data = new PlayerData();
        InventoryList list = new InventoryList();

        Button[] headButtons = heads.GetComponentsInChildren<Button>();
        Text headText = headButtons[slot].GetComponentInChildren<Text>();

        //projede vsechny buttony, získá component Image a nastaví je na bílo
        for (int i = 0; i < 7; i++)
        {
            headButtons[i].GetComponent<Image>().color = Color.white;
        }
        //nastaví máčknutý button na zeleno (veme si component Image)
        headButtons[slot].GetComponent<Image>().color = Color.green;
        data.headChecked = slot;


    }
}

class InventoryList
{
    //jména itemů: ID {0,1,2,3,4} - 0 vždy prázdno
    public string[] headName = { "Prázdno","Dřevěná helma","Kamenná helma","Bronzová helma","Stříbrná helma","Zlatá helma","Diamantová helma"};
}

[Serializable]
class PlayerData
{
    //id itemu co má v inventáři př {0,240,12,70,33,109} - první vždy 0
    public int[] head = {0,1,2,3,4,5,6 };
    public int headChecked=2;
}