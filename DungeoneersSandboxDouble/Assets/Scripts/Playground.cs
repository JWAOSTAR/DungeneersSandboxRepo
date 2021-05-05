using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Playground : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		//TODO SET UP MAP AND TILES
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
