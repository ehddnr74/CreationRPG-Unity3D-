using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipDT : MonoBehaviour, IPointerClickHandler
{
    public GameObject item;
    private Inventory inv;
    private ItemDataBase itemDataBase;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        itemDataBase = GameObject.Find("ItemDataBase").GetComponent<ItemDataBase>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UnEquip();
        }
    }
    void UnEquip()
    {
        Image itemIcon = gameObject.GetComponent<Image>();
        Item item = itemDataBase.FetchItemByIcon(itemIcon.sprite);

        if (item != null)
        {
            if (itemIcon.sprite != null)
            {
                if (item.Type == "Weapon")
                {
                    playerController.EquipWeapon(null, item.prefabPath);  // 무기 장착 해제 
                }
                else if (item.Type == "Shield")
                {
                    playerController.EquipShield(null, item.prefabPath); // 방패 장착 해제
                }

                inv.AddItem(item.ID);
                itemIcon.sprite = null;

                Color tempColor = itemIcon.color;
                tempColor.a = 0f; // 불투명하게 설정 
                itemIcon.color = tempColor;
            }
        }
    }
}
