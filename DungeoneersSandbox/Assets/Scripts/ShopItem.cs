using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "DungoneersSandbox/Shop Item", order = 2)]
public class ShopItem : ScriptableObject
{
    enum ItemType
    {
        Dice,
        DiceTray,
        MapBlock,
        Miniture,
    }

    [SerializeField]
    Texture2D thumbnail;
    [SerializeField]
    string itemID;
    [SerializeField]
    string name;
    [SerializeField]
    ItemType itemType;
    [SerializeField]
    float price;
}
