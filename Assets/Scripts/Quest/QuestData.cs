using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public string name;
    public string description;
    public string objective;
    public Reward reward;
    public string status; // "Not Started", "In Progress", "Completed"
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

