using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDT : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector2 offset;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private QuickSlot qSlot;

    private SkillToolTip skillTooltip;

    public GameObject skillNameText;
    public GameObject skillLvText;
    public GameObject skillLevelText;
    public GameObject skillUpBtn;

    public GameObject nonDragableskillIcon;

    public string skillName;
    public string skillType;
    public int skillMasterLevel;
    public int skillMinLevel;
    public int skillLevel;
    public Sprite skillIcon;

    void Start()
    {
        qSlot = GameObject.Find("QuickSlot").GetComponent<QuickSlot>();
        skillTooltip = GameObject.Find("SkillUI").GetComponent<SkillToolTip>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (skillLevel > 0 && skillType != "패시브")
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
            originalParent = this.transform.parent;
            this.transform.SetParent(canvas.transform); // 최상위 Canvas로 이동
            this.transform.SetAsLastSibling(); // 최상위로 설정
            this.transform.position = eventData.position - offset;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillLevel > 0 && skillType != "패시브")
        {
            this.transform.position = eventData.position - offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillLevel > 0 && skillType != "패시브")
        {
            // Raycast를 사용하여 포인터가 어떤 UI 요소 위에 있는지 확인
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("QuickSlot"))
                {
                    QuickSlotDT quickSlot = result.gameObject.GetComponent<QuickSlotDT>();
                    if (quickSlot != null)
                    {
                        quickSlot.itemIcon = skillIcon;
                        //해당 퀵슬롯에 아이템 추가
                        qSlot.AddItemToQuickSlot(skillIcon, quickSlot.slotNum, 0);
                        //break;
                    }
                }
            }
            this.transform.SetParent(originalParent);
            this.transform.position = originalParent.position;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        skillTooltip.Activate(skillName, skillMinLevel, skillMasterLevel, skillLevel);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillTooltip.Deactivate();
    }
}
