using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemToolTip : MonoBehaviour
{
    private Item item;
    private Shop shop;
    private GameObject shopItemTooltip;

    public GameObject itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemRequiredLevelText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemEffectText;


    // Start is called before the first frame update
    void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        shopItemTooltip = GameObject.Find("ShopItemToolTip");
        shopItemTooltip.SetActive(false);
    }

    private void Update()
    {
        if (!shop.visibleShop)
        {
            Deactivate();
        }
    }

    public void Activate(Item newItem)
    {
        shopItemTooltip.SetActive(true);
        shopItemTooltip.transform.position = Input.mousePosition;

        item = newItem;

        SetName(item);
        SetIcon(item);
        SetRequiredLevel(item);
        SetDescription(item);
        SetEffect(item);
    }
    public void Deactivate()
    {
        shopItemTooltip.SetActive(false);
    }

    private void SetName(Item item)
    {
        if (item.Name == "혈검 : 적")
        {
            itemNameText.text = $"<color=red>'{item.Name}'</color>";
        }
        else if (item.Name == "성검 : 청")
        {
            itemNameText.text = $"<color=#00FFFF>'{item.Name}'</color>";
        }
        else if (item.Name == "광검 : 황")
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
            itemEffectText.text = $"공격력 : +{item.increaseAttackPower}";
        }
        else if (item.Type == "Shield")
        {
            itemEffectText.text = $"방어력 : +{item.increaseDefense}";
        }
        else
        {
            itemEffectText.text = "";
        }
    }
}
