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

    // 아이템 변경 플래그
    public bool itemsChanged = false;

    public bool activeInventory = false;

    public GameObject confirmationDialog; // 확인 대화상자
    public Button confirmButton; // 확인 버튼
    public Button cancelButton; // 취소 버튼
    public TextMeshProUGUI confirmationText; // 확인 메시지 텍스트
    public Image confirmationImage; // 아이템 이미지


    public GameObject stackableConfirmationDialog; // 확인 대화상자
    public Button stackableConfirmButton; // 확인 버튼
    public Button stackableCancelButton; // 취소 버튼
    public TMP_InputField CountInputField; // 판매 수량 입력 필드
    public Image stackableConfirmationImage; // 아이템 이미지

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
        confirmationText.text = $"'{item.Name}'을(를) 판매하시겠습니까?"; // 확인 메시지 설정
        if (item.Name == "혈검 : 적")
        {
            confirmationText.text = $"<color=red>'{item.Name}'</color>을(를) 판매하시겠습니까?";
        }
        else if (item.Name == "성검 : 청")
        {
            confirmationText.text = $"<color=#00FFFF>'{item.Name}'</color>을(를) 판매하시겠습니까?";
        }
        else if (item.Name == "광검 : 황")
        {
            confirmationText.text = $"<color=yellow>'{item.Name}'</color>을(를) 판매하시겠습니까?";
        }
        else
        {
            confirmationText.text = $"<color=white>'{item.Name}'</color>을(를) 판매하시겠습니까?";
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
        CountInputField.text = ""; // 입력 필드 초기화
        stackableConfirmButton.onClick.RemoveAllListeners();
        stackableConfirmButton.onClick.AddListener(() => ConfirmStackableSellItem(item, slot));

        // onValueChanged 이벤트 설정 (slot 값 전달)
        CountInputField.onValueChanged.RemoveAllListeners();
        CountInputField.onValueChanged.AddListener((value) => ValidateInputField(slot));

        // 입력 필드 초기화 및 포커스 설정
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

    public void AddItem(int id, int slotNum, int a) // 장비 착용할 때 
    {
        Item itemToAdd = ItemDataBase.instance.FetchItemByID(id);

        if (items[slotNum].ID == -1)
        {
            items[slotNum] = itemToAdd;
            GameObject itemObj = Instantiate(inventoryItem);
            ItemDT itemDT = itemObj.GetComponent<ItemDT>();
            itemDT.item = itemToAdd;
            itemDT.amount = 1; // 새로운 아이템의 개수는 1로 설정
            itemDT.slot = slotNum; // 추가된 아이템의 슬롯 인덱스를 설정

            itemDT.transform.SetParent(slots[slotNum].transform, false);
            itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
            itemObj.name = itemToAdd.Name;
            itemObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // 슬롯 중앙에 배치            
        }
        itemsChanged = true; // 아이템이 추가되었을 때 플래그 설정
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
                    itemDT.amount = amount; // 새로운 아이템의 개수는 1로 설정
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
                if (data.amount > 1)
                {
                    // 스택 가능한 아이템의 경우 수량을 감소시킴
                    data.amount--;
                }
                else
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

    public void RemoveItem(int itemID, int slotNum)
    {
        ItemDT data = slots[slotNum].transform.GetChild(0).GetComponent<ItemDT>();

        if (data.amount > 1)
        {
            // 스택 가능한 아이템의 경우 수량을 감소시킴
            data.amount--;
        }
        else
        {
            // 수량이 1인 경우 아이템을 제거
            items[slotNum] = new Item();
            Destroy(slots[slotNum].transform.GetChild(0).gameObject);
        }

        itemsChanged = true; // 아이템이 제거되었을 때 플래그 설정
    }

    public void RemoveItem(int itemID, int slotNum, int amount)
    {
        ItemDT data = slots[slotNum].transform.GetChild(0).GetComponent<ItemDT>();

        data.amount -= amount;

        if(data.amount <=0)
        {
            // 수량이 1인 경우 아이템을 제거
            items[slotNum] = new Item();
            Destroy(slots[slotNum].transform.GetChild(0).gameObject);
        }

        itemsChanged = true; // 아이템이 제거되었을 때 플래그 설정
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
            // 인벤토리 정보를 기반으로 슬롯에 아이템 배치
            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                Item item = ItemDataBase.instance.FetchItemByID(inventoryItem.ID);
                items[inventoryItem.slotnum] = item;

                // 슬롯에 아이템 배치
                Slot slot = slots[inventoryItem.slotnum].GetComponent<Slot>();
                slot.UpdateSlot(item, inventoryItem.amount);
            }
        }
    }
    void Update()
    {
        // 아이템이 변경된 경우 인벤토리를 저장
        if (itemsChanged)
        {
            SaveInventory();
            itemsChanged = false; // 플래그 초기화
        }
    }
}
