using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "DungoneersSandbox/Shop Item", order = 2)]
public class ShopItem : ScriptableObject
{
    public enum ItemType
    {
        Dice,
        DiceTray,
        MapBlock,
        Miniture,
    }

    [Serializable]
    public class PurchaseEvent : UnityEvent { }

    [SerializeField]
    public Texture2D thumbnail;
    [SerializeField]
    public string itemID;
    [SerializeField]
    public string title;
    [SerializeField]
    public ItemType itemType;
    [SerializeField]
    public float price;
    [SerializeField]
    public List<string> files = new List<string>();
    [SerializeField]
    public bool isCollection;

    public void PurchaseComplete(Product _product)
    {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/");
        }
        if (!Directory.Exists("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/"))
        {
            Directory.CreateDirectory("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/");
        }

        switch (itemType)
        {
            case ItemType.Dice:
                {
                    for (int id = 0; id < files.Count; id++)
                    {
                        File.Copy(files[id], ("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/dice/skins/purchased/" + files[id].Split('/')[files[id].Split('/').Length - 1]));
                    }
                }
                break;
            case ItemType.DiceTray:
                {
                    for (int idt = 0; idt < files.Count; idt++)
                    {
                        File.Copy(files[idt], ("C:/Users/" + Environment.UserName + "/AppData/Local/DungeoneersSamdbox/diceTrays/skins/purchased/" + files[idt].Split('/')[files[idt].Split('/').Length - 1]));
                    }
                }
                break;
            case ItemType.MapBlock:
                {
                    //
                }
                break;
            case ItemType.Miniture:
                {
                    //
                }
                break;
            default:
                break;
        }
#endif

    }

    public void PurchaseFailed()
    {

    }
}
