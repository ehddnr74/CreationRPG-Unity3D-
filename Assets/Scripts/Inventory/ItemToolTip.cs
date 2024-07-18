using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    private Item item;
    private Inventory inv;
    private Shop shop;
    private GameObject itemTooltip;

    public GameObject itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemRequiredLevelText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemEffectText;
    public TextMeshProUGUI itemAmountText;
    public TextMeshProUGUI itemSellPriceText;


    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        itemTooltip = GameObject.Find("ItemToolTip");
        itemTooltip.SetActive(false);
    }

    private void Update()
    {
        if (!inv.activeInventory)
        {
            Deactivate();
        }
    }

    public void Activate(Item newItem, int amount)
    {
        itemTooltip.SetActive(true);
        itemTooltip.transform.position = Input.mousePosition;

        item = newItem;

        SetName(item);
        SetIcon(item);
        SetRequiredLevel(item);
        SetDescription(item);
        SetEffect(item);
        SetAmount(item, amount);
        SetSellPrice(item);
    }

    public void Deactivate()
    {
        itemTooltip.SetActive(false);
    }
    private void SetName(Item item)
    {
        if (item.Name == "���� : ��")
        {
            itemNameText.text = $"<color=red>'{item.Name}'</color>";
        }
        else if (item.Name == "���� : û")
        {
            itemNameText.text = $"<color=#00FFFF>'{item.Name}'</color>";
        }
        else if (item.Name == "���� : Ȳ")
        {
            itemNameText.text = $"<color=yellow>'{item.Name}'</color>";
        }
        else
        {
            itemNameText.text = $"<color=white>'{item.Name}'</color>";
        }
    }
    private void SetIcon(Item item)
    {
        Image itemImg = itemImage.GetComponent<Image>();
        itemImg.sprite = item.Icon;
    }
    private void SetRequiredLevel(Item item)
    {
        if (item.requiredLevel != 0)
        {
            if (DataManager.instance.playerData.level < item.requiredLevel)
            {
                itemRequiredLevelText.text = $"<color=red>'REQ LEV : {item.requiredLevel}'</color>";
            }
            else
            {
                itemRequiredLevelText.text = $"<color=white>'REQ LEV : {item.requiredLevel}'</color>";
            }
        }
        else
        {
            itemRequiredLevelText.text = "";
        }
    }
    private void SetDescription(Item item)
    {
        itemDescriptionText.text = item.Description;
    }
    private void SetEffect(Item item)
    {
        if (item.Type == "Weapon")
        {
            itemEffectText.text = $"���ݷ� : +{item.increaseAttackPower}";
        }
        else if(item.Type == "Shield")
        {
            itemEffectText.text = $"���� : +{item.increaseDefense}";
        }
        else
        {
            itemEffectText.text = "";
        }
    }
    private void SetAmount(Item item, int amount)
    {
        if (item.Stackable)
        {
            itemAmountText.text = $"���� : {amount}";
        }
        else
        {
            itemAmountText.text = "";
        }
    }
    private void SetSellPrice(Item item)
    {
        if(shop.visibleShop)
        {
            itemSellPriceText.text = $"�Ǹűݾ� : <color=yellow>'{item.SellPrice}'</color>";
        }
        else
        {
            itemSellPriceText.text = "";
        }
    }
}
