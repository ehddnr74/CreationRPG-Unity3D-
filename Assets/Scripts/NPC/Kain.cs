using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kain : MonoBehaviour
{
    public int playerLevel;
    public bool possibleQuest = false;
    DialogueManager dialogManager;

    private void Start()
    {
        dialogManager = GameObject.Find("DialogueUI").GetComponent<DialogueManager>();
    }

    public bool CheckPossibleQuest()
    {
        playerLevel = DataManager.instance.playerData.level;

        if (playerLevel >= QuestManager.instance.questData.quests[0].possibleLevel
            && QuestManager.instance.questData.quests[0].status == "���� ����")
        {
            possibleQuest = true;
            dialogManager.quest = QuestManager.instance.questData.quests[0];
        }
        if (QuestManager.instance.questData.quests[0].status == "���� ��")
        {
            possibleQuest = false;
        }
        if (QuestManager.instance.questData.quests[0].status == "�Ϸ�")
        {
            possibleQuest = false;
        }

        return possibleQuest;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            QuestManager.instance.StartQuest(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            QuestManager.instance.CompleteQuest(0);
        }
    }
}
