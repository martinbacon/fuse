using UnityEngine;
using System.Collections;

public class MenuManagerNew : MonoBehaviour {

    Animator anim;
    AudioSource[] sources;

	// Use this for initialization
	void Start ()
    {
        anim = Camera.main.GetComponent<Animator>();
        sources = GetComponents<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void InventoryClick()
    {
        anim.SetBool("Inventory", true);
        sources[1].Play();

    }
}
