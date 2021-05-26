using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

//[RequireComponent(typeof(PhotonView))]
public class PlayerListItem : MonoBehaviour
{

    [SerializeField]
    Image m_playerImage;

    [SerializeField]
    Text playerName;

    [SerializeField]
    MinitureControler miniture;

    [Space]
    [SerializeField]
    GameObject localUserIcon;
    [SerializeField]
    GameObject serverHostIcon;
    [SerializeField]
    GameObject DMIcon;


    Player m_player;
    PhotonView m_photonView;


    int uniqueID;
    public Sprite PlayerImage 
    { 
        get 
        { 
            return m_playerImage.sprite; 
        } 
        set 
        { 
            m_playerImage.sprite = value;
            if (m_photonView != null) {
                m_photonView.RPC("RecivePlayerUpdate", RpcTarget.Others, uniqueID, (int)'p', m_playerImage.sprite.texture.EncodeToJPG());
            }
        } 
    }

    public int UniqueID { get { return uniqueID; } }
    public Player player 
    { 
        set
        { 
            m_player = value;
            playerName.text = value.NickName;
            uniqueID = value.ActorNumber;
            Texture2D newTex = new Texture2D(2, 2);
            newTex.LoadImage((byte[])value.CustomProperties["playerImage"]);
            m_playerImage.sprite = Sprite.Create(newTex, new Rect(0,0,newTex.width,newTex.height), Vector2.zero);
        } 
    }
    public string PlayerName 
    { 
        get 
        { 
            return playerName.text; 
        } 
        set 
        { 
            playerName.text = value;
            BinaryFormatter binConverter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binConverter.Serialize(stream, playerName.text);
            m_photonView.RPC("RecivePlayerUpdate", RpcTarget.Others, uniqueID, (int)'n', stream.ToArray());
        } 
    }

    public Color NameColor 
    { 
        get 
        { 
            return playerName.color; 
        } 
        set 
        { 
            playerName.color = value;
            BinaryFormatter binConverter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binConverter.Serialize(stream, new float[] { playerName.color.r, playerName.color.g, playerName.color.b });
            m_photonView.RPC("RecivePlayerUpdate", RpcTarget.Others, uniqueID, (int)'c', stream.ToArray());
        } 
    }

    int playerType = 0;

    public int PlayerType 
    { 
        get 
        { 
            return playerType; 
        } 
        set 
        {
            playerType = value;
			localUserIcon.SetActive((playerType & GlobalVariables.YOU) == GlobalVariables.YOU);
            serverHostIcon.SetActive((playerType & GlobalVariables.SERVER_HOST) == GlobalVariables.SERVER_HOST);
            DMIcon.SetActive((playerType & GlobalVariables.DUNGEON_MASTER) == GlobalVariables.DUNGEON_MASTER);
            BinaryFormatter binConverter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binConverter.Serialize(stream, playerType);
            m_photonView.RPC("RecivePlayerUpdate", RpcTarget.Others, uniqueID, (int)'t', stream.ToArray());
        } 
    }

    public PhotonView photonView { get { return m_photonView; } }

    void Awake()
	{
        if (GameObject.FindObjectOfType<Playground>() != null)
        {
            gameObject.GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<PlayerSettings>().SelectPlayer(this); });
        }
        if(TryGetComponent<EventTrigger>(out EventTrigger et))
		{
            EventTrigger.Entry func = new EventTrigger.Entry();
            func.eventID = EventTriggerType.PointerEnter;
            func.callback.AddListener((data) => { FindObjectOfType<PlayerSettings>().OverPlayer = true; });
            func.callback.AddListener((data) => { FindObjectOfType<PlayerSettings>().hoverPlayer = this; });
            et.triggers.Add(func);
            func = new EventTrigger.Entry();
            func.eventID = EventTriggerType.PointerExit;
            func.callback.AddListener((data) => { FindObjectOfType<PlayerSettings>().hoverPlayer = null; });
            et.triggers.Add(func);
		}
        m_photonView = FindObjectOfType<Playground>().gameObject.GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_photonView == null)
        {
            m_photonView = FindObjectOfType<Playground>().gameObject.GetComponent<PhotonView>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void SendPlayerUpdate(int playerID, char code, byte[] data)
    //{
    //    Color test = Color.white;
    //}
    public void RecivePlayerUpdate(int code, byte[] data, PhotonMessageInfo info)
	{
        MemoryStream stream = new MemoryStream();
        stream.Write(data, 0, data.Length);
        stream.Seek(0, SeekOrigin.Begin);
        BinaryFormatter binConverter = new BinaryFormatter();
        switch ((char)code)
		{
            case 'c':
                {
                    float[] color = (float[])binConverter.Deserialize(stream);
                    playerName.color = new Color(color[0], color[1], color[2], 1.0f);
                }
                break;
            case 'n':
                playerName.text = (string)binConverter.Deserialize(stream);
                break;
            case 'p':
				{
                    Texture2D newTex = new Texture2D(2, 2);
                    newTex.LoadImage(data);
                    m_playerImage.sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), Vector2.zero);
                }
                break;
            case 't':
                playerType = (int)binConverter.Deserialize(stream);
                break;
            default:
                break;
        }
	}

}
