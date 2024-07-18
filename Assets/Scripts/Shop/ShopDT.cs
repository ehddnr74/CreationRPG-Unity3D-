using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopDT : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int slot;

    private ShopItemToolTip shopItemToolTip;

    void Start()
    {
        shopItemToolTip = GameObject.Find("ShopUI").GetComponent<ShopItemToolTip>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            shopItemToolTip.Deactivate();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && !DialogManager.instance.visibleShopDialogs)
        {
            MovableUI._globalSortingOrderCounter++;
            transform.GetComponentInParent<Canvas>().sortingOrder = MovableUI._globalSortingOrderCounter;
            shopItemToolTip.Activate(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item != null)
        {
            shopItemToolTip.Deactivate();
        }
    }

}
