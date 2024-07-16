using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Item item;
    public int amount;
    public int slot;

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
        // Raycast�� ����Ͽ� �����Ͱ� � UI ��� ���� �ִ��� Ȯ��
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
                    // �ش� �����Կ� ������ �߰�
                    qSlot.AddItemToQuickSlot(item.Icon, quickSlot.slotNum, amount);
                    break;
                }
            }
        }
        inv.items[slot].ID = item.ID;
        this.transform.SetParent(inv.slots[slot].transform); // ������ �θ�� �ǵ���
        this.transform.position = inv.slots[slot].transform.position; // ������ ��ġ�� �̵�  
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (shop.visibleShop) // ������ �������� �� 
            {
                if (!item.Stackable)
                    inv.ShowConfirmationDialog(item, slot); // Ȯ�� ��ȭ����
                else
                    inv.ShowStackableConfirmationDialog(item, slot); // Ȯ�� ��ȭ����
            }
            else // ������ �������� �� 
            {
                if(item.Type == "Weapon")
                {
                    playerController = GameObject.Find("Player").GetComponent<PlayerController>();

                    playerController.EquipWeapon(item.Prefab, item.prefabPath); // ���� ����
                                                               
                    EquipSetting(3, slot); //Equipâ�� UI���Ű� Inventory ���� 
                }
                else if(item.Type == "Shield")
                {
                    playerController = GameObject.Find("Player").GetComponent<PlayerController>();

                    playerController.EquipShield(item.Prefab, item.prefabPath); // �ǵ� ����

                    EquipSetting(2, slot); //Equipâ�� UI���Ű� Inventory ���� 
                }
            }
        }
    }


    private void EquipSetting(int equipNum, int slot) // (equip ����) , (Slot ��ȣ) 
    {
        Image equipImage = equip.equipitem[equipNum].GetComponent<Image>();

        if (equipImage.sprite != null)
        {
            Item previousItem = ItemDataBase.instance.FetchItemByIcon(equipImage.sprite);
            inv.RemoveItem(item.ID, slot);
            inv.AddItem(previousItem.ID, slot, 0);
            equipImage.sprite = item.Icon;

            Color tempColor = equipImage.GetComponent<Image>().color;
            tempColor.a = 1f; // �������ϰ� ���� 
            equipImage.GetComponent<Image>().color = tempColor;
        }
        else
        {
            inv.RemoveItem(item.ID, slot);
            equipImage.sprite = item.Icon;

            Color tempColor = equipImage.GetComponent<Image>().color;
            tempColor.a = 1f; // �������ϰ� ���� 
            equipImage.GetComponent<Image>().color = tempColor;
        }
    }

}
