using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class QuickSlot : MonoBehaviour
{
    private static QuickSlot instance;

    public int slotAmount;
    public GameObject quickSlotPanel;
    public GameObject quickSlot;
    public GameObject quickSlotItem;
    public GameObject keyCodeRound;

    private AnimationEventForwarder mAnimationEventForwarder;
    private Inventory inv;
    private Equip equip;
    private CameraController cameraController;

    public bool itemsChanged = false;
    public bool interactable = false;
    public bool checkInteraction = false;

    public List<GameObject> slots = new List<GameObject>();

    // 슬롯과 키 매핑
    private Dictionary<KeyCode, int> keyToSlotMap = new Dictionary<KeyCode, int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        mAnimationEventForwarder = GameObject.Find("Paladin").GetComponent<AnimationEventForwarder>();
        equip = GameObject.Find("Equip").GetComponent<Equip>();
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();

        for (int i = 0; i < slotAmount; i++)
        {
            CreateQuickSlot(i);
        }

        // 키 매핑 초기화
        InitializeKeyMappings();
        //퀵슬롯 로딩
        LoadQuickSlot();
    }

    private void Update()
    {
        // 매핑된 키를 검사하여 해당 슬롯을 활성화
        foreach (var kvp in keyToSlotMap)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                ActivateSlot(kvp.Value);
            }
        }
        if (itemsChanged)
        {
            itemsChanged = false;
            SaveQuickSlot();
        }
    }

    void CreateQuickSlot(int num)
    {
        slots.Add(Instantiate(quickSlot));
        slots[num].GetComponent<QuickSlotSlot>().SetID(num);
        slots[num].transform.SetParent(quickSlotPanel.transform, false);
        GameObject slotItem = Instantiate(quickSlotItem);
        slotItem.transform.SetParent(slots[num].transform, false);
        slotItem.GetComponent<QuickSlotDT>().slotNum = num;
        GameObject keyKodeRoundObj = Instantiate(keyCodeRound);
        keyKodeRoundObj.transform.SetParent(slots[num].transform, false);

        //아이템 이미지 투명하게 설정
        Image slotItemImage = slotItem.GetComponent<Image>();
        if (slotItemImage != null)
        {
            Color tempColor = slotItemImage.color;
            tempColor.a = 0f; // 투명하게 설정
            slotItemImage.color = tempColor;
        }
    }

    public void AddAmountQuicktSlotItem(int slotIndex, int amount)
    {
        QuickSlotDT slotDT = slots[slotIndex].GetComponentInChildren<QuickSlotDT>();

        if (slotDT.itemAmount > 0)
        {
            slotDT.itemAmount += amount;
            itemsChanged = true;
        }
    }

    //인벤토리 아이템(ItemDT)로 부터 퀵슬롯에 Icon이 옮겨지는 과정 
    public void AddItemToQuickSlot(Sprite icon, int quickSlotID, int amount)
    {
        foreach (var slot in slots)
        {
            QuickSlotDT quickSlotDT = slot.GetComponentInChildren<QuickSlotDT>();
            if (quickSlotDT != null)
            {
                Image slotImage = quickSlotDT.GetComponent<Image>();
                if (slotImage != null && slotImage.sprite != null && slotImage.sprite.name == icon.name)
                {
                    QuickSlotDT qSlotDT = slots[quickSlotID].GetComponentInChildren<QuickSlotDT>();
                    qSlotDT.itemIcon = null;
                    qSlotDT.iconPath = null;
                    qSlotDT.itemAmount = 0;
                    // 이미 해당 아이콘이 있는 경우 리턴
                    return;
                }
            }
        }
        if (quickSlotID >= 0 && quickSlotID < slots.Count)
        {
            QuickSlotSlot quickSlotSlot = slots[quickSlotID].GetComponent<QuickSlotSlot>();
            if (quickSlotSlot != null)
            {
                QuickSlotDT quickSlotDT = quickSlotSlot.GetComponentInChildren<QuickSlotDT>();

                if (quickSlotDT != null)
                {
                    Image slotImage = quickSlotDT.GetComponent<Image>();

                    if (slotImage != null)
                    {
                        slotImage.sprite = icon;
                        Color tempColor = slotImage.color;
                        tempColor.a = 1f; // 불투명하게 설정
                        slotImage.color = tempColor;
                        quickSlotDT.itemIcon = icon;
                        quickSlotDT.iconPath = icon.name; // 아이콘의 경로를 저장
                        quickSlotDT.itemAmount = amount;
        
                        // 아이템 추가가 완료된 후 데이터 저장
                        itemsChanged = true;
                    }
                }
            }
        }
    }

    public void RemoveQuicktSlotItem(string iconPath, int slotIndex, int amount)
    {
        QuickSlotDT slotDT = slots[slotIndex].GetComponentInChildren<QuickSlotDT>();

        slotDT.itemAmount -= amount;

        if (slotDT.itemAmount <= 0)
        {
            // 마우스 오른쪽 클릭 시 아이템 아이콘을 지우고, 해당 슬롯의 정보를 초기화
            slotDT.itemIcon = null;
            slotDT.iconPath = null;
            slotDT.GetComponent<Image>().sprite = null;
            slotDT.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // 투명하게 설정
            slotDT.itemAmount = 0;
        }

        itemsChanged = true;
    }

    // 두 퀵슬롯 간의 아이템 정보를 교환하는 메서드
    public void SwapItems(int slotNum1, int slotNum2)
    {
        if (slotNum1 >= 0 && slotNum1 < slots.Count && slotNum2 >= 0 && slotNum2 < slots.Count)
        {
            QuickSlotDT slot1 = slots[slotNum1].GetComponentInChildren<QuickSlotDT>();
            QuickSlotDT slot2 = slots[slotNum2].GetComponentInChildren<QuickSlotDT>();

            if (slot1 != null && slot2 != null)
            {
                // 아이콘 교환
                Sprite tempIcon = slot1.itemIcon;
                slot1.itemIcon = slot2.itemIcon;
                slot2.itemIcon = tempIcon;

                // 아이콘 경로 교환
                string tempIconPath = slot1.iconPath;
                slot1.iconPath = slot2.iconPath;
                slot2.iconPath = tempIconPath;

                // 아이템 수량 교환
                int tempAmount = slot1.itemAmount;
                slot1.itemAmount = slot2.itemAmount;
                slot2.itemAmount = tempAmount;

                Image slot1Image = slot1.GetComponent<Image>();
                Image slot2Image = slot2.GetComponent<Image>();

                if (slot1Image != null && slot2Image != null)
                {
                    slot1Image.sprite = slot1.itemIcon;
                    slot2Image.sprite = slot2.itemIcon;

                    Color tempColor1 = slot1Image.color;
                    tempColor1.a = slot1.itemIcon != null ? 1f : 0f;
                    slot1Image.color = tempColor1;

                    Color tempColor2 = slot2Image.color;
                    tempColor2.a = slot2.itemIcon != null ? 1f : 0f;
                    slot2Image.color = tempColor2;
                }
            }
        }
    }

    private void InitializeKeyMappings()
    {
        KeyCode[] keys = { KeyCode.Q, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
                           KeyCode.B, KeyCode.N, KeyCode.M, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Z,KeyCode.X,
                           KeyCode.C, KeyCode.V, KeyCode.Alpha1, KeyCode.Alpha2};

        for (int i = 0; i < slots.Count && i < keys.Length; i++)
        {
            keyToSlotMap[keys[i]] = i;
            TextMeshProUGUI textComponent = slots[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                if (keys[i].ToString() == "Alpha1")
                    textComponent.text = 1.ToString();
                else if (keys[i].ToString() == "Alpha2")
                    textComponent.text = 2.ToString();
                else if (keys[i].ToString() == "Alpha3")
                    textComponent.text = 3.ToString();
                else if (keys[i].ToString() == "Alpha4")
                    textComponent.text = 4.ToString();
                else if (keys[i].ToString() == "Alpha5")
                    textComponent.text = 5.ToString();
                else
                    textComponent.text = keys[i].ToString();
            }
        }
    }
    private void ActivateSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            QuickSlotDT quickSlotDT = slots[slotIndex].GetComponentInChildren<QuickSlotDT>();

            /////// Use Item
            if (quickSlotDT.iconPath == "hp")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item hpPotion = ItemDataBase.instance.FetchItemByIconPath(quickSlotDT.iconPath);
                    StatManager.instance.AddHP(300); // Json에 recoveryHP 추가 해도됨 일단 임시로 ...
                    inv.RemoveItem(hpPotion.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }

            if (quickSlotDT.iconPath == "mp")
            {
                if (quickSlotDT.itemAmount > 0)
                {
                    Item mpPotion = ItemDataBase.instance.FetchItemByIconPath(quickSlotDT.iconPath);
                    StatManager.instance.AddMP(300); // Json에 recoveryMP 추가 해도됨 일단 임시로 ...
                    inv.RemoveItem(mpPotion.ID);
                    RemoveQuicktSlotItem(quickSlotDT.iconPath, slotIndex, 1);
                }
            }


            /////// Use UI 
            if (quickSlotDT.iconPath == "Inventory")
            {
                inv.activeInventory = !inv.activeInventory;
                inv.inventoryPanel.SetActive(inv.activeInventory);

                cameraController.SetUIActiveCount(inv.activeInventory);
            }

            if (quickSlotDT.iconPath == "Quest")
            {
                QuestManager.instance.visibleQuest = !QuestManager.instance.visibleQuest;
                QuestManager.instance.questPanel.SetActive(QuestManager.instance.visibleQuest);

                cameraController.SetUIActiveCount(QuestManager.instance.visibleQuest);
            }

            if (quickSlotDT.iconPath == "Equip")
            {
                equip.visibleEquip = !equip.visibleEquip;
                equip.equipPanel.SetActive(equip.visibleEquip);

                cameraController.SetUIActiveCount(equip.visibleEquip);
            }

            if (quickSlotDT.iconPath == "Skill")
            {
                SkillManager.instance.visibleSkill = !SkillManager.instance.visibleSkill;
                SkillManager.instance.skillPanel.SetActive(SkillManager.instance.visibleSkill);

                cameraController.SetUIActiveCount(SkillManager.instance.visibleSkill);
            }

            if (quickSlotDT.iconPath == "Stat")
            {
                StatManager.instance.visibleStat = !StatManager.instance.visibleStat;
                StatManager.instance.statPanel.SetActive(StatManager.instance.visibleStat);

                cameraController.SetUIActiveCount(StatManager.instance.visibleStat);
            }

            if (quickSlotDT.iconPath == "Interaction")
            {
                if (interactable && !checkInteraction)
                {
                    checkInteraction = true;
                }
            }

            ///////  Use Skill
            if (quickSlotDT.iconPath == "Nature_10") // 벽력일섬
            {
                int level = SkillManager.instance.skillCollection.skills["벽력일섬"].skillLevel;
                if (level > 0)
                {
                    if (mAnimationEventForwarder.swordSkill)
                    {
                        mAnimationEventForwarder.lightningstrike = true;
                        mAnimationEventForwarder.comboStep = 2;
                        mAnimationEventForwarder.ComboAttackTrigger();
                    }
                }
            }

            if (quickSlotDT.iconPath == "Fire_10") // 극악무도
            {
                int level = SkillManager.instance.skillCollection.skills["극악무도"].skillLevel;
                if (level > 0)
                {
                    if (mAnimationEventForwarder.swordSkill)
                    {
                        mAnimationEventForwarder.atrocities = true;
                        mAnimationEventForwarder.comboStep = 2;
                        mAnimationEventForwarder.ComboAttackTrigger();
                    }
                }
            }
            if (quickSlotDT.iconPath == "Fire_12") // 천재지변
            {
                int level = SkillManager.instance.skillCollection.skills["천재지변"].skillLevel;
                if (level > 0)
                {
                    if (mAnimationEventForwarder.swordSkill)
                    {
                        mAnimationEventForwarder.naturaldisaster = true;
                        mAnimationEventForwarder.comboStep = 2;
                        mAnimationEventForwarder.ComboAttackTrigger();
                    }
                }
            }
            if (quickSlotDT.iconPath == "Nature_7") // 금강불괴
            {
                int level = SkillManager.instance.skillCollection.skills["금강불괴"].skillLevel;

                if (level > 0)
                {
                    BuffManager.instance.ActivateBuff("금강불괴", quickSlotDT.itemIcon, SkillManager.instance.skillCollection.skills["금강불괴"].levelEffects[level].buffDuration);
                    StatManager.instance.UpdateStatActiveHyperBody();
                }
            }

            if (quickSlotDT.iconPath == "Dark_11") // 화룡점정
            {
                int level = SkillManager.instance.skillCollection.skills["화룡점정"].skillLevel;

                if (level > 0)
                {
                    BuffManager.instance.ActivateBuff("화룡점정", quickSlotDT.itemIcon, SkillManager.instance.skillCollection.skills["화룡점정"].levelEffects[level].buffDuration);
                    StatManager.instance.statData.criticalProbability = StatManager.instance.statData.originCriticalProbability + (int)SkillManager.instance.skillCollection.skills["화룡점정"].levelEffects[level].criticalChanceIncrease;
                    StatManager.instance.UpdateStatActiveCriticalSkill();
                }
            }
        }
    }

    private void SaveQuickSlot()
    {
        List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();

        foreach (var slot in slots)
        {
            QuickSlotDT quickSlotDT = slot.GetComponentInChildren<QuickSlotDT>();

            if (quickSlotDT != null && quickSlotDT.itemIcon != null)
            {
                quickSlotItems.Add(new QuickSlotItem(quickSlotDT.slotNum, quickSlotDT.iconPath, quickSlotDT.itemAmount));
            }
        }

        string quickSlotDataJson = JsonConvert.SerializeObject(quickSlotItems, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/QuickSlot.json", quickSlotDataJson);
    }
    private void LoadQuickSlot()
    {
        string quickSlotDataPath = Application.persistentDataPath + "/QuickSlot.json";
        if (File.Exists(quickSlotDataPath))
        {
            string quickSlotDataJson = File.ReadAllText(quickSlotDataPath);
            List<QuickSlotItem> quickSlotItem = JsonConvert.DeserializeObject<List<QuickSlotItem>>(quickSlotDataJson);

            // 인벤토리 정보를 기반으로 슬롯에 아이템 배치
            foreach (QuickSlotItem item in quickSlotItem)
            {
                if (!string.IsNullOrEmpty(item.iconPath))
                {
                    Sprite icon = Resources.Load<Sprite>("Items/" + item.iconPath);
                    AddItemToQuickSlot(icon, item.slotNum, item.itemAmount);
                }
            }
        }
    }
}
