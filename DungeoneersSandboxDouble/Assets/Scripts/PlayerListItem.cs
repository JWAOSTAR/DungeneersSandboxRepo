using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour
{

    [SerializeField]
    Image m_playerImage;

    [SerializeField]
    Text playerName;

    [SerializeField]
    GameObject localUserIcon;
    [SerializeField]
    GameObject serverHostIcon;
    [SerializeField]
    GameObject DMIcon;

    Player m_player;

    int uniqueID;
    public Sprite PlayerImage { get { return m_playerImage.sprite; } set { m_playerImage.sprite = value; } }

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
    public string PlayerName { get { return playerName.text; } }

    public Color NameColor { get { return playerName.color; } set { playerName.color = value; } }

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
        } 
    }

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
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
