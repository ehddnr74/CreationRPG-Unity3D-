using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Events;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private Inventory inv;
    private CameraController cameraController;
    public QuestData questData;
    private ItemDataBase itemDataBase;
    private string path;

    public GameObject questPanel;
    public GameObject notStartedContent;
    public GameObject progressContent;
    public GameObject completeContent;
    public GameObject titlePrefab;

    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI questSummaryTitle;
    public TextMeshProUGUI questSummaryText;
    public TextMeshProUGUI RewardTitle;


    public GameObject rewardItem;
    public GameObject rewardPanel;
    private int rewardSlotCount = 4;

    public List<GameObject> rewardSlots = new List<GameObject>();

    public bool visibleQuest = false;
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

        path = Application.persistentDataPath + "/Quest.json";
        LoadQuests();
    }

    private void Start()
    {
        itemDataBase = GameObject.Find("ItemDataBase").GetComponent<ItemDataBase>();
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        questPanel.SetActive(visibleQuest);

        InitializeQuestUI();
        InitializeRewardSlots();
    }

    private void Update()
    {
        if (!visibleQuest)
        {
            HideQuestInfo();
        }
    }

    public void SaveQuests()
    {
        string data = JsonConvert.SerializeObject(questData, Formatting.Indented);
        File.WriteAllText(path, data);
    }

    public void LoadQuests()
    {
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            questData = JsonConvert.DeserializeObject<QuestData>(data);
            Debug.Log("Quests loaded successfully");
        }
        else
        {
            Debug.LogWarning("Quest file not found, creating new one.");
            questData = new QuestData { quests = new List<Quest>() };
        }
    }

    public Quest GetQuestById(int id)
    {
        return questData.quests.Find(quest => quest.id == id);
    }

    public void StartQuest(int id)
    {
        Quest quest = GetQuestById(id);
        if (quest != null && quest.status == "시작 가능")
        {
            quest.status = "진행 중";
            SaveQuests();

            // 시작 가능 목록에서 제거하고 진행 중 목록에 추가
            UpdateQuestUI(quest);
        }
    }

    public void CompleteQuest(int id)
    {
        Quest quest = GetQuestById(id);
        if (quest != null && quest.status == "진행 중")
        {
            quest.status = "완료";

            // 아이템 보상 추가
            foreach (var rewardItem in quest.reward.items)
            {
                inv.AddItem(rewardItem.itemId, rewardItem.amount);
            }

            // UI 업데이트: 진행 중에서 제거하고 완료에 추가
            UpdateQuestUI(quest);

            SaveQuests();
        }
    }

    private void UpdateQuestUI(Quest quest)
    {
        Transform parentTransform = null;

        // 퀘스트의 현재 상태에 따른 부모 Transform 설정
        if (quest.status == "진행 중")
        {
            parentTransform = progressContent.transform;
        }
        else if (quest.status == "완료")
        {
            parentTransform = completeContent.transform;
        }

        // 각 상태에 따른 UI 업데이트
        if (parentTransform != null)
        {
            // 이전 상태에 따라 적절한 목록에서 제거
            if (quest.status == "진행 중")
            {
                RemoveQuestFromUI(notStartedContent.transform, quest.name);
            }
            else if (quest.status == "완료")
            {
                RemoveQuestFromUI(progressContent.transform, quest.name);
            }

            // 새로운 상태의 목록에 추가
            GameObject obj = Instantiate(titlePrefab, parentTransform, false);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
            obj.GetComponent<QuestInfoButton>().quest = quest;
        }
    }

    private void RemoveQuestFromUI(Transform parentTransform, string questName)
    {
        foreach (Transform child in parentTransform)
        {
            if (child.GetComponentInChildren<TextMeshProUGUI>().text == questName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }


    // 게임 시작할 때 QuestUI
    private void InitializeQuestUI()
    {

        // 기존의 UI 요소들 제거
        ClearQuestUI(notStartedContent.transform);
        ClearQuestUI(progressContent.transform);
        ClearQuestUI(completeContent.transform);

        // 플레이어 레벨을 가져오기 (예시로 플레이어 레벨이 13이라고 가정)
        int playerLevel = DataManager.instance.playerData.level;

        // 퀘스트 상태에 따라 UI 요소 생성
        foreach (var quest in questData.quests)
        {
            if (quest.status == "시작 가능" && quest.possibleLevel <= playerLevel)
            {
                GameObject obj = Instantiate(titlePrefab, notStartedContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
            else if (quest.status == "진행 중")
            {
                GameObject obj = Instantiate(titlePrefab, progressContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
            else if (quest.status == "완료")
            {
                GameObject obj = Instantiate(titlePrefab, completeContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
        }
    }

    private void ClearQuestUI(Transform parentTransform)
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitializeRewardSlots()
    {
        for (int i = 0; i < rewardSlotCount; i++)
        {
            GameObject slot = Instantiate(rewardItem, rewardPanel.transform);
            rewardSlots.Add(slot);
            rewardPanel.SetActive(false);
            rewardSlots[i].SetActive(false);
        }
    }

    public void ShowQuestInfo(Quest quest)
    {
        questTitleText.text = quest.name;
        descriptionText.text = quest.description;
        questSummaryTitle.text = "퀘스트 요약";
        questSummaryText.text = quest.objective;
        RewardTitle.text = "보상";

        // 모든 보상 슬롯 초기화
        foreach (var slot in rewardSlots)
        {
            Image rewardItemImage = slot.transform.GetChild(0).GetComponentInChildren<Image>();
            rewardItemImage.sprite = null;
            Color tempColor = rewardItemImage.color;
            tempColor.a = 0f;
            rewardItemImage.color = tempColor;
        }

        // 보상 아이템 추가
        for (int i = 0; i < quest.reward.items.Count; i++)
        {
            Image rewardItemImage = rewardSlots[i].transform.GetChild(0).GetComponentInChildren<Image>();

            Item item = itemDataBase.FetchItemByID(quest.reward.items[i].itemId);
            rewardItemImage.sprite = item.Icon;

            Color tempColor = rewardItemImage.color;
            tempColor.a = 1f;
            rewardItemImage.color = tempColor;
        }

        for(int i=0;i< rewardSlots.Count; i++)
        {
            rewardSlots[i].SetActive(true);
        }
        rewardPanel.SetActive(true);
    }

    public void HideQuestInfo()
    {
        questTitleText.text = "";
        descriptionText.text = "";
        questSummaryTitle.text = "";
        questSummaryText.text = "";
        RewardTitle.text = "";

        // 모든 보상 슬롯 초기화
        foreach (var slot in rewardSlots)
        {
            Image rewardItemImage = slot.transform.GetChild(0).GetComponentInChildren<Image>();
            rewardItemImage.sprite = null;
            Color tempColor = rewardItemImage.color;
            tempColor.a = 0f;
            rewardItemImage.color = tempColor;
        }

        for (int i = 0; i < rewardSlots.Count; i++)
        {
            rewardSlots[i].SetActive(false);
        }
        rewardPanel.SetActive(false);
    }

}
