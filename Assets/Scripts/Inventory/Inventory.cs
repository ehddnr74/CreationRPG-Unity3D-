using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class Inventory : MonoBehaviour
{
    private CameraController cameraController;

    public GameObject inventoryPanel;
    public GameObject slotPanel;
    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public GameObject content;

    private QuickSlot quickSlot;

    public int slotAmount;

    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();

    // ������ ���� �÷���
    public bool itemsChanged = false;

    public bool activeInventory = false;

    public GameObject confirmationDialog; // Ȯ�� ��ȭ����
    public Button confirmButton; // Ȯ�� ��ư
    public Button cancelButton; // ��� ��ư
    public TextMeshProUGUI confirmationText; // Ȯ�� �޽��� �ؽ�Ʈ
    public Image confirmationImage; // ������ �̹���


    public GameObject stackableConfirmationDialog; // Ȯ�� ��ȭ����
    public Button stackableConfirmButton; // Ȯ�� ��ư
    public Button stackableCancelButton; // ��� ��ư
    public TMP_InputField CountInputField; // �Ǹ� ���� �Է� �ʵ�
    public Image stackableConfirmationImage; // ������ �̹���

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();

        slotPanel = GameObject.Find("Slot Panel");

        inventoryPanel = GameObject.Find("Inventory Panel");
        inventoryPanel.SetActive(activeInventory);

        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(inventorySlot));
            slots[i].GetComponent<Slot>().id = i;
            slots[i].transform.SetParent(content.transform, false);
        }

        LoadInventory();

        confirmationDialog.SetActive(false);
        stackableConfirmationDialog.SetActive(false);

        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);

        stackableConfirmButton.onClick.AddListener(OnStackableConfirmButtonClick);
        stackableCancelButton.onClick.AddListener(OnStackableCancelButtonClick);
    }

    public void ShowConfirmationDialog(Item item, int slot)
    {
        DialogManager.instance.ShowDialog(confirmationDialog);
        confirmationImage.sprite = item.Icon;
        confirmationText.text = $"'{item.Name}'��(��) �Ǹ��Ͻðڽ��ϱ�?"; // Ȯ�� �޽��� ����
        if (item.Name == "���� : ��")
        {
            confirmationText.text = $"<color=red>'{item.Name}'</color>��(��) �Ǹ��Ͻðڽ��ϱ�?";
        }
        else if (item.Name == "���� : û")
        {
            confirmationText.text = $"<color=#00FFFF>'{item.Name}'</color>��(��) �Ǹ��Ͻðڽ��ϱ�?";
        }
        else if (item.Name == "���� : Ȳ")
        {
            confirmationText.text = $"<color=yellow>'{item.Name}'</color>��(��) �Ǹ��Ͻðڽ��ϱ�?";
        }
        else
        {
            confirmationText.text = $"<color=white>'{item.Name}'</color>��(��) �Ǹ��Ͻðڽ��ϱ�?";
        }
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => ConfirmSellItem(item, slot));
    }

    private void ConfirmSellItem(Item item, int slot)
    {
        RemoveItem(item.ID, slot);
        DataManager.instance.AddGold(item.SellPrice);
        confirmationDialog.SetActive(false);
    }

    private void OnConfirmButtonClick()
    {
        confirmationDialog.SetActive(false);
    }

    private void OnCancelButtonClick()
    {
        confirmationDialog.SetActive(false);
    }


    public void ShowStackableConfirmationDialog(Item item, int slot)
    {
        DialogManager.instance.ShowDialog(stackableConfirmationDialog);
        stackableConfirmationImage.sprite = item.Icon;
        CountInputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
        stackableConfirmButton.onClick.RemoveAllListeners();
        stackableConfirmButton.onClick.AddListener(() => ConfirmStackableSellItem(item, slot));

        // onValueChanged �̺�Ʈ ���� (slot �� ����)
        CountInputField.onValueChanged.RemoveAllListeners();
        CountInputField.onValueChanged.AddListener((value) => ValidateInputField(slot));

        // �Է� �ʵ� �ʱ�ȭ �� ��Ŀ�� ����
        CountInputField.ActivateInputField();
        CountInputField.Select();
    }

    private void ValidateInputField(int slot)
    {
        int sellAmount;
        if (int.TryParse(CountInputField.text, out sellAmount))
        {
            int availableAmount = slots[slot].GetComponentInChildren<ItemDT>().amount;
            if (sellAmount > availableAmount)
            {
                CountInputField.text = availableAmount.ToString();
            }
        }
    }

    private void ConfirmStackableSellItem(Item item, int slot)
    {
        int sellAmount;
        if (int.TryParse(CountInputField.text, out sellAmount))
        {
            int availableAmount = slots[slot].GetComponentInChildren<ItemDT>().amount;

            if (sellAmount > 0 && sellAmount <= availableAmount)
            {
                RemoveItem(item.ID, slot, sellAmount);
                DataManager.instance.AddGold(item.SellPrice * sellAmount);
                stackableConfirmationDialog.SetActive(false);

                for (int i = 0; i < quickSlot.slotAmount; i++)
                {
                    QuickSlotDT qSlotDT = quickSlot.slots[i].GetComponentInChildren<QuickSlotDT>();
                    if (qSlotDT != null && qSlotDT.iconPath == item.IconPath)
                    {
                        quickSlot.RemoveQuicktSlotItem(qSlotDT.iconPath, qSlotDT.slotNum, sellAmount);
                        break;
                    }
                }
            }
        }
    }

    private void OnStackableConfirmButtonClick()
    {
        stackableConfirmationDialog.SetActive(false);
    }

    private void OnStackableCancelButtonClick()
    {
        stackableConfirmationDialog.SetActive(false);
    }

    public void AddItem(int id)
    {
        Item itemToAdd = ItemDataBase.instance.FetchItemByID(id);
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

    public void AddItem(int id, int slotNum, int a) // ��� ������ �� 
    {
        Item itemToAdd = ItemDataBase.instance.FetchItemByID(id);

        if (items[slotNum].ID == -1)
        {
            items[slotNum] = itemToAdd;
            GameObject itemObj = Instantiate(inventoryItem);
            ItemDT itemDT = itemObj.GetComponent<ItemDT>();
            itemDT.item = itemToAdd;
            itemDT.amount = 1; // ���ο� �������� ������ 1�� ����
            itemDT.slot = slotNum; // �߰��� �������� ���� �ε����� ����

            itemDT.transform.SetParent(slots[slotNum].transform, false);
            itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
            itemObj.name = itemToAdd.Name;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // ���� �߾ӿ� ��ġ            
        }
        itemsChanged = true; // �������� �߰��Ǿ��� �� �÷��� ����
    }

    public void AddItem(int id, int amount)
    {
        Item itemToAdd = ItemDataBase.instance.FetchItemByID(id);
        if (itemToAdd.Stackable && CheckIfItemIsInInventory(itemToAdd))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                    data.amount += amount;
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
                    itemDT.amount = amount; // ���ο� �������� ������ 1�� ����
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
                if (data.amount > 1)
                {
                    // ���� ������ �������� ��� ������ ���ҽ�Ŵ
                    data.amount--;
                }
                else
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

    public void RemoveItem(int itemID, int slotNum)
    {
        ItemDT data = slots[slotNum].transform.GetChild(0).GetComponent<ItemDT>();

        if (data.amount > 1)
        {
            // ���� ������ �������� ��� ������ ���ҽ�Ŵ
            data.amount--;
        }
        else
        {
            // ������ 1�� ��� �������� ����
            items[slotNum] = new Item();
            Destroy(slots[slotNum].transform.GetChild(0).gameObject);
        }

        itemsChanged = true; // �������� ���ŵǾ��� �� �÷��� ����
    }

    public void RemoveItem(int itemID, int slotNum, int amount)
    {
        ItemDT data = slots[slotNum].transform.GetChild(0).GetComponent<ItemDT>();

        data.amount -= amount;

        if(data.amount <=0)
        {
            // ������ 1�� ��� �������� ����
            items[slotNum] = new Item();
            Destroy(slots[slotNum].transform.GetChild(0).gameObject);
        }

        itemsChanged = true; // �������� ���ŵǾ��� �� �÷��� ����
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


    public void SaveInventory()
    {
        List<InventoryItem> inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0 && items[i].ID != -1)
            {
                ItemDT data = slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                inventoryItems.Add(new InventoryItem(items[i].ID, data.amount, data.slot));
            }
        }

        string inventoryDataJson = JsonConvert.SerializeObject(inventoryItems, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/Inventory.json", inventoryDataJson);
    }

    public void LoadInventory()
    {
        string inventoryDataPath = Application.persistentDataPath + "/Inventory.json";
        if (File.Exists(inventoryDataPath))
        {
            string inventoryDataJson = File.ReadAllText(inventoryDataPath);
            List<InventoryItem> inventoryItems = JsonConvert.DeserializeObject<List<InventoryItem>>(inventoryDataJson);

            items.Clear();

            for (int i = 0; i < slotAmount; i++)
            {
                items.Add(new Item());
                slots[i].GetComponent<Slot>().ClearSlot();
            }
            // �κ��丮 ������ ������� ���Կ� ������ ��ġ
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                Item item = ItemDataBase.instance.FetchItemByID(inventoryItem.ID);
                items[inventoryItem.slotnum] = item;

                // ���Կ� ������ ��ġ
                Slot slot = slots[inventoryItem.slotnum].GetComponent<Slot>();
                slot.UpdateSlot(item, inventoryItem.amount);
            }
        }
    }
    void Update()
    {
        // �������� ����� ��� �κ��丮�� ����
        if (itemsChanged)
        {
            SaveInventory();
            itemsChanged = false; // �÷��� �ʱ�ȭ
        }
    }
}
