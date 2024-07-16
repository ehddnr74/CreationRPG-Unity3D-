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

    public int itemAmount; // ������ ���� 

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
            originalSiblingIndex = this.transform.GetSiblingIndex(); // ������ ���� �ε����� ����

            parentLayoutGroup = originalParent.GetComponentInParent<GridLayoutGroup>(); // �θ��� ���̾ƿ� �׷��� ����

            if (parentLayoutGroup != null)
                parentLayoutGroup.enabled = false; // �巡�װ� ���۵Ǹ� ���̾ƿ� �׷� ��Ȱ��ȭ

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
                this.transform.SetSiblingIndex(originalSiblingIndex); // ������ ���� �ε����� ����
                this.transform.position = originalParent.position;

                if (parentLayoutGroup != null)
                    parentLayoutGroup.enabled = true; // �巡�װ� ������ ���̾ƿ� �׷� Ȱ��ȭ

                quickSlot.SwapItems(slotNum, targetSlotDT.slotNum);
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            this.transform.SetParent(originalParent);
            this.transform.SetSiblingIndex(originalSiblingIndex); // ������ ���� �ε����� ����
            this.transform.position = originalParent.position;

            if (parentLayoutGroup != null)
            parentLayoutGroup.enabled = true; // �巡�װ� ������ ���̾ƿ� �׷� Ȱ��ȭ

            canvasGroup.blocksRaycasts = true;
        }
        quickSlot.itemsChanged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (iconPath != "Inventory" && iconPath != "Quest" && iconPath != "Equip"
                && iconPath != "Skill" && iconPath != "Stat")
            {
                // ���콺 ������ Ŭ�� �� ������ �������� �����, �ش� ������ ������ �ʱ�ȭ
                itemIcon = null;
                GetComponent<Image>().sprite = null;
                GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // �����ϰ� ����
                itemAmount = 0;

                quickSlot.itemsChanged = true;
            }
        }
    }
}
