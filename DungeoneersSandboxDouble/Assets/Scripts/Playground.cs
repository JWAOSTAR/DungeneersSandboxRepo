﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

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
	[SerializeField]
	GameObject CreateRoomDialog;
	[SerializeField]
	Text RoomTitle;
	[SerializeField]
	GameObject FaildToConnectScreen;
	[SerializeField]
	Sprite m_defaultPlayerImage;

	GameObject roomListContent;

	Player current;

	//public static Dictionary<string, RoomInfo> rooms;
	//public static Dictionary<string, PlayerListItem> players;

	bool connectionTimeOut = false;
	static bool firstConnect = false;
    // Start is called before the first frame update
    void Start()
    {
		RoomList.SetActive(false);
		RoomListItem.SetActive(false);
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.GameVersion = Application.version;
			PhotonNetwork.ConnectUsingSettings();
			LoadingScreen.SetActive(true);
			StartCoroutine(WaitForConnection());
		}
		if(GlobalVariables.rooms == null)
		{
			GlobalVariables.rooms = new Dictionary<string, RoomInfo>();
		}

		roomListContent = RoomList.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
		
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
		//WaitForConnection();
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

	public void ConnectToRoom(Text _room)
	{
		PhotonNetwork.JoinRoom(_room.text);
	}

	public void ConnectToRoom(string _room)
	{
		PhotonNetwork.JoinRoom(_room);
	}

	public void DisconnectFromRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("CONNECTED TO SERVER");
		LoadingScreen.SetActive(false);
		if (!firstConnect)
		{
			ConnectScreen.SetActive(true);
			StartCoroutine("ConnectScreenTimeOut");
			firstConnect = true;
		}
        PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		//TODO SET UP MAP AND TILES
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("WELCOME TO THE " + PhotonNetwork.CurrentRoom.Name.ToUpper() + " ROOM!");
		RoomTitle.transform.parent.parent.gameObject.SetActive(true);
		RoomTitle.text = PhotonNetwork.CurrentRoom.Name;
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("ROOM CREATED");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		//Instantiate(RoomListItem, RoomListItem.transform.parent).transform.GetComponentInChildren<Text>().text = roomList[i].Name;
		for (int i = 0; i < roomList.Count; i++)
		{
			if (roomList[i].RemovedFromList) 
			{
				GlobalVariables.rooms.Remove(roomList[i].Name);
				Destroy(Array.Find(roomListContent.transform.GetComponentsInChildren<Text>(), p => p.text == roomList[i].Name).transform.parent.gameObject);
				i--;
			}
			else
			{
				
				GameObject newEntry = Instantiate(RoomListItem, roomListContent.transform);
				newEntry.transform.GetComponentInChildren<Text>().text = roomList[i].Name;
				newEntry.SetActive(true);
				for(int j = roomListContent.transform.childCount - 2; j >= 0; j--)
				{
					if(roomListContent.transform.GetChild(j).GetComponentInChildren<Text>().text.CompareTo(roomList[i].Name) > 0)
					{
						newEntry.transform.SetSiblingIndex(j);
					}
					else
					{
						break;
					}
				}
				string roomName = roomList[i].Name;
				newEntry.GetComponent<Button>().onClick.AddListener(delegate { ConnectToRoom(roomName); RoomList.SetActive(false); });
				GlobalVariables.rooms[roomList[i].Name] = roomList[i];
			}
		}
		base.OnRoomListUpdate(roomList);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("DISCONNECTED FROM SERVER: " + cause.ToString());
		firstConnect = false;
	}

	public override void OnLeftLobby()
	{
		
	}

	public override void OnLeftRoom()
	{
		Debug.Log("SEE YOU LATER!");
		RoomTitle.transform.parent.parent.gameObject.SetActive(false);
		RoomTitle.text = "Room";
	}

	IEnumerator ConnectScreenTimeOut()
	{
		yield return new WaitForSeconds(0.8f);
		ConnectScreen.SetActive(false);
	}

	IEnumerator Timmer(float seconds)
	{
		connectionTimeOut = false;
		yield return new WaitForSeconds(seconds);
		connectionTimeOut = true;
	}

	IEnumerator WaitForConnection()
	{
		Coroutine c = StartCoroutine(Timmer(120.0f));
		yield return new WaitUntil(() => PhotonNetwork.IsConnected || connectionTimeOut);
		if (PhotonNetwork.IsConnected)
		{
			StopCoroutine(c);
		}
		else
		{
			connectionTimeOut = false;
			LoadingScreen.SetActive(false);
			FaildToConnectScreen.SetActive(true);
		}
	}
}
