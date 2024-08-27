using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Events;
using System;
using static UnityEditor.Progress;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private Inventory inv;
    private CameraController cameraController;
    public QuestData questData;
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
    public TextMeshProUGUI npcText;
    public TextMeshProUGUI npcNameText;


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

    public Quest GetQuestById(int id)
    {
        return questData.quests.Find(quest => quest.id == id);
    }

    public Quest FindQuestByName(string questName)
    {
        foreach (Quest quest in questData.quests)
        {
            if (quest.name == questName)
            {
                return quest;
            }
        }
        return null; // �ش� �̸��� ����Ʈ�� ���� ���
    }

    public void StartQuest(int id)
    {
        Quest quest = GetQuestById(id);
        if (quest != null && quest.status == "���۰���")
        {
            if(quest.name == "������ ����")
            {
                inv.AddItem(0, 10);
                inv.AddItem(1, 10);
            }
            quest.status = "������";
            SaveQuests();

            // ���� ���� ��Ͽ��� �����ϰ� ���� �� ��Ͽ� �߰�
            UpdateQuestUI(quest);
        }
    }

    public void CompleteQuest(int id)
    {
        Quest quest = GetQuestById(id);
        if (quest != null && quest.status == "������")
        {
            quest.status = "�Ϸ�";

            DataManager.instance.AddExperience(quest.reward.experience);
            DataManager.instance.AddGold(quest.reward.gold);

            // ������ ���� �߰�
            foreach (var rewardItem in quest.reward.items)
            {
                if (rewardItem.itemId != 12 && rewardItem.itemId !=14) // == 12 : Gold(�κ��丮 Icon���� �߰��Ǽ��� �ȵ�)
                {                                                      // == 14 : Experience(����ġ)
                    inv.AddItem(rewardItem.itemId, rewardItem.amount);
                }
            }
            
            // UI ������Ʈ: ���� �߿��� �����ϰ� �Ϸῡ �߰�
            UpdateQuestUI(quest);

            SaveQuests();
        }
    }

    public void UpdateQuestUI(Quest quest)
    {
        Transform parentTransform = null;

        // ����Ʈ�� ���� ���¿� ���� �θ� Transform ����
        if (quest.status == "������")
        {
            parentTransform = progressContent.transform;
        }
        else if (quest.status == "�Ϸ�")
        {
            parentTransform = completeContent.transform;
        }

        // �� ���¿� ���� UI ������Ʈ
        if (parentTransform != null)
        {
            // ���� ���¿� ���� ������ ��Ͽ��� ����
            if (quest.status == "������")
            {
                RemoveQuestFromUI(notStartedContent.transform, quest.name);
            }
            else if (quest.status == "�Ϸ�")
            {
                RemoveQuestFromUI(progressContent.transform, quest.name);
            }

            // ���ο� ������ ��Ͽ� �߰�
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


    // ���� ������ �� QuestUI
    private void InitializeQuestUI()
    {
        // ������ UI ��ҵ� ����
        ClearQuestUI(notStartedContent.transform);
        ClearQuestUI(progressContent.transform);
        ClearQuestUI(completeContent.transform);

        int playerLevel = DataManager.instance.playerData.level;

        // ����Ʈ ���¿� ���� UI ��� ����
        foreach (var quest in questData.quests)
        {
            if (quest.status == "���۰���" && quest.possibleLevel <= playerLevel)
            {
                GameObject obj = Instantiate(titlePrefab, notStartedContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
            else if (quest.status == "������")
            {
                GameObject obj = Instantiate(titlePrefab, progressContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
            else if (quest.status == "�Ϸ�")
            {
                GameObject obj = Instantiate(titlePrefab, completeContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
        }
    }

    public void LevelUpToShowQuest()
    {
        int playerLevel = DataManager.instance.playerData.level;

        foreach (var quest in questData.quests)
        {
            if (quest.status == "���۰���" && quest.possibleLevel <= playerLevel && !quest.alreadyAccept)
            {
                quest.alreadyAccept = true;
                GameObject obj = Instantiate(titlePrefab, notStartedContent.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = quest.name;
                obj.GetComponent<QuestInfoButton>().quest = quest;
            }
        }
        SaveQuests();
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
        questSummaryTitle.text = "����Ʈ ���";

        npcText.text = "NPC : ";

        if(quest.npcName == "ī��")
        {
            npcNameText.text = $"[<color=red>�����</color>]\nī��";
        }
        if (quest.npcName == "�Ƶ�")
        {
            npcNameText.text = $"[<color=#00FFFF>�����</color>]\n�Ƶ�";
        }

        if (quest.name == "������ ����" && quest.status == "������")
        {
            questSummaryText.text = $"�巡�� ��� {quest.currentKillCount} / {quest.targetKillCount}";
        }
        else if(quest.name == "���ʸ� ��������!" && quest.status == "������")
        {
            for (int i = 0; i < inv.items.Count; i++)
            {
                if (inv.items[i].ID == 0)
                {
                    ItemDT data = inv.slots[i].transform.GetChild(0).GetComponent<ItemDT>();
                    questSummaryText.text = $" ������ ü�¹��� {data.amount} / {quest.maxCollectionCount}";
                }
            }
        }
        else
        {
            questSummaryText.text = quest.objective;
        }
        RewardTitle.text = "����";

        // ��� ���� ���� �ʱ�ȭ
        foreach (var slot in rewardSlots)
        {
            Image rewardItemImage = slot.transform.GetChild(0).GetComponentInChildren<Image>();
            rewardItemImage.sprite = null;
            Color tempColor = rewardItemImage.color;
            tempColor.a = 0f;
            rewardItemImage.color = tempColor;
        }

        // ���� ������ �߰�
        for (int i = 0; i < quest.reward.items.Count; i++)
        {
            Image rewardItemImage = rewardSlots[i].transform.GetChild(0).GetComponentInChildren<Image>();

            Item item = ItemDataBase.instance.FetchItemByID(quest.reward.items[i].itemId);
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
        npcText.text = "";
        npcNameText.text = "";

        // ��� ���� ���� �ʱ�ȭ
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
}
