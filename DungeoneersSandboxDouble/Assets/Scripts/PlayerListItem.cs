using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviour
{

    [SerializeField]
    Image m_playerImage;

    [SerializeField]
    Text playerName;

    Player m_player;

    public Sprite PlayerImage { get { return m_playerImage.sprite; } set { m_playerImage.sprite = value; } }
    public Player player 
    { 
        set
        { 
            m_player = value;
            playerName.text = value.NickName;
        } 
    }
    public string PlayerName { get { return playerName.text; } }

    public int playerType = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
