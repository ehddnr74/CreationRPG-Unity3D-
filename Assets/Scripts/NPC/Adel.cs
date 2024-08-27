using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Adel : MonoBehaviour
{
    public string npcName;
    public int playerLevel;
    public bool possibleQuest = false;
    DialogueManager dialogManager;

    public Transform nameTagPosition;


    private void Start()
    {
        dialogManager = GameObject.Find("DialogueUI").GetComponent<DialogueManager>();
    }

    public bool CheckPossibleQuest(int questID)
    {
        playerLevel = DataManager.instance.playerData.level;

        if (playerLevel >= QuestManager.instance.questData.quests[questID].possibleLevel
            && QuestManager.instance.questData.quests[questID].status == "���۰���")
        {
            possibleQuest = true;
            dialogManager.quest = QuestManager.instance.questData.quests[questID];
        }
        if(QuestManager.instance.questData.quests[questID].status == "������")
        {
            possibleQuest = false;
        }
        if (QuestManager.instance.questData.quests[questID].status == "�Ϸ�")
        {
            possibleQuest = false;
        }

        return possibleQuest;
    }
}
