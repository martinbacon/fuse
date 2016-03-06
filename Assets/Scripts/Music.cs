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

[RequireComponent(typeof (AudioSource))]
public class Music : MonoBehaviour {

    AudioSource zdroj;
    private float[] samps;
    public Transform elementyScale;
    public Transform elementyRotatation;
    public Transform elementyScaleUp;
    Transform[] poleObjektuScale;
    Transform[] poleObjektuRotation;
    Transform[] poleObjektuScaleUp;
    public float lowerBassScale = 20;
    public float lowerBassRotation = 0.5F;
    public float lowerBassScaleUp = 2;

    // Use this for initialization
    void Start ()
    {
        zdroj = GetComponent<AudioSource>();
        samps = new float[1024];
        poleObjektuScale = new Transform[elementyScale.childCount];
        poleObjektuRotation = new Transform[elementyRotatation.childCount];
        poleObjektuScaleUp = new Transform[elementyScaleUp.childCount];
        int i = 0;
        foreach (Transform child in elementyScale)
        {
            poleObjektuScale[i] = child;
            i++;
        }
        i = 0;
        foreach (Transform child in elementyRotatation)
        {
            poleObjektuRotation[i] = child;
            i++;
        }
        i = 0;
        foreach (Transform child in elementyScaleUp)
        {
            poleObjektuScaleUp[i] = child;
            i++;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        zdroj.GetOutputData(samps, 0); // fill array with samples
        float Sum = 0;
        for (int i = 0; i < 1024; i++)
        {
            Sum += samps[i] * samps[i]; // sum squared samples
        }

        float rmsValue = Mathf.Sqrt(Sum / 1024); // rms = square root of average
        float dbValue = 20 * Mathf.Log10(rmsValue / 0.1f); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
        for (int i = 0; i < poleObjektuScale.Length; i++)
        {
            poleObjektuScale[i].localScale = Vector3.Lerp(new Vector3(poleObjektuScale[i].transform.localScale.x, poleObjektuScale[i].transform.localScale.y, poleObjektuScale[i].transform.localScale.z), new Vector3(dbValue / lowerBassScale, dbValue / lowerBassScale, dbValue / lowerBassScale), 10 * Time.deltaTime);
        }
        for (int i = 0; i < poleObjektuRotation.Length; i++)
        {
            poleObjektuRotation[i].eulerAngles = Vector3.Lerp(new Vector3(0, 0, poleObjektuRotation[i].transform.eulerAngles.z), new Vector3(0, 0, poleObjektuRotation[i].transform.eulerAngles.z+(dbValue / lowerBassRotation)), 10 * Time.deltaTime);
        }
        for (int i = 0; i < poleObjektuScaleUp.Length; i++)
        {
            poleObjektuScaleUp[i].localScale = Vector3.Lerp(new Vector3(poleObjektuScaleUp[i].transform.localScale.x, poleObjektuScaleUp[i].transform.localScale.y, poleObjektuScaleUp[i].transform.localScale.z), new Vector3(poleObjektuScaleUp[i].transform.localScale.x, poleObjektuScaleUp[i].transform.localScale.y, dbValue / lowerBassScaleUp), 10 * Time.deltaTime);

        }
        //elementyScale.Rotate(Vector3.forward * Time.deltaTime);
        //elementyRotatation.Rotate(Vector3.forward * Time.deltaTime);
    }
}
