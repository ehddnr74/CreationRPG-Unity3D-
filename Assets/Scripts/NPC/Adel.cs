using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adel : MonoBehaviour
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

        if (playerLevel >= QuestManager.instance.questData.quests[1].possibleLevel
            && QuestManager.instance.questData.quests[1].status == "���� ����")
        {
            possibleQuest = true;
            dialogManager.quest = QuestManager.instance.questData.quests[1];
        }
        if(QuestManager.instance.questData.quests[1].status == "���� ��")
        {
            possibleQuest = false;
        }
        if (QuestManager.instance.questData.quests[1].status == "�Ϸ�")
        {
            possibleQuest = false;
        }

        return possibleQuest;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            QuestManager.instance.StartQuest(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            QuestManager.instance.CompleteQuest(1);
        }
    }

}
