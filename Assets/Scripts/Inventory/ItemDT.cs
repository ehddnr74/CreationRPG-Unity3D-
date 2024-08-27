using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int amount;
    public int slot;

    private ItemToolTip itemToolTip;

    private Inventory inv;
    private Shop shop;
    private Equip equip;
    private PlayerController playerController;
    private QuickSlot qSlot;

    private Vector2 offset;



    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        equip = GameObject.Find("Equip").GetComponent<Equip>();
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        itemToolTip = GameObject.Find("InventoryUI").GetComponent<ItemToolTip>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            inv.items[slot] = new Item();
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            this.transform.SetParent(this.transform.parent.parent.parent.parent);
            this.transform.position = eventData.position - offset;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast를 사용하여 포인터가 어떤 UI 요소 위에 있는지 확인
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("QuickSlot"))
            {
                if(item.Type == "Weapon")
                {
                    break;
                }
                QuickSlotDT quickSlot = result.gameObject.GetComponent<QuickSlotDT>();
                if (quickSlot != null)
                {
                    inv.items[slot].ID = item.ID;
                    quickSlot.itemIcon = item.Icon;
                    quickSlot.iconPath = item.IconPath;
                    quickSlot.itemAmount = amount;
                    // 해당 퀵슬롯에 아이템 추가
                    qSlot.AddItemToQuickSlot(item.Icon, quickSlot.slotNum, amount);
                    break;
                }
            }
        }
        inv.items[slot].ID = item.ID;
        this.transform.SetParent(inv.slots[slot].transform); // 원래의 부모로 되돌림
        this.transform.position = inv.slots[slot].transform.position; // 원래의 위치로 이동  
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            itemToolTip.Deactivate();
            if (shop.visibleShop) // 상점이 열려있을 때 
            {
                if (!item.Stackable)
                    inv.ShowConfirmationDialog(item, slot); // 확인 대화상자
                else
                    inv.ShowStackableConfirmationDialog(item, slot); // 확인 대화상자
            }
            else // 상점이 닫혀있을 때 
            {
                if (item.requiredLevel <= DataManager.instance.playerData.level) // 착용가능레벨 이상일 때만 착용
                {
                    if (item.Type == "Weapon")
                    {
                        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

                        playerController.EquipWeapon(item, item.Prefab, item.prefabPath); // 무기 장착

                        EquipSetting(3, slot); //Equip창의 UI갱신과 Inventory 갱신 
                    }
                    else if (item.Type == "Shield")
                    {
                        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

                        playerController.EquipShield(item,item.Prefab, item.prefabPath); // 실드 장착

                        EquipSetting(2, slot); //Equip창의 UI갱신과 Inventory 갱신 
                    }
                }
            }
        }
    }


    private void EquipSetting(int equipNum, int slot) // (equip 부위) , (Slot 번호) 
    {
        Image equipImage = equip.equipitem[equipNum].GetComponent<Image>();

        if (equipImage.sprite != null)
        {
            Item previousItem = ItemDataBase.instance.FetchItemByIcon(equipImage.sprite);
            inv.RemoveItem(item.ID, slot);
            inv.AddItem(previousItem.ID, slot, 0);
            equipImage.sprite = item.Icon;

            Color tempColor = equipImage.GetComponent<Image>().color;
            tempColor.a = 1f; // 불투명하게 설정 
            equipImage.GetComponent<Image>().color = tempColor;
        }
        else
        {
            inv.RemoveItem(item.ID, slot);
            equipImage.sprite = item.Icon;

            Color tempColor = equipImage.GetComponent<Image>().color;
            tempColor.a = 1f; // 불투명하게 설정 
            equipImage.GetComponent<Image>().color = tempColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
            // _globalSortingOrderCounter를 static으로 접근
            MovableUI._globalSortingOrderCounter++;
            transform.GetComponentInParent<Canvas>().sortingOrder = MovableUI._globalSortingOrderCounter;
            itemToolTip.Activate(item, amount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemToolTip.Deactivate();
    }

}
