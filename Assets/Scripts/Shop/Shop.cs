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

    ItemDataBase itemdataBase;
    private Inventory inv;
    private QuickSlot quickSlot;
    private PlayerData playerData;

    public int slotAmount;

    public GameObject shopPanel;
    public GameObject shopSlotPanel;
    public GameObject shopSlot;
    public GameObject shopitem;

    private Button buyBtn;

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
        // ������ �Ŵ����� ���� �÷��̾� ������ �ε�
        if (DataManager.instance != null)
        {
            DataManager.instance.LoadPlayerData();
            playerData = DataManager.instance.playerData;
        }

        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        itemdataBase = GameObject.Find("ItemDataBase").GetComponent<ItemDataBase>();
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
        }


        // �߰��ϰ��� �� slotIndex�� dataBase�� ������ִ� �������� ID�� �־��ָ� ���� �ǸŸ�� ���� ����
        CreateSlotItem(0, 0);
        CreateSlotItem(1, 1);
        CreateSlotItem(2, 2);
        CreateSlotItem(3, 3);
        CreateSlotItem(4, 4);

        purchaseConfirmationDialog.SetActive(false);
        stackablePurchaseConfirmationDialog.SetActive(false);

        purchaseConfirmButton.onClick.AddListener(OnConfirmButtonClick);
        purchaseCancelButton.onClick.AddListener(OnCancelButtonClick);

        stackablePurchaseConfirmButton.onClick.AddListener(OnStackableConfirmButtonClick);
        stackablePurchaseCancelButton.onClick.AddListener(OnStackableCancelButtonClick);
    }

    public void ShowConfirmationDialog(Item item)
    {
        DialogManager.instance.ShowDialog(purchaseConfirmationDialog);
        purchaseConfirmationImage.sprite = item.Icon;
        purchaseConfirmationText.text = $"'{item.Name}'��(��) �����Ͻðڽ��ϱ�?"; // Ȯ�� �޽��� ����
        purchaseConfirmButton.onClick.RemoveAllListeners();
        purchaseConfirmButton.onClick.AddListener(() => ConfirmBuyItem(item));
    }

    private void ConfirmBuyItem(Item item)
    {
        if (playerData.gold - item.Price >= 0)
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
            if (playerData.gold - (item.Price * buyAmount) >= 0)
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
    }


    private void CreateSlotItem(int slotIndex, int dataBaseID)
    {
        Image itemIcon = slots[slotIndex].transform.Find("Image").transform.GetChild(1).GetComponent<Image>();

        itemIcon.GetComponent<Image>().sprite = itemdataBase.dataBase[dataBaseID].Icon; // ������ �߰�
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
        priceText.text = itemdataBase.dataBase[dataBaseID].Price.ToString();
    }

    private void OnBuyButtonClicked(int itemID)
    {
        Item item = itemdataBase.FetchItemByID(itemID);

        if (!item.Stackable)
            ShowConfirmationDialog(item);
        else
            ShowStackableConfirmationDialog(item);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            visibleShop = !visibleShop;
            shopPanel.SetActive(visibleShop);

            cameraController.SetUIActiveCount(visibleShop);
        }
    }

}
