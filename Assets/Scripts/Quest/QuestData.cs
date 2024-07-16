using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public int possibleLevel; // ���� ���� ����
    public bool alreadyAccept; // ������ �ִ� ����Ʈ���� 
    public string name; // ����Ʈ �̸� 
    public string description; // ���� 
    public string objective; // �ؾ��� ��  
    public Reward reward;
    public string status; // ���� ���� , ���� �� , �Ϸ�
}

[System.Serializable]
public class Reward
{
    public int experience;
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

