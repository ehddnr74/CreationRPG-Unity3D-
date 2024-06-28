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

    // ������ ���� �÷���
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
                    itemDT.amount = 1; // ���ο� �������� ������ 1�� ����
                    itemDT.slot = i; // �߰��� �������� ���� �ε����� ����

                    itemDT.transform.SetParent(slots[i].transform, false);
                    itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
                    itemObj.name = itemToAdd.Name;
                    itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // ���� �߾ӿ� ��ġ
                    break;
                }
            }
        }
        itemsChanged = true; // �������� �߰��Ǿ��� �� �÷��� ����
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
                    // ���� ������ �������� ��� ������ ���ҽ�Ŵ
                    data.amount--;
                    data.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.amount.ToString();
                }
                else
                {
                    // ������ 1�� ��� �������� ����
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }

                if (items[i].ID == 6 && items[i].ID == 7)
                {
                    // ������ 1�� ��� �������� ����
                    items[i] = new Item();
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                }


                itemsChanged = true; // �������� ���ŵǾ��� �� �÷��� ����
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
