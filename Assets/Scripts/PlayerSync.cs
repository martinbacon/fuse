using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings (channel =0,sendInterval = 0.1F)]
public class PlayerSync : NetworkBehaviour {

    [SyncVar (hook ="SyncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransofrm;

    [SerializeField]
    private float lerpRate = 15;
    private float normalLerpRate = 18;
    private float fasterLerpRate = 27;

    private List<Vector3> syncPosList = new List<Vector3>();
    [SerializeField]
    private bool useHistoricalLepring = false;

    private float closeEnough = 0.1F;

	
    void Update()
    {
        LerpPosition();
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        TransmitPosition();
	}

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            if (useHistoricalLepring)
            {
                HistoricalLerping();
            }
            else
            {
                NormalLerping();
            }
        }
    }

    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePositionToServer(myTransofrm.position);
        }
    }

    [Client]
    void SyncPositionValues(Vector3 latestPos)
    {
        syncPos = latestPos;
        syncPosList.Add(syncPos);
    }

    void NormalLerping()
    {
        myTransofrm.position = Vector3.Lerp(myTransofrm.position, syncPos, Time.deltaTime * lerpRate);
    }

    void HistoricalLerping()
    {
        if (syncPosList.Count>0)
        {
            myTransofrm.position = Vector3.Lerp(myTransofrm.position, syncPosList[0], Time.deltaTime * lerpRate);
            if (Vector3.Distance(myTransofrm.position,syncPosList[0])< closeEnough)
            {
                syncPosList.RemoveAt(0);
            }

            if (syncPosList.Count>10)
            {
                lerpRate = fasterLerpRate;
            }
            else
            {
                lerpRate = normalLerpRate;
            }
        }

    }
}
