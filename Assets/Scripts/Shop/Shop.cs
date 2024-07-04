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

    public bool visibleShop = false; // 상점이 보여지고 있는가

    public GameObject purchaseConfirmationDialog; // 확인 대화상자
    public Button purchaseConfirmButton; // 확인 버튼
    public Button purchaseCancelButton; // 취소 버튼
    public TextMeshProUGUI purchaseConfirmationText; // 확인 메시지 텍스트
    public Image purchaseConfirmationImage; // 아이템 이미지

    public GameObject stackablePurchaseConfirmationDialog; // 확인 대화상자
    public Button stackablePurchaseConfirmButton; // 확인 버튼
    public Button stackablePurchaseCancelButton; // 취소 버튼
    public TMP_InputField CountInputField; // 판매 수량 입력 필드
    public Image stackablePurchaseConfirmationImage; // 아이템 이미지

    private void Start()
    {
        // 데이터 매니저를 통해 플레이어 데이터 로드
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

        for (int i = 0; i < slotAmount; i++) // 샵UI 아이템 구성 
        {
            slots.Add(Instantiate(shopSlot));
            slots[i].transform.SetParent(shopSlotPanel.transform, false);
            slots[i].GetComponent<ShopSlot>().slotID = i;

            GameObject item = Instantiate(shopitem);
            item.transform.SetParent(slots[i].transform.GetChild(0), false);
        }


        // 추가하고자 할 slotIndex와 dataBase에 저장되있는 아이템의 ID를 넣어주면 상점 판매목록 갱신 가능
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
        purchaseConfirmationText.text = $"'{item.Name}'을(를) 구매하시겠습니까?"; // 확인 메시지 설정
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
        CountInputField.text = ""; // 입력 필드 초기화
        stackablePurchaseConfirmButton.onClick.RemoveAllListeners();
        stackablePurchaseConfirmButton.onClick.AddListener(() => ConfirmStackableBuyItem(item));

        // 입력 필드 초기화 및 포커스 설정
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

        itemIcon.GetComponent<Image>().sprite = itemdataBase.dataBase[dataBaseID].Icon; // 아이콘 추가
        Color tempColor = itemIcon.GetComponent<Image>().color;
        tempColor.a = 1f; // 불투명하게 설정 
        itemIcon.GetComponent<Image>().color = tempColor;

        buyBtn = slots[slotIndex].transform.Find("BuyBtn").GetComponent<Button>(); //구매 버튼 클릭 리스너 
        if(buyBtn != null)
        {
            buyBtn.onClick.AddListener(() => OnBuyButtonClicked(dataBaseID));
        }

        Color btnColor = buyBtn.GetComponent<Image>().color;
        btnColor.a = 1f; // 불투명하게 설정 
        buyBtn.GetComponent<Image>().color = btnColor;

        TextMeshProUGUI priceText = slots[slotIndex].transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>(); // 해당 아이템 판매 가격 추가 
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
