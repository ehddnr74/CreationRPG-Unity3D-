using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EquipmentManager : MonoBehaviour
{
    [System.Serializable]
    public class EquippedItem
    {
        public string slotName;
        public int itemId; // 아이템 ID
        public string itemName; // 아이템 이름
        public string prefabPath; // 프리팹 경로
    } // Json에 저장할 정보 

    [System.Serializable]
    public class EquipmentSlot
    {
        public string slotName;
        public Transform mountPoint;
        public Vector3 rotationOffset;
        public GameObject currentItem;
        public string currentItempath;
    }

    public List<EquipmentSlot> equipmentSlots;

    private ItemDataBase itemDataBase;
    private Equip equip;

    private void Start()
    {
        itemDataBase = GameObject.Find("ItemDataBase").GetComponent<ItemDataBase>();
        equip = GameObject.Find("Equip").GetComponent<Equip>();
        LoadEquippedItems();
    }

    public void EquipItem(GameObject itemPrefab, string slotName, string prefabPath)
    {
        EquipmentSlot slot = equipmentSlots.Find(s => s.slotName == slotName);

        if (slot != null)
        {
            if (slot.currentItem != null)
            {
                Destroy(slot.currentItem);
            }

            if (itemPrefab != null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.mountPoint);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.Euler(slot.rotationOffset);
                slot.currentItem = newItem;
                slot.currentItempath = prefabPath;
            }
            else
            {
                slot.currentItempath = null;
            }
        }
    }

    public void SaveEquippedItems()
    {
        List<EquippedItem> equippedItems = new List<EquippedItem>();

        foreach (var slot in equipmentSlots)
        {
            if (slot.currentItem != null && slot.currentItempath != null)
            {
                Item item = itemDataBase.FetchItemByPrefabPath(slot.currentItempath);

                EquippedItem equippedItem = new EquippedItem
                {
                    slotName = slot.slotName,
                    itemId = item.ID,
                    itemName = item.Name,
                    prefabPath = item.prefabPath
                };
                equippedItems.Add(equippedItem);
            }
        }

        string json = JsonConvert.SerializeObject(equippedItems, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/EquippedItems.json", json);
    }

    public void LoadEquippedItems()
    {
        string path = Application.persistentDataPath + "/EquippedItems.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            List<EquippedItem> equippedItems = JsonConvert.DeserializeObject<List<EquippedItem>>(json);

            foreach (var equippedItem in equippedItems)
            {
                GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/" + equippedItem.prefabPath);
                EquipItem(itemPrefab, equippedItem.slotName, equippedItem.prefabPath);

                Item item = itemDataBase.FetchItemByPrefabPath(equippedItem.prefabPath);

                if (item.Type == "Weapon")
                {
                    LoadEquipUI(3, item); // 3 : Weapon
                }
                else if(item.Type == "Shield")
                {
                    LoadEquipUI(2, item); // 2 : Shield
                }
            }
        }
    }

    void LoadEquipUI(int equipNum, Item item)
    {
        Image equipImage = equip.equipitem[equipNum].GetComponent<Image>();

        if (equipImage != null)
        {
            equipImage.sprite = item.Icon;

            Color tempColor = equipImage.GetComponent<Image>().color;
            tempColor.a = 1f; // 불투명하게 설정 
            equipImage.GetComponent<Image>().color = tempColor;
        }
    }
}
