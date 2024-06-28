using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject slotPanel;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public GameObject content;

    ItemDataBase itemdataBase;

    public int slotAmount;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    // 아이템 변경 플래그
    public bool itemsChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        slotPanel = GameObject.Find("Slot Panel");
        itemdataBase = GetComponent<ItemDataBase>();


        for (int i = 0; i < slotAmount; i++) 
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(content.transform, false);
        }

        AddItem(0);
        AddItem(1);
        AddItem(2);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = itemdataBase.FetchItemByID(id);
        if (itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                    data.amount++;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == -1)
                {
                    items[i] = itemToAdd;
                    GameObject itemObj = Instantiate(inventoryItem);
                    ItemDT itemDT = itemObj.GetComponent<ItemDT>();
                    itemDT.item = itemToAdd;
                    itemDT.amount = 1; // 새로운 아이템의 개수는 1로 설정
                    itemDT.slot = i; // 추가된 아이템의 슬롯 인덱스를 설정

                    itemDT.transform.SetParent(slots[i].transform, false);
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
                    itemObj.name = itemToAdd.Name;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 슬롯 중앙에 배치
                    break;
                }
            }
        }
        itemsChanged = true; // 아이템이 추가되었을 때 플래그 설정
    }

    public void RemoveItem(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();

                if (data.amount > 1 && items[i].ID != 6 && items[i].ID != 7)
                {
                    // 스택 가능한 아이템의 경우 수량을 감소시킴
                    data.amount--;
                    data.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.amount.ToString();
                }
                else
                {
                    // 수량이 1인 경우 아이템을 제거
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }

                if (items[i].ID == 6 && items[i].ID == 7)
                {
                    // 수량이 1인 경우 아이템을 제거
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }


                itemsChanged = true; // 아이템이 제거되었을 때 플래그 설정
                break;
            }
        }
    }

    bool CheckIfItemIsInInventory(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
                return true;
        }
        return false;
    }

}
