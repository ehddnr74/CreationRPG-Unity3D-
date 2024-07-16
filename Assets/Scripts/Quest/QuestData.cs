using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public int possibleLevel; // 시작 가능 레벨
    public bool alreadyAccept; // 받은적 있는 퀘스트인지 
    public string name; // 퀘스트 이름 
    public string description; // 설명 
    public string objective; // 해야할 것  
    public Reward reward;
    public string status; // 시작 가능 , 진행 중 , 완료
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

