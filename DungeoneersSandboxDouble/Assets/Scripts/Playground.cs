using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using RedExtentions;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Playground : MonoBehaviourPunCallbacks
{
	[SerializeField]
	GameObject LoadingScreen;
	[SerializeField]
	GameObject ConnectScreen;
	[SerializeField]
	Text ErrorScreen;
	[SerializeField]
	GameObject RoomList;
	[SerializeField]
	GameObject PlayerList;
	[SerializeField]
	GameObject RoomListItem;
	[SerializeField]
	GameObject PlayerListItem;
	[SerializeField]
	GameObject CreateRoomDialog;
	[SerializeField]
	Text RoomTitle;
	[SerializeField]
	GameObject FaildToConnectScreen;
	[SerializeField]
	Sprite m_defaultPlayerImage;
	[SerializeField]
	GameObject BaseMiniture;

	GameObject roomListContent;
	GameObject playerListContent;


	//public static Dictionary<string, RoomInfo> rooms;
	//public static Dictionary<string, PlayerListItem> players;

	bool connectionTimeOut = false;
	static bool firstConnect = false;

	bool joiningAsDM = false;
	bool joiningAsPlayer = false;

	public bool JoinAsDM { get { return joiningAsDM; } set { joiningAsDM = value; } }
    // Start is called before the first frame update
    void Start()
    {
		if(GlobalVariables.userImage == null || (GlobalVariables.userName == null || GlobalVariables.userName == string.Empty))
		{
			GlobalVariables.LoadSettings();
		}
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
		playerListContent = PlayerList.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
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
		if (joiningAsDM)
		{
			PhotonNetwork.LocalPlayer.CustomProperties["isDM"] = joiningAsDM;
		}
		PhotonNetwork.CreateRoom(_roomName.text);
		//joiningAsDM = true;
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
		if (joiningAsDM)
		{
			PhotonNetwork.LocalPlayer.CustomProperties["isDM"] = joiningAsDM;
		}
		PhotonNetwork.JoinRoom(_room.text);
	}

	public void ConnectToRoom(string _room)
	{
		if (joiningAsDM)
		{
			PhotonNetwork.LocalPlayer.CustomProperties["isDM"] = joiningAsDM;
		}
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
		PhotonNetwork.NickName = GlobalVariables.userName;
		Hashtable playerProperties = new Hashtable();
		Texture2D scaledPic = new Texture2D(2, 2);
		scaledPic.LoadImage(GlobalVariables.userImage.sprite.texture.EncodeToPNG());
		scaledPic.ScaleBilinear(128,128);
		byte[] userImage = scaledPic.EncodeToJPG();
		playerProperties["playerImage"] = userImage;
		bool DM = false;
		playerProperties["isDM"] = DM;
		PhotonNetwork.LocalPlayer.CustomProperties = playerProperties;
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("WELCOME TO THE " + PhotonNetwork.CurrentRoom.Name.ToUpper() + " ROOM!");
		RoomTitle.transform.parent.parent.gameObject.SetActive(true);
		RoomTitle.text = PhotonNetwork.CurrentRoom.Name;
		PlayerList.SetActive(true);
		if (GlobalVariables.players == null) 
		{
			GlobalVariables.players = new Dictionary<int, PlayerListItem>();
		}
		foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
		{
			GameObject newEntry = Instantiate(PlayerListItem, playerListContent.transform);
			PlayerListItem newPLI = newEntry.GetComponent<PlayerListItem>();
			newPLI.player = player.Value; 
			if (player.Value.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				newPLI.PlayerImage = GlobalVariables.userImage.sprite;
				newPLI.PlayerType |= GlobalVariables.YOU;
			}
			else
			{

			}

			if(PhotonNetwork.CurrentRoom.MasterClientId == player.Value.ActorNumber)
			{
				newPLI.PlayerType |= GlobalVariables.SERVER_HOST;
				newPLI.PlayerType |= GlobalVariables.ADMIN; 
			}
			//TODO: Add check for DM after establishing custom properties
			//if(PhotonNetwork.CurrentRoom.masterClientId == player.Value.ActorNumber && joiningAsDM)
			//{
			//	newPLI.PlayerType |= GlobalVariables.DUNGEON_MASTER;
			//}
			if((bool)player.Value.CustomProperties["isDM"])
			{
				newPLI.PlayerType |= GlobalVariables.DUNGEON_MASTER;
			}

			GlobalVariables.players[newPLI.UniqueID] = newPLI;
		}
		for (int i = playerListContent.transform.childCount - 2, j = playerListContent.transform.childCount - 1; i >= 0; i--)
		{
			if (playerListContent.transform.GetChild(i).GetComponent<PlayerListItem>().UniqueID > playerListContent.transform.GetChild(j).GetComponent<PlayerListItem>().UniqueID)
			{
				playerListContent.transform.GetChild(j).SetSiblingIndex(i);
				j = i;
			}
			else
			{
				break;
			}
		}
		GlobalVariables.players[PhotonNetwork.CurrentRoom.masterClientId].transform.SetSiblingIndex(0);
		

		
		//GlobalVariables.players
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		GameObject newEntry = Instantiate(PlayerListItem, playerListContent.transform);
		newEntry.GetComponent<PlayerListItem>().player = newPlayer;
		GlobalVariables.players[newPlayer.ActorNumber] = newEntry.GetComponent<PlayerListItem>();
		if (newPlayer.ActorNumber == PhotonNetwork.MasterClient.ActorNumber)
		{
			GlobalVariables.players[newPlayer.ActorNumber].PlayerType |= GlobalVariables.SERVER_HOST;
		}
		for (int i = playerListContent.transform.childCount - 2; i >= 0; i--)
		{
			if (playerListContent.transform.GetChild(i).GetComponent<PlayerListItem>().UniqueID > newPlayer.ActorNumber)
			{
				GlobalVariables.players[newPlayer.ActorNumber].transform.SetSiblingIndex(i);
			}
			else
			{
				break;
			}
		}
		//TODO: Add check for DM after establishing custom properties
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

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		ErrorScreen.text = "- - ERROR CODE " + returnCode.ToString() + ": " + message + " - -";
		ErrorScreen.transform.parent.gameObject.SetActive(true);
	}

	public override void OnLeftLobby()
	{
		
	}

	public override void OnLeftRoom()
	{
		Debug.Log("SEE YOU LATER!");
		RoomTitle.transform.parent.parent.gameObject.SetActive(false);
		RoomTitle.text = "Room";
		foreach (KeyValuePair<int, PlayerListItem> player in GlobalVariables.players)
		{
			Destroy(player.Value.gameObject);
		}
		PlayerList.SetActive(false);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		Destroy(GlobalVariables.players[otherPlayer.ActorNumber].gameObject);
		GlobalVariables.players.Remove(otherPlayer.ActorNumber);
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
