using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;

    public List<Item> dataBase = new List<Item>();
    private ItemData itemData;

    string path;
    string itemDataFileName = "Item"; // 아이템 데이터 파일명


    private void Awake()
    {
        path = Application.persistentDataPath + "/";

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadItemData();
        ConstructItemDataBase();
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
            dataBase.Add(new Item(item.ID, item.Name, item.Type, item.Description, item.Price, item.SellPrice, item.IconPath,
                item.Stackable, item.ToolTipPath, item.prefabPath, item.dropPrefabPath, item.gold,
                item.requiredLevel, item.increaseAttackPower, item.increaseDefense, item.increaseMaxHP, item.increaseMoveSpeed));
        }
    }

    private void LoadItemData()
    {
        string itemDataPath = path + itemDataFileName + ".json";

        if (File.Exists(itemDataPath))
        {
            string itemDataJson = File.ReadAllText(itemDataPath);
            itemData = JsonConvert.DeserializeObject<ItemData>(itemDataJson);
            Debug.Log("Item data loaded successfully");
        }
    }

}



public class Item
{
    public int ID { get; set; }
    public string Name { get; set; }

    public int requiredLevel { get; set; }
    public int increaseAttackPower { get; set; }
    public int increaseDefense { get; set; }
    public int increaseMaxHP { get; set; }
    public int increaseMoveSpeed { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public int SellPrice { get; set; }
    public string IconPath { get; set; }

    public string ToolTipPath { get; set; }

    public string prefabPath { get; set; }

    public string dropPrefabPath { get; set; }

    public int gold { get; set; }



    [JsonIgnore]
    public Sprite ToolTip { get; set; }
    public Sprite Icon { get; set; }
    public bool Stackable { get; set; }

    public GameObject Prefab { get; set; } // 프리펩 변수 추가

    public GameObject DropPrefab { get; set; } // 드랍 프리펩

    public Item(int id, string name, string type, string description, int price, int sellPrice, string iconPath,
        bool stackable, string toolTipPath, string prefabPath, string dropPrefabPath, int gold,
        int requiredLevel, int increaseAttackPower, int increaseDefense, int increaseMaxHP, int increaseMoveSpeed)
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
        this.dropPrefabPath = dropPrefabPath;
        this.DropPrefab = Resources.Load<GameObject>("Prefabs/" + dropPrefabPath); // 드랍 프리펩 로드 
        this.gold = gold;
        this.requiredLevel = requiredLevel;
        this.increaseAttackPower = increaseAttackPower;
        this.increaseDefense = increaseDefense;
        this.increaseMaxHP = increaseMaxHP;
        this.increaseMoveSpeed = increaseMoveSpeed;
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

