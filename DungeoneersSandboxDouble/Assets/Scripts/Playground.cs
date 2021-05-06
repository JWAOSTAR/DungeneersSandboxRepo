using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Playground : MonoBehaviourPunCallbacks
{
	[SerializeField]
	GameObject LoadingScreen;
	[SerializeField]
	GameObject ConnectScreen;
	[SerializeField]
	GameObject RoomList;
	[SerializeField]
	GameObject RoomListItem;
    // Start is called before the first frame update
    void Start()
    {
		RoomList.SetActive(false);
		RoomListItem.SetActive(false);
    }

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.N))
		{
			Instantiate(RoomListItem, RoomListItem.transform.parent);
		}
	}

	public void CreateRoom()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = Application.version;
			PhotonNetwork.ConnectUsingSettings();
			LoadingScreen.SetActive(true);
		}
		//TODO: Add dialog to ask for room name, have dialog cal CreateRoom(string)
	}

	public void CreateRoom(Text _roomName)
	{
		PhotonNetwork.CreateRoom(_roomName.text);
	}

	public void ConnectToRoom()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = Application.version;
			PhotonNetwork.ConnectUsingSettings();
			LoadingScreen.SetActive(true);
		}
		//TODO: Add dialog to ask for room name, have dialog cal ConnectToRoom(string)
	}

	public void ConnectToRoom(string _room)
	{
		PhotonNetwork.JoinRoom(_room);
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("CONNECTED TO SERVER");
		LoadingScreen.SetActive(false);
		ConnectScreen.SetActive(true);
		StartCoroutine("ConnectScreenTimeOut");
		base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();
		//TODO SET UP MAP AND TILES
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("ROOM CREATED");
		base.OnCreatedRoom();
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		for(int i = RoomListItem.transform.parent.childCount - 1; i < roomList.Count; i++)
		{
			RoomList.SetActive(true);
			if (i == 0)
			{
				RoomListItem.SetActive(true);
				RoomListItem.transform.GetComponentInChildren<Text>().text = roomList[i].Name;
			}
			else
			{
				Instantiate(RoomListItem, RoomListItem.transform.parent).transform.GetComponentInChildren<Text>().text = roomList[i].Name;
			}
		}
		base.OnRoomListUpdate(roomList);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("DISCONNECTED FROM SERVER: " + cause.ToString());
		base.OnDisconnected(cause);
	}

	IEnumerator ConnectScreenTimeOut()
	{
		yield return new WaitForSeconds(0.8f);
		ConnectScreen.SetActive(false);
	}
}
