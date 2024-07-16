using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipDT : MonoBehaviour, IPointerClickHandler
{
    public GameObject item;
    private Inventory inv;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
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
        Item item = ItemDataBase.instance.FetchItemByIcon(itemIcon.sprite);

        if (item != null)
        {
            if (itemIcon.sprite != null)
            {
                if (item.Type == "Weapon")
                {
                    playerController.EquipWeapon(null, item.prefabPath);  // ���� ���� ���� 
                }
                else if (item.Type == "Shield")
                {
                    playerController.EquipShield(null, item.prefabPath); // ���� ���� ����
                }

                inv.AddItem(item.ID);
                itemIcon.sprite = null;

                Color tempColor = itemIcon.color;
                tempColor.a = 0f; // �������ϰ� ���� 
                itemIcon.color = tempColor;
            }
        }
    }
}
