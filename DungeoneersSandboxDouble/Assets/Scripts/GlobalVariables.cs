using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GlobalVariables
{
	public static string userName;
	public static Image userImage;
	public static Dictionary<string, RoomInfo> rooms;
	public static Dictionary<string, PlayerListItem> players;
}
