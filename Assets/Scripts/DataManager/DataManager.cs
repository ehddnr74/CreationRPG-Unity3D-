using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    // �̱���
    public static DataManager instance;

    public PlayerData playerData;

    private PlayerManager playerManager;

    string path;
    string playerDataFileName = "PlayerData"; // �÷��̾� ������ ���ϸ�
    
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

        path = Application.persistentDataPath + "/";
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();

        LoadPlayerData(); // �÷��̾� ������ �ε�
        LoadStatTableData(); // ���� �� HP/MP ���̺� �ε�
        LoadExperienceTableData();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            AddExperience(50);
        }
    }

    public void AddGold(int gold)
    {
        playerData.gold += gold;
        playerManager.UpdateGoldText();

        SavePlayerData();
    }
    public void LoseGold(int gold)
    {
        if (playerData.gold - gold < 0)
        {
            return;
        }
        playerData.gold -= gold;
        playerManager.UpdateGoldText();

        SavePlayerData();
    }
    public void AddExperience(int amount)
    {
        playerData.experience += amount;
        if (playerData.experience >= playerData.experienceTable[playerData.level])
        {
            playerData.level++;
            playerData.experience = 0; // ������ �� ����ġ �ʱ�ȭ (�Ǵ� ���� ����ġ ���)

            // ������ �� HP / MP�� ��������ŭ �÷���
            StatManager.instance.statData.maxHp += playerData.baseHPTable[playerData.level] - playerData.baseHPTable[playerData.level - 1];
            StatManager.instance.statData.maxMp += playerData.baseMPTable[playerData.level] - playerData.baseMPTable[playerData.level - 1];

            StatManager.instance.UpdateStatLevel();

            if (!StatManager.instance.hyperBody)
            {
                StatManager.instance.UpdateStatStatus();
            }
            else
            {
                StatManager.instance.UpdateStatActiveHyperBody();
            }

            StatManager.instance.UpdateStatExperience();
            QuestManager.instance.LevelUpToShowQuest();

            SkillManager.instance.skillCollection.skillPoint += 3;
            SkillManager.instance.itemChanged = true;
        }
        SavePlayerData();
    }













    public void SavePlayerData()
    {
        string data = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(path + playerDataFileName + ".json", data);
    }

    public void LoadPlayerData()
    {
        string playerDataPath = path + playerDataFileName + ".json";
        Debug.Log("Loading Player Data from: " + playerDataPath);

        if (File.Exists(playerDataPath))
        {
            string playerDataJson = File.ReadAllText(playerDataPath);
            Debug.Log("Player Data JSON: " + playerDataJson); // JSON ���� Ȯ��
            playerData = JsonConvert.DeserializeObject<PlayerData>(playerDataJson);
            Debug.Log("Player data loaded successfully: " + (playerData != null ? playerData.name : "null")); // �ε� ��� Ȯ��
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }

    private void LoadStatTableData()
    {
        string HpLevelPath = path + "BaseHpTable.json";
        string MpLevelPath = path + "BaseMpTable.json";

        if (File.Exists(HpLevelPath))
        {
            string HpLevelJson = File.ReadAllText(HpLevelPath);
            playerData.baseHPTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(HpLevelJson);
        }
        else
        {
            Debug.LogWarning("Level baseHPTable data file not found");
        }

        if (File.Exists(MpLevelPath))
        {
            string MpLevelJson = File.ReadAllText(MpLevelPath);
            playerData.baseMPTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(MpLevelJson);
        }
        else
        {
            Debug.LogWarning("Level baseHPTable data file not found");
        }
    }
    private void LoadExperienceTableData()
    {
        string levelExperiencePath = path + "ExperienceTable.json";

        if (File.Exists(levelExperiencePath))
        {
            string levelExperienceJson = File.ReadAllText(levelExperiencePath);
            playerData.experienceTable = JsonConvert.DeserializeObject<Dictionary<int, int>>(levelExperienceJson);
        }
        else
        {
            Debug.LogWarning("Level experience data file not found");
        }
    }
}
