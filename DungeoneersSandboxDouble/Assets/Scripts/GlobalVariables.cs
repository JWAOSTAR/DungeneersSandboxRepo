using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public static class GlobalVariables
{
    public static string userName;
	public static Image userImage;
	public static Dictionary<string, RoomInfo> rooms;
	public static Dictionary<int, PlayerListItem> players;

    public const int YOU = 1;
    public const int DUNGEON_MASTER = 2;
    public const int SERVER_HOST = 4;

    public static void LoadSettings()
	{
        string path;
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path + "settings/"))
        {
            Directory.CreateDirectory(path + "settings/");
        }
        path += "settings/";
        //string name;
        if (File.Exists(path + "player"))
        {
            BinaryReader file = new BinaryReader(File.Open(path + "player", FileMode.Open));
            userName = file.ReadString();
            int imageSize = file.ReadInt32();
            Texture2D userPic = new Texture2D(2, 2);
            userPic.LoadImage(file.ReadBytes(imageSize));
            //ProfilePicture.sprite = Sprite.Create(userPic, new Rect(0, 0, userPic.width, userPic.height), Vector2.zero);
            //ProfilePicture.color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), 1.0f);
            userImage = (new GameObject()).AddComponent<Image>();
            GameObject.Destroy(userImage.gameObject);
            userImage.sprite = Sprite.Create(userPic, new Rect(0, 0, userPic.width, userPic.height), Vector2.zero);
            userImage.color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), 1.0f);
            file.Close();
        }
        else
        {
            userName = Environment.UserName;
            Texture2D userPic = new Texture2D(2, 2);
            userPic.LoadImage(Resources.Load<Sprite>("DefaultProfile").texture.EncodeToPNG());
            userImage.sprite = Sprite.Create(userPic, new Rect(0, 0, userPic.width, userPic.height), Vector2.zero);
            //ProfilePicture.color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
        }

    }
}
