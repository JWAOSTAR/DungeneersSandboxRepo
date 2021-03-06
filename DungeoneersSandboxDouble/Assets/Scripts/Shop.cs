﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
#if (UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine.Android;
#endif

public class Shop : MonoBehaviour
{
    bool m_modelMobile = false;
    Coroutine m_mobilityCountdown = null;
    //List<ShopItem> m_itmes = new List<ShopItem>();
    Predicate<ShopItem>[] searchFuncs = { FindDice, FindTrays, FindMapBlock, FindMinitures };
    int currentItemType = -1;
    int currentItemPageIndex = 0;
    int currentModelIndex = 0;
    int currentItemIndex = 0;

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
    GameObject collectionsMenu;
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
    [SerializeField]
    GameObject m_collectionNotification;



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

    /// <summary>
    /// After 10 seconds of inaction starts auto rotation
    /// </summary>
    IEnumerator StartRotationCountdown()
    {
        yield return new WaitForSeconds(10);
        m_mover.SetMobility(m_modelMobile);
        m_rotator.SetMobility(!m_modelMobile);
    }

    /// <summary>
    /// Switches between current menu to a given menu reprisented by an intiger value
    /// </summary>
    /// <param name="_menu">Intiger value of the menu to switch to</param>
    public void OpenMenu(int _menu)
    {
        if (menues[2].activeSelf && _menu != 2)
        {
            m_purchaseButton.enabled = false;
            collectionsMenu.transform.GetChild(0).gameObject.SetActive(false);
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

    /// <summary>
    ///Set up item menu filling it up with the stored shop items
    /// </summary>
    /// <param name="_itemType">Interger representing the item type (i.e Dice)</param>
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

    /// <summary>
    /// Set the purchase button to execute the purchase of a given item or make none exacutable if item is already purchased
    /// </summary>
    /// <param name="_item">Index of the item to be purchased on button click</param>
    public void SetPurchaseButton(int _item)
    {
        currentItemIndex = _item;
        collectionsMenu.SetActive(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].isCollection);
        m_purchaseButton.productId = m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex*10].itemID;
        m_purchaseButton.enabled = true;
        m_purchaseButton.onPurchaseComplete.AddListener(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].PurchaseComplete);

        switch ((ShopItem.ItemType)currentItemType)
        {
            case ShopItem.ItemType.Dice:
                {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                    string[] purchasedFiles = Directory.GetFiles("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/");
                    for(int i =0; i < purchasedFiles.Length; i++)
                    {
                        if (!collectionsMenu.activeSelf) {
                            TextAsset bin_asset = Resources.Load(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files[0]) as TextAsset;
                            byte[] temp = (bin_asset).bytes;
                            if (BinaryFilesEqual(purchasedFiles[i], temp))
                            {
                                m_purchaseButton.enabled = false;
                                m_purchaseButton.transform.GetChild(1).gameObject.SetActive(true);
                                break;
                            }
                            else if (i == (purchasedFiles.Length - 1))
                            {
                                m_purchaseButton.enabled = true;
                                m_purchaseButton.transform.GetChild(1).gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            for(int j = 0; j < m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files.Count; j++)
                            {
                                if (BinaryFilesEqual(purchasedFiles[i], Resources.Load<TextAsset>(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files[j]).bytes))
                                {
                                    m_purchaseButton.enabled = false;
                                    m_purchaseButton.transform.GetChild(1).gameObject.SetActive(true);
                                    m_collectionNotification.SetActive(true);
                                    break;
                                }
                                else if (i == (purchasedFiles.Length - 1))
                                {
                                    m_purchaseButton.enabled = true;
                                    m_purchaseButton.transform.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                            if(m_collectionNotification.activeSelf)
                            {
                                break;
                            }
                        }
                    }
#endif
                    BinaryReader file = new BinaryReader(new MemoryStream(Resources.Load<TextAsset>(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files[0]).bytes));

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
                    BinaryReader file = new BinaryReader(new MemoryStream(Resources.Load<TextAsset>(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].files[0]).bytes));

                    int trayType = file.ReadInt32();
                    int matType = file.ReadInt32();

                    //If Material 0 is a texture
                    if ((matType & 4) == 4)
                    {
                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        if (m_renderer.materials[1] == null)
                        {
                            m_renderer.materials[1] = new Material(Shader.Find("Standard"));
                        }
                        m_renderer.materials[1].color = Color.white;
                        m_renderer.materials[1].SetTexture("_MainTex", newTex);
                    }
                    //If Material 0 is a tile
                    else if ((matType & 8) == 8)
                    {
                        //TODO: Add in file readng when shader is written
                    }
                    //IF Material 0 is a color
                    else
                    {
                        m_renderer.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                    }

                    //If Material 1 is a texture
                    if ((matType & 1) == 1)
                    {
                        int arraySize = file.ReadInt32();
                        Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                        newTex.LoadImage(file.ReadBytes(arraySize));
                        if (m_renderer.materials[0] == null)
                        {
                            m_renderer.materials[0] = new Material(Shader.Find("Standard"));
                        }
                        m_renderer.materials[0].color = Color.white;
                        m_renderer.materials[0].SetTexture("_MainTex", newTex);
                    }
                    //If Material 1 is a tile
                    else if ((matType & 2) == 2)
                    {
                        //TODO: Add in file readng when shader is written
                    }
                    //IF Material 1 is a color
                    else
                    {
                        m_renderer.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                    }
                    file.Close();
                    m_mesh.mesh = m_trayMesh[trayType];
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
        if(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].isCollection)
        {
            m_mesh.gameObject.SetActive(false);
            collectionsMenu.transform.GetChild(0).gameObject.SetActive(true);
            collectionsMenu.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].thumbnail, new Rect(0.0f, 0.0f, m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].thumbnail.width, m_itmes.FindAll(searchFuncs[currentItemType])[_item + currentItemPageIndex * 10].thumbnail.height), new Vector2(0.0f, 0.0f));
        }
        //m_mesh
    }

    /// <summary>
    /// Changes model to the next model listed in the current collection in purchase view
    /// </summary>
    public void NextItemInCollection()
    {
        currentModelIndex++;
        
            if (currentModelIndex <= m_itmes.FindAll(searchFuncs[currentItemType])[currentItemIndex + currentItemPageIndex * 10].files.Count)
            {
                if(!m_mesh.gameObject.activeSelf)
                {
                    m_mesh.gameObject.SetActive(true);
                    collectionsMenu.transform.GetChild(0).gameObject.SetActive(false);
                }
                switch ((ShopItem.ItemType)currentItemType)
                {
                    case ShopItem.ItemType.Dice:
                        {
                            BinaryReader file = new BinaryReader(new MemoryStream(Resources.Load<TextAsset>(m_itmes.FindAll(searchFuncs[currentItemType])[currentItemIndex + currentItemPageIndex * 10].files[currentModelIndex - 1]).bytes));

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
                            BinaryReader file = new BinaryReader(new MemoryStream(Resources.Load<TextAsset>(m_itmes.FindAll(searchFuncs[currentItemType])[currentItemIndex + currentItemPageIndex * 10].files[currentModelIndex - 1]).bytes));

                            int trayType = file.ReadInt32();
                            int matType = file.ReadInt32();

                            //If Material 0 is a texture
                            if((matType & 4) == 4)
                            {
                                int arraySize = file.ReadInt32();
                                Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                                newTex.LoadImage(file.ReadBytes(arraySize));
                                if(m_renderer.materials[1] == null)
                                {
                                    m_renderer.materials[1] = new Material(Shader.Find("Standard"));
                                }
                                m_renderer.materials[1].color = Color.white;
                                m_renderer.materials[1].SetTexture("_MainTex", newTex);
                            }
                            //If Material 0 is a tile
                            else if((matType & 8) == 8)
                            {
                                //TODO: Add in file readng when shader is written
                            }
                            //IF Material 0 is a color
                            else
                            {
                                m_renderer.materials[1].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                            }

                            //If Material 1 is a texture
                            if ((matType & 1) == 1)
                            {
                                int arraySize = file.ReadInt32();
                                Texture2D newTex = new Texture2D(file.ReadInt32(), file.ReadInt32());
                                newTex.LoadImage(file.ReadBytes(arraySize));
                                if(m_renderer.materials[0] == null)
                                {
                                    m_renderer.materials[0] = new Material(Shader.Find("Standard"));
                                }
                                m_renderer.materials[0].color = Color.white;
                                m_renderer.materials[0].SetTexture("_MainTex", newTex);
                            }
                            //If Material 1 is a tile
                            else if ((matType & 2) == 2)
                            {
                                //TODO: Add in file readng when shader is written
                            }
                            //IF Material 1 is a color
                            else
                            {
                                m_renderer.materials[0].color = new Color(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());
                            }
                            file.Close();
                            m_mesh.mesh = m_trayMesh[trayType];
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
            }
            else
            {
                currentModelIndex = 0;
                m_mesh.gameObject.SetActive(false);
                collectionsMenu.transform.GetChild(0).gameObject.SetActive(true);
            }
        
    }

    /// <summary>
    /// Changes model to the previous model listed in the current collection in purchase view
    /// </summary>
    public void PrevItemInCollection()
    {

    }

    //Search predicates
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

    //[OBSALETE]
    bool FindAllItemType(ShopItem _item, ShopItem.ItemType _itemType)
    {
        return (_item.itemType == _itemType);
    }

    /// <summary>
    /// BinaryFilesEqual checks whether two binary files contain the exact same data
    /// </summary>
    /// <param name="_file_0">Path for the first binary file</param>
    /// <param name="_file_1">Path for the second binary file</param>
    /// <returns></returns>
    bool BinaryFilesEqual(string _file_0, string _file_1)
    {
        byte[] _bytes_0 = File.ReadAllBytes(_file_0);
        byte[] _bytes_1 = File.ReadAllBytes(_file_1);

        if(_bytes_0.Length == _bytes_1.Length)
        {
            if(_bytes_0.Take(_bytes_0.Length).SequenceEqual(_bytes_1.Take(_bytes_1.Length)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// BinaryFilesEqual checks whether two binary files contain the exact same data
    /// </summary>
    /// <param name="_file_0">Path for the first binary file</param>
    /// <param name="_file_1">Array of bytes contained in the second file</param>
    /// <returns></returns>
    bool BinaryFilesEqual(string _file_0, byte[] _file_1)
	{
        byte[] _bytes_0 = File.ReadAllBytes(_file_0);
        //byte[] _bytes_1 = File.ReadAllBytes(_file_1);

        if (_bytes_0.Length == _file_1.Length)
        {
            if (_bytes_0.Take(_bytes_0.Length).SequenceEqual(_file_1.Take(_file_1.Length)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
