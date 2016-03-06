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

public class CameraMovement : MonoBehaviour {

    Vector3 offset;
    public GameObject player;
    RaycastHit hit;
    GameObject last;
    MeshRenderer gmMesh;
    bool first = true;
    // Use this for initialization
    void Start ()
    {
        //odečte vzdálenost hráče od kamery
        offset = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player!=null)
        {
            transform.position = player.transform.position + offset;
        }

        //if (Physics.Raycast(transform.TransformPoint(0,0.4F,0),transform.forward,out hit))
        //{
        //    if (hit.transform.tag!="Player")
        //    {
        //        if (!first)
        //        {
        //            gmMesh.material = Resources.Load("Materials/Opaque/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
        //            last.GetComponent<MeshRenderer>().material.color = new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 1F);
        //        }
        //        gmMesh = hit.transform.gameObject.GetComponent<MeshRenderer>();
        //        last = hit.transform.gameObject;
        //        gmMesh.material = Resources.Load("Materials/Transparent/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
        //        gmMesh.material.color= new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 0.4F);
        //        first = false;
        //    }
        //    else
        //    {
        //        if (!first)
        //        {
        //            gmMesh.material = Resources.Load("Materials/Opaque/" + gmMesh.material.name.Substring(0, gmMesh.material.name.Length - 11)) as Material;
        //            last.GetComponent<MeshRenderer>().material.color = new Color(gmMesh.material.color.r, gmMesh.material.color.g, gmMesh.material.color.b, 1F); 
        //        }
        //    }
        //}
        //Debug.DrawRay(transform.TransformPoint(0, 0.4F, 0), transform.forward*10);
	}
}
