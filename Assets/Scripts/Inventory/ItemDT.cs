using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Item item;
    public int amount;
    public int slot;

    private Inventory inv;
    private Shop shop;

    private Vector2 offset;

    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
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
        inv.items[slot].ID = item.ID;
        this.transform.SetParent(inv.slots[slot].transform); // ������ �θ�� �ǵ���
        this.transform.position = inv.slots[slot].transform.position; // ������ ��ġ�� �̵�  
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (shop.visibleShop)
            {
                if (!item.Stackable)
                    inv.ShowConfirmationDialog(item, slot); // Ȯ�� ��ȭ����
                else
                    inv.ShowStackableConfirmationDialog(item, slot); // Ȯ�� ��ȭ����
            }
        }
    }

}
