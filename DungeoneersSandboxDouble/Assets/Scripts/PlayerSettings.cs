using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerSettings : MonoBehaviour
{
    [Space]
    [Header("UIs")]
    [SerializeField]
    GameObject UI;
    [SerializeField]
    ContextMenu AdminMenu;
    [SerializeField]
    ContextMenu DMMenu;
    [SerializeField]
    ContextMenu CommonMenu;

    [Space]
    [Header("UI Elements")]
    [SerializeField]
    ColorPicker m_colorPicker;
    [SerializeField]
    Image m_playerPic;
    [SerializeField]
    InputField m_playerName;


    PlayerListItem selectedPlayer;
    [HideInInspector]
    public PlayerListItem hoverPlayer;
    bool m_overPlayer = false;

    public bool OverPlayer { get { return m_overPlayer; } set { m_overPlayer = value;} }

    public bool UIActive { get { return UI.activeSelf; } set { UI.SetActive(value); } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_overPlayer && Input.GetMouseButtonDown(1))
        {
            selectedPlayer = hoverPlayer;
            //RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit))
            //{
            //    if (hit.collider.gameObject.transform.TryGetComponent<PlayerListItem>(out selectedPlayer))
            //    {
                    if((GlobalVariables.players[PhotonNetwork.LocalPlayer.ActorNumber].PlayerType & GlobalVariables.ADMIN) == GlobalVariables.ADMIN)
					{
                        AdminMenu.ShowMenu();
					}
                    else if((GlobalVariables.players[PhotonNetwork.LocalPlayer.ActorNumber].PlayerType & GlobalVariables.DUNGEON_MASTER) == GlobalVariables.DUNGEON_MASTER)
					{
                        DMMenu.ShowMenu();
					}
                    else if((selectedPlayer.PlayerType & GlobalVariables.YOU) == GlobalVariables.YOU)
					{
                        CommonMenu.ShowMenu();
                    }
            //    }
            //}
        }
    }

	private void OnEnable()
	{
		if(selectedPlayer)
		{
            m_playerPic.sprite = Sprite.Create(selectedPlayer.PlayerImage.texture, new Rect(0, 0, selectedPlayer.PlayerImage.texture.width, selectedPlayer.PlayerImage.texture.height), Vector2.zero);
            m_playerName.text = selectedPlayer.PlayerName;
        }
	}

	public void SetPlayerLableColor(Color _col)
    {
        selectedPlayer.NameColor = _col;
        //TODO: Notify other clients of change
    }

    public void SetPlayerLable(string _name)
	{
        selectedPlayer.PlayerName = _name;
        //TODO: Notify other clients of change
    }

    public void SetPlayerImage()
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
            m_playerPic.sprite = selectedPlayer.PlayerImage = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero);
        }
#endif
        //TODO: Notify other clients of change
    }

    public void SelectPlayer(PlayerListItem _pli)
    {
        m_playerPic.sprite = Sprite.Create(_pli.PlayerImage.texture, new Rect(0, 0, _pli.PlayerImage.texture.width, _pli.PlayerImage.texture.height), Vector2.zero);
        m_playerName.text = _pli.PlayerName;
        //TODO: Add the load for dice set, tray, miniture
        //UI.gameObject.SetActive(true);
        selectedPlayer = _pli;
    }

    public void ShowUI()
	{
        UI.SetActive(true);
        if (selectedPlayer)
        {
            m_playerPic.sprite = Sprite.Create(selectedPlayer.PlayerImage.texture, new Rect(0, 0, selectedPlayer.PlayerImage.texture.width, selectedPlayer.PlayerImage.texture.height), Vector2.zero);
            m_playerName.text = selectedPlayer.PlayerName;
        }
    }
}
