using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    // �̱���
    public static DataManager instance;

    public ItemData itemData; // ItemData ���� �߰�
    string path;

    string itemDataFilename = "Item"; // ������ ������ ���ϸ�

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
        LoadItemData(); // ������ ������ �ε�
    }

    public void LoadItemData()
    {
        string itemDataPath = path + itemDataFilename + ".json";

        if (File.Exists(itemDataPath))
        {
            string itemDataJson = File.ReadAllText(itemDataPath);
            itemData = JsonConvert.DeserializeObject<ItemData>(itemDataJson);
            Debug.Log("Item data loaded successfully");
        }
    }
}
