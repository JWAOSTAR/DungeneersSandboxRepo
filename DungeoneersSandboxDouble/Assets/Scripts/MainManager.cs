using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    InputField Username;
    [SerializeField]
    Image ProfilePicture;
    [SerializeField]
    Text ProfilePictureFileName;
    [SerializeField]
    Sprite DefaultPicture;
    // Start is called before the first frame update
    void Start()
    {
        LoadGameSettings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UploadUserPic()
	{
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        OpenFileDialog UserPicOpenDialog = new OpenFileDialog();
        UserPicOpenDialog.Filter = "PNG files (*.png)|*.png|JPG files (*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
        UserPicOpenDialog.FilterIndex = 0;
        UserPicOpenDialog.RestoreDirectory = true;
        if (UserPicOpenDialog.ShowDialog() == DialogResult.OK)
        {
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage(File.ReadAllBytes(UserPicOpenDialog.FileName));
            ProfilePicture.sprite = Sprite.Create(newTex, new Rect(0,0,newTex.width,newTex.height), Vector2.zero);
            ProfilePicture.color = Color.white;
            ProfilePictureFileName.text = UserPicOpenDialog.FileName.Split('\\')[UserPicOpenDialog.FileName.Split('\\').Length - 1];
        }
#endif
    }

    public void SetScreenName(String _username)
	{
        //GlobalVariables.userName = _username;
        Username.SetTextWithoutNotify(_username);
    }

    public void LoadGameSettings()
    {
        string path;
        //TODO: Add load of settings file
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path + "settings/"))
        {
            Directory.CreateDirectory(path + "settings/");
        }
        path += "settings/";
        string name;
        if (File.Exists(path + "player"))
        {
            BinaryReader file = new BinaryReader(File.Open(path + "player", FileMode.Open));
            name = file.ReadString();
            int imageSize = file.ReadInt32();
            Texture2D userPic = new Texture2D(2, 2);
            userPic.LoadImage(file.ReadBytes(imageSize));
            ProfilePicture.sprite = Sprite.Create(userPic, new Rect(0, 0, userPic.width, userPic.height), Vector2.zero);
            ProfilePicture.color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), 1.0f);
            file.Close();
        }
        else
        {
            name = Environment.UserName;
            ProfilePicture.color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1.0f);
        }
        Texture2D cpyTex = new Texture2D(2, 2);
        cpyTex.LoadImage(ProfilePicture.sprite.texture.EncodeToPNG());
        GlobalVariables.userImage = (new GameObject()).AddComponent<Image>();
        GameObject.Destroy(GlobalVariables.userImage.gameObject);
        GlobalVariables.userName = Username.text = name;
        GlobalVariables.userImage.sprite = Sprite.Create(cpyTex, new Rect(0, 0, cpyTex.width, cpyTex.height), Vector2.zero);
        GlobalVariables.userImage.color = new Color(ProfilePicture.color.r, ProfilePicture.color.g, ProfilePicture.color.b, 1.0f);
    }
    public void SaveGameSettings()
	{
        string path;
        //TODO: Add load of settings file
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        path = "C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/";
#endif
        if (!Directory.Exists(path + "settings/"))
        {
            Directory.CreateDirectory(path + "settings/");
        }
        path += "settings/";
        //string name;
        BinaryWriter file = new BinaryWriter(File.Open(path + "player", FileMode.OpenOrCreate));
        file.Write(Username.text);
        file.Write(ProfilePicture.sprite.texture.EncodeToPNG().Length);
        file.Write(ProfilePicture.sprite.texture.EncodeToPNG());
        file.Write(ProfilePicture.color.r);
        file.Write(ProfilePicture.color.g);
        file.Write(ProfilePicture.color.b);
        file.Close();

        GlobalVariables.userName = Username.text = name;
        GlobalVariables.userImage = ProfilePicture;
    }        

    public void OnCancel()
	{
        //TODO: Add default image
        //ProfilePicture.sprite = 
        Username.text = GlobalVariables.userName;
        ProfilePicture.sprite = GlobalVariables.userImage.sprite;
        ProfilePicture.color = GlobalVariables.userImage.color;
    }
}
