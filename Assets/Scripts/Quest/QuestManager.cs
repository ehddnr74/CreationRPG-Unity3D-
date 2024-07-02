using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private Inventory inv;
    public QuestData questData;
    private string path;

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
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
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

            SaveQuests();
        }
    }
}
