using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    // �̱���
    public static DataManager instance;

    public PlayerData playerData;
    public ItemData itemData; // ItemData ���� �߰�

    private Shop shop;
    private Inventory inv;
    private PlayerManager playerManager;

    string path;
    string playerDataFileName = "PlayerData"; // �÷��̾� ������ ���ϸ�
    string itemDataFileName = "Item"; // ������ ������ ���ϸ�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + "/";
    }

    private void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        playerManager = FindObjectOfType<PlayerManager>();

        //LoadPlayerData(); // �÷��̾� ������ �ε�
        LoadItemData(); // ������ ������ �ε�
    }
    public void SavePlayerData()
    {
        string data = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path + playerDataFileName + ".json", data);
    }

    public void LoadPlayerData()
    {
        string playerDataPath = path + playerDataFileName + ".json";
        Debug.Log("Loading Player Data from: " + playerDataPath);

        if (File.Exists(playerDataPath))
        {
            string playerDataJson = File.ReadAllText(playerDataPath);
            Debug.Log("Player Data JSON: " + playerDataJson); // JSON ���� Ȯ��
            playerData = JsonConvert.DeserializeObject<PlayerData>(playerDataJson);
            Debug.Log("Player data loaded successfully: " + (playerData != null ? playerData.name : "null")); // �ε� ��� Ȯ��
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }

    public void LoadItemData()
    {
        string itemDataPath = path + itemDataFileName + ".json";

        if (File.Exists(itemDataPath))
        {
            string itemDataJson = File.ReadAllText(itemDataPath);
            itemData = JsonConvert.DeserializeObject<ItemData>(itemDataJson);
            Debug.Log("Item data loaded successfully");
        }
    }

    public void AddGold(int gold)
    {
        playerData.gold += gold;
        playerManager.UpdateInventoryGoldText(playerData);

        if (shop.visibleShop)
            playerManager.UpdateShopGoldText(playerData);

        SavePlayerData();
    }
    public void LoseGold(int gold)
    {
        if (playerData.gold - gold < 0)
        {
            return;
        }
        playerData.gold -= gold;
        playerManager.UpdateInventoryGoldText(playerData);

        if (shop.visibleShop)
            playerManager.UpdateShopGoldText(playerData);

        SavePlayerData();
    }


}
