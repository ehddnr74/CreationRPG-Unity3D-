using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Shop : MonoBehaviour
{
    private CameraController cameraController;
    private Inventory inv;
    private QuickSlot quickSlot;

    public int slotAmount;

    public GameObject shopPanel;
    public GameObject shopSlotPanel;
    public GameObject shopSlot;
    public GameObject shopitem;

    private Button buyBtn;
    public Button ExitBtn;

    public List<GameObject> slots = new List<GameObject>();

    public bool visibleShop = false; // ������ �������� �ִ°�

    public GameObject purchaseConfirmationDialog; // Ȯ�� ��ȭ����
    public Button purchaseConfirmButton; // Ȯ�� ��ư
    public Button purchaseCancelButton; // ��� ��ư
    public TextMeshProUGUI purchaseConfirmationText; // Ȯ�� �޽��� �ؽ�Ʈ
    public Image purchaseConfirmationImage; // ������ �̹���

    public GameObject stackablePurchaseConfirmationDialog; // Ȯ�� ��ȭ����
    public Button stackablePurchaseConfirmButton; // Ȯ�� ��ư
    public Button stackablePurchaseCancelButton; // ��� ��ư
    public TMP_InputField CountInputField; // �Ǹ� ���� �Է� �ʵ�
    public Image stackablePurchaseConfirmationImage; // ������ �̹���

    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();

        shopSlotPanel = GameObject.Find("ShopSlotPanel");
        shopPanel = GameObject.Find("ShopPanel");

        shopPanel.SetActive(visibleShop);

        for (int i = 0; i < slotAmount; i++) // ��UI ������ ���� 
        {
            slots.Add(Instantiate(shopSlot));
            slots[i].transform.SetParent(shopSlotPanel.transform, false);
            slots[i].GetComponent<ShopSlot>().slotID = i;

            GameObject item = Instantiate(shopitem);
            item.transform.SetParent(slots[i].transform.GetChild(0), false);
            item.GetComponent<ShopDT>().slot = i;
        }


        // �߰��ϰ��� �� slotIndex�� dataBase�� ������ִ� �������� ID�� �־��ָ� ���� �ǸŸ�� ���� ����
        CreateSlotItem(0, 0);
        CreateSlotItem(1, 1);
        CreateSlotItem(3, 3);
        CreateSlotItem(4, 4);
        CreateSlotItem(5, 13);
        CreateSlotItem(6, 2);
        CreateSlotItem(7, 5);
        CreateSlotItem(8, 6);

        purchaseConfirmationDialog.SetActive(false);
        stackablePurchaseConfirmationDialog.SetActive(false);

        purchaseConfirmButton.onClick.AddListener(OnConfirmButtonClick);
        purchaseCancelButton.onClick.AddListener(OnCancelButtonClick);

        stackablePurchaseConfirmButton.onClick.AddListener(OnStackableConfirmButtonClick);
        stackablePurchaseCancelButton.onClick.AddListener(OnStackableCancelButtonClick);

        ExitBtn.onClick.AddListener(OnExit);
    }

    public void ShowConfirmationDialog(Item item)
    {
        DialogManager.instance.ShowDialog(purchaseConfirmationDialog);
        purchaseConfirmationImage.sprite = item.Icon;
        purchaseConfirmationText.text = $"'{item.Name}'��(��) �����Ͻðڽ��ϱ�?"; // Ȯ�� �޽��� ����
        if (item.Name == "���� : ��")
        {
            purchaseConfirmationText.text = $"<color=red>'{item.Name}'</color>��(��) �����Ͻðڽ��ϱ�?";
        }
        else if (item.Name == "���� : û")
        {
            purchaseConfirmationText.text = $"<color=#00FFFF>'{item.Name}'</color>��(��) �����Ͻðڽ��ϱ�?";
        }
        else if (item.Name == "���� : Ȳ")
        {
            purchaseConfirmationText.text = $"<color=yellow>'{item.Name}'</color>��(��) �����Ͻðڽ��ϱ�?";
        }
        else
        {
            purchaseConfirmationText.text = $"<color=white>'{item.Name}'</color>��(��) �����Ͻðڽ��ϱ�?";
        }
        purchaseConfirmButton.onClick.RemoveAllListeners();
        purchaseConfirmButton.onClick.AddListener(() => ConfirmBuyItem(item));
    }

    private void ConfirmBuyItem(Item item)
    {
        if (DataManager.instance.playerData.gold - item.Price >= 0)
        {
            inv.AddItem(item.ID);
            DataManager.instance.LoseGold(item.Price);
            purchaseConfirmationDialog.SetActive(false);
        }
    }

    private void OnConfirmButtonClick()
    {
        purchaseConfirmationDialog.SetActive(false);
    }

    private void OnCancelButtonClick()
    {
        purchaseConfirmationDialog.SetActive(false);
        DialogManager.instance.visibleShopDialogs = false;
    }

    public void ShowStackableConfirmationDialog(Item item)
    {
        DialogManager.instance.ShowDialog(stackablePurchaseConfirmationDialog);
        stackablePurchaseConfirmationImage.sprite = item.Icon;
        CountInputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
        stackablePurchaseConfirmButton.onClick.RemoveAllListeners();
        stackablePurchaseConfirmButton.onClick.AddListener(() => ConfirmStackableBuyItem(item));

        // �Է� �ʵ� �ʱ�ȭ �� ��Ŀ�� ����
        CountInputField.ActivateInputField();
        CountInputField.Select();
    }

    private void ConfirmStackableBuyItem(Item item)
    {
        int buyAmount;
        if (int.TryParse(CountInputField.text, out buyAmount))
        {
            if (DataManager.instance.playerData.gold - (item.Price * buyAmount) >= 0)
            {
                inv.AddItem(item.ID, buyAmount);
                DataManager.instance.LoseGold(item.Price * buyAmount);
                stackablePurchaseConfirmationDialog.SetActive(false);

                for (int i = 0; i < quickSlot.slotAmount; i++)
                {
                    QuickSlotDT qSlotDT = quickSlot.slots[i].GetComponentInChildren<QuickSlotDT>();
                    if (qSlotDT != null && qSlotDT.iconPath == item.IconPath)
                    {
                        quickSlot.AddAmountQuicktSlotItem(qSlotDT.slotNum, buyAmount);
                        break;
                    }
                }
            }
        }
    }

    private void OnStackableConfirmButtonClick()
    {
        //stackablePurchaseConfirmationDialog.SetActive(false);
    }

    private void OnStackableCancelButtonClick()
    {
        stackablePurchaseConfirmationDialog.SetActive(false);
        DialogManager.instance.visibleShopDialogs = false;
    }

    private void OnExit()
    {
        visibleShop = !visibleShop;
        shopPanel.SetActive(visibleShop);

        cameraController.SetUIActiveCount(visibleShop);
    }

    private void CreateSlotItem(int slotIndex, int dataBaseID)
    {
        slots[slotIndex].transform.Find("Image").GetChild(2).GetComponent<ShopDT>().item = ItemDataBase.instance.dataBase[dataBaseID];
        Image itemIcon = slots[slotIndex].transform.Find("Image").transform.GetChild(1).GetComponent<Image>();

        itemIcon.GetComponent<Image>().sprite = ItemDataBase.instance.dataBase[dataBaseID].Icon; // ������ �߰�
        Color tempColor = itemIcon.GetComponent<Image>().color;
        tempColor.a = 1f; // �������ϰ� ���� 
        itemIcon.GetComponent<Image>().color = tempColor;

        buyBtn = slots[slotIndex].transform.Find("BuyBtn").GetComponent<Button>(); //���� ��ư Ŭ�� ������ 
        if(buyBtn != null)
        {
            buyBtn.onClick.AddListener(() => OnBuyButtonClicked(dataBaseID));
        }

        Color btnColor = buyBtn.GetComponent<Image>().color;
        btnColor.a = 1f; // �������ϰ� ���� 
        buyBtn.GetComponent<Image>().color = btnColor;

        TextMeshProUGUI priceText = slots[slotIndex].transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>(); // �ش� ������ �Ǹ� ���� �߰� 
        priceText.text = ItemDataBase.instance.dataBase[dataBaseID].Price.ToString();
    }

    private void OnBuyButtonClicked(int itemID)
    {
        Item item = ItemDataBase.instance.FetchItemByID(itemID);

        if (!item.Stackable)
            ShowConfirmationDialog(item);
        else
            ShowStackableConfirmationDialog(item);
    }
}
