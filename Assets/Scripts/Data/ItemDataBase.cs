using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public List<Item> dataBase = new List<Item>();
    private ItemData itemData;

    void Awake()
    {
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadItemData();
            itemData = DataManager.instance.itemData;
            ConstructItemDataBase();
        }
    }

    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < dataBase.Count; i++)
        {
            if (dataBase[i].ID == id)
            {
                return dataBase[i];
            }
        }
        return null;
    }
    public Item FetchItemByIconPath(string iconPath)
    {
        for (int i = 0; i < dataBase.Count; i++)
        {
            if (dataBase[i].IconPath == iconPath)
            {
                return dataBase[i];
            }
        }
        return null;
    }

    public Item FetchItemByIcon(Sprite icon)
    {
        for (int i = 0; i < dataBase.Count; i++)
        {
            if (dataBase[i].Icon == icon)
            {
                return dataBase[i];
            }
        }
        return null;
    }

    public Item FetchItemByPrefabPath(string prefabPath)
    {
        for (int i = 0; i < dataBase.Count; i++)
        {
            if (dataBase[i].prefabPath == prefabPath)
            {
                return dataBase[i];
            }
        }
        return null;
    }


    void ConstructItemDataBase()
    {
        foreach (Item item in itemData.items)
        {
            dataBase.Add(new Item(item.ID, item.Name, item.Type, item.Description, item.Price, item.SellPrice, item.IconPath, item.Stackable, item.ToolTipPath, item.prefabPath));
        }
    }
}



public class Item
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public int SellPrice { get; set; }
    public string IconPath { get; set; }

    public string ToolTipPath { get; set; }

    public string prefabPath { get; set; }



    [JsonIgnore]
    public Sprite ToolTip { get; set; }
    public Sprite Icon { get; set; }
    public bool Stackable { get; set; }

    public GameObject Prefab { get; set; } // 프리팹 변수 추가

    public Item(int id, string name, string type, string description, int price, int sellPrice, string iconPath, bool stackable, string toolTipPath, string prefabPath)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Description = description;
        this.Price = price;
        this.SellPrice = sellPrice;
        this.Stackable = stackable;
        this.IconPath = iconPath;
        this.Icon = Resources.Load<Sprite>("Items/" + iconPath);
        this.ToolTipPath = toolTipPath;
        this.ToolTip = Resources.Load<Sprite>("ToolTips/" + toolTipPath);
        this.prefabPath = prefabPath;
        this.Prefab = Resources.Load<GameObject>("Prefabs/" + prefabPath); // 프리팹 로드
    }

    public Item()
    {
        this.ID = -1;
    }
}

[System.Serializable]
public class DropItem
{
    public int itemId;
    public float dropChance;
}

