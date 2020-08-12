using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    public Texture2D thumbnail;
    [SerializeField]
    public string itemID;
    [SerializeField]
    public string name;
    [SerializeField]
    public ItemType itemType;
    [SerializeField]
    public float price;
}
