using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    bool m_modelMobile = false;
    Coroutine m_mobilityCountdown = null;
    //List<ShopItem> m_itmes = new List<ShopItem>();
    Predicate<ShopItem>[] searchFuncs = { FindDice, FindTrays, FindMapBlock, FindMinitures };
    int currentItemType = -1;
    int currentItemPageIndex = 0;

    [SerializeField]
    DiceAxisMovement m_mover;
    [SerializeField]
    InfinateRotation m_rotator;
    [SerializeField]
    MeshCollider m_colider;
    [SerializeField]
    MeshFilter m_mesh;
    [SerializeField]
    MeshRenderer m_renderer;
    [SerializeField]
    IAPButton m_purchaseButton;
    [SerializeField]
    GameObject[] menues = new GameObject[3];
    [SerializeField]
    GameObject[] itemPannels;
    [SerializeField]
    List<ShopItem> m_itmes = new List<ShopItem>();
    [SerializeField]
    Mesh[] m_diceMesh;
    [SerializeField]
    Mesh[] m_trayMesh;



    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < itemPannels.Length; i++)
        {
            itemPannels[i].SetActive(false);
        }
        m_mover.SetMobility(m_modelMobile);
        m_rotator.SetMobility(!m_modelMobile);
        m_colider.sharedMesh = m_mesh.mesh;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButton(0) && !m_modelMobile)
        {
            if(m_mobilityCountdown != null)
            {
                StopCoroutine(m_mobilityCountdown);
                m_mobilityCountdown = null;
            }
            m_modelMobile = true;
            m_mover.SetMobility(m_modelMobile);
            m_rotator.SetMobility(!m_modelMobile);
        }
        else if(m_modelMobile)
        {
            m_modelMobile = false;
            m_mobilityCountdown = StartCoroutine("StartRotationCountdown");
        }
    }

    IEnumerator StartRotationCountdown()
    {
        yield return new WaitForSeconds(10);
        m_mover.SetMobility(m_modelMobile);
        m_rotator.SetMobility(!m_modelMobile);
    }

    public void OpenMenu(int _menu)
    {
        if (menues[2].activeSelf && _menu != 2)
        {
            m_purchaseButton.enabled = false;
        }
        for(int i = 0; i < menues.Length; i++)
        {
            if(i == _menu)
            {
               menues[i].SetActive(true);
            }
            else
            {
                menues[i].SetActive(false);
            }
        }
    }

    public void SetItemMenu(int _itemType)
    {
        for (int j = 0; j < itemPannels.Length; j++)
        {
            itemPannels[j].SetActive(false);
        }
        currentItemType = _itemType;
        List<ShopItem> itemsToDisplay = m_itmes.FindAll(searchFuncs[_itemType]);
        for(int i = 0; i < ((itemsToDisplay.Count < 8) ? itemsToDisplay.Count : 8); i++)
        {
            itemPannels[i].SetActive(true);
            itemPannels[i].transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(itemsToDisplay[i].thumbnail, new Rect(0.0f, 0.0f, itemsToDisplay[i].thumbnail.width, itemsToDisplay[i].thumbnail.height), new Vector2(0.0f, 0.0f));
            itemPannels[i].transform.GetChild(1).GetComponent<Text>().text = itemsToDisplay[i].title;
        }
    }

    public void SetPurchaseButton(int _item)
    {
        m_purchaseButton.productId = m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex*10].itemID;
        m_purchaseButton.enabled = true;
        switch((ShopItem.ItemType)currentItemType)
        {
            case ShopItem.ItemType.Dice:
                {
                    BinaryReader file = new BinaryReader(File.Open(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files[0], FileMode.Open));

                    bool isTexture = file.ReadBoolean();
                    int diceType = file.ReadInt32();
                    if (!isTexture)
                    {
                        m_renderer.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                        m_renderer.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                    }
                    else
                    {
                        int array_size = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(array_size));
                        m_renderer.materials[0].mainTexture = newTex;
                    }
                    
                    file.Close();

                    m_mesh.mesh = m_diceMesh[diceType];
                }
                break;
            case ShopItem.ItemType.DiceTray:
                { 
                    
                }
                break;
            case ShopItem.ItemType.MapBlock:
                {
                    
                }
                break;
            case ShopItem.ItemType.Miniture:
                {

                }
                break;
            default:
                break;
        }
        //m_mesh
    }

    private static bool FindDice(ShopItem _item)
    {
        return (_item.itemType == ShopItem.ItemType.Dice);
    }

    private static bool FindTrays(ShopItem _item)
    {
        return (_item.itemType == ShopItem.ItemType.DiceTray);
    }

    private static bool FindMapBlock(ShopItem _item)
    {
        return (_item.itemType == ShopItem.ItemType.MapBlock);
    }

    private static bool FindMinitures(ShopItem _item)
    {
        return (_item.itemType == ShopItem.ItemType.Miniture);
    }

    bool FindAllItemType(ShopItem _item, ShopItem.ItemType _itemType)
    {
        return (_item.itemType == _itemType);
    }
}
