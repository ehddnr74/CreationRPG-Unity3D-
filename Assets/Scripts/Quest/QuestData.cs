using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public int possibleLevel; // ���� ���� ����
    public string npcName; // ����Ʈ�� ���� NPC �̸�
    public bool alreadyAccept; // ������ �ִ� ����Ʈ���� 
    public string name; // ����Ʈ �̸� 
    public string description; // ���� 
    public string objective; // �ؾ��� ��  
    public int currentKillCount; // ��� �Ƿ� �� ���� ���� ���� �� 
    public int targetKillCount; // ��� �Ƿ� �� ��ƾ��ϴ� ���� ��
    public int currentCollectionCount; // ���� �Ƿ� �� ���� ���� ����
    public int maxCollectionCount; // ���� �Ƿ� �� ��ƾ��ϴ� ����
    public Reward reward;
    public string status; // ���� ���� , ���� �� , �Ϸ�
}

[System.Serializable]
public class Reward
{
    public int experience;
    public int gold;
    public List<RewardItem> items;
}

[System.Serializable]
public class RewardItem
{
    public int itemId;
    public int amount;
}

[System.Serializable]
public class QuestData
{
    public List<Quest> quests;
}

