using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int slotNum;
    public Sprite itemIcon;
    public string iconPath;

    public int itemAmount; // 아이템 수량 

    private QuickSlot quickSlot;
    private Vector2 offset;
    private Transform originalParent;
    private int originalSiblingIndex;

    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private GridLayoutGroup parentLayoutGroup;

    private void Start()
    {
        quickSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            originalParent = this.transform.parent;
            originalSiblingIndex = this.transform.GetSiblingIndex(); // 원래의 형제 인덱스를 저장

            parentLayoutGroup = originalParent.GetComponentInParent<GridLayoutGroup>(); // 부모의 레이아웃 그룹을 저장

            if (parentLayoutGroup != null)
                parentLayoutGroup.enabled = false; // 드래그가 시작되면 레이아웃 그룹 비활성화

            this.transform.SetParent(originalParent.parent);
            this.transform.position = eventData.position - offset;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemIcon != null)
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject pointerEnterObject = eventData.pointerEnter;

        if (pointerEnterObject != null && pointerEnterObject.CompareTag("QuickSlot"))
        {
            QuickSlotDT targetSlotDT = pointerEnterObject.GetComponent<QuickSlotDT>();
            if (targetSlotDT != null)
            {
                this.transform.SetParent(originalParent);
                this.transform.SetSiblingIndex(originalSiblingIndex); // 원래의 형제 인덱스로 설정
                this.transform.position = originalParent.position;

                if (parentLayoutGroup != null)
                    parentLayoutGroup.enabled = true; // 드래그가 끝나면 레이아웃 그룹 활성화

                quickSlot.SwapItems(slotNum, targetSlotDT.slotNum);
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            this.transform.SetParent(originalParent);
            this.transform.SetSiblingIndex(originalSiblingIndex); // 원래의 형제 인덱스로 설정
            this.transform.position = originalParent.position;

            if (parentLayoutGroup != null)
            parentLayoutGroup.enabled = true; // 드래그가 끝나면 레이아웃 그룹 활성화

            canvasGroup.blocksRaycasts = true;
        }
        quickSlot.itemsChanged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (iconPath != "Inventory" && iconPath != "Quest" && iconPath != "Equip"
                && iconPath != "Skill" && iconPath != "Stat" && iconPath != "Interaction")
            {
                // 마우스 오른쪽 클릭 시 아이템 아이콘을 지우고, 해당 슬롯의 정보를 초기화
                itemIcon = null;
                GetComponent<Image>().sprite = null;
                GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // 투명하게 설정
                itemAmount = 0;

                quickSlot.itemsChanged = true;
            }
        }
    }
}
